using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;
using System.Runtime.InteropServices;
using AOT;
using System.Reflection;

namespace TabTale.Plugins.PSDK {
	/// <summary>
	/// Psdk splash listner service.
	/// </summary>
	public class PsdkEventSystem : PsdkSingleton<PsdkEventSystem>  {

		
		#region public
		
		public static readonly string MUSIC_PAUSE_CALLER_LOC_MGR = "locMgr";
		public static readonly string MUSIC_PAUSE_CALLER_SPLASH = "splash";
		public static readonly string MUSIC_PAUSE_CALLER_REWARDED_ADS = "rewardedAds";
		
		// Resume event
		public event System.Action<AppLifeCycleResumeState> onResumeEvent;

		//PurchaseValidation
		// Parameters: price, currency, productId, validation response
		public event System.Action<string, string, string, bool> onValidPurchaseResponseEvent;

		//Startup
		public event System.Action onConfigurationReady;
		public event System.Action onPsdkReady;
		
		//AppShelf
		public event System.Action<AudioClip> onPlaySoundEvent;
		public event System.Action onStartAnimationEndedEvent;
		
		//Banners
		public event System.Action onBannerShownEvent;
		public event System.Action onBannerFailedEvent;
		public event System.Action onBannerConfigurationUpdateEvent;
		public event System.Action onBannerWillDisplayEvent;
		public event System.Action onBannerCloseEvent;
		public event System.Action onBannerHiddenEvent;
		
		//LocationMgr
		public event System.Action<bool> 			pauseGameMusicEvent;
		public event System.Action<string, long> 	onLocationLoadedEvent;
		public event System.Action<string, string> 	onLocationFailedEvent;
		public event System.Action<string, long> 	onShownEvent;
		public event System.Action<string, long> 	onShownFailedEvent;
		public event System.Action<string, long> 	onClosedEvent;
		public event System.Action 					onConfigurationLoadedEvent;
		
		//RewardedAds
		public event System.Action onRewardedAdIsReadyEvent;
		public event System.Action onRewardedAdIsNotReadyEvent;
		public event System.Action onRewardedAdWillShowEvent;
		//		public event System.Action onRewardedAdDidCloseEvent;
		//		public event System.Action onRewardedAdShouldRewardEvent;
		//		public event System.Action onRewardedAdShouldNotRewardEvent;
		public event System.Action<bool> onRewardedAdDidClosedWithResultEvent;
		
		//Splash
		public event System.Action onSplashAdded;
		public event System.Action onSplashRemoved;
		
		
		//Configuration
		public event System.Action onConfigurationLoaded;


		//InAppDelegate
		public delegate string GetStringAction();
		public event GetStringAction onGetSceneName;

		public delegate int GetIntAction(string currencyId);
		public event GetIntAction onGetCurrencyBalance;

		public delegate bool IsReadyForStoreSaleHandler(string productId);
		public event IsReadyForStoreSaleHandler onIsReadyForStoreSale;

		public delegate bool IsReadyForVCSaleHandler(string itemId, int amount, string currencyId);
		public event IsReadyForVCSaleHandler onIsReadyForVCSale;

		public delegate string GetPriceStringHandler(string productId);
		public event GetPriceStringHandler onGetPriceString;

		public event System.Action<string> onStorePurchase;
		public event System.Action<string,int,string> onVCPurchase;

		private Dictionary<string,bool> _musicPauseDic;

		public delegate int OnGetPlayerScoreDelegate();
		public event OnGetPlayerScoreDelegate OnGetPlayerScore;

		//Social
		public event System.Action<bool>  onSocialAuthenticate;
		public event System.Action onSocialSignOut; //TO-DO make sure event fires only in android.


		//Billing
		public event System.Action<PurchaseIAPResult> onBillingPurchased;
		public event System.Action<BillerErrors> onBillingInit;
		public event System.Action<bool> onBillingPurchaseRestored;

		//Android Permissions Wrapper
		public event System.Action<string[],bool[]> OnRequestPermissionsResultEvent;

		//ICloud
		public event System.Action<string[], CDPReason> OnCDPStoreChangeEvent;
		public event System.Action<string[]> OnCDPUpdateEvent;

		//Analytics
		public event System.Action<string,Dictionary<string,object>> OnRequestEngagementCompleteEvent;

		//Game Level Data
		public event System.Action OnGLDReady;

		public event System.Action<ConsentFormType> OnRemoteConsentModeReadyEvent;

        //Called when app is supposed to block all touch since PSDK needs to raise a view above it. bool value is true if touch should be blocked, false if it should be unblocked.
        public event System.Action<bool> OnBlockTouchesEvent;

		public string Name() {
			return this.name;
		}
		
		public void Init() {
			// Do nothing, exist only for instantiation command by calling class
		}
		
		#endregion
		
		void Start() {
			LocationMgrStart();
		}
		
		// AppShelf
		private static string _appShelfAudioClipSourcePath;
		private static AudioClip _ac;
		
		
		
		
		// block outside instantiation
		protected PsdkEventSystem () {}

		//GSDK Event workarounds (to keep all events in the PsdkEventSystem singelton
		internal void NotifyOnSocialAuthenticate(bool result)
		{
			if(onSocialAuthenticate != null)
				onSocialAuthenticate(result);
		}

		internal void NotifyOnBillingInit(BillerErrors result)
		{
			if(onBillingInit != null)
				onBillingInit(result);
		}

		internal void NotifyOnBillingPurchased(PurchaseIAPResult result)
		{
			if(onBillingPurchased != null)
				onBillingPurchased(result);
		}

		internal void NotifyOnBillingPurchaseRestored(bool result)
		{
			if(onBillingPurchaseRestored != null)
				onBillingPurchaseRestored(result);
		}

