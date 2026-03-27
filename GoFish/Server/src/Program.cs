using Network;

static class Program
{
	static private void Main()
	{
		Start();

		Update();

		Exit();
	}

	static private void Start()
	{
		TcpServer.Run();
	}

	static private void Update()
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
