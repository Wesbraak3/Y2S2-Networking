using System.Net;
using System.Net.Sockets;

namespace Network;

static class TcpServer
{
    static private readonly TcpListener listener = new(IPAddress.Any, 50001);
    static private bool running = false;

    static public void Start()
    {
        if (running)
            return;

        listener.Start();
        running = true;

        Console.WriteLine($"Starting TCP server on port {50001} - listening for incoming connection requests");
        Console.WriteLine("Press Q to stop the server");
    }

    static public void CheckPendingSessions()
    {
        while (listener.Pending())
        {
            SessionManager.AddSession(listener.AcceptTcpClient());
        }
    }

    static public void Shutdown()
    {
        Console.WriteLine("Stopping server");

        SessionManager.ClearSessions();
        listener.Stop();

        Console.WriteLine("Server stopped");
    }
}