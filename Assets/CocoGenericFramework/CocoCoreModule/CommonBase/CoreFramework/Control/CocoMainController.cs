using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using System.IO;
using TabTale;
using UnityEngine.UI;
using TabTale.Publishing;
using System;
using TabTale.Plugins.PSDK;

#if !COCO_FAKE
using Game;
using GameStartModule = Game.GameStartModule;
using CocoSceneID = Game.CocoSceneID;
#else
using GameStartModule = CocoPlay.Fake.CocoFakeStartModule;
using CocoSceneID = CocoPlay.Fake.CocoSceneID;
using CocoStoreKey = CocoPlay.Fake.CocoStoreKey;
#endif

namespace CocoPlay
{
	#region ABTest

	public enum GPType {
		None,
		Test_A,
		Test_B,
	}

	#endregion

	public class CocoMainController : CocoModuleContainer
	{
		public static CocoMainController Instance;

		[Inject]
		public NetworkCheck networkCheck { get; set; }

		[Inject]
		public CocoGlobalRecordModel globalRecordModel {get; set;}

		#region AB Test

        public Action OnPsdkInit;

		protected override void AddListeners ()
		{
			base.AddListeners ();

			#if ABTEST

//			if (PsdkEventSystem.Instance != null)
//			{
			Debug.LogError ("---------AddListeners--------");
			PsdkEventSystem.Instance.onConfigurationLoaded += OnConfigurationLoaded;
			if (CocoABTestControl.ConfigurationLoaded){
				OnConfigurationLoaded ();
			}
//			}

			#endif
		}

		protected override void RemoveListeners ()
		{
			#if ABTEST
//			if (PsdkEventSystem.Instance != null)
//			{
			Debug.LogError ("---------RemoveListeners--------");
				PsdkEventSystem.Instance.onConfigurationLoaded -= OnConfigurationLoaded;
//			}
			#endif

			base.RemoveListeners ();
		}

		#if ABTEST
		bool haveGetCallBack = false;

		public bool ConfigurationLoaded = false;

		void OnConfigurationLoaded()
		{
//			if (haveGetCallBack)
//				return;
//			haveGetCallBack = true;

			Debug.LogError("---------OnConfigurationLoaded--------");

			string locationkey = PSDKMgr.Instance.GetExternalConfiguration().GetExperimentGroup();

		#if UNITY_EDITOR
			switch (UnityEngine.Random.Range (0, 3)){
			case 0:
				locationkey = "A";
				break;

				case 1:
				locationkey = "B";
				break;

				default:
				locationkey = "N/A";
				break;
			}

//			locationkey = "B";
		#endif

			locationkey = locationkey.ToUpper();
			Debug.LogError("---------locationkey:---" + locationkey);

			switch (locationkey)
			{
			case "A":
				if (!haveGetCallBack)
					globalRecordModel.CurGPType = GPType.Test_A;
				globalRecordModel.CurLocalGPType = GPType.Test_A;
				break;
			case "B":
				if (!haveGetCallBack)
					globalRecordModel.CurGPType = GPType.Test_B;
				globalRecordModel.CurLocalGPType = GPType.Test_B;
				break;
			}

			Debug.LogError ("~~~~~~~~~ CurGPType is : " + globalRecordModel.CurGPType);
//			PsdkEventSystem.Instance.onConfigurationLoaded -= OnConfigurationLoaded;
			if (!haveGetCallBack){
				if (globalRecordModel.CurGPType == GPType.None)
					globalRecordModel.CurGPType = globalRecordModel.CurLocalGPType;
				haveGetCallBack = true;
                if(OnPsdkInit != null)
                    OnPsdkInit();
			}
			ConfigurationLoaded = true;
		}
		#endif

		#endregion

		#region ADs Control

		[SerializeField]
		CocoAdsControl adsControl;

		/// <summary>
		/// Gets the ads control.
		/// </summary>
		/// <value>The ads control.</value>
		static public CocoAdsControl AdsControl {
			get {
				return CocoMainController.Instance.adsControl;
			}
		}

		#endregion

		#region GamePlugin

		[SerializeField]
		CocoPluginManager pluginManager;

		/// <summary>
		/// Gets the plugin manager.
		/// </summary>
		/// <value>The plugin manager.</value>
		static public CocoPluginManager PluginManager {
			get {
				return CocoMainController.Instance.pluginManager;
			}
		}

