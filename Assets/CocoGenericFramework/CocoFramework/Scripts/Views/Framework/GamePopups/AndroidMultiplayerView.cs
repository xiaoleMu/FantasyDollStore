using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace TabTale {
    
    public class AndroidMultiplayerView : GameView 
    {
        [Inject]
        public RequestMultiplayerQuickMatchSignal multiplayerQuickMatch {get; set;}

        [Inject]
        public RequestMultiplayerInviteSignal multiplayerInvite {get; set;}

        [Inject]
        public RequestMultiplayerWaitingRoomSignal multiplayerWaitForInvite {get; set;}

        public void OnQuickMatchClick()
        {
            multiplayerQuickMatch.Dispatch();
        }

        public void OnInviteClick()
        {
            multiplayerInvite.Dispatch();
        }

        public void OnWaitingForInviteClick()
        {
            multiplayerWaitForInvite.Dispatch();
        }

    }
    
}

public class RequestMultiplayerQuickMatchSignal:Signal
{
    
}

public class RequestMultiplayerInviteSignal:Signal
{
    
}

public class RequestMultiplayerWaitingRoomSignal:Signal
{
    
}