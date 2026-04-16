namespace CardGames.GoFish;

public class Stockpile
{
    private readonly Random rand = new(Guid.NewGuid().GetHashCode());
    private readonly List<Card> stockpile = [];

    public void Setup()
    {
        stockpile.Clear();
        stockpile.AddRange(PackOfCards.Get());

        Shuffle();
    }
    public void Shuffle()
    {
        for (int i = stockpile.Count - 1; i >= 0; i--)
        {
            int r = rand.Next(0, i);
            (stockpile[i], stockpile[r]) = (stockpile[r], stockpile[i]);
        }
        for (int i = stockpile.Count - 1; i >= 0; i--)
        {
            int r = rand.Next(0, i);
            (stockpile[i], stockpile[r]) = (stockpile[r], stockpile[i]);
        }
        for (int i = stockpile.Count - 1; i >= 0; i--)
        {
            int r = rand.Next(0, i);
            (stockpile[i], stockpile[r]) = (stockpile[r], stockpile[i]);
        }
    }
    public Card? DrawCard()
    {
        if (stockpile.Count <= 0)
        {
            return null;
        }

        Card drawnCard = stockpile[0];
        stockpile.RemoveAt(0);

        return drawnCard;
    }
}