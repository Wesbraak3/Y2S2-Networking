

namespace GoFish;

static class GameManager
{
    private readonly List<GoFish> ongoingGames = [];

    public void InstaniateNewInstance()
    {
        
    }
}

public class GoFish
{
    /*
    Game settings
        int player count between 2 and 8
        int SetsNeededToWin between 1 and 13
            (default is player count / x)
            if no more cards or someone cant draw game ends
        int Turn timout
        bool Must ask if able 
        int amountOfDecks
    
    CardsInDeck
    private List<Player> players = []
    Player ActivePlayer;
    */

    StandardDeck deck = new();

    public void StartGame()
    {
        
    }

    public bool AskPlayer()
    {
        
    }
}

public class StandardDeck(int amountOfDecks)
{
    private int suits = 4;
    private int ranks = 13;

    List<Card> deck = Initialize(amountOfDecks);

    private List<Card> Initialize(int amountOfDecks)
    {
        
        Shuffle();
    }

    public void Shuffle()
    {
        
    }

    public void DrawCard()
    {
        
    }
}

public class Player
{
    /*
    List<Card> cardsInHand
    SetsMade
        Card x
    */
    
    private List<Card> cardsInHand = [];
    private List<Set> setsOwned = [];

    public void AddSet(int rank)
    {
        setsOwned.Add(new(rank));
    }
}

public class Card(int suit, int rank)
{
    // 0 -> 3 (clubs hearts spades diamonds)
    private int suit = suit;
    private int rank = rank;
}

public class Set(int rank)
{
    private int rank = rank;
}

/*
{
    rank : player
    rank : player
}
*/