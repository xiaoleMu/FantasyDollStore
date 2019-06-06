using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using RSG;
using strange.extensions.injector.api;
using System.IO;

namespace TabTale
{
	public class DebugPanel : MainView
	{
		[Inject]
		public ServerTime serverTime { get; set; }

		[Inject]
		public NetworkCheck networkCheck { get; set; }

		[Inject]
		public PlayerInfoService playerInfoService { get; set; }

		[Inject]
		public ConfigurationService configurationService { get; set; }

        [Inject]
        public GeneralParameterConfigModel generalParameterConfigModel { get; set; }

		[Inject]
		public SettingsStateModel settingsStateModel { get; set; }

		[Inject]
		public ISocialService socialService { get; set; }

        [Inject]
		public IModalityManager modalityManager { get; set; }

		[Inject]
		public IInjector injector { get; set; }

		[Inject]
		public SocialSync socialSync { get; set; }

		[Inject]
		public PsdkRestartAppSignal restartAppSignal { get; set; }

		public Button skipTime;

		public Text localTime;

		public Text serverAddress;

		public Text playerId;

		public InputField playerIdToSwitch;

		public GameObject detailsView;

		public Text detailsText;

		public Toggle fakeNoNetworkToggle;

		public Toggle noAdsToggle;

        public Image debugViewBackground;

        public ScrollRect scrollRect;

        public GraphicRaycaster graphicRaycaster;

        public Button ClearLogBttn, SendEmailBttn;

		public GameObject AdditionalDebugPanel;

        public Text log;

        System.Text.StringBuilder _debugData = new System.Text.StringBuilder();

        System.Text.StringBuilder _logData = new System.Text.StringBuilder();

        private bool transparency = false;

		private DebugPanelLogger _debugPanelLogger;

		protected override void OnRegister()
		{
			base.OnRegister();

			RefreshUI();

			var se = new InputField.SubmitEvent();
			se.AddListener(OnSubmitNewPlayerId);
			playerIdToSwitch.onEndEdit = se;
            log = detailsView.GetComponentInChildren<Text>();


        }


        private void OnSubmitNewPlayerId(string newPlayerId)
		{
			socialSync.SwitchUser(newPlayerId);
		}

		private void RefreshUI()
		{
			serverAddress.text = "Server:" + configurationService.GetServerUrl();
			fakeNoNetworkToggle.isOn = networkCheck.IsFakingNoConnection();
			noAdsToggle.isOn = !settingsStateModel.ShowingAds();
		}

		public enum SkipTimeType
		{
			Day,
			Hour,
			Minute
		}

		public void OnSkipTime(string skipTimeType)
		{
			SkipTimeType skipType = (SkipTimeType)Enum.Parse(typeof(SkipTimeType), skipTimeType);
			switch(skipType)
			{
			case SkipTimeType.Day:
				serverTime.SkipTimeForDebug += TimeSpan.FromDays(1.0f);
				break;
			case SkipTimeType.Hour:
				serverTime.SkipTimeForDebug += TimeSpan.FromHours(1.0f);
				break;
			case SkipTimeType.Minute:
				serverTime.SkipTimeForDebug += TimeSpan.FromMinutes(1.0f);
				break;
			}

		}

		void Update()
		{
			localTime.text = serverTime.GetLocalTime().ToString();

			playerId.text = playerInfoService.PlayerId;

			if(detailsView.gameObject.activeSelf)
			{
                log.text = _logData.ToString();
            }
		}

		public void ToggleFakeNoConnection()
		{
			networkCheck.SetFakeNoConnection( ! networkCheck.IsFakingNoConnection() );

			RefreshUI();
		}

		public void ToggleNoAds()
		{
			settingsStateModel.SetNoAds(settingsStateModel.ShowingAds());

			RefreshUI();
		}

        #region Test Cases

        public void OnTestLogging()
        {
            _debugPanelLogger = new DebugPanelLogger();

			if(detailsView.gameObject.activeSelf)
			{
				detailsView.gameObject.SetActive(false);
				_debugPanelLogger.OnLogEvent -= UpdateLogView;
				return;
			}

#if UNITY_2017_1_OR_NEWER
			Debug.unityLogger.logHandler = _debugPanelLogger;
#else
			Debug.logger.logHandler = _debugPanelLogger;
#endif

			detailsView.gameObject.SetActive(true);
			_debugPanelLogger.OnLogEvent += UpdateLogView;
		}

		public void OnTestRequests()
		{
			LoadModalWithTestSuite<RequestsTestSuite>();
		}

		public void OnTestEvents()
		{
			LoadModalWithTestSuite<EventsTestSuite>();
		}

