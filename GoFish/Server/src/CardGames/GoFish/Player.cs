using Network;

namespace CardGames.GoFish;

public class Player(Connection connection, bool isHost = false)
{
    public readonly Connection connection = connection;
    private readonly List<Card> cardsInHand = [];
    private readonly List<Book> booksOwned = [];

    public bool IsHost { get; private set; } = isHost;

    public void SetAsHost() => IsHost = true;
    public int GetBookCount() => booksOwned.Count;
    public List<Card> GetCardsInHand() => cardsInHand;

    public Card? HasCard(string rank)
    {
        foreach (Card card in cardsInHand)
            if (card.Rank == rank)
                return card;
        return null;
    }

    public void RemoveCard(Card card)
       => cardsInHand.Remove(card);

    public void AddCard(Card card)
    {
        cardsInHand.Add(card);
        CheckForBooks();
    }

    private void CheckForBooks()
    {
        var rankGroups = cardsInHand.GroupBy(card => card.Rank);

        foreach (var group in rankGroups)
        {
            if (group.Count() >= 4)
            {
                // Remove the 4 cards
                var cardsToRemove = group.Take(4).ToList();

                foreach (var card in cardsToRemove)
                {
                    cardsInHand.Remove(card);
                }

                // Add book
                booksOwned.Add(new Book(group.Key));

                Console.WriteLine($"Book completed: {group.Key}");
            }
        }
    }
}
