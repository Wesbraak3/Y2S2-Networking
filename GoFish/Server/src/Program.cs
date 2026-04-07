using Network;

static class Server
{
	static public readonly NetworkManager networkManager = new();
	static public readonly SessionManager sessionManager = new();

	static public bool IsRunnning { get; private set; } = false;

	static private void Main() =>
		Initialize();

	static private void Initialize()
	{
		IsRunnning = true;

		InitializeServer();

		Console.WriteLine("types help for console commands");

		Run();
	}

	static private void Run()
	{
		while (IsRunnning)
		{
			string? input = Console.ReadLine();

			if (string.IsNullOrWhiteSpace(input))
				continue;

			HandleCommand(input.Trim().ToLower());
		}
	}

	static private void HandleCommand(string input)
	{
		switch (input)
		{
			case "q":
			case "quit":
			case "exit":
				Shutdown();
				break;

			case "list":
				ListSessions();
				break;

			case "help":
				PrintHelp();
				break;

			case "broadcast":
				sessionManager.Broadcast(System.Text.Encoding.UTF8.GetBytes("Server message"));
				break;

			case "kickall":
				foreach (var session in sessionManager.GetAllSessions())
				{
					foreach (var conn in session.GetConnections())
					{
						conn.SendMessage(System.Text.Encoding.UTF8.GetBytes("You were kicked"));
					}
				}
				break;

			default:
				Console.WriteLine($"Unknown command: {input}");
				break;
		}
	}

	static private void ListSessions()
	{
		Console.WriteLine("Active sessions:");

		foreach (var session in sessionManager.GetAllSessions())
		{
			Console.WriteLine(
				$"{session.GetType().Name} ({session.Id}) - Connections: {session.GetConnections().Count}"
			);
		}
	}

	static private void PrintHelp()
	{
		Console.WriteLine("Available commands:");
		Console.WriteLine("  help       - show commands");
		Console.WriteLine("  list       - list sessions");
		Console.WriteLine("  broadcast       - tell everyone something");
		Console.WriteLine("  kickall       - Kick everyone");
		Console.WriteLine("  quit / q   - stop server");
	}

	static private void Shutdown()
	{
		IsRunnning = false;

		ShutdownServer();
	}

	static private void InitializeServer(int port = 50011)
	{
		sessionManager.Initialize();
		networkManager.Initialize(port);
	}
	static private void ShutdownServer()
	{
		networkManager.Shutdown();
		sessionManager.Shutdown();
	}
}
