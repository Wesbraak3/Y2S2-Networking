using System.Net.Sockets;
using System.Net;
using System.Reflection.Metadata;

namespace Network;

class Session(TcpClient tcpClient)
{
	private readonly TcpClient client = tcpClient;
	private readonly Socket socket = tcpClient.Client;
	private readonly NetworkStream stream = tcpClient.GetStream();
	public IPEndPoint EndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint!;

	private Queue<byte[]> messageQue = new();
	private bool _isBusy = false;

	public bool IsConnected() => client.Connected;
	public void Close()
	{
		while (_isBusy)
			Thread.Sleep(10);
		client.Close();
	}

	public void ReadPackets()
	{
		if (!IsConnected())
		{
			Console.WriteLine("NetworkConnection.Update: socket closed by remote");
			return;
		}

		if (socket.Available <= 0)
			return;

		_isBusy = true;

		while (socket.Available > 0)
		{
			if (socket.Available <= 4)
			{
				Console.WriteLine("Packet to small waiting for more");
				break;
			}

			int _packetSize;

			// Read header
			byte[] header = new byte[4];
			stream.Read(header, 0, 4);

			_packetSize = BitConverter.ToInt32(header, 0);
			Console.WriteLine("Incoming packet of length {0}" + _packetSize);

			// Add message to que
			byte[] message = new byte[_packetSize];

			stream.Read(message, 0, _packetSize);
			messageQue.Enqueue(message);
		}

		_isBusy = false;
	}

	public byte[]? GetFirstMessage()
	{
		ReadPackets();

		if (messageQue.Count <= 0)
			return null;

		byte[] first = messageQue.Dequeue();
		return first;
	}

	public Queue<byte[]> GetAllMessages()
	{
		ReadPackets();

		if (messageQue.Count <= 0)
			return new();

		Queue<byte[]> allMessages = new(messageQue);
		messageQue.Clear();

		return allMessages;
	}

	public void Send(byte[] packet)
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
				stream.Write(BitConverter.GetBytes(packet.Length), 0, 4);
				stream.Write(packet, 0, packet.Length);
			}
			else
			{
				Console.WriteLine("Error: cannot send, because cannot write to network stream");
			}
		}
		catch (Exception error)
		{
			Console.WriteLine("NetworkConnection.Send: " + error.Message);
			Close();
		}
	}
}

