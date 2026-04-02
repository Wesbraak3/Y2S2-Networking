using System.Net;
using System.Net.Sockets;

namespace Network;

class SessionManager
{
    private readonly List<Session> activeSessions = [];

    public  void Initialize ()
    {
        NewSession();
    }

    private async void Run()
    {
        
    }

    public  void Shutdown ()
    {
        
    }

    public Session NewSession()
    {
        Session newSession;

        
        return newSession
    }

    public void NewConnection(TcpClient client)
    {
        
    }



     public void AddSession(TcpClient newClient)
    {
        clientSessions.Add(new(newClient));
        Console.WriteLine($"Client connected from remote end point {newClient.Client.RemoteEndPoint}");
    }

     public Session? GetSession(IPEndPoint endpoint)
    {
        foreach (Session session in clientSessions)
        {
            if (session.GetEndPoint().Equals(endpoint))
                return session;
        }

        Console.WriteLine("No Session found for " + endpoint);
        return null;
    }

     public void ClearSessions()
    {
        while (clientSessions.Count > 0)
        {
            Session session = clientSessions[0];

            session.Close();
            clientSessions.Remove(session);
        }
    }

     public void UpdateSessions()
    {
        foreach (Session session in clientSessions)
            session.ReadPackets();
    }

     public Dictionary<Session, Queue<byte[]>> GetSessionMessages()
    {
        Dictionary<Session, Queue<byte[]>> sessionMessagesDict = [];

        foreach (Session session in clientSessions)
        {
            Queue<byte[]> sessionMessages = session.GetAllMessages();

            sessionMessagesDict.Add(session, sessionMessages);
        }

        return sessionMessagesDict;
    }

     public void Broadcast(byte[] message)
    {
        foreach (Session session in clientSessions)
        {
            session.Send(message);
        }
    }

     public void InactiveClients()
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
}
