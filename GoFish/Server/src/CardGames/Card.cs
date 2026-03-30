namespace CardGames;

public class Card(string suit, string rank)
{
    public string Suit { get; } = suit;
    public string Rank { get; } = rank;
}