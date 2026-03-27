using Network;

static class Program
{
	static private void Main()
	{
		Start();

		Run();

		Exit();
	}

	static private void Start()
	{
		TcpServer.Start();
	}

	static private void Run()
	{
		while (true)
		{

			TcpServer.CheckPendingSessions();
			SessionManager.CheckMessages();

			if (OnExitPressed())
				break;

			Thread.Sleep(10);
		}
	}

	static private bool OnExitPressed()
	{
		if (Console.KeyAvailable)
		{
			char input = Console.ReadKey(true).KeyChar;
			if (input == 'q')
				return true;
		}
		return false;
	}

	static private void Exit()
	{
		TcpServer.Shutdown();
	}
}
