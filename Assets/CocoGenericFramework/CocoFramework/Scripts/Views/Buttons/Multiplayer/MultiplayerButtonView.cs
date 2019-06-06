using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace TabTale {
    
	public class MultiplayerButtonView : MainView 
    {
        [Inject]
        public RequestMultiplayerMatchSignal requestMultiplayerMatchSignal {get; set;}
        
		[Inject]
		public NetworkCheck networkCheck { get; set; }

        public void OnClick()
        {
			if(networkCheck.HasInternetConnection())
			{
				requestMultiplayerMatchSignal.Dispatch();
			}
			else
			{
				networkCheck.ShowNoConnectionPopup();
			}  
        }
    }

    public class RequestMultiplayerMatchSignal : Signal { }
}