		#endregion

		#region ShowPopup

		[Inject]
		public IModalityManager popManager { get; set; }

		/// <summary>
		/// Shows the popup.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="maskType">Mask type.</param>
		public static void ShowPopup (string name, IModalMaskType maskType = IModalMaskType.NonMasked, CocoAudioID audioID = CocoAudioID.None)
		{
			CocoAudio.PlaySound (audioID);
			CocoMainController.Instance.popManager.Add (new AppModalHandle (name, maskType), true);
		}

		public static T ShowPopup<T> (string name, IModalMaskType maskType = IModalMaskType.NonMasked, CocoAudioID audioID = CocoAudioID.None)
		{
			CocoAudio.PlaySound (audioID);
			AppModalHandle modalHandle = CocoMainController.Instance.popManager.Add (new AppModalHandle (name, maskType), true) as AppModalHandle;
			return modalHandle.AppModalView.GetComponentInChildren<T>(true);
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="GameMainController"/> is popup showing.
		/// </summary>
		/// <value><c>true</c> if is popup showing; otherwise, <c>false</c>.</value>
		public static bool isPopupShowing {
			get { return CocoMainController.Instance.popManager.IsModalPresent; }
		}
		public void ShowMainStorePopup (Vector3 clickPosition, CocoStoreID storeID){
			#if ABTEST
			switch (globalRecordModel.CurGPType){
			case GPType.Test_A:
				CocoMainStorePopManager.MiniStoreID = storeID;
				ShowPopup (CocoStoreKey.MainStorePopupPath_TypeA);
				FindObjectOfType<CocoMainStorePopManager>().m_ClickButtonPos = clickPosition;
				break;

			case GPType.Test_B:
				CocoMainStorePopManager.MiniStoreID = storeID;
				ShowPopup (CocoStoreKey.MainStorePopupPath_TypeB);
				FindObjectOfType<CocoMainStorePopManager>().m_ClickButtonPos = clickPosition;
				break;

				default:
				CocoMainStorePopManager.MiniStoreID = storeID;
				ShowPopup (CocoStoreKey.MainStorePopupPath);
				FindObjectOfType<CocoMainStorePopManager>().m_ClickButtonPos = clickPosition;
				break;
			}
			#else
//			CocoMainStorePopManager.MiniStoreID = storeID;
//			FindObjectOfType<CocoMainStorePopManager>().m_ClickButtonPos = clickPosition;
			#endif
		}


		#endregion

		#region UIControl

		public static Camera MainCamera {
			get {
				return Camera.main;
			}
		}

		private Camera m_UICamera;

		/// <summary>
		/// Gets or sets the user interface camera.
		/// </summary>
		/// <value>The user interface camera.</value>
		public static Camera UICamera {
			get {
				if (CocoMainController.Instance.m_UICamera == null) {
					CocoMainController.Instance.m_UICamera = GameObject.FindWithTag ("UICamera").GetComponent<Camera> ();
				}
				return CocoMainController.Instance.m_UICamera;
			}
			set {
				CocoMainController.Instance.m_UICamera = value;  
			}
		}

		private GameObject m_CanvasUI;

		/// <summary>
		/// Gets or sets a value indicating canvas_UI.
		/// </summary>
		/// <value><c>true</c> if canvas U; otherwise, <c>false</c>.</value>
		public static GameObject Canvas_UI {
			get {
				if (CocoMainController.Instance.m_CanvasUI == null) {
					CocoMainController.Instance.m_CanvasUI = GameObject.FindWithTag ("Canvas_UI");
				}
				return CocoMainController.Instance.m_CanvasUI;
			}
			set {
				CocoMainController.Instance.m_CanvasUI = value;
			}
		}

		[SerializeField]
		GameObject m_ScreenMask;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GameMainController"/> touch enable.
		/// </summary>
		/// <value><c>true</c> if touch enable; otherwise, <c>false</c>.</value>
		public bool TouchEnable {
			get {
				return !m_ScreenMask.activeSelf;
			}
			set {
				m_ScreenMask.SetActive (!value);
			}
		}

		[SerializeField]
		FingerGestures m_FingerGestures;


        [SerializeField]
        CocoFingerGestureController m_FingerControl;

        static public CocoFingerGestureController FingerControl{
            get{

                return CocoMainController.Instance.m_FingerControl;
            }
        }

		/// <summary>
		/// Sets the finger enable.
		/// </summary>
		/// <param name="enable">If set to <c>true</c> enable.</param>
		public void SetFingerEnable (bool enable)
		{
			m_FingerGestures.enabled = enable;
		}

		/// <summary>
		/// Gets a value indicating whether this instance is user interface touched.
		/// </summary>
		/// <value><c>true</c> if this instance is user interface touched; otherwise, <c>false</c>.</value>
		public bool IsUITouched {
			get {
				return IsPointerOverUIObject(Input.mousePosition);
				if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) {
					return EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId);
				} else {
					return EventSystem.current.IsPointerOverGameObject ();
				}
			}
		}

