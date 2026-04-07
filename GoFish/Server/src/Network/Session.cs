using OSCTools;

namespace Network;

public abstract class Session
{
	public event Action<Connection>? OnEnterSession;
	public event Action<Connection>? OnLeaveSession;

	public readonly Guid Id = Guid.NewGuid();

	private readonly List<Connection> connections = [];

	// osc
	protected readonly OSCDispatcher dispatcher = new();

	public List<Connection> GetConnections() => [.. connections];

	virtual public void Initialize()
	{
		dispatcher.ShowIncomingMessages = true;
	}

	virtual public void HandleMessage(Connection connection, byte[] message)
	{
		dispatcher.HandleMessage(message, connection.EndPoint);
	}

	public void Broadcast(byte[] message)
	{
		foreach (Connection connection in GetConnections())
			connection.SendMessage(message);
	}

	virtual public void AddConnection(Connection connection)
	{
		connection.ActiveSession = this;
		connections.Add(connection);

		OnEnterSession?.Invoke(connection);
	}

	virtual public void RemoveConnection(Connection connection)
	{
		connection.ActiveSession = null;
		connections.Remove(connection);

		OnLeaveSession?.Invoke(connection);
	}
}