using System.Net;
using OSCTools;

namespace Network;

public class LobbySession : Session
{
    public override void Initialize()
    {
        base.Initialize();

        dispatcher.AddListener("/CreateGoFishGame", SetupGoFish);
        dispatcher.AddListener("/CreateGoFishGame", JoinExisting);
    }

    private void JoinExisting(OSCMessageIn message, IPEndPoint endPoint)
    {
    }

    private void SetupGoFish(OSCMessageIn message, IPEndPoint endPoint)
    {
    }
}