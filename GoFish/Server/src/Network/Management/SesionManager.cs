
using OSCTools;

namespace Network;

class SessionManager
{
    public readonly LoginSession LoginSession = new();
    public readonly LobbySession LobbySession = new();

    private readonly Dictionary<Guid, Session> activeSessions = [];

    public void Initialize()
    {
        AddSession(LoginSession);
        AddSession(LobbySession);
    }

    public void Shutdown()
    {

    }

    public void TransferConnection(Connection connection, Session target)
    {
        Session? origin = connection.ActiveSession;

        if (origin == null)
            return;

        origin.RemoveConnection(connection);

        // if you are the last person in the session clean up the session
        if (origin.GetConnections().Count <= 0 && origin != LoginSession && origin != LobbySession)
            RemoveSession(origin.Id);

        OSCMessageOut messageOut = new("/SwapScene");

        string scene = target.GetType().Name;
        messageOut.AddString(scene);

        connection.SendMessage(messageOut.GetBytes());

        target.AddConnection(connection);
    }

    public void AddSession(Session session)
    {
        activeSessions.Add(session.Id, session);
        session.Initialize();
    }

    public Session? GetSession(Guid guid) =>
        activeSessions.TryGetValue(guid, out var session)
            ? session
            : null;

    public IEnumerable<Session> GetAllSessions() =>
    activeSessions.Values;

    public void RemoveSession(Guid guid) =>
        activeSessions.Remove(guid);

    public void Broadcast(byte[] message)
    {
        foreach (var session in activeSessions.Values)
            session.Broadcast(message);
    }
}
