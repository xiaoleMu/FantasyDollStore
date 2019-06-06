using UnityEngine;
using System.Collections;

namespace TabTale  
{
	public interface IRealtimeMultiplayerService : IService
	{
        void Init();

        // Shows the matchmaker UI.  minPlayers and maxPlayers must be between 2 and 4.
        void ShowMatchmakerWithMinMaxPlayers( int minPlayers, int maxPlayers );

        // Performs quick match making with no UI shown
        void StartQuickMatch(int minPlayers, int maxPlayers);

        // Sends a raw byte array message to all connected devices
        string SendRawMessageToAllPeers( byte[] bytes, bool reliably );

        // Disconnects from the current match.
        void DisconnectFromMatch();

        bool IsMaster();
	}
}