		public void ExecCoroutine(IEnumerator routine)
		{
			StartCoroutine(routine);
		}
		
		#region Startup listener
		
		void OnConfigurationReady() {
			if (null != onConfigurationReady)
				onConfigurationReady();
			if (OnGLDReady != null)
				OnGLDReady();
		}
		
		public void OnPSDKReady() {
			if (null != onPsdkReady)
				onPsdkReady();
		}

		void OnRemoteConsentModeReady(string message)
		{
			Debug.Log("PsdkEventSystem::OnRemoteConsentModeReady jsonMessage: " + message);
			ConsentFormType consentFormType = ConsentFormType.NONE;
			IDictionary<string, object> paramsDict = DeserializedJsonMessage(message);
			if(paramsDict != null){
				string consentFormTypeStr = (string)paramsDict["consentFormType"];
				if(consentFormTypeStr.Equals("NONE", System.StringComparison.InvariantCultureIgnoreCase)){
					consentFormType = ConsentFormType.NONE;
				}
				else if(consentFormTypeStr.Equals("ANY", System.StringComparison.InvariantCultureIgnoreCase)){
					consentFormType = ConsentFormType.ANY;
				}
				else if(consentFormTypeStr.Equals("NO_PURCHASE", System.StringComparison.InvariantCultureIgnoreCase)){
					consentFormType = ConsentFormType.NO_PURCHASE;
				}
			}
			if(OnRemoteConsentModeReadyEvent != null){
				OnRemoteConsentModeReadyEvent(consentFormType);
			}
			else {
				Debug.Log("PsdkEventSystem::OnRemoteConsentModeReadyEvent is null");
			}

		}
		#endregion

		#region ICloud

		void OnCDPStoreChange(string message)
		{
			if(OnCDPStoreChangeEvent != null){
				IDictionary<string, object> paramsDict = DeserializedJsonMessage (message);
				if(paramsDict != null){
					string keysChangedStr = (string)paramsDict["keysChanged"];
					long reasonInt = (long)paramsDict["reason"];
					string[] keysChanged = keysChangedStr.Split(';');
					OnCDPStoreChangeEvent(keysChanged, ConvertCDPReason(reasonInt));
				}
			}
			else {
				PSDKMgr.Instance.GetCrossDevicePersistency().OnStoreChangeResponse(CDPResponse.INTERNAL_PSDK);
			}
		}

		void OnCDPUpdate(string message)
		{
			if(OnCDPUpdateEvent != null){
				IDictionary<string, object> paramsDict = DeserializedJsonMessage (message);
				if(paramsDict != null){
					string keysChangedStr = (string)paramsDict["keysChanged"];
					string[] keysChanged = keysChangedStr.Split(';');
					OnCDPUpdateEvent(keysChanged);
				}
			}
		}

		private CDPReason ConvertCDPReason(long i)
		{
			if(i == 0){
				return CDPReason.SERVER_CHANGE;
			} else if (i == 1) {
				return CDPReason.ACCOUNT_CHANGE;
			}
			return CDPReason.SERVER_CHANGE;
		}

		#endregion

		#region Android Permissions Wrapper

		void OnPermissionsRequestResult(string message)
		{
			Debug.Log("PsdkEventSystem::OnPermissionsRequestResult jsonMessage: " + message);
			string[] messageArr = message.Split (';');
			string[] permissions = new string[messageArr.Length/2];
			bool[] granted = new bool[messageArr.Length / 2];

			int i = 0;
			int j = 0;
			foreach (string str in messageArr){
				if(i % 2 == 0){
					permissions [j] = str;
				}
				else {
					bool isGranted = false;
					bool.TryParse (messageArr [i], out isGranted);
					granted [j] = isGranted;
					j++;
				}
				i++;
			}
			AndroidPermissionsWrapper.Instance.PassOnRequestPermissionsResult(permissions,granted);
			if(OnRequestPermissionsResultEvent != null)
				OnRequestPermissionsResultEvent (permissions, granted);
		}

		#endregion

		public int CallGetPlayerScore()
		{
			if (OnGetPlayerScore != null) {
				return OnGetPlayerScore ();
			} else {
				return -1;
			}
		}

		#region Purchase Validator


		void OnValidateResponse(string jsonMessage)
		{
			Debug.Log("PsdkEventSystem::OnValidateResponse jsonMessage: " + jsonMessage);
           IDictionary<string, object> paramsDict = DeserializedJsonMessage (jsonMessage);

           string price = "0";
           string currency = "USD"; 
           string productId = ""; 
           bool valid = false;
           if (paramsDict != null) {
                   object value;
                   if (paramsDict.TryGetValue("price", out value))
                               price = value as string;
                   if (paramsDict.TryGetValue("currency", out value))
                               currency = value as string;
                   if (paramsDict.TryGetValue("productId", out value))
                               productId = value as string;
                               
				if (paramsDict.TryGetValue ("valid", out value)) {
					if (value is bool) {
						valid = (bool)value;

						if (valid) {
							IPsdkAnalytics analyticsService = PSDKMgr.Instance.GetAnalyticsService ();
							if (analyticsService == null) {
								Debug.LogError("PSDKAnalytics is not initialized - will not send event.");	
							}
							else{
								IDictionary<string,object> item = analyticsService.generateItem(1, productId, productId);
								IDictionary<string,object>[] items = new IDictionary<string,object>[]{ item };
								IDictionary<string,object> rCurrency = analyticsService.generateRealCurrency(price, currency);
								IDictionary<string,object> pSpent = analyticsService.generateProductsSpent(items, rCurrency, null);

								analyticsService.LogTransaction("Store Purchase", null, pSpent, null);
							}
						}
					} else {
						UnityEngine.Debug.LogError ("OnValidateResponse:: Casting valid from to bool failed ! productId:" + productId);
					}
				}
			}
			if(onValidPurchaseResponseEvent != null){
				onValidPurchaseResponseEvent (price, currency, productId, valid);
			}
		}

