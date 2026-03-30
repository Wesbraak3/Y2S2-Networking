
namespace CardGames;

public static class PackOfCards
{
    static private readonly List<string> suits = ["Spades", "Clubs", "Diamonts", "Hearts"];
    static private readonly List<string> ranks = ["2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A"];

    static private List<Card> Deck => Initialize();

    static private List<Card> Initialize()
    {
        List<Card> newDeck = [];

        foreach (string suit in suits)
        {
            foreach (string rank in ranks)
            {
                newDeck.Add(new(suit, rank));
            }
        }

        return newDeck;
    }

    static public List<Card> Get() => Deck;
}