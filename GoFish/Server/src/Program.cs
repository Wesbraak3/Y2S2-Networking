using Network;

static class Server // server
{
	static public readonly NetworkManager networkManager = new();
	static public readonly SessionManager sessionManager = new();

	static public bool IsRunnning {get; private set;} = false;

	static private void Main() =>
		Initialize();

	static private void Initialize()
	{
		IsRunnning = true;
	
		sessionManager.Initialize();
		networkManager.Initialize();

		Run();
	}

	static async private void Run()
	{
		while (true)
		{
			OnExitPressed();
			Thread.Sleep(100);
		}
	}

	static private void OnExitPressed()
	{
		if (Console.KeyAvailable)
		{
			char input = Console.ReadKey(true).KeyChar;
			if (input == 'q')
				Shutdown();
		}
		return;
	}

	static private void Shutdown()
	{
		networkManager.Shutdown();
		sessionManager.Shutdown();
	}
}
