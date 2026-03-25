using System;

public static class EventBus
{
    public static event Action<string> OnMessage;

    public static void Publish(string msg)
    {
        OnMessage?.Invoke(msg);
    }
}