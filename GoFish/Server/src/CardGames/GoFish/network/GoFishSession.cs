using Network;
using System.Net;
using OSCTools;

namespace CardGames.GoFish;

public class GoFishSession : Session
{
    private readonly GameManager gameManager = new();

    public override void Initialize()
    {
        base.Initialize();

        dispatcher.AddListener("/Leave", Leave);

        dispatcher.AddListener("/Play", Play);
        dispatcher.AddListener("/Spectate", Spectate);

        dispatcher.AddListener("/CanStart", CanStart);
        dispatcher.AddListener("/Start", Start);
    }

    public GameManager GetGameManager() => gameManager;

    public override void AddConnection(Connection connection)
    {
        base.AddConnection(connection);

        gameManager.AddPlayer(connection);
    }

    public override void RemoveConnection(Connection connection)
    {
        base.RemoveConnection(connection);

        gameManager.RemovePlayer(connection);
        if (GetConnections().Count <= 0)
            SessionManager.RemoveSession(Id);
    }

    private void Leave(OSCMessageIn message, IPEndPoint endPoint)
    {
        Connection? connection = GetConnection(endPoint);
        if (connection == null)
            return;

        SessionManager.TransferConnection(connection, SessionManager.LobbySession);
    }

    private void CanStart(OSCMessageIn message, IPEndPoint endPoint)
    {
        bool canStart = gameManager.CanStart();


        // TODO: Return MSG to Connection if you can start the game
    }

    private void Start(OSCMessageIn message, IPEndPoint endPoint)
    {
        Connection? connection = GetConnection(endPoint);
        if (connection == null)
            return;

        gameManager.StartGame(connection);
    }

    public bool GameOngoing() => gameManager.GameOver;

    private void Play(OSCMessageIn message, IPEndPoint endPoint)
    {

    }
    private void Spectate(OSCMessageIn message, IPEndPoint endPoint)
    {

    }
}