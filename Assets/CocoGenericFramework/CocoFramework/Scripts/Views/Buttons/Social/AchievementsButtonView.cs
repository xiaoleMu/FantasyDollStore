using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace TabTale {
    
	public class AchievementsButtonView : MainView 
    {
        [Inject]
        public RequestShowAchievementsSignal showAchievementsSignal {get; set;}
        
		[Inject]
		public NetworkCheck networkCheck { get; set; }

		[Inject]
		public ISocialService socialService { get; set; }

        [Inject]
        public SoundManager soundManager { get; set; }  

        public void OnClick()
        {
			Debug.Log ("AchievementsButton.OnClick");
            soundManager.PlaySound(SoundMapping.Map.GeneralButtonClick, SoundLayer.Main);

			if(networkCheck.HasInternetConnection())
			{
				if(socialService.IsAuthenticated)
				{
					showAchievementsSignal.Dispatch();
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
    public class RequestShowAchievementsSignal : Signal { }
}

