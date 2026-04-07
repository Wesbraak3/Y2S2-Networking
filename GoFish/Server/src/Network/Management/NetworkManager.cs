using System.Net;
using System.Net.Sockets;

namespace Network;

class NetworkManager
{
    private TcpListener listener = new(IPAddress.Any, 50011);
    private bool isProcessing = false;

    public void Initialize(int port = 50011)
    {
        listener = new(IPAddress.Any, port);
        listener.Start();

        Console.WriteLine($"Starting TCP server on port {port} - listening for incoming connection requests");

        Task.Run(Run);
    }

    async private void Run()
    {
        while (Server.IsRunnning)
        {
            isProcessing = true;

            AcceptPendingClients();

            isProcessing = false;
            Thread.Sleep(10);
        }
    }

    private void AcceptPendingClients()
    {
        while (listener.Pending())
        {
            // TODO: Existing connection handling
            // can add checks to see if its good by sending and receiving program info
            // for now just make a new connection
            Connection newConnection = new(listener.AcceptTcpClient());

            // TODO: Abrupt disconnect & reconnect handling 
            //check and get session if connection was disconnected arubtly
            // if (Server.sessionManager.HasActiveSession(newConnection))
            // {
            //     // update connection with gamestate 
            //     _ = newConnection.Run();
            // }

            // for now just accept and bring to login session
            Server.sessionManager.LoginSession.AddConnection(newConnection);

            // fire and forget connection runtime
            _ = newConnection.Run();
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