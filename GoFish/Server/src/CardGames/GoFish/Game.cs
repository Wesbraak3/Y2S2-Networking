
namespace CardGames.GoFish;

class Game
{
    private readonly Random rand = new(Guid.NewGuid().GetHashCode());

    private readonly Stockpile stockpile = new();

    private List<Player> players = [];
    private int activePlayerIndex;

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

        stockpile.Setup();

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
        return false;
    }
}