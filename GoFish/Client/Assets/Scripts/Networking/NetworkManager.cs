using UnityEngine;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class NetworkManager
{
	/// <summary>
	/// (static void) Main is the "entry point" for any (Console) program - this is where the program starts.
	/// The string array [args] contains the command line arguments, when the program is started from a terminal window.
	/// </summary>

	public string host = "127.0.0.1";
	public int port = 50001;

	private TcpClient client;
	private NetworkStream stream;
	private Thread receiveThread;

	public event Action<string> OnMessageReceived;
	public event Action OnConnected;
	public event Action<string> OnConnectionFailed;
	public event Action OnDisconnected;

	public async void ConnectAsync()
	{
		try
		{
			client = new TcpClient();
			await client.ConnectAsync(host, port);

			stream = client.GetStream();
		}
		catch (Exception e)
		{
			OnConnectionFailed?.Invoke(e.Message);
			return;
		}

		OnConnected?.Invoke();
		ReceiveLoopAsync();
	}

	public void Send(string message)
	{
		if (client == null || !client.Connected) return;

		byte[] data = Encoding.UTF8.GetBytes(message);
		stream.Write(data, 0, data.Length);
	}

	private async void ReceiveLoopAsync()
	{
		byte[] buffer = new byte[1024];

		try
		{
			while (client != null && client.Connected)
			{
				int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); // async
				if (bytesRead == 0) break;

				string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);

				// Already on Unity thread if awaited correctly
				OnMessageReceived?.Invoke(msg);
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Receive error: " + e.Message);
		}

		Disconnect();
	}

	public void Disconnect()
	{
		stream?.Close();
		client?.Close();
		OnDisconnected?.Invoke();
	}
}
