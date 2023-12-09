using Elements.Core;
using FrooxEngine;
using Rug.Osc;
using ReSounding;
using System.Data.Common;

namespace Impressive;

public class SteamLinkDriver : IInputDriver
{
    private InputInterface? input;
    private Eyes? eyes;
    private Mouth? mouth;
    
    private readonly OSCBridge bridge = new();
    private readonly SteamEyes eyeData = new();
    private readonly SteamFace faceData = new();
    
    private readonly object _lock = new();
    public int UpdateOrder => 150;
    
    public DateTime DEBUG_TIME = DateTime.Now;


    public void CollectDeviceInfos(DataTreeList list)
    {
        Impressive.Msg("Collecting SteamLink device info");

        // Eye tracking
        DataTreeDictionary eyeDict = new();

        eyeDict.Add("Name", "SteamLink Eye Datastream");
        eyeDict.Add("Type", "Eye Tracking");
        eyeDict.Add("Model", "SteamLink");
        list.Add(eyeDict);


        // Mouth tracking
        DataTreeDictionary mouthDict = new();

        mouthDict.Add("Name", "SteamLink Face Datastream");
        mouthDict.Add("Type", "Lip Tracking");
        mouthDict.Add("Model", "SteamLink");
        list.Add(mouthDict);
    }


    public void RegisterInputs(InputInterface i)
    {
        input = i;
        Impressive.Msg("Attempting to start OSC listener");
        try
        {
            if (bridge.TryStartListen())
            {
                OSCMapper.RegisterConverters(typeof(OSCTypeConverters)); // Register custom type converter(s)
                Impressive.Msg("Starting SteamLink datastream!");

                // Register eye and mouth tracking devices
                eyes = new(input, "Steam Link Datastream");
                mouth = new(input, "Steam Link Datastream");

                // Subscribe events for receiving packets, changing config options, and shutting down
                bridge.ReceivedPacket += OnNewPacket;
                Impressive.Port_Config.OnChanged += OnSettingChanged;
                i.Engine.OnShutdown += Shutdown;
            }
        }
        catch (Exception ex)
        {
            Impressive.Msg($"Failed to initialize OSC server! Exception: {ex}");
        }
    }


    void OnNewPacket(object sender, OscPacket packet)
    {
        // DEBUG_TRY_LOG(packet);
        if (packet is OscMessage msg)
        {
            Map(msg.Address, msg.ToArray());
        }
        else if (packet is OscBundle bundle)
        {
            foreach (var pkt in bundle)
            {
                if (pkt is OscMessage m)
                {
                    Map(m.Address, m.ToArray());
                }
            }
        }
    }


    void DEBUG_TRY_LOG(OscPacket pckt)
    {
        if (DateTime.Now - DEBUG_TIME < TimeSpan.FromSeconds(2f))
            return;
        
        DEBUG_TIME = DateTime.Now;
        if (pckt is OscMessage msg)
        {
            Impressive.Msg("---- DEBUG MESSAGE ----");
            Impressive.Msg(msg);
            Impressive.Msg("---- END MSG ----");
            Impressive.Msg("");
        }
        else if (pckt is OscBundle bnd)
        {
            Impressive.Msg("---- DEBUG BUNDLE ----");
            foreach (var pkt in bnd)
            {
                Impressive.Msg(pkt);
            }
            Impressive.Msg("---- END BUNDLE ----");
            Impressive.Msg("");
        }
    }


    void Map(string addr, object[] data)
    {
        lock (_lock)
        {
            try
            {
                OSCMapper.TryMapOSC(eyeData, addr, data);
                OSCMapper.TryMapOSC(faceData, addr, data);
            }
            catch (Exception ex)
            {
                Impressive.Msg($"Invalid mapping to address \"{addr}\"! Exception: {ex}");
            }
        }
    }


    public void UpdateInputs(float dt)
    {
        if (eyes != null && mouth != null)
        {
            eyes.IsDeviceActive = Impressive.Enabled;
            eyes.IsEyeTrackingActive = Impressive.Enabled;
            lock (_lock)
            {
                UpdateEye(eyeData.EyeLeft, eyes.LeftEye);
                UpdateEye(eyeData.EyeRight, eyes.RightEye);
                UpdateEye(eyeData.EyeCombined, eyes.CombinedEye);
                UpdateFace(mouth);
            }
        }
    }


    public void UpdateEye(SteamLinkEye source, Eye dest)
    {
        dest.Direction = source.EyeDirection;
        dest.Widen = source.ExpandedSqueeze;
        dest.Openness = source.Eyelid;
        dest.PupilDiameter = 0.004f;
        dest.Squeeze = 0f;
        dest.IsTracking = true;
    }


    public void UpdateFace(Mouth mouth)
    {
        mouth.IsDeviceActive = true;
        mouth.IsTracking = true;
        mouth.CheekLeftPuffSuck = faceData.CheekPuffSuckL;
        mouth.CheekRightPuffSuck = faceData.CheekPuffSuckR;
        mouth.JawOpen = faceData.JawDown - faceData.LipsToward;
        mouth.Jaw = faceData.JawPos + new float3(0f, mouth.JawOpen, 0f); // Negate jaw open if the jaw & lips are actually open so we don't keep the ape face.
        mouth.MouthPout = (faceData.LipPuckerL + faceData.LipPuckerR) * 0.5f;
    }


    private void OnSettingChanged(object? o)
    {
        bridge.StopListen();
        bridge.TryStartListen();
    }


    private void Shutdown()
    {
        bridge.ReceivedPacket -= OnNewPacket;
        bridge.StopListen();
    }
}