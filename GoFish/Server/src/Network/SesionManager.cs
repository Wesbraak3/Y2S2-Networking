using System.Net.Sockets;

namespace Network;

class SessionManager
{
    private readonly List<Session> clientSessions = [];

    public List<Session> GetSessions() => clientSessions;

    public void AddSession(TcpClient newClient)
    {
        clientSessions.Add(new(newClient));
        Console.WriteLine($"Client connected from remote end point {newClient.Client.RemoteEndPoint}");
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
