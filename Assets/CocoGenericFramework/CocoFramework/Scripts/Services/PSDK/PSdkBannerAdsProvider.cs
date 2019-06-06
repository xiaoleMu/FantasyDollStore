using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TabTale.Publishing
{
	public class PSdkBannerAdsProvider : IBannerAds
	{
		[Inject]
		public BannerAdsShownSignal bannerAdsShownSignal { get; set; }

		[Inject]
		public BannerAdsHiddenSignal bannerAdsHiddenSignal { get; set; }

		[Inject]
		public BannerAdsWillDisplaySignal bannerAdsWillDisplaySignal { get; set; }

		[Inject]
		public BannerAdsClosedSignal bannerAdsClosedSignal { get; set; }

		private IPsdkServiceManager _psdkMgr;

		private IPromise showBannersPromise = new Promise();
		private IPromise hideBannersPromise = new Promise();

		[PostConstruct]
		public void Init()
		{
			_psdkMgr = PSDKMgr.Instance;
			
			AddListeners();
		}
		
		private void AddListeners()
		{
			PsdkEventSystem.Instance.onBannerShownEvent 		+= () => 
			{  
				showBannersPromise.Dispatch(); 
				showBannersPromise = new Promise();  
				_isShowingBanners = true;

				bannerAdsShownSignal.Dispatch();
			};
			
			PsdkEventSystem.Instance.onBannerCloseEvent			+= () => 
			{  
				bannerAdsClosedSignal.Dispatch();
			};
			PsdkEventSystem.Instance.onBannerHiddenEvent	 	+= () => 
			{  
				hideBannersPromise.Dispatch(); 
				hideBannersPromise = new Promise(); 

				bannerAdsHiddenSignal.Dispatch();
			};

			PsdkEventSystem.Instance.onBannerWillDisplayEvent	+= () => 
			{  
				bannerAdsWillDisplaySignal.Dispatch();
			};
		}

		#region IBannerAds implementation

		private bool _isShowingBanners = false;
		public bool IsShowing { get { return _isShowingBanners; } }

		public IPromise Show ()
		{
			Debug.Log ("PSdkBannerAdsProvider: showing banner ads");
			_psdkMgr.GetBannersService().Show();
			return showBannersPromise;
		}
		public IPromise Hide ()
		{
			Debug.Log ("PSdkBannerAdsProvider: hiding banner ads");
			_psdkMgr.GetBannersService().Hide();
			return hideBannersPromise;
		}
		public bool IsAlignedToTop ()
		{
			return _psdkMgr.GetBannersService().IsAlignedToTop();
		}
		public float GetAdHeight ()
		{
			return _psdkMgr.GetBannersService().GetAdHeight();
		}
		public float GetAdHeightInPercentage()
		{
			// calculate the percentage of the screen that the banner takes up
			float bannerPercent = 0.09f;

			#if UNITY_IPHONE && !UNITY_EDITOR
			float scale = 1.0f;
			if (Screen.dpi > 163.0f) 
			{
				// every device with a dpi over 163 is retina
				scale = 2.0f;
			}
			
			float bannerHeight = _psdkMgr.GetBannersService().GetAdHeight();
			float pointsHeight = Screen.height / scale;
			bannerPercent = bannerHeight / pointsHeight;
			PsdkUtils.NativeLog (" ScreenDPI:  " + Screen.dpi.ToString()  + " Banner Height: " + bannerHeight.ToString() + " Screen Height: " + Screen.height.ToString() +  " Banner Percent:  " + bannerPercent.ToString());
			#elif UNITY_ANDROID
			
			float bannerHeight = _psdkMgr.GetBannersService().GetAdHeight();
			float bannerScale = Screen.dpi / 160.0f;
			bannerPercent = (bannerHeight * bannerScale) / Screen.height;
			
			
			Debug.Log (" ScreenDPI: " + Screen.dpi.ToString() + " BannerScale: " + bannerScale.ToString() + " Banner Height: " + bannerHeight.ToString() + " Screen Height: " + Screen.height.ToString() +  " Banner Percent:  " + bannerPercent.ToString());
			
			#elif UNITY_EDITOR
			bannerPercent =  0.06578948F;
			#endif

			return bannerPercent;
		}

		public bool IsActive() 
		{
			return _psdkMgr.GetBannersService().IsActive();
		}
		#endregion

	}
}