using Network;

static class Program
{
	static private void Main()
	{
		Initialize();

		Run();

		Exit();
	}

	static private void Initialize()
	{
		TCPServer.Initialize();
	}

	static async private void Run()
	{
		while (true)
		{
			OnExitPressed();
			Thread.Sleep(10);
		}
	}

	static private void OnExitPressed()
	{
		if (Console.KeyAvailable)
		{
			char input = Console.ReadKey(true).KeyChar;
			if (input == 'q')
				Exit();
		}
		return;
	}

	static private void Exit()
	{
		TCPServer.Shutdown();
	}
}