	public bool IsPointerOverUIObject(Vector2 screenPosition)
	{
		//实例化点击事件
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		//将点击位置的屏幕坐标赋值给点击事件
		eventDataCurrentPosition.position = new Vector2(screenPosition.x, screenPosition.y);

		List<RaycastResult> results = new List<RaycastResult>();
		//向点击处发射射线
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

		return results.Count > 0;
	}

		#endregion

		#region CommonFunction

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start ()
		{
			Instance = this;
			base.Start ();

			init ();

			StartCoroutine (RecodeGameDuration());
			RecodeSessionNumber ();
		}

		private IEnumerator RecodeGameDuration (){
			while(true)
			{
				yield return new WaitForSeconds(60f);
				globalRecordModel.GameDuration ++;
			}
		}

		private void RecodeSessionNumber (){
			globalRecordModel.SessionNumber ++;
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		private void init ()
		{
			InitGlobalModules ();

			if (CocoDebugSettingsData.Instance.IsFPSHudEnabled) {
				CocoHudFPS hudFPS = CocoLoad.GetOrAddComponent<CocoHudFPS> (gameObject);
				hudFPS.startPos = CocoDebugSettingsData.Instance.FPSHudStartPos;
			}
		}

		/// <summary>
		/// Raises the application quit event.
		/// </summary>
		void OnApplicationQuit ()
		{
		}

		bool m_IsGamePaused = false;

		/// <summary>
		/// Raises the application pause event.
		/// </summary>
		/// <param name="isPause">If set to <c>true</c> is pause.</param>
		void OnApplicationPause (bool isPause)
		{
//			if (isPause == m_IsGamePaused) {
//				return;
//			}

			if (isPause) {
				useIceGround = false;
//				PauseGame ();
			} else {
				useIceGround = true;
//				ResumeGame ();
			}
		}

		public static bool useIceGround = true;

		public static CocoSceneSwitchControl sceneSwitchControl;

		/// <summary>
		/// Enters the scene.
		/// </summary>
		/// <param name="ID">I.</param>
		public static void EnterScene (CocoSceneID ID)
		{
			Debug.LogError ("EnterScene is " + ID);

			if (sceneSwitchControl != null)
				return;

			sceneSwitchControl = CocoSceneSwitchControl.Create (ID);
		}

		public static void CleanResources ()
		{
			Instance.StartCoroutine (CocoWait.WaitForFrame (1, () => {
				Resources.UnloadUnusedAssets ();
				System.GC.Collect ();
			}));
		}

		float CurTimeScale = 1;

		/// <summary>
		/// Pauses the game.
		/// </summary>
		public void PauseGame ()
		{
			if (m_IsGamePaused) return;
			m_IsGamePaused = true;
			CurTimeScale = Time.timeScale;
			Time.timeScale = 0;
		}

		/// <summary>
		/// Resumes the game.
		/// </summary>
		public void ResumeGame ()
		{
			m_IsGamePaused = false;
			Time.timeScale = CurTimeScale;
		}

		#endregion


		#region Module Management

		public CocoStartModule StartModule { get; set; }

		void InitGlobalModules ()
		{
			// basic modules
			AddModule<CocoUIBasicModule> (string.Empty);

			// start module
			StartModule = AddModule<CocoStartModule, GameStartModule> ("GameStartModule");

			// start game
			CocoAudio.IsOn = true;
			StartModule.StartGame ();
		}

		void CleanGlobalModules ()
		{
			RemoveModule<CocoUIBasicModule> ();
		}
		#endregion
	}
}