		public void OnTestEnergy()
		{
			LoadModalWithTestSuite<EnergyTestSuite>();
		}

		public void OnTestTeams()
		{
			LoadModalWithTestSuite<TeamsTestSuite>();
		}

		public void OnTestPublishing()
		{
			LoadModalWithTestSuite<PublishingTestSuite>();
		}

		public void OnTestSocial()
		{
			LoadModalWithTestSuite<SocialTestSuite>();
		}

		public void OnTestStore()
		{
			LoadModalWithTestSuite<StoreTestSuite>();
		}

		#endregion

		public void UpdateLogView(LogType logType, UnityEngine.Object context, string format, params object[] args)
		{
			//TODO: Replace with scrolling later
			if(_logData.Length > 15000)
			{
                _logData.Length = 14000;
			}

            if(_debugData.Length > 15000)
            {
                _debugData.Length = 14000;
            }

			_debugData.Append(String.Format(format,args));
			_debugData.Append("\n");

            _logData.Append(String.Format(format, args));
            _logData.Append("\n");

            Canvas.ForceUpdateCanvases();
            scrollRect.velocity = new Vector2(0f, 80f);          
            Canvas.ForceUpdateCanvases();

        }

		private void LoadModalWithTestSuite<T>(string modalPrefab = "Debug/TestSuites/DebugTestSuite") where T : TestSuite, new()
		{
			var testSuite = new T();
			injector.Inject(testSuite);

			LoadModal(testSuite, modalPrefab);
		}

        public void ClearLog()
        {
            _debugData.Remove(0, _debugData.Length);
            _logData.Remove(0, _logData.Length);
        }

		/// <summary>
		/// Load a modal without using the modality system, since the dialog canvas of modality changes between games
		/// Can use modality if resolution is adapted
		/// </summary>
		/// <param name="testSuite">Test suite.</param>
		/// <param name="prefabPath">Prefab path.</param>
		private void LoadModal(TestSuite testSuite, string prefabPath)
		{
			GameObject modalGo = Instantiate(Resources.Load(prefabPath, typeof(GameObject))) as GameObject;

			IModalDataReceiver<TestSuite> dataReceiver = modalGo.GetComponentOrInterface<IModalDataReceiver<TestSuite>>();
			if(dataReceiver!=null)
				dataReceiver.SetData(testSuite);

			modalGo.gameObject.transform.parent = gameObject.transform.parent;
			modalGo.gameObject.transform.localScale = new Vector3 (1f, 1f, 1f);
			modalGo.gameObject.transform.localPosition = new Vector3 (0,0,0);

			modalGo.GetComponent<TestSuiteView>().LoadTestCaseViews();
		}

        public void ReportLog()
        {

            //email Id to send the mail to
            string email = generalParameterConfigModel.GetString("GSDKAdminEmail", "gsdk.support@tabtale.com");
            //subject of the mail
            string subject = MyEscapeURL(Application.productName + " - Device log");
            //body of the mail which consists of Device Model and its Operating System
            string body = MyEscapeURL("Please Enter your message here \n\n\n\n" +
             "________" +
             "\n\n Please Do Not Modify This\n\n" +
             "Model: " + SystemInfo.deviceModel + "\n\n" +
                "OS: " + SystemInfo.operatingSystem + "\n\n" +
             "Device log: " + "\n\n"  +
             "------------------" +
             _debugData + "\n\n" +
             "------------------"
             );

            //Open the Default Mail App for any platform
            Debug.Log("DebugPanel: " + "Email sent to " + email + " body: " + _debugData);
            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
            
        }

        string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        public void TransparencyToggle()
        {
           if (transparency)
           {
                transparency = false;
                SendEmailBttn.interactable = true;
                ClearLogBttn.interactable = true;
                scrollRect.GetComponent<Image>().raycastTarget = true;
                debugViewBackground.canvasRenderer.SetAlpha(1);    
           }
           else
           {
                transparency = true;
                SendEmailBttn.interactable = false;
                ClearLogBttn.interactable = false;
                scrollRect.GetComponent<Image>().raycastTarget = false;
                debugViewBackground.canvasRenderer.SetAlpha(0);
           }
        }

		public void ToggleAdditionalDebugPanel()
		{
			if(AdditionalDebugPanel != null)
			{
				AdditionalDebugPanel.SetActive(! AdditionalDebugPanel.activeSelf);
			}
		}

		public void ResetAchievements()
		{
			socialService.ResetAllAchievements();
		}

		public void RestartApp()
		{
			restartAppSignal.Dispatch();
		}
    }
}


