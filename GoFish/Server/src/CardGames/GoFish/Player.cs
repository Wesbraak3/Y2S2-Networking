namespace CardGames.GoFish;

public class Player
{
    private List<Card> cardsInHand = [];
    private List<Book> booksOwned = [];

    public bool FishForCard(Player targetPlayer, string fishingFor)
    {
        return false;
    }

    public bool HasCard(string rank)
    {
        foreach (Card card in cardsInHand)
        {
            if (card.Rank == rank)
            {

            }
        }
        return false;
    }

    public void AddCard(Card card)
    {
        cardsInHand.Add(card);

        TryAddBook();
    }

    private void TryAddBook()
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
