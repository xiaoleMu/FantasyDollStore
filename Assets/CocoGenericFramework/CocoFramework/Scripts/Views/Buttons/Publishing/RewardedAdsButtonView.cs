using UnityEngine.UI;
using TabTale.Publishing;

namespace TabTale 
{
    public class RewardedAdsButtonView : MainView
    {
		[Inject]
		public IRewardedAds rewardedAds { get; set; }

		public GameElementType		 	rewardType;
		public string 					key;
		public int 						amount		= 1;

        [Inject]
        public RequestRewardedAdsSignal requestRewardedAdsSignal {get; set;}
        [Inject]
        public SoundManager soundManager { get; set; }

        public bool hideIfNoVideosAvalable = false;

        override protected void OnRegister()
        {
			base.OnRegister();

			if(!rewardedAds.IsAdReady()) {
	            if(hideIfNoVideosAvalable)
				{
	                gameObject.SetActive(false);
				}
				else 
				{
					gameObject.GetComponent<Button>().interactable = false;
				}
			}
        }

        public void OnClick()
        {
            soundManager.PlaySound(SoundMapping.Map.GeneralButtonClick, SoundLayer.Main);

			GameElementData gameElementData = new GameElementData(rewardType, key, amount);
            requestRewardedAdsSignal.Dispatch(gameElementData);
        }


    }

}
