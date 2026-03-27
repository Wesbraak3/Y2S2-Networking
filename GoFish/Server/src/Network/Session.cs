using System.Net.Sockets;

namespace Network;

class Session(TcpClient tcpClient)
{
	private readonly TcpClient client = tcpClient;
	private readonly Socket socket = tcpClient.Client;
	private readonly NetworkStream stream = tcpClient.GetStream();

	public string EndPoint => socket.RemoteEndPoint.ToString();

	public bool IsConnected() => client.Connected;
	public void Close() => client.Close();

	public void ReadMessages()
	{
		if (socket.Available <= 0)
			return;

		int packetLength = socket.Available;
		byte[] data = new byte[packetLength];

		stream.Read(data, 0, packetLength);
		Console.WriteLine($"Received a message of length {packetLength} from {client.Client.RemoteEndPoint} - echoing");

		// For now, we don't do anything special with the incoming message - 
		// just send it straight back to the sender:
		stream.Write(data);
	}
}