		#endregion

		#region Configuration listener
		
		void OnConfigurationLoaded()
		{
			if (null != onConfigurationLoaded){
				onConfigurationLoaded ();
			}
		}
		
		#endregion
		
		
		#region AppShelf listener
		void onPlaySound(string sourceFilePath) {
			
			if (_appShelfAudioClipSourcePath != null && sourceFilePath == _appShelfAudioClipSourcePath && _ac != null) {
				if (null != onPlaySoundEvent)
					onPlaySoundEvent(_ac);
				return;
			}
			StartCoroutine(downloadAppShelfAudioClipAndPlay(sourceFilePath));
		}
		
		IEnumerator downloadAppShelfAudioClipAndPlay(string sourceFilePath) {
			WWW wwwaudioclip = new WWW("file://" + sourceFilePath);
			yield return wwwaudioclip;
			if (string.IsNullOrEmpty(wwwaudioclip.error)) {
				_ac = wwwaudioclip.GetAudioClip(false);
				if (_ac != null) {
					_appShelfAudioClipSourcePath  = sourceFilePath;
					if (null != onPlaySoundEvent)
						onPlaySoundEvent(_ac);
				}
			}
		}
		
		
		void onStartAnimationEnded() {
			if (null != onStartAnimationEndedEvent)
				onStartAnimationEndedEvent();
		}
		#endregion
		
		#region Banners listener
		void onBannerShown() {
			if (null != onBannerShownEvent)
				onBannerShownEvent();
		}
		
		void onBannerFailed() {
			Debug.Log ("PsdkEventSystem: Banners failed !!");
			if (null != onBannerFailedEvent)
				onBannerFailedEvent();
		}
		
		void onBannerConfigurationUpdate() {
			if (null != onBannerConfigurationUpdateEvent)
				onBannerConfigurationUpdateEvent();
		}	
		
		void onBannerWillDisplay() {
			if (null != onBannerWillDisplayEvent)
				onBannerWillDisplayEvent();
		}	
		
		void onBannerClose() {
			if (null != onBannerCloseEvent)
				onBannerCloseEvent();
		}	
		
		void onBannerHidden() {
			if (null != onBannerHiddenEvent)
				onBannerHiddenEvent();
		}	
		
		#endregion

		#region Analytics listener

		void OnRequestEngagementComplete (string jsonMessage)
		{
			if(OnRequestEngagementCompleteEvent != null){
				IDictionary<string, object> messageDict = DeserializedJsonMessage (jsonMessage);
				if(messageDict != null){
					string decsionPoint = (string)messageDict["decisionPoint"];
					Dictionary<string,object> parameters = (Dictionary<string,object>)messageDict["parameters"];
					OnRequestEngagementCompleteEvent(decsionPoint,parameters);
				}
			}


		}


		#endregion

		#region LocationMgr listener
		void LocationMgrStart() {
		}
		
		public void SetAnalyticsDelegate(IPsdkAnalytics analyticsService) {
			_analytics = analyticsService;
		}
		
		
		
		private static float _resumeTime = 0f;
		private static float _pausedTime = 0f;
		
		private IPsdkAnalytics _analytics;
		
		
		void OnLocationLoaded(string jsonMessage) {
			if (PSDKMgr.Instance.DebugMode)
				Debug.Log("PsdkEventSystem : UnityLocationMgrDelegate::OnLocationLoaded " + jsonMessage);
			
			string location = "ErrorLocation";
			long attributes = 0L;
			IDictionary<string, object> paramsDict = DeserializedJsonMessage (jsonMessage);
			if (paramsDict != null) {
				object value;
				if (paramsDict.TryGetValue("location", out value))
					location = value as string;
				if (paramsDict.ContainsKey("attributes")) {
					attributes = (long)paramsDict["attributes"];
				}
			}
			if (null != onLocationLoadedEvent)
				onLocationLoadedEvent(location, attributes);
		}
		
		IDictionary<string, object> DeserializedJsonMessage(string jsonMessage) {
			IDictionary<string, object> paramsDict = null;
			try {
				paramsDict = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
			}
			catch (System.Exception e) {
				UnityEngine.Debug.LogException(e);
			}
			if (paramsDict == null) {
				Debug.LogError("Didn't manage to convert from json string " + jsonMessage);
			}
			return paramsDict;
		}
		
		void OnLocationFailed(string jsonMessage) {
			if (PSDKMgr.Instance.DebugMode)
				Debug.Log("PsdkEventSystem : UnityLocationMgrDelegate::OnLocationFailed " + jsonMessage);
			
			IDictionary<string, object> paramsDict = DeserializedJsonMessage (jsonMessage);
			
			string location = "";
			string psdkError = "";
			
			
			if (paramsDict != null) {
				object value;
				if (paramsDict.TryGetValue("location", out value))
					location = value as string;
				if (paramsDict.TryGetValue("psdkError", out value))
					psdkError = value as string;
			}
			
			if (onLocationFailedEvent != null)
				onLocationFailedEvent(location, psdkError);
		}
		
		public void PauseGameMusicEventNotification(long attributes, bool pause, string caller) {

			if (PSDKMgr.Instance.GetLocationManagerService () != null) {
				if (PSDKMgr.Instance.GetLocationManagerService ().IsViewVisible ()) {
					return;
				}
			}

			if(_musicPauseDic == null){
				_musicPauseDic = new Dictionary<string,bool> ();
			}

			_musicPauseDic[caller] = pause;

			bool allUnpaused = true;
			foreach(KeyValuePair<string,bool> kvPair in _musicPauseDic){
				if(PSDKMgr.Instance.DebugMode){
					Debug.Log ("Current music pause dictionary entry :: " + kvPair.Key + ", " + kvPair.Value);
				}
				if(kvPair.Value){
					allUnpaused = false;
				}
				
			}


			bool shouldCallEvent = pause || (!pause && allUnpaused);


			if (shouldCallEvent && (attributes & LocationMgrAttributes.LOCATION_MGR_ATTR_PLAYING_MUSIC) > 0) {
				if (null != pauseGameMusicEvent )
					pauseGameMusicEvent (pause);
			}
		}
		
