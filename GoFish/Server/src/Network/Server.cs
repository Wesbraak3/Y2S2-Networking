using System.Net;
using System.Net.Sockets;

namespace Network;

static class Server
{
    static private TcpListener listener;

    static private bool running = false;

    static public void Run(int port)
    {
        if (running)
            return;

        // Start listening for TCP connection requests, on the given port:
        listener = new(IPAddress.Any, port);

        listener.Start();
        running = true;

        Console.WriteLine($"Starting TCP server on port {port} - listening for incoming connection requests");
        Console.WriteLine("Press Q to stop the server");

        while (running)
        {
            Thread.Sleep(10);

            AcceptPendingClients();
            SessionManager.CheckMessages();

            OnExitPressed();
        }

        Shutdown();
    }

    static private void OnExitPressed()
    {
        if (Console.KeyAvailable)
        {
            char input = Console.ReadKey(true).KeyChar;
            if (input == 'q')
                running = false;
        }
    }

    static private void AcceptPendingClients()
    {
        while (listener.Pending())
        {
            SessionManager.AddSession(listener.AcceptTcpClient());
        }
    }

    static private void Shutdown()
    {
        Console.WriteLine("Stopping server");

        listener.Stop();

        Console.WriteLine("Server stopped");
    }
}