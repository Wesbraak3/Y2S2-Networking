using Network;

namespace CardGames.GoFish;

public class GameManager()
{
    public readonly List<Player> Players = [];
    public readonly List<Player> Spectators = [];
    private readonly Random rand = new(Guid.NewGuid().GetHashCode());
    private readonly Stockpile stockpile = new();

    // Game settings
    public int CardsPerPlayer { get; private set; } = 7;
    public int MinPlayers { get; private set; } = 2;
    public int MaxPlayers { get; private set; } = 4;

    // Game Logic variables
    public bool GameActive { get; private set; } = false;
    public bool GameOver { get; private set; } = false;
    public int ActivePlayerIndex { get; private set; } = 0;

    private Player? GetPlayer(Connection connection)
            => Players.FirstOrDefault(p => p.connection == connection) ?? Spectators.FirstOrDefault(p => p.connection == connection);

    #region Game Lobby
    public void AddPlayer(Connection connection)
    {
        if (GetPlayer(connection) != null)
        {
            Console.WriteLine("Player already in game");
            return;
        }
        if (Players.Count >= 6 || GameActive)
            Spectators.Add(new Player(connection));
        else
            Players.Add(new Player(connection, Players.Count == 0));
    }

    public void RemovePlayer(Connection connection)
    {
        Player? player = GetPlayer(connection);
        if (player == null)
        {
            Console.WriteLine("Player not in Game");
            return;
        }

        bool isPlayer = Players.FirstOrDefault(p => p.connection == connection) == null;
        bool wasHost = player.IsHost;

        if (isPlayer)
            Players.Remove(player);
        else
            Spectators.Remove(player);

        if (wasHost)
            SetNewHost();
    }

    public void BecomePlayer(Connection connection)
    {
        if (GameActive && Players.Count >= MaxPlayers)
            return;
        else
        {
            Player? player = GetPlayer(connection);
            if (player == null)
                return;

            Spectators.Remove(player);
            Players.Add(player);
        }
    }

    public void BecomeSpectator(Connection connection)
    {
        if (GameActive)
            return;
        else
        {
            Player? player = GetPlayer(connection);
            if (player == null)
                return;

            Players.Remove(player);
            Spectators.Add(player);
        }
    }

    private void SetNewHost()
    {
        if (Players.Count > 0)
            Players.FirstOrDefault()?.SetAsHost();
        else if (Spectators.Count > 0)
            Spectators.FirstOrDefault()?.SetAsHost();
        else
            Console.WriteLine("No more players to promote");
    }

    public bool CanStart() => Players.Count >= MinPlayers;

    public void StartGame(Connection connection)
    {
        Player? player = GetPlayer(connection);
        // return when player cant be found/ not enouth players/ Game is already ongoing/ player is not host
        if (player == null || !CanStart() || GameActive || !player.IsHost)
            return;

        GameActive = true;
        GameOver = false;

        stockpile.Setup();
        DealCards();

        ActivePlayerIndex = rand.Next(0, Players.Count);

        void DealCards()
        {
            foreach (Player player in Players)
                for (int i = 0; i < CardsPerPlayer; i++)
                    player.AddCard(stockpile.DrawCard()!);
        }
    }

    #endregion

    #region Game logic

    public bool IsPlayersTurn(Connection connection)
    {
        int index = Players.FindIndex(p => p.connection == connection);
        return index != -1 && index == ActivePlayerIndex;
    }
    public bool IsPlayersTurn(Player player)
    {
        int index = Players.FindIndex(p => p == player);
        return index != -1 && index == ActivePlayerIndex;
    }

    public void FishForCard(Connection connection, int targetPlayerIndex, string fishingFor)
    {
        Player? player = GetPlayer(connection);
        if (player == null || !IsPlayersTurn(player))
            return;

        Player targetPlayer = Players[targetPlayerIndex];
        Card? card = targetPlayer.HasCard(fishingFor);

        if (card == null)
            DrawCard(player);
        else
        {
            targetPlayer.RemoveCard(card);
            player.AddCard(card);
        }
    }

    public void DrawCard(Connection connection)
    {
        Player? player = GetPlayer(connection);
        if (player == null)
            return;

        DrawCard(player);
    }
    public void DrawCard(Player player)
    {
        Card? card = stockpile.DrawCard();
        if (card == null)
            return;

        player.AddCard(card);
        NextTurn();
    }

    public void NextTurn()
        => ActivePlayerIndex = (ActivePlayerIndex + 1) % Players.Count;

    public void Surrender()
    {
        // TODO
    }

    public void CheckGameOver()
    {
        // TODO
        // game is over after 
        //      all cards are removed from the stockpile
        //      players cant ask for cards anymore

        GameOver = true;
        GameActive = false;
    }

    #endregion

    #region Game over

    public void ReturnToLobby(Connection connection)
    {
        Player? player = GetPlayer(connection);
        if (player == null || !player.IsHost || GameActive)
            return;

        // TODO
        // Return player to lobby
    }

    # endregion
}