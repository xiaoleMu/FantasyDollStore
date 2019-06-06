using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;

public class PSDKCrashToolDemo : MonoBehaviour {


	void Awake() {
		PsdkEventSystem.Instance.onPsdkReady += OnPsdkReady;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void OnPsdkReady () {
		StartCoroutine(crashToolCoro());
	}

	IEnumerator crashToolCoro() {
		IPsdkCrashTool ct = PSDKMgr.Instance.GetCrashMonitoringToolService ();
		if (ct == null) {
			UnityEngine.Debug.LogError("CrashMonitoringToolService not enabled");
			yield break;
		}
		ct.AddBreadCrumb ("CrashMonitoringTool demo, you'll see me in the crash description in Hockey site");
		yield return new WaitForSeconds (10f);
		ct.AddBreadCrumb ("CrashMonitoringTool demo, waited 10 seconds !, not crashing, throwing null");
		throw null;
	}
}
