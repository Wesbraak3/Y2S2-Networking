using CardGames.GoFish;
using Network;

static class Server
{
	static public readonly NetworkManager networkManager = new();
	static public readonly SessionManager sessionManager = new();

	static public bool IsRunnning { get; private set; } = false;

	static private void Main()
	{
		Initialize();
	}

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
		string[] commands = input.Split();

		switch (commands[0])
		{
			case "q":
			case "quit":
			case "exit":
				Shutdown();
				break;

			case "list":
				ListSessions();
				break;

			case "game":
				if (commands.Length <= 1)
					break;

				PrintGameState(commands);
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

	static private void PrintGameState(string[] commands)
	{
		if (!Guid.TryParse(commands[1], out Guid gameId))
		{
			Console.WriteLine("Invalid game ID");
			return;
		}
		if (sessionManager.GetSession(gameId) is not GoFishSession session)
		{
			Console.WriteLine("Session is not a GoFishSession");
			return;
		}

		Console.WriteLine($"Game ID: {gameId}");
		Console.WriteLine($"Ongoing: {session.GameOngoing()}");

		GameManager gameManager = session.GetGameManager();

		Console.WriteLine($"Players: {gameManager.Players.Count}");
		Console.WriteLine($"Spectators: {gameManager.Spectators.Count}");
		Console.WriteLine();
		Console.WriteLine($"Active player: {gameManager.ActivePlayerIndex}");
		Console.WriteLine();

		if (session.GameOngoing())
		{
			var players = gameManager.Players;

			List<List<string>> columns = [];

			foreach (Player player in players)
			{
				columns.Add(BuildPlayerLines(player));
			}

			// Determine layout
			int maxRows = columns.Max(col => col.Count);
			int colWidth = columns
				.SelectMany(col => col)
				.Max(line => line.Length) + 2;

			// Print row by row
			for (int row = 0; row < maxRows; row++)
			{
				foreach (var col in columns)
				{
					string text = row < col.Count ? col[row] : "";
					Console.Write(text.PadRight(colWidth) + "| ");
				}

				Console.WriteLine();
			}
		}

		static List<string> BuildPlayerLines(Player player)
		{
			List<string> lines = new();

			lines.Add($"Player: {player.connection.Username}");
			lines.Add($"Books: {player.GetBookCount()}");

			var grouped = player.GetCardsInHand()
				.GroupBy(c => c.Rank)
				.OrderBy(g => g.Key);

			foreach (var group in grouped)
			{
				lines.Add($"  {group.Key}: {group.Count()}");
			}

			return lines;
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