		void OnShown(string jsonMessage) {
			
			if (PSDKMgr.Instance.DebugMode)
				Debug.Log("PsdkEventSystem : UnityLocationMgrDelegate::OnShown " + jsonMessage);
			
			string location = "ErrorLocation";
			long attributes = 0L;
			
			IDictionary<string, object> paramsDict = DeserializedJsonMessage (jsonMessage);
			if (paramsDict != null) {
				object value;
				if (paramsDict.TryGetValue("location", out value))
					location = value as string;
				if (paramsDict.ContainsKey("attributes")) {
					attributes = (long)paramsDict["attributes"];
				}
			}
			
			BlockTouches();
			
			if (null != onShownEvent)
				onShownEvent(location, attributes);
		}
		
		
		
		void OnShownFailed(string jsonMessage) {

			if (PSDKMgr.Instance.DebugMode)
				Debug.Log("PsdkEventSystem : UnityLocationMgrDelegate::OnShownFailed " + jsonMessage);
			
			string location = "ErrorLocation";
			long attributes = 0L;
			
			IDictionary<string, object> paramsDict = DeserializedJsonMessage (jsonMessage);
			if (paramsDict != null) {
				object value;
				if (paramsDict.TryGetValue("location", out value))
					location = value as string;
				if (paramsDict.ContainsKey("attributes")) {
					attributes = (long)paramsDict["attributes"];
				}
			}
			
			PauseGameMusicEventNotification (attributes, false, MUSIC_PAUSE_CALLER_LOC_MGR);
			
			if (null != onShownFailedEvent)
				onShownFailedEvent(location, attributes);
		}
		
		
		
		void OnClosed(string jsonMessage) {

			if (PSDKMgr.Instance.DebugMode)
				Debug.Log("PsdkEventSystem : UnityLocationMgrDelegate::OnClosed " + jsonMessage);
			
			string location = "ErrorLocation";
			long attributes = 0L;
			
			IDictionary<string, object> paramsDict = DeserializedJsonMessage (jsonMessage);
			if (paramsDict != null) {
				object value;
				if (paramsDict.TryGetValue("location", out value))
					location = value as string;
				if (paramsDict.ContainsKey("attributes")) {
					attributes = (long)paramsDict["attributes"];
				}
			}
			
			UnblockTouches();
			
			PauseGameMusicEventNotification (attributes, false, MUSIC_PAUSE_CALLER_LOC_MGR);
			
			if (null != onClosedEvent)
				onClosedEvent(location, attributes);
		}
		
		
		void OnLocMgrConfigurationLoaded() {
			if (PSDKMgr.Instance.DebugMode)
				Debug.Log("PsdkEventSystem : UnityLocationMgrDelegate::OnConfigurationLoaded ");
			
			if (null != onConfigurationLoadedEvent)
				onConfigurationLoadedEvent();
		}
		
		void HandleLocationMgrOnApplicationPause(bool paused) {
			if (paused) {
				_pausedTime = Time.realtimeSinceStartup;
			}
			else // resumed
			{
				if (( Time.realtimeSinceStartup - _pausedTime) >= 5) {
					_resumeTime = Time.realtimeSinceStartup;
				}
			}
			
		}
		
		void OnApplicationPause(bool paused) {
			HandleLocationMgrOnApplicationPause(paused);
			//HandleAdColonyMissingDidCloseEventWhenComingFromBackgroud (paused); //TODO:IPIPIP test if not needed anymore
			
			
			#if UNITY_EDITOR
			// Otherwise done by native activity/controller
			if (PsdkEventSystem.Instance != null) {
				
				if (! PSDKMgr.Instance.NativePsdkStarted)
					return;
				
				if (paused) {
					onPaused();
				}
				else {
					onResumed();                        }
			}
			#endif
		}
		
		
		
		
		#region block_touch_collider
		Dictionary<Camera,int> _cameraEventMasks = new Dictionary<Camera,int>();
		List<Component> _addedCanvasGroupsComponenets = new List<Component>();
		List<Component> _existedCanvasGroupsComponenets = new List<Component>();
		
		public event System.Action<bool> onBlockTouches;	
		protected bool _touchesBlocked = false; 
		
		
		// Handle new scene load behind visible location
		
		void Update() {
			BlockTouchesIfNeeded ();
		}
		
		
		int _numOfCameras = -1;
		int _firstCameraId = -1;
		int _cid;
		int _prevLevelIndex = -1;
		
		int _levelWasLoaded = 0;
		
