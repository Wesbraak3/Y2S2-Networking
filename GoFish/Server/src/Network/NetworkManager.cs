using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace Network;

class NetworkManager
{
    private readonly TcpListener listener = new(IPAddress.Any, 50011);

    private bool isProcessing = false;

    public void Initialize()
    {
        listener.Start();

        Console.WriteLine($"Starting TCP server on port {50001} - listening for incoming connection requests");
        Console.WriteLine("Press Q to stop the server");

        Run();
    }

    async private void Run()
    {
        while (Server.IsRunnning)
        {
            isProcessing = true;

            CheckPendingSessions();

            isProcessing = false;
            Thread.Sleep(10);
        }
    }

    private void CheckPendingSessions()
    {
        while (listener.Pending())
        {
            // can add checks to see if its good by sending and receiving program info
            // for now just make a new connection
            Server.sessionManager.NewConnection(listener.AcceptTcpClient());
        }
    }

     public void Shutdown()
    {
        Console.WriteLine("Stopping network");

        while (isProcessing)
        {
            Thread.Sleep(10);
        }

        listener.Stop();

        Console.WriteLine("Server stopped");
    }
}