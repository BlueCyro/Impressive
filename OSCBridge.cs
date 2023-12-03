using System.Net;
using FrooxEngine;
using Rug.Osc;
using System.Threading;

namespace Impressive;

public class OSCBridge
{
    public bool Listening { get; private set; }
    public int Port => Impressive.Config!.GetValue(Impressive.Port_Config);
    public EventHandler<OscPacket>? ReceivedPacket;
    private Thread? listenThread;
    private CancellationTokenSource tkSrc = new();
    private ReaderWriterLockSlim slimLock = new();

    public bool TryStartListen()
    {
        if (listenThread != null && listenThread.ThreadState == ThreadState.Running)
            return false;
        
        tkSrc = new();
        try
        {
            OscReceiver recv = new(IPAddress.Any, Port);
            listenThread = new(new ThreadStart(() => ListenLoop(recv, tkSrc.Token)));
            recv.Connect();
            listenThread.Start();
            Listening = true;
            return true;
        }
        catch (Exception ex)
        {
            Impressive.Msg($"Exception initializing OSCBridge: {ex.Message}");
            Listening = false;
            return false;
        }
    }

    public void StopListen()
    {
        tkSrc.Cancel();
        tkSrc.Dispose();
    }

    void ListenLoop(OscReceiver recv, CancellationToken token)
    {
        Listening = true;
        AutoResetEvent ev = new(false);
        try
        {
            while (recv.State != OscSocketState.Closed)
            {

                if (token.IsCancellationRequested)
                {
                    Impressive.Msg($"OSCListener on {recv.LocalEndPoint}");
                    break;
                }
                var packet = recv.Receive();

                ReceivedPacket?.Invoke(recv, packet);
            }
        }
        catch (Exception ex)
        {
            if (recv.State == OscSocketState.Connected)
            {
                Impressive.Msg($"Exception in listener loop: {ex.Message}");
                Listening = false;
            }
        }
    }
}