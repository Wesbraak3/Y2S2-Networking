using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using CardGames.GoFish;
using OSCTools;

namespace Network;

public class LobbySession : Session
{
    public override void Initialize()
    {
        base.Initialize();

        dispatcher.AddListener("/Logout", Logout);
        dispatcher.AddListener("/CreateNew", CreateNew);
        dispatcher.AddListener("/GetExisting", GetExisting);
        dispatcher.AddListener("/JoinExisting", JoinExisting);
    }

    private void Logout(OSCMessageIn message, IPEndPoint endPoint)
    {
        Connection? connection = GetConnection(endPoint);
        if (connection == null)
            return;

        SessionManager.TransferConnection(connection, SessionManager.LoginSession);
    }

    private void CreateNew(OSCMessageIn message, IPEndPoint endPoint)
    {
        Connection? connection = GetConnection(endPoint);
        if (connection == null)
            return;

        GoFishSession newSession = new();
        SessionManager.AddSession(newSession);

        SessionManager.TransferConnection(connection, newSession);
    }

    private void GetExisting(OSCMessageIn message, IPEndPoint endPoint)
    {
        Connection? connection = GetConnection(endPoint);
        if (connection == null)
            return;

        OSCMessageOut messageOut = new("/ExistingGames");
        JsonArray sessionDetails = [];

        foreach (GoFishSession session in SessionManager.GetAllSessions().OfType<GoFishSession>())
        {
            JsonObject sessionDetail = new()
            {
                ["id"] = session.Id,
                ["ongoing"] = session.GameOngoing(),
                ["host"] = session.GetConnections()[0].Username,
                ["playerCount"] = session.GetConnections().Count
            };

            sessionDetails.Add(sessionDetail);
        }

        string json = JsonSerializer.Serialize(sessionDetails);

        messageOut.AddBlob(Encoding.UTF8.GetBytes(json)); // conver to byte[]
        connection.SendMessage(messageOut.GetBytes());
    }

    private void JoinExisting(OSCMessageIn message, IPEndPoint endPoint)
    {
        Connection? connection = GetConnection(endPoint);
        if (connection == null)
            return;

        string guidString = message.ReadString();

        if (Guid.TryParse(guidString, out Guid guid))
        {
            Session? existingGameSession = SessionManager.GetSession(guid);
            if (existingGameSession == null)
                return;

            // TODO: Dont let poeple use LoginSession or LobbySession maybe
            SessionManager.TransferConnection(connection, existingGameSession);
        }
    }
}