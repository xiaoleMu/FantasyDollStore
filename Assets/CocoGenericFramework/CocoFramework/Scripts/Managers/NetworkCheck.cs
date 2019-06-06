using UnityEngine;
using System;
using System.Net;
using System.Collections;

namespace TabTale
{
	public class NetworkCheck
	{
		[Inject]
		public IGeneralDialogService genericDialog { get; set; }

		[Inject]
		public GeneralParameterConfigModel generalParameterConfigModel { get; set; }

		[Inject]
		public IRoutineRunner routineRunner { get; set; }

		[Inject]
		public NetworkConnectionChangedSignal networkConnectionChangedSignal { get; set; }

		private bool _isConnected=false;
		private ICoroutineFactory _coroutineFactory;
		private bool _isRunningNetworkCheck = false;
		private float _checkDelayTime = 5f;
		private bool _fakeNoNetworkConnection = false;

		private const string Tag = "NetworkCheck";

		private static string _networkCheckUrl = "https://ping.ttpsdk.info/TabTale-Test";

		public static string NetworkCheckUrl 
		{
			get { return _networkCheckUrl; }
		}

		[PostConstruct]
		public void Init()
		{
			string url = generalParameterConfigModel.GetString("NetworkCheckUrl");
			if(!string.IsNullOrEmpty(url))
			{
				if (Debug.isDebugBuild) Debug.Log("Overriding default network check url with url: " + url);

				_networkCheckUrl = url;
			}
			else
			{
				string paramHeader = "?bundleId=";
				if (!_networkCheckUrl.Contains(paramHeader))
				{
					#if UNITY_5_6_OR_NEWER
					_networkCheckUrl = string.Format("{0}{1}{2}", _networkCheckUrl, paramHeader, Application.identifier);
					#else
					_networkCheckUrl = string.Format("{0}{1}{2}", _networkCheckUrl, paramHeader, Application.bundleIdentifier);
					#endif
				}
			}

			Init (GameApplication.Instance.CoroutineFactory);
		}
		public void Init(ICoroutineFactory coroutineFactory)
		{
			_coroutineFactory = coroutineFactory;

			_coroutineFactory.StartCoroutine(() => TestCoro());
		}


		IEnumerator TestCoro() 
		{
			while(true)
			{
				string antiCacheRandomizer = "?p=" + UnityEngine.Random.Range(1,100000000).ToString();
				WWW www = new WWW(_networkCheckUrl + antiCacheRandomizer);
				yield return www;

				bool internetAvailable = (www.isDone && www.bytesDownloaded > 0);

				if(internetAvailable)
				{
					if( !_isConnected )
						networkConnectionChangedSignal.Dispatch(true);

					_isConnected=true;
				}
				else
				{
					if(_isConnected)
						networkConnectionChangedSignal.Dispatch(false);

					_isConnected=false;
				}

				yield return new WaitForSeconds(_checkDelayTime);
			}
		}

		public bool HasInternetConnection() 
		{
			if( ! _isRunningNetworkCheck )
			{
				_isRunningNetworkCheck = true;
				_coroutineFactory.StartCoroutine(() => TestCoro());
			}

			if(Debug.isDebugBuild && _fakeNoNetworkConnection)
			{
				return false;
			}

			return _isConnected;
		}

		public void ShowNoConnectionPopup()
		{
			ShowNoConnectionPopup("","No internet Connection");
		}

		public void ShowNoConnectionPopup(string title, string message)
		{
			GeneralDialogData data = new GeneralDialogData ();
			string dismissButton = "Ok";
			data.title = title;
			data.hasCloseButton = false;
			data.message = message;
			data.buttons.Add(new BasicDialogButtonData("OK"));
			genericDialog.Show (data);
		}

		public void ShowNoSocialServicePopup()
		{
			if(Application.platform == RuntimePlatform.Android)
				ShowNoConnectionPopup("Not Connected to Google Play","Login to Google Play through device settings");
			else
				ShowNoConnectionPopup("Not connected to Game Center","Login to Game Center through device settings");
        }
        
		public void CheckInternetConnectionAsync(Action<bool> callback) 
		{
			routineRunner.StartCoroutine(CheckInternet(callback));
		}
		
		private IEnumerator CheckInternet(Action<bool> callback) 
		{
			WWW www = new WWW(_networkCheckUrl);
			yield return www;
			
			bool isConnected = false;
			if (www.error==null || www.error=="") {
				isConnected=true;
			}
			
			callback(isConnected);
		}



		public void SetFakeNoConnection(bool fakeNoConnection)
		{
#if UNITY_2017_1_OR_NEWER
			Debug.unityLogger.Log(Tag,"SetFakeNoConnection : " + fakeNoConnection);
#else
			Debug.logger.Log(Tag,"SetFakeNoConnection : " + fakeNoConnection);
#endif

			if(! Debug.isDebugBuild)
			{
#if UNITY_2017_1_OR_NEWER
				Debug.unityLogger.LogError(Tag, "Faking no connection is only allowed in development builds");
#else
				Debug.logger.LogError(Tag, "Faking no connection is only allowed in development builds");
#endif
				return;
			}

			_fakeNoNetworkConnection = fakeNoConnection;
		}

		public bool IsFakingNoConnection()
		{
			return _fakeNoNetworkConnection;
		}

    }
}
