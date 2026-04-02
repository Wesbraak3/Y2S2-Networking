using System.Net;
using Network;

namespace CardGames.GoFish;

class GameManager(Session session)
{
    private readonly Random rand = new(Guid.NewGuid().GetHashCode());
    private int SessionKey = 1234;

    private bool GameActive = false;

    private readonly Stockpile stockpile = new();

    private readonly List<Player> players = [new(session, true)];
    private int activePlayerIndex;

    public bool IsGameActive() => GameActive;
    public int GetSessionKey() => SessionKey;

    public bool IsPlayerInGame(Session session)
    {
        foreach (Player player in players)
        {
            if (player.GetSession() == session)
            {
                return true;
            }
        }
        return false;
    }

    public void AddPlayer(Session session)
    {
        foreach (Player player in players)
        {
            if (player.GetSession() == session)
            {
                Console.WriteLine("Player already in game: " + session.GetEndPoint());
                return;
            }
        }

        // players.Add(new Player(new Session(endpoint), isHost));
    }

    public bool CanStart()
    {
        if (players.Count >= 2)
        {
            return true;
        }

        Console.WriteLine("Cant start Game");
        return false;
    }

    public void Initialize()
    {
        if (!CanStart())
            return;

        GameActive = true;

        stockpile.Reset();
        DealCards();

        activePlayerIndex = rand.Next(0, players.Count);
    }

    private void DealCards()
    {
        int cardsPerPlayer = players.Count <= 3 ? 7 : 5; // TODO: Make Cards drawn dynamic with player set settings

        foreach (Player player in players)
        {
            for (int i = 0; i < cardsPerPlayer; i++)
            {
                player.AddCard(stockpile.DrawCard()!);
            }
        }
    }

    public void NextTurn()
    {
        activePlayerIndex = (activePlayerIndex + 1) % players.Count;
    }

    private bool GameWon()
    {
        // TODO
        GameActive = false;
        return false;
    }

    // public void Restart(Session session)
    // {
    //     Player? sender = GetPlayer(remote);

    //     if (sender == null || !sender.IsHost())
    //     {
    //         Console.WriteLine("Player not found for " + remote);
    //         Console.WriteLine("Only Host can restart the game");
    //         return;
    //     }

    //     Console.WriteLine("Restarting Game Session for " + remote);
    //     Initialize();
    // }
}