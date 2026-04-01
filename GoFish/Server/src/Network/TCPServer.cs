using System.Net;
using System.Net.Sockets;
using OSCTools;

using CardGames.GoFish;

namespace Network;

static class TCPServer
{
    static private readonly OSCDispatcher dispatcher = new();

    static private readonly TcpListener listener = new(IPAddress.Any, 50001);
    static private readonly SessionManager sessionManager = new();

    static private bool shuttingDown = false;
    static private bool isProcessing = false;

    static public void Initialize()
    {
        listener.Start();

        // Debug OSC
        dispatcher.ShowIncomingMessages = true;

        // Setup game
        InitializeGoFish();

        Console.WriteLine($"Starting TCP server on port {50001} - listening for incoming connection requests");
        Console.WriteLine("Press Q to stop the server");

        Run();
    }

    static async private void Run()
    {
        while (!shuttingDown)
        {
            isProcessing = true;

            CheckPendingSessions();
            sessionManager.UpdateSessions();

            foreach (var (session, messages) in sessionManager.GetSessionMessages())
                while (messages.Count > 0)
                    dispatcher.HandleMessage(messages.Dequeue(), session.EndPoint);

            isProcessing = false;
            Thread.Sleep(10);
        }
    }

    static private void CheckPendingSessions()
    {
        while (listener.Pending())
        {
            sessionManager.AddSession(listener.AcceptTcpClient());
        }
    }

    static public void Shutdown()
    {
        Console.WriteLine("Stopping server");
        shuttingDown = true;

        while (isProcessing)
        {
            Thread.Sleep(10);
        }

        sessionManager.ClearSessions();
        listener.Stop();

        Console.WriteLine("Server stopped");
    }


    static Game goFishGame;
    static void InitializeGoFish()
    {
        goFishGame = new();

        // board.OnActivePlayerChange += ActivePlayerChangeRpc;
        // board.OnCellChange += CellChangeRpc;
        // board.OnGameOver += GameOverRpc;
    }

    static void Broadcast()
    {
        sessionManager.Broadcast([]);
    }
}