		protected void BlockTouchesIfNeeded() {
			// When new scene loaded while a location in visible
			// Check if new scene loaded
			
			if (_touchesBlocked) {
				
				// Making sure in the new level 2 frames, after pplication.isLoadingLevel
				if (_levelWasLoaded >= 2) {
					_levelWasLoaded = 0;
					_numOfCameras = Camera.allCameras.Length;
					if (_numOfCameras > 0)
						_firstCameraId = Camera.allCameras [_numOfCameras-1].GetInstanceID();
					BlockTouches();
				}
				else { 
					if (_levelWasLoaded > 0)
						_levelWasLoaded++;
				}
				
				if (Application.isLoadingLevel) {
					_levelWasLoaded++;
				}
				
				if (_prevLevelIndex != Application.loadedLevel) {
					_prevLevelIndex = Application.loadedLevel;
					_numOfCameras = Camera.allCameras.Length;
					if (_numOfCameras > 0)
						_firstCameraId = Camera.allCameras [_numOfCameras-1].GetInstanceID();
					BlockTouches ();
					return;
				}
				Camera[] cameras = Camera.allCameras;
				if (cameras.Length != _numOfCameras) {
					_numOfCameras = cameras.Length;
					if (_numOfCameras > 0)
						_firstCameraId = cameras [_numOfCameras-1].GetInstanceID();
					BlockTouches ();
				} else {
					if (cameras.Length > 0) {
						_cid = cameras [cameras.Length-1].GetInstanceID ();
						if (_firstCameraId != _cid) {
							_firstCameraId = _cid;
							BlockTouches ();
						}
					}
				}
			}
		}
		
		protected void BlockTouches() {

            if(OnBlockTouchesEvent != null){
                OnBlockTouchesEvent(true);
            }

			try {
				_touchesBlocked = true;
				
				if (onBlockTouches != null)
					onBlockTouches(true);
				
				Camera[] cameras = Camera.allCameras;
				
				
				
				foreach (Camera camera in cameras) {
					
					bool cameraInList = false;
					foreach (KeyValuePair<Camera,int> kv in _cameraEventMasks)
					{
						if (kv.Key == camera)
						{
							cameraInList = true;
							break;
						}
					}
					if (cameraInList) {
						camera.eventMask = 0;
						continue;
					}
					
					_cameraEventMasks.Add(camera,camera.eventMask);
					camera.eventMask = 0;
					
				}
				
				Canvas[] canvases = GameObject.FindObjectsOfType<Canvas> ();
				foreach (Canvas canvas in canvases) {
					CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
					if (cg != null) {
						if (cg.interactable)
							if (! _existedCanvasGroupsComponenets.Contains(cg))
								_existedCanvasGroupsComponenets.Add(cg);
					}
					else {
						cg = canvas.gameObject.AddComponent<CanvasGroup>();
						_addedCanvasGroupsComponenets.Add(cg);
					}
					cg.interactable = false;
					cg.blocksRaycasts = false;
				}
			} catch (System.Exception e) {
				Debug.LogException(e);
			}
			
		}
		
		protected void UnblockTouches() {
			
            if (OnBlockTouchesEvent != null)
            {
                OnBlockTouchesEvent(false);
            }

			_touchesBlocked = false;
			
			if (onBlockTouches != null)
				onBlockTouches(false);
			
			// unblock touches 
			foreach (CanvasGroup cg in _existedCanvasGroupsComponenets) {
				try {
					if (cg == null) continue;
					cg.interactable = true;
					cg.blocksRaycasts = true;
				} catch (System.Exception) {}
			}
			_existedCanvasGroupsComponenets.Clear ();
			
			foreach (CanvasGroup cg in _addedCanvasGroupsComponenets) {
				try {
					if (cg == null) continue;
					cg.interactable = true;
					cg.blocksRaycasts = true;
					GameObject.Destroy(cg);
				} catch (System.Exception) {}
			}
			_addedCanvasGroupsComponenets.Clear ();
			
			foreach (KeyValuePair<Camera, int> cg in _cameraEventMasks) {
				try {
					if (cg.Key == null) continue;
					cg.Key.eventMask = cg.Value;
				} catch (System.Exception e) {
					Debug.LogException(e);
				}
			}
			_cameraEventMasks.Clear ();
			
		}
		#endregion 
		
		#endregion
		
		#region RewardedAds listener
		
		// getting called from iOS/Android for initializing the analytics with the given ksy
		void OnRewardedAdIsReady() {
			if (onRewardedAdIsReadyEvent != null) 
				onRewardedAdIsReadyEvent();
		}
		
		void OnRewardedAdIsNotReady() {
			if (onRewardedAdIsNotReadyEvent != null) 
				onRewardedAdIsNotReadyEvent();
		}
		
		private bool rwClosed = false;
		private bool rwGotResult = false;
		private bool rewardedResult = false;
		private bool rwDidShow = false;
		
		// Register synchronious callback from ios
		static void RewardedAdsSynchroniusCallBacksRegistration() {
			RegisterNativeCallback("OnRewardedAdWillShow",OnRewardedAdWillShowStaticCallback);
		}
		
		// being called synchroniously from ios rewarded ads delegate (through p/inkoke callback registration)
		static void OnRewardedAdWillShowStaticCallback(string param) {
			if (Instance != null) 
				Instance.OnRewardedAdWillShow ();
		}
		
		
		
		void OnRewardedAdWillShow() {
			Debug.Log ("PsdkEventSystem::OnRewardedAdWillShow.");
			rwDidShow = true;
			rwClosed = false;
			rwGotResult = false;
			rewardedResult = false;
			
			
			if (onRewardedAdWillShowEvent != null) 
				onRewardedAdWillShowEvent();
		}
		
		void OnRewardedAdDidClose() {
			Debug.Log ("PsdkEventSystem::OnRewardedAdDidClose.");
			rwClosed = true;
			PauseGameMusicEventNotification (LocationMgrAttributes.LOCATION_MGR_ATTR_PLAYING_MUSIC, false, MUSIC_PAUSE_CALLER_REWARDED_ADS);
			
			OnRewardedAdDidCloseWithResult();
		}
		
		void OnRewardedAdShouldReward() {
			Debug.Log ("PsdkEventSystem::OnRewardedAdShouldReward.");
			rwGotResult = true;
			rewardedResult = true;
			
			OnRewardedAdDidCloseWithResult();
		}
		
