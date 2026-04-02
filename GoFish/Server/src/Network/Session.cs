using System.Net.Sockets;
using System.Net;

namespace Network;

class Session(string sessionName, int sessionKey)
{
	public string SessionName {get; private set;}= sessionName;
	public int SessionKey {get; private set; } = sessionKey;
	public List<Connection> Connections {get; private set;} = [];

	public void AddConnection()
	{
		Connections.Add(new())
	}
}

