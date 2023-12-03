using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using Elements.Core;
using FrooxEngine;
using Rug.Osc;

namespace Impressive;

public class SteamLinkDriver : IInputDriver
{
    private InputInterface? input;
    private Eyes? eyes;
    private OscPacket? Data
    {
        get
        {
            lock (_lock)
            {
                return latestData;
            }
        }
        set
        {
            lock (_lock)
            {
                latestData = value;
            }
        }
    }
    private OscPacket? latestData;
    private readonly object _lock = new();
    private readonly OSCBridge bridge = new();

    public void RegisterInputs(InputInterface i)
    {
        input = i;
        if (bridge.TryStartListen())
        {
            eyes = new(input, "Steam Link Datastream");
            bridge.ReceivedPacket += OnNewPacket;
            i.Engine.OnShutdown += bridge.StopListen;
        }
    }

    void OnNewPacket(object sender, OscPacket packet)
    {
        Data = packet;
    }

    public void UpdateInputs(float dt)
    {
        if (Data is OscMessage msg)
        {
            var objects = msg.ToArray();
            MemoryMarshal.AsBytes(objects.AsSpan());
        }
    }

    private void Shutdown()
    {
        bridge.ReceivedPacket -= OnNewPacket;
        bridge.StopListen();
    }
}