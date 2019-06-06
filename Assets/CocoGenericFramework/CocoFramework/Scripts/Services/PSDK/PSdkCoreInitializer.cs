using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;
using TabTale.Analytics;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TabTale {

public class PSdkCoreInitializer : IPsdkCoreInitializer 
{
		[Inject]
		public PsdkSessionStartSignal psdkSessionStartSignal { get; set; }

		[Inject]
		public PsdkRestartAppSignal psdkRestartAppSignal { get; set; }

		[Inject]
		public FirstSceneLoadedSignal firstSceneLoadedSignal { get; set; }

		private IPromise initializedPromise = new Promise();

		private bool _firstSceneLoaded = false;
		public PSdkCoreInitializer() { }

		public IPromise Init() 
		{ 
			firstSceneLoadedSignal.AddOnce( _ => { _firstSceneLoaded = true; });

			ListenToPsdkEvents ();

			// Don't init psdk on Standalone builds (not supported)
			if(Application.platform.IsStandalone())
			{
				initializedPromise.Dispatch();
			}
			else
			{
				InitPsdkCore ();
			}

			return initializedPromise; 
		}

		#region PSDK service setup and initialization 
		private void ListenToPsdkEvents()
		{
			PsdkEventSystem.Instance.onPsdkReady += OnPsdkReady;
			PsdkEventSystem.Instance.onResumeEvent += OnPsdkResumed;
		}

		private void InitPsdkCore ()
		{
			Debug.Log ("--------- Initializing PSDK Core");
			//Init the service manager      
			if (PSDKMgr.Instance.Setup ()) 
			{  
				initializedPromise.Dispatch();
				PSDKMgr.Instance.AppIsReady();
			}
		}

		private void OnPsdkReady()
		{
			Debug.Log ("PsdkCoreInitialzer : PSDK is Ready - attempting to display session start");

			psdkSessionStartSignal.Dispatch();
		}

		private void CollectAnalytics()
		{
			Debug.Log ("PSDK Core Initalizer.CollectAnalytics");
		}

		#endregion

		private void OnPsdkResumed(AppLifeCycleResumeState rs)
		{
			Debug.Log("Psdk.OnPsdkResumed :" + rs.ToString() + ", First scene loaded: " + _firstSceneLoaded);
			PSDKMgr.Instance.AppIsReady();

			if(rs == AppLifeCycleResumeState.ALCRS_RESTART_APP)
			{
				Debug.Log("OnPsdkResumed.AppRestart -" + _firstSceneLoaded);
				if(_firstSceneLoaded)
					psdkRestartAppSignal.Dispatch();
			}
//
//			if(rs == AppLifeCycleResumeState.ALCRS_NEW_SESSION ||
//			   rs == AppLifeCycleResumeState.ALCRS_RESTART_APP)
//			{
//				if(_firstSceneLoaded)
//				{
//					psdkSessionStartSignal.Dispatch();
//				}
//			}
        }

}

}