		void OnRewardedAdShouldNotReward() {
			Debug.Log ("PsdkEventSystem::OnRewardedAdShouldNotReward.");
			rwGotResult = true;
			rewardedResult = false;
			
			OnRewardedAdDidCloseWithResult();
		}
		
		void OnRewardedAdDidCloseWithResult() {
			
			if (! rwClosed ){
				Debug.Log ("PsdkEventSystem::OnRewardedAdDidCloseWithResult did not get close yet.");
				return;
			}
			if (! rwGotResult){
				Debug.Log ("PsdkEventSystem::OnRewardedAdDidCloseWithResult did not get result yet.");
				return;
			}
			
			Debug.Log ("PsdkEventSystem::OnRewardedAdDidCloseWithResult got result and close.");
			if (onRewardedAdDidClosedWithResultEvent != null){
				Debug.Log ("PsdkEventSystem::OnRewardedAdDidCloseWithResult sent onRewardedAdDidClosedWithResultEvent");
				onRewardedAdDidClosedWithResultEvent(rewardedResult);
			}
			
			rwClosed = false;
			rwGotResult = false;
			rewardedResult = false;
			rwDidShow = false;
		}
		
		void HandleAdColonyMissingDidCloseEventWhenComingFromBackgroud(bool paused) {
			if (paused)
				return;
			#if UNITY_ANDROID
			StartCoroutine(HandleAdColonyMissingDidCloseEventWhenComingFromBackgroudCoro(paused));
			#endif
		}
		
		
		IEnumerator HandleAdColonyMissingDidCloseEventWhenComingFromBackgroudCoro(bool paused) {
			// Handle the bug, when coming from background and AdColony didCloseEvent didn't arrive
			
			// Waiting unity to get all messages from native
			for (int i=0; i < 7; i++)
				yield return new WaitForEndOfFrame ();
			
			if (! rwDidShow) 
				yield break;
			
			if (rwClosed && rwGotResult)
				yield break;
			
			if (! rwGotResult)
				OnRewardedAdShouldNotReward ();
			
			if (! rwClosed)
				OnRewardedAdDidClose ();
			
		}
		
		#endregion

		//Splash
		#region Splash listener
		
		void OnSplashAdded() {
			BlockTouches();
			if (null != onSplashAdded)
				onSplashAdded();
			
			PauseGameMusicEventNotification (LocationMgrAttributes.LOCATION_MGR_ATTR_PLAYING_MUSIC, true, MUSIC_PAUSE_CALLER_SPLASH);
		}
		
		void OnSplashRemoved() {
			UnblockTouches();
			if (null != onSplashRemoved)
				onSplashRemoved();
			
			PauseGameMusicEventNotification (LocationMgrAttributes.LOCATION_MGR_ATTR_PLAYING_MUSIC, false, MUSIC_PAUSE_CALLER_SPLASH);
		}
		#endregion
		
		
		
		
		
		
		void OnResumeEvent(AppLifeCycleResumeState resumeState) {
			Debug.Log("PSDKEventSystem: OnResumeEvent - " + resumeState);
			if (onResumeEvent != null)
				StartCoroutine(OnResumeCoroutine(resumeState));
		}
		
		private IEnumerator OnResumeCoroutine(AppLifeCycleResumeState resumeState)
		{
			yield return new WaitForEndOfFrame();
			onResumeEvent(resumeState);
		}
		
		protected override void Awake() {
			base.Awake();
		}
		
		protected override void OnDestroy() {
			if (PSDKMgr.Instance != null) {
			}
			base.OnDestroy();
		}
		
		#region events from native UnityPlayerNativeActivity/UnityAppController
		
		// Common Internal, not called from outside
		public void onPaused() {
			// used for ios, currently by passes by android
			if (PSDKMgr.Instance.GetAppLifecycleManager() != null)
				PSDKMgr.Instance.GetAppLifecycleManager().OnPaused();
		}
		
		public void onResumed() {
			if (PSDKMgr.Instance.GetAppLifecycleManager () != null) {
				AppLifeCycleResumeState or = PSDKMgr.Instance.GetAppLifecycleManager ().OnResume();
				Debug.Log ("psdkService resume with state:" + or.ToString());
				if (or != AppLifeCycleResumeState.ALCRS_NONE) {
					if (onResumeEvent != null)
						onResumeEvent(or);
				}
			}
		}
		
		void onStop() {
			if (PSDKMgr.Instance.GetAppLifecycleManager() != null)
				PSDKMgr.Instance.GetAppLifecycleManager().OnStop();
		}
		
		void onStart() {
			if (PSDKMgr.Instance.GetAppLifecycleManager() != null)
				PSDKMgr.Instance.GetAppLifecycleManager().OnStart();
		}
		
		
		// ios
		void didFinishLaunchingWithOptions() {
			Debug.Log ("NATIVE CALL didFinishLaunchingWithOptions");
		}
		
		static void applicationDidEnterBackgroundStaticCallback(string param) {
			if (Instance != null) 
				Instance.applicationDidEnterBackground ();
		}
		
		
		void applicationDidEnterBackground() {
			Debug.Log ("NATIVE CALL applicationDidEnterBackground");
			onPaused ();
		}
		
		void applicationWillEnterForeground(){
			Debug.Log ("NATIVE CALL applicationWillEnterForeground");
		}
		
		void applicationDidFinishLaunching() {
			Debug.Log ("NATIVE CALL applicationDidFinishLaunching");
		}
		
		void willFinishLaunchingWithOptions() {
			Debug.Log ("NATIVE CALL willFinishLaunchingWithOptions");
		}
		
		
		void applicationDidBecomeActive() {
			Debug.Log ("NATIVE CALL applicationDidBecomeActive");
			onResumed ();
		}
		
		static void applicationWillResignActiveStaticCallback(string param) {
			if (Instance != null) 
				Instance.applicationWillResignActive ();
		}
		
