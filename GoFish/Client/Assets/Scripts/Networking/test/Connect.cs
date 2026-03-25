using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkBehaviour : MonoBehaviour
{
    private NetworkManager network;

    async void Start()
    {
        network = new NetworkManager();

        // Subscribe to events BEFORE connecting
        network.OnConnected += () => Debug.Log("Connected!");
        network.OnConnectionFailed += (err) => Debug.LogError("Failed: " + err);
        network.OnDisconnected += () => Debug.Log("Disconnected!");
        network.OnMessageReceived += (msg) => Debug.Log("Received: " + msg);

        network.ConnectAsync();
    }

    void Update()
    {
        // Example: send message when pressing space
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            network.Send("Hello from Unity");
        }
    }

    void OnDestroy()
    {
        network?.Disconnect();
    }
}