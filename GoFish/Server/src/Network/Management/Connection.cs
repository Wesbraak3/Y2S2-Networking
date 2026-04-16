using System.Net;
using System.Net.Sockets;

namespace Network;

public class Connection(TcpClient tcpClient)
{
	private readonly TcpClient client = tcpClient;
	private readonly Socket socket = tcpClient.Client;
	private readonly NetworkStream stream = tcpClient.GetStream();

	public Session? ActiveSession { get; internal set; }

	public IPEndPoint EndPoint { get; private set; } = (IPEndPoint)tcpClient.Client.RemoteEndPoint!;

	// TODO
	// Get player stats after login instead of creating new player
	public string Username { get; private set; } = "";
	public void SetUsername(string username) => Username = username;

	public bool IsConnected() => client.Connected;

	public async Task Run()
	{
		try
		{
			while (IsConnected())
			{
				byte[] message = await ReadMessageAsync();
				ActiveSession?.HandleMessage(this, message);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Connection closed: {ex.Message}");
		}
		finally
		{
			OnDisconnect();
		}
	}

	private async Task ReadPacketsAsync(byte[] buffer, int length)
	{
		int totalRead = 0;

		while (totalRead < length)
		{
			int read = await stream.ReadAsync(buffer, totalRead, length - totalRead);

			if (read == 0)
				throw new Exception("Disconnected");

			totalRead += read;
		}
	}

	private async Task<byte[]> ReadMessageAsync()
	{
		// Read header (4 bytes)
		byte[] header = new byte[4];
		await ReadPacketsAsync(header, 4);

		int packetSize = BitConverter.ToInt32(header, 0);
		Console.WriteLine($"Incoming packet of length {packetSize}");

		// Read body
		byte[] message = new byte[packetSize];
		await ReadPacketsAsync(message, packetSize);

		return message;
	}

	public void SendMessage(byte[] message)
	{
		if (!IsConnected())
		{
			Console.WriteLine("NetworkConnection.Send: skip, since status = " + IsConnected());
			return;
		}

		try
		{
			stream.WriteTimeout = 1;
			if (stream.CanWrite)
			{
				stream.Write(BitConverter.GetBytes(message.Length), 0, 4);
				stream.Write(message, 0, message.Length);
			}
			else
			{
				Console.WriteLine("Error: cannot send, because cannot write to network stream");
			}
		}
		catch (Exception error)
		{
			Console.WriteLine("NetworkConnection.Send: " + error.Message);
			OnDisconnect();
		}
	}

	private void OnDisconnect()
	{
		ActiveSession?.RemoveConnection(this);

		client.Close();
		stream.Close();
	}
}
