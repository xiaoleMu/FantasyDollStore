using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace TabTale.Plugins.PSDK {

	public class PsdkBannersComponent : MonoBehaviour {

		public bool keepAlive = true;
		public bool showAutomatically = false;

		public UnityEvent OnShown;
		public UnityEvent OnHidden;

		// Use this for initialization
		void Start () {
			if(keepAlive)
				DontDestroyOnLoad(this);
			PsdkEventSystem.Instance.onBannerShownEvent += OnBannersShown;
			PsdkEventSystem.Instance.onBannerHiddenEvent += OnBannersHidden;
			PsdkEventSystem.Instance.onPsdkReady += OnPsdkReady;

			if(PSDKMgr.Instance != null && PSDKMgr.Instance.GetBannersService() != null && showAutomatically){
				PSDKMgr.Instance.GetBannersService().Show();
			}
		}

		void OnDestroy()
		{
			PsdkEventSystem.Instance.onBannerShownEvent -= OnBannersShown;
			PsdkEventSystem.Instance.onBannerHiddenEvent -= OnBannersHidden;
			PsdkEventSystem.Instance.onPsdkReady -= OnPsdkReady;
		}

		// Update is called once per frame
		void Update () {
		
		}

		public void Show()
		{
			if(PSDKMgr.Instance != null && PSDKMgr.Instance.GetBannersService() != null)
				PSDKMgr.Instance.GetBannersService().Show();
		}

		public void Hide()
		{
			if(PSDKMgr.Instance != null && PSDKMgr.Instance.GetBannersService() != null)
				PSDKMgr.Instance.GetBannersService().Hide();
		}

		private void OnBannersShown()
		{
			if(OnShown != null)
				OnShown.Invoke();
		}

		private void OnBannersHidden()
		{
			if(OnHidden != null)
				OnHidden.Invoke();
		}

		private void OnPsdkReady()
		{
			if(PSDKMgr.Instance != null && PSDKMgr.Instance.GetBannersService() != null && showAutomatically){
				PSDKMgr.Instance.GetBannersService().Show();
			}
		}
	}
}