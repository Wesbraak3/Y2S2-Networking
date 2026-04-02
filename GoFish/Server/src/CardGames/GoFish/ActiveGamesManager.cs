using System.Net;
using OSCTools;
using Network;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace CardGames.GoFish;

class ActiveGamesManager
{
    readonly List<GameManager> ongoingGames = [];

    private static Session? GetSession(IPEndPoint remote)
    {
        Session? session = TCPServer.GetSessionManger().GetSession(remote);

        if (session == null)
            Console.WriteLine("No Session found for " + remote);

        return session;
    }

    public void CreateGame(OSCMessageIn message, IPEndPoint remote)
    {
        Session? session = GetSession(remote);

        if (session == null)
        {
            return;
        }

        OSCMessageOut messageOut = new("/Create");

        foreach (GameManager game in ongoingGames)
        {
            if (game.IsPlayerInGame(session))
            {
                messageOut.AddBool(false);
                messageOut.AddString("Player already in a game.");
                session.Send(messageOut.GetBytes());
                return;
            }
        }

        GameManager newGame = new(session);
        ongoingGames.Add(newGame);

        messageOut.AddBool(true);
        messageOut.AddInt(newGame.GetSessionKey());

        session.Send(messageOut.GetBytes());

        Console.WriteLine("Created new game with session key: " + newGame.GetSessionKey());
    }


    // public void ResetGame(OSCMessageIn message, IPEndPoint remote)
    // {
    //     GameManager? game = GameSessionSearch(remote);

    //     if (game == null)
    //         return;

    //     game.Restart(remote);
    // }
}