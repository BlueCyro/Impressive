using Elements.Core;
using FrooxEngine;
using Rug.Osc;
using OSCMapper;

namespace Impressive;

public class SteamLinkDriver : IInputDriver
{
    private InputInterface? input;
    private Eyes? eyes;
    
    private readonly OSCBridge bridge = new();
    private readonly SteamFace faceData = new();
    
    private readonly object _lock = new();
    public int UpdateOrder => 150;
    
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
        if (bridge.TryStartListen())
        {
            Impressive.Msg("Starting SteamLink datastream!");
            eyes = new(input, "Steam Link Datastream");
            bridge.ReceivedPacket += OnNewPacket;
            Impressive.Port_Config.OnChanged += OnSettingChanged;
            i.Engine.OnShutdown += Shutdown;
        }
    }

    void OnNewPacket(object sender, OscPacket packet)
    {
        if (packet is OscMessage msg)
        {
            lock (_lock)
            {
                faceData.TryMapOSC(msg.Address, msg.ToArray());
            }
        }
        else if (packet is OscBundle bundle)
        {
            lock (_lock)
            {
                foreach (var pkt in bundle)
                {
                    if (pkt is OscMessage m)
                    {
                        faceData.TryMapOSC(m.Address, m.ToArray());
                    }
                }
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
        dest.PupilDiameter = 0.003f;
        dest.Squeeze = source.SqueezeToggle;
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