using UnityEngine;
using System.Net.Sockets;
using System.Net;
using OSCTools;
using TMPro;
using System;

public enum Protocol { TCP, UDP, TCPConnection, UDPConnection};

public class NetworkManager : MonoBehaviour
{
	[SerializeField]
	TMP_InputField IPInput;
	[SerializeField]
	TMP_InputField PortInput;
	[SerializeField]
	TMP_InputField LocalPortInput;
	[SerializeField]
	TextMeshProUGUI LocalIPText;

	UdpClient udpClient = null;
	TcpClient tcpClient = null;
	IPAddress localIP;
	Protocol protocol = Protocol.UDP;

	Logger logger;

    void Start()
    {
		logger = FindFirstObjectByType<Logger>();
		SetLocalPort();
		SetLocalIP();
    }

	// See https://stackoverflow.com/questions/6803073/get-local-ip-address
	//   -> It's more complicated than you would expect!
	void SetLocalIP() {
		localIP = null;
		using (UdpClient client = new UdpClient()) {
			client.Connect(IPAddress.Parse("8.8.8.8"), 65530); // Data doesn't really matter (this is Google's DNS server btw)
			localIP = ((IPEndPoint)client.Client.LocalEndPoint).Address;
			client.Close(); // Not needed with "using", but still added for clarity
		}
		LocalIPText.text = localIP.ToString();
	}

	public IPAddress GetLocalIP() {
		return localIP;
	}

	// Called by protocol DropDown:
	public void SetProtocol(int option) {
		protocol = (Protocol)option; // Ensure the dropdown list matches!
		Debug.Log("Protocol selected: " + option + " protocol: "+protocol);
		CreateClient(protocol);
	}

	// Called by remote IP / port InputFields:
	public void SetRemote() {
		if (protocol==Protocol.TCP || protocol==Protocol.TCPConnection) {
			CreateClient(protocol);
		}
	}
	// Called by local port InputField:
	public void SetLocalPort() {
		if (protocol==Protocol.UDP || protocol == Protocol.UDPConnection) {
			CreateClient(protocol);
		}
	}

	// Note: localPort is only used for UDP; it's ignored for TCP:
	public void CreateClient(Protocol proto) {
		if (udpClient != null) {
			Log("Closing UDP client");
			udpClient.Dispose();
			udpClient = null;
		}
		if (tcpClient != null) {
			Log("Closing TCP client");
			tcpClient.Dispose();
			tcpClient = null;
		}
		if (proto == Protocol.UDP || proto == Protocol.UDPConnection) {
			int localPort = GetLocalPort();
			if (localPort >= 0 && localPort < (1 << 16)) {
				udpClient = new UdpClient(localPort);
				Log("Starting UDP client on port " + localPort);
			}
		} else {
			tcpClient = new TcpClient(); // any port - local port field is ignored
			IPEndPoint target = GetRemote();
			tcpClient.Connect(target);
			Log("Starting TCP client connected to " + target);
		}
	}

	public int GetLocalPort() {
		if (int.TryParse(LocalPortInput.text, out int port)) {
			return port;
		} else {
			return 40000;
		}
	}

	IPEndPoint GetRemote() {
		IPAddress remote;
		if (!IPAddress.TryParse(IPInput.text, out remote)) {
			remote = IPAddress.Loopback; // 127.0.0.1 ; localhost
		}
		int port;
		if (!int.TryParse(PortInput.text, out port)) {
			port = 50000;
		}
		IPEndPoint target = new IPEndPoint(remote, port);
		return target;
	}

	public void SendMessage(OSCMessageOut message) {
		IPEndPoint target = GetRemote();

		Log($"Sending packet to {target}:      \t{message}");

		byte[] packet = message.GetBytes();
		switch (protocol) {
			case Protocol.UDP:
				udpClient.Send(packet, packet.Length, target);
				break;
			case Protocol.UDPConnection:
				// We're not really using our custom RUDP here,
				// just adding a compatible header:
				byte[] headerPacket = new byte[packet.Length + 3];
				// header = 0,0,0 (= unreliable packet):
				Array.Copy(packet, 0, headerPacket, 3, packet.Length);
				udpClient.Send(headerPacket, headerPacket.Length, target);
				break;
			case Protocol.TCP:
				tcpClient.GetStream().Write(packet);
				break;
			case Protocol.TCPConnection:
				var stream = tcpClient.GetStream();
				stream.Write(BitConverter.GetBytes(packet.Length));
				stream.Write(packet);
				break;
		}
	}

    void Update()
    {
        if (protocol == Protocol.UDP || protocol == Protocol.UDPConnection) {
			if (udpClient.Available > 0) {
				IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
				byte[] packet = udpClient.Receive(ref remote);
				if (protocol==Protocol.UDPConnection) {
					// Not really doing custom RUDP; ignoring the header:
					// (This doesn't work well when acks are expected!)
					byte[] payload = new byte[packet.Length - 3];
					Array.Copy(packet, 3, payload, 0, payload.Length);
					packet = payload;
				}
				// bundles are not supported:
				OSCMessageIn mess = new OSCMessageIn(packet);
				HandleMessage(mess, remote);
			}
		} else {
			if (tcpClient.Available>0) {
				var stream = tcpClient.GetStream();
				byte[] packet;
				if (protocol==Protocol.TCP) {
					packet = new byte[tcpClient.Available];
					stream.Read(packet);
				} else {
					// Not truly implementing TcpConnection here - 
					//  A malicious remote client can cause this to block!
					//  Bad network connections can cause this to fail! (Not reading entire packet)
					packet = new byte[4];
					stream.Read(packet);
					int numBytes = BitConverter.ToInt32(packet);
					packet = new byte[numBytes];
					stream.Read(packet);
				}
				OSCMessageIn mess = new OSCMessageIn(packet);
				HandleMessage(mess, (IPEndPoint)tcpClient.Client.RemoteEndPoint);
			}
		}
    }

	void HandleMessage(OSCMessageIn mess, IPEndPoint remote) {
		if (!mess.corrupt) {
			Log($"Received packet from {remote}: \t{mess}");
			if (mess.header == "/Ping") {
				Pong(remote);
			}
		} else {
			Log("Received non-OSC message from {remote}");
		}
	}

	void Pong(IPEndPoint sender) {
		OSCMessageOut pong = new OSCMessageOut("/Pong");
		byte[] packet = pong.GetBytes();
		udpClient.Send(packet, packet.Length, sender);
		Log("    Sending /Pong");
	}

	void Log(string text) {
		Debug.Log(text);
		logger.AddText(text);
	}
}
