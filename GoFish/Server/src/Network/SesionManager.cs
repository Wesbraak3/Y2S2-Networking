using System.Net.Sockets;

namespace Network;

static class SessionManager
{
    static private readonly List<Session> clientSessions = [];
    static private bool processing = false;

    static public List<Session> GetClients() => clientSessions;
    static public void AddSession(TcpClient newClient)
    {
        clientSessions.Add(new(newClient));
        Console.WriteLine($"Client connected from remote end point {newClient.Client.RemoteEndPoint}");
    }

    static public void CheckMessages()
    {
        processing = true;

        foreach (Session session in clientSessions)
            session.ReadMessages();

        processing = false;
    }

    static public void InactiveClients()
    {
        clientSessions.RemoveAll(session =>
        {
            if (!session.IsConnected())
            {
                session.Close();
                Console.WriteLine($"Client removed. Count: {clientSessions.Count - 1}");
                return true;
            }
            return false;
        });
    }

    static public void CleanupClients()
    {
        while (processing)
            Thread.Sleep(10);

        clientSessions.Clear();
    }
}
