using System.Net;
using OSCTools;

namespace Network;

public class LoginSession : Session
{
    public override void Initialize()
    {
        base.Initialize();

        dispatcher.AddListener("/Login", Login);
    }

    public override void AddConnection(Connection connection)
    {
        base.AddConnection(connection);
    }

    private void Login(OSCMessageIn message, IPEndPoint endPoint)
    {
        string name = message.ReadString();

        if (string.IsNullOrWhiteSpace(name))
            return;

        // Find the connection that sent this
        var connection = GetConnections()
            .FirstOrDefault(c => c.EndPoint.Equals(endPoint));

        if (connection == null)
            return;

        connection.Player.SetName(name);

        Console.WriteLine($"Player logged in: {name}");

        Server.sessionManager.TransferConnection(connection, Server.sessionManager.LobbySession);
    }
}