		void applicationWillResignActive() {
			Debug.Log ("NATIVE CALL applicationWillResignActive");
			if (PSDKMgr.Instance.GetAppLifecycleManager() != null)
				PSDKMgr.Instance.GetAppLifecycleManager().ApplicationWillResignActive();
		}
		
		
		
		// android
		void OnNativeAndroidStart() {
			Debug.Log ("NATIVE CALL OnNativeAndroidStart");
			onStart ();
		}
		
		void OnNativeAndroidStop() {
			Debug.Log ("NATIVE CALL OnNativeAndroidStop");
			onStop();
		}
		
		void OnNativeAndroidDestroy() {
			Debug.Log ("NATIVE CALL OnNativeAndroidDestroy");
			OnDestroy ();
		}
		
		void OnNativeAndroidBackPressed() {
			Debug.Log ("NATIVE CALL OnNativeAndroidBackPressed");
		}
		
		void OnNativeAndroidResume() {
			Debug.Log ("NATIVE CALL OnNativeAndroidResume");
			onResumed ();
		}
		
		//		void OnNativeAndroidPause() {
		//			// Currently not being called, by passed directly to AppLifeCycleManagr onPause by the ttunity.jar
		//			Debug.Log ("NATIVE CALL OnNativeAndroidPaused");
		//			onPaused ();
		//		}
		
		public static void RegisterNativeCallbacksAfterPsdkInitialization() {
			// the following must be called after PSDK setup for not getting call, before appLifeCycle initialization.
			RegisterSendDirectMessageToUnityCallback(PsdkEventCallbackFromNative);
			RegisterSendDirectMessageToUnityCallbackWithReturn(PsdkEventCallbackFromNativeWithReturn);
			RegisterNativeCallback("applicationWillResignActive",applicationWillResignActiveStaticCallback);
			RegisterNativeCallback("applicationDidEnterBackground",applicationDidEnterBackgroundStaticCallback);
			RewardedAdsSynchroniusCallBacksRegistration ();
		}
		
		static IDictionary<string, IList<System.Action<string> > > _nativeEventsDict = new Dictionary<string, IList<System.Action<string> >>();
		public static void RegisterNativeCallback(string message, System.Action<string> cb) {
			if (! _nativeEventsDict.ContainsKey(message)) {
				_nativeEventsDict.Add(message, new List<System.Action<string>>());
			}
			if (! _nativeEventsDict[message].Contains(cb)) {
				_nativeEventsDict[message].Add(cb);
			}
		}
		public static void UnregisterNativeCallback(string message, System.Action<string> cb) {
			if (! _nativeEventsDict.ContainsKey(message)) {
				return;
			}
			if (_nativeEventsDict[message].Contains(cb)) {
				_nativeEventsDict[message].Remove(cb);
			}
		}
		#region blocking (sync) native call from applicationWillResignActive
		//#if UNITY_IOS	
		// Handling Application will resign active, blocking call the call from native
		delegate void PsdkEventSystemCallbackFromNative_callback_t (string message, string param);
		delegate string PsdkEventSystemCallbackFromNativeWithReturn_callback_t (string message, string param);


		#if UNITY_IOS && ! UNITY_EDITOR
		[DllImport ("__Internal")]
		extern static void RegisterSendDirectMessageToUnityCallback (PsdkEventSystemCallbackFromNative_callback_t cb);
		[DllImport ("__Internal")]
		extern static void RegisterSendDirectMessageToUnityCallbackWithReturn (PsdkEventSystemCallbackFromNativeWithReturn_callback_t cb);
		#else
		static void RegisterSendDirectMessageToUnityCallback (PsdkEventSystemCallbackFromNative_callback_t cb) 
		{ // Stub for Editor, Registration for Android
			#if UNITY_ANDROID && ! UNITY_EDITOR
			using (AndroidJavaClass unityUtils = new AndroidJavaClass("com.tabtale.publishingsdk.unity.UnityUtils")) {
				if (unityUtils == null) {
					UnityEngine.Debug.LogError("Didn't find com.tabtale.publishingsdk.unity.UnityUtils class !!");
					return;
				}
				// This will trigger UnitySendEvent messages from android psdk waiting queue.
				unityUtils.CallStatic("registerPsdkEventSystemGameObjectListener","PsdkEventSystem");
			}
			#endif
		}
		static void RegisterSendDirectMessageToUnityCallbackWithReturn (PsdkEventSystemCallbackFromNativeWithReturn_callback_t cb)
		{
			//stub
		}
		#endif
		
		[MonoPInvokeCallback(typeof(PsdkEventSystemCallbackFromNative_callback_t))]
		static void PsdkEventCallbackFromNative(string message, string param) {
			
			if (param == null) 
				param = "";
			
			if (message == null) { 
				Debug.LogError ("P/Invoke PsdkEventCallbackFromNative( NULL ," + param +")");
				return;
			}
			Debug.Log ("P/Invoke PsdkEventCallbackFromNative(" + message + "," + param +")");
			bool cbCalled = false;
			if (_nativeEventsDict.ContainsKey (message)) {
				foreach(System.Action<string> cb in _nativeEventsDict[message]) {
					cb(param);
					cbCalled = true;
				}
			}
			if (! cbCalled) 
				PsdkEventSystem.Instance.gameObject.SendMessage (message, param, SendMessageOptions.DontRequireReceiver);
		}

