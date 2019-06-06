using UnityEngine;
using System.Collections;

namespace TabTale
{
    public class NullRealtimeService : MonoService, IRealtimeMultiplayerService
    {
        public void Init() { }

        public void ShowMatchmakerWithMinMaxPlayers( int minPlayers, int maxPlayers )
        {

        }

        public void StartQuickMatch(int minPlayers, int maxPlayers)
        {

        }

        public string SendRawMessageToAllPeers( byte[] bytes, bool reliably )
        {
            return "";
        }

        public void DisconnectFromMatch()
        {

        }

        public bool IsMaster() 
        {
            return true;
        }
        
    }
}