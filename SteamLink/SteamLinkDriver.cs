using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using Elements.Core;
using FrooxEngine;
using Rug.Osc;

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
        DataTreeDictionary dict = new();

        dict.Add("Name", "SteamLink Eye Datastream");
        dict.Add("Type", "Eye Tracking");
        dict.Add("Model", "SteamLink");
        list.Add(dict);
    }

    public void RegisterInputs(InputInterface i)
    {
        input = i;
        if (bridge.TryStartListen())
        {
            eyes = new(input, "Steam Link Datastream");
            bridge.ReceivedPacket += OnNewPacket;
            Impressive.Port_Config.OnChanged += OnSettingChanged;
            i.Engine.OnShutdown += bridge.StopListen;
        }
    }

    void OnNewPacket(object sender, OscPacket packet)
    {
        if (packet is OscBundle bundle)
        {
            lock (_lock)
            {
                foreach (OscPacket pckt in bundle)
                {
                    if (pckt is OscMessage msg)
                    {
                        faceData.TryMapOSC(msg.Address, msg.ToArray());
                    }
                }
            }
        }
    }

    public void UpdateInputs(float dt)
    {
        lock (_lock)
        {
            UpdateEye(faceData.EyeLeft, eyes!.LeftEye);
            UpdateEye(faceData.EyeRight, eyes!.RightEye);
        }
    }

    public void UpdateEye(SteamLinkEye source, Eye dest)
    {
        dest.Direction = source.EyeDirection;
        dest.Widen = source.ExpandedSqueeze;
        dest.Openness = source.Eyelid;
        dest.PupilDiameter = 0.03f;
        dest.Squeeze = source.SqueezeToggle;
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