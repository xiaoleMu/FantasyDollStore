using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace TabTale {
    
	public class LeaderboardButtonView : MainView 
    {
        [Inject]
        public RequestShowLeaderboardSignal showLeaderboardSignal { get; set; }
        
		[Inject]
		public NetworkCheck networkCheck { get; set; }

        [Inject]
        public SoundManager soundManager { get; set; }

		[Inject]
		public ISocialService socialService { get; set; }

        public void OnClick()
        {
			Debug.Log ("LeaderBoardButton.OnClick");
            soundManager.PlaySound(SoundMapping.Map.GeneralButtonClick, SoundLayer.Main);

			if(networkCheck.HasInternetConnection())
			{
				Debug.Log ("PassedConnectionCheck");
				if(socialService.IsAuthenticated)
				{
					Debug.Log ("LeaderBoardButton.IsAuthenticated");
					showLeaderboardSignal.Dispatch();
				}
				else
				{
					socialService.Authenticate();

					if(Application.platform == RuntimePlatform.IPhonePlayer)
						networkCheck.ShowNoSocialServicePopup();
				}
			}
			else
			{
				networkCheck.ShowNoConnectionPopup();
			}
        }
    }
    public class RequestShowLeaderboardSignal : Signal
    {

    }
}


