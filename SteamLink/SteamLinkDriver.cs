using Elements.Core;
using FrooxEngine;
using Rug.Osc;
using ReSounding;

namespace Impressive;

public class SteamLinkDriver : IInputDriver
{
    private InputInterface? input;
    private Eyes? eyes;
    
    private readonly OSCBridge bridge = new();
    private readonly SteamEyes faceData = new();
    
    private readonly object _lock = new();
    public int UpdateOrder => 150;
    
    public DateTime DEBUG_TIME = DateTime.Now;

    public void CollectDeviceInfos(DataTreeList list)
    {
        Impressive.Msg("Collecting SteamLink device info");
        DataTreeDictionary dict = new();

        dict.Add("Name", "SteamLink Eye Datastream");
        dict.Add("Type", "Eye Tracking");
        dict.Add("Model", "SteamLink");
        list.Add(dict);
    }

    public void RegisterInputs(InputInterface i)
    {
        input = i;
        Impressive.Msg("Attempting to start OSC listener");
        try
        {
            if (bridge.TryStartListen())
            {
                OSCMapper.RegisterConverters(typeof(OSCTypeConverters));
                Impressive.Msg("Starting SteamLink datastream!");
                eyes = new(input, "Steam Link Datastream");
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
        if (eyes != null)
        {
            eyes.IsDeviceActive = Impressive.Enabled;
            eyes.IsEyeTrackingActive = Impressive.Enabled;
            lock (_lock)
            {
                UpdateEye(faceData.EyeLeft, eyes.LeftEye);
                UpdateEye(faceData.EyeRight, eyes.RightEye);
                UpdateEye(faceData.EyeCombined, eyes.CombinedEye);
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