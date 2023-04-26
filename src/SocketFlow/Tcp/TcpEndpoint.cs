using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SocketFlow.Tcp;

public class TcpEndpoint : IEndpoint
{
    private IPEndPoint endPoint;
    private Socket socket;
    private readonly List<IObserver<HoldableBuffer>> observers = new();
    private readonly List<Task> sentTasks = new();
    private bool closed;

    public TcpEndpoint(Socket socket)
    {
        this.socket = socket;
    }

    public TcpEndpoint(IPEndPoint endPoint)
    {
        this.endPoint = endPoint;
    }

    public async Task Send(ReadOnlyMemory<byte> data)
    {
        await socket.SendAsync(new ArraySegment<byte>(data.ToArray(), 0, data.Length), SocketFlags.None);
    }

    public async Task StartAsync(CancellationToken token)
    {
        await (socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            .ConnectAsync(endPoint)
            .ContinueWith(_ => { }, token);
    }

    public async Task ObserveAsync(CancellationToken token)
    {
        token.Register(() =>
        {
            socket.Close();
        });
        try
        {
            while (socket.Connected && !token.IsCancellationRequested)
            {
                var buffer = new HoldableBuffer(1024);
                var read = await socket.ReceiveAsync(buffer.AsArraySegment(), SocketFlags.None);
                if (read == 0)
                {
                    (buffer as IDisposable).Dispose();
                    break;
                }

                buffer.Trim(read);
                foreach (var observer in observers)
                    observer.OnNext(buffer);
            }
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
        catch (SocketException e)
        {
            if (e.SocketErrorCode != SocketError.OperationAborted && e.SocketErrorCode != SocketError.ConnectionReset)
            {
                foreach (var observer in observers)
                    observer.OnError(e);
            }
        }
        catch (Exception e)
        {
            foreach (var observer in observers)
                observer.OnError(e);
        }
        finally
        {
            foreach (var observer in observers)
                observer.OnCompleted();
        }
    }

    public IDisposable Subscribe(IObserver<HoldableBuffer> observer)
    {
        observers.Add(observer);
        return null;
    }

    public override string ToString()
    {
        return $"{socket.ProtocolType.ToString().ToLower()}://{socket.RemoteEndPoint}";
    }
}
