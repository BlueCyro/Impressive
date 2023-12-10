using System.Net;
using Rug.Osc;

namespace Impressive;

public class OSCBridge
{
    public bool Listening { get; private set; }
    public int Port => Impressive.Config!.GetValue(Impressive.Port_Config);
    public EventHandler<OscPacket>? ReceivedPacket;
    private Thread? listenThread;
    private CancellationTokenSource tkSrc = new();

    public bool TryStartListen()
    {
        Impressive.Msg("Trying to start OSC listener");
        if (listenThread != null && listenThread.ThreadState == ThreadState.Running)
            return false;
        
        Impressive.Msg("Starting OSC listening thread");
        tkSrc = new();
        try
        {
            Impressive.Msg("Creating receiver");
            OscReceiver recv = new(IPAddress.Any, Port);
            Impressive.Msg("Creating thread loop");
            listenThread = new(new ThreadStart(() => ListenLoop(recv, tkSrc.Token)));
            Impressive.Msg("Connecting receiver");
            recv.Connect();
            Impressive.Msg("Starting thread");
            listenThread.Start();
            Impressive.Msg("Thread started, listening!");
            Listening = true;
            return true;
        }
        catch (Exception ex)
        {
            Impressive.Msg($"Exception initializing OSCBridge: {ex}");
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
                    Impressive.Msg($"OSCListener on {recv.LocalEndPoint} was closed by request.");
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
                Impressive.Msg($"Exception in listener loop: {ex}");
                Listening = false;
            }
        }
    }
}