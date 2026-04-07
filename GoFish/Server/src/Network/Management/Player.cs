namespace Network;

public class Player
{
    public string Name { get; private set; }

    public void SetName(string name)
    {
        Name = name;
    }
}