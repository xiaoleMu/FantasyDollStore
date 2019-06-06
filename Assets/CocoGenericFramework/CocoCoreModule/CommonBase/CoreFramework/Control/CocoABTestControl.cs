using UnityEngine;
using System.Collections;
using TabTale;
using TabTale.Plugins.PSDK;



namespace CocoPlay {
	public class CocoABTestControl : MonoBehaviour {
		#if ABTEST

		[Inject]
		public CocoGlobalRecordModel globalRecordModel {get; set;}

		// Use this for initialization
		void Start () {
			PsdkEventSystem.Instance.onConfigurationLoaded += OnConfigurationLoaded;
			Debug.LogError ("CocoABTestControl => ---------AddListeners--------");

		#if UNITY_EDITOR
			OnConfigurationLoaded ();
		#endif
		}

		bool haveGetCallBack = false;

		public static bool ConfigurationLoaded = false;
		void OnConfigurationLoaded()
		{
			Debug.LogError ("CocoABTestControl => OnConfigurationLoaded");
			ConfigurationLoaded = true;
		}

		#endif
	}
}