		[MonoPInvokeCallback(typeof(PsdkEventSystemCallbackFromNativeWithReturn_callback_t))]
		static string PsdkEventCallbackFromNativeWithReturn(string methodName, string message) {
			try{
				Debug.Log("PsdkEventCallbackFromNativeWithReturn:: " + methodName + ", " + message + ". Instance = " + Instance);
			}
			catch (System.Exception e) {
				Debug.LogException(e);
			}
			try{
				if(message != null){
					MethodInfo mi = Instance.GetType ().GetMethod (methodName,  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					if(mi != null){
						object retObj = mi.Invoke (Instance,message.Length > 0 ? new object[]{ message } : null);
						if(retObj != null && retObj.GetType() == typeof(string)){
							return (string)retObj;
						}
						else {
							Debug.Log("PsdkEventCallbackFromNativeWithReturn:: " + methodName + " - return object is null or not string.");
						}
					}
					else {
						Debug.Log("PsdkEventCallbackFromNativeWithReturn:: " + methodName + " - not found.");
					}
				}
			}
			catch (System.Exception e) {
				Debug.LogException(e);
			}
			return null;
		}


		//#endif
		#endregion
		
		
		#endregion


		string OnGetSceneName()
		{
			if(onGetSceneName != null){
				string result = onGetSceneName();
				if(result != null){
					IDictionary<string, object> paramsDict = new Dictionary<string,object>();
					paramsDict.Add("sceneName",result);
					return Json.Serialize(paramsDict);
				}
			}
			return null;
		}

		string OnGetPriceString(string jsonMessage)
		{
			if(onGetPriceString != null){
				IDictionary<string, object> paramsDict = DeserializedJsonMessage (jsonMessage);
				if(paramsDict != null){
					object productId = null;
					if(paramsDict.TryGetValue("productId", out productId) && productId.GetType() == typeof(string)){
						string result = onGetPriceString((string)productId);
						if(result != null){
							IDictionary<string, object> resultDict = new Dictionary<string,object>();
							resultDict.Add("priceString",result);
							return Json.Serialize(resultDict);
						}
					}
				}
			}
			return null;
		}

		string OnGetCurrencyBalance(string jsonMessage)
		{
			if(onGetCurrencyBalance != null){
				IDictionary<string, object> paramsDict = DeserializedJsonMessage (jsonMessage);
				if(paramsDict != null){
					object currencyId = null;
					if (paramsDict.TryGetValue ("currencyId", out currencyId) && currencyId.GetType () == typeof(string)) {
						string result = "" + onGetCurrencyBalance((string)currencyId);
						if(result != null){
							IDictionary<string, object> resParamsDict = new Dictionary<string,object>();
							resParamsDict.Add("currencyBalance",result);
							return Json.Serialize(resParamsDict);
						}
					}
				}
			}
			return null;
		}

		string OnIsReadyForSale(string jsonMessage)
		{
			try{
				if(onIsReadyForStoreSale != null){
					string retStr = null;
					IDictionary<string, object> paramsDict = DeserializedJsonMessage (jsonMessage);
					if(paramsDict.Count == 1){
						object productId = null;
						if(paramsDict.TryGetValue("productId",out productId) && productId.GetType() == typeof(string)){
							if (onIsReadyForStoreSale != null) {
								if (onIsReadyForStoreSale ((string)productId)) {
									retStr = "True";
								}
								else {
									retStr = "False";
								}
							} 
							else {
								retStr = "False";
							}
						}
						else {
							Debug.Log("PsdkEventSystem:: OnIsReadyForSale, could not parse productId for store sale.");
						}
					}
					else {
						object itemId, currencyId, amountStr = null;
						int amount = -1;
						if(paramsDict.TryGetValue("itemId", out itemId) && itemId.GetType() == typeof(string) &&
							paramsDict.TryGetValue("currencyId",out currencyId) && currencyId.GetType() == typeof(string) &&
							paramsDict.TryGetValue("amount", out amountStr) && amountStr.GetType() == typeof(string) &&
							int.TryParse((string)amountStr, out amount)){
							if (onIsReadyForVCSale != null) {
								if (onIsReadyForVCSale((string)itemId,amount,(string)currencyId)) {
									retStr = "True";
								}
								else {
									retStr = "False";
								}
							} 
							else {
								retStr = "False";
							}
						}
						else {
							Debug.Log("PsdkEventSystem:: OnIsReadyForSale, could not parse itemId, amount or currencyId for vc sale.");
						}
					}
					if(retStr != null){
						IDictionary<string, object> resultDict = new Dictionary<string,object>();
						resultDict.Add("result",retStr);
						return Json.Serialize(resultDict);
					}
					Debug.Log("OnIsReadyForSale:: internal error: failed to parse action - " + jsonMessage);
				}
			}
			catch (System.Exception e) {
				Debug.LogException(e);
			}
			return null;
		}

		void OnPurchase(string jsonMessage)
		{
			IDictionary<string, object> paramsDict = DeserializedJsonMessage (jsonMessage);
			if(paramsDict != null){
				if(paramsDict.Count == 1){
					if(onStorePurchase != null){
						object productId = null;
						if(paramsDict.TryGetValue("productId",out productId) && productId.GetType() == typeof(string)){
							onStorePurchase((string)productId);
						}
					}
				}
				else {
					if(onVCPurchase != null){
						object itemId, currencyId, amountStr = null;
						int amount = -1;
						if(paramsDict.TryGetValue("itemId", out itemId) && itemId.GetType() == typeof(string) &&
							paramsDict.TryGetValue("currencyId",out currencyId) && currencyId.GetType() == typeof(string) &&
							paramsDict.TryGetValue("amount", out amountStr) && amountStr.GetType() == typeof(string) &&
							int.TryParse((string)amountStr, out amount)){
							onVCPurchase((string)itemId,amount,(string)currencyId);
						}
					}
				}
			}
			else {
				Debug.Log("OnPurchase:: internal error: failed to parse action - " + jsonMessage);	
			}


		}

		public void CallSocialSignOutEvent()
		{
			if(onSocialSignOut != null)
				onSocialSignOut();
		}
	}
		
}
