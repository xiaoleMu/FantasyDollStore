using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale.Plugins.PSDK;

namespace TabTale.Plugins.PSDK {
	public class PsdkRewardedAdsButtons : PsdkSingleton<PsdkRewardedAdsButtons> {

		private bool _visible = false;
		private bool _adIsReady = true;
		private bool _adIsPlaying = false;

		public void Show() {
			_visible = true;
			_adIsPlaying = true;
			PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent += onClosed;
			PsdkEventSystem.Instance.SendMessage ("BlockTouches");
		}

		public void Hide() {
			_visible = false;
		}
	
		public bool AdIsReady {
			get { return _adIsReady; }
		}

		public bool AdIsPlaying {
			get { return _adIsPlaying; }
		}

		void ScaleScreenForOnGUI() {
			float screenScale = Screen.width / 480.0f;
			GUI.matrix = Matrix4x4.Scale(new Vector3(screenScale,screenScale,screenScale));
		}

		void onClosed(bool rewarded) {
			Debug.Log ("Rewarded:" + rewarded);
			PsdkEventSystem.Instance.onRewardedAdDidClosedWithResultEvent -= onClosed;
			_visible = false;
			_adIsPlaying = false;
			PsdkEventSystem.Instance.SendMessage ("UnblockTouches");
		}


		void OnGUI() {

			//ScaleScreenForOnGUI ();

			if (_visible) {
				GUI.Label(new Rect(5,10,340,40),"Should Show -> Should Reward / Should Not Reward -> Close");
				
				_adIsReady= GUI.Toggle (new Rect (350, 10, 80, 20), _adIsReady, "AdIsReady");

				if (GUI.Button (new Rect (440, 10, 20, 20), "X")) {
					onClosed(false);
				}

				if (GUI.Button (new Rect (5, 60, 100, 40), "Will Show"))
					PsdkEventSystem.Instance.transform.gameObject.SendMessage ("OnRewardedAdWillShow");

								if (GUI.Button (new Rect (110, 60, 100, 40), "Should Reward"))
					PsdkEventSystem.Instance.transform.gameObject.SendMessage ("OnRewardedAdShouldReward");
			
								if (GUI.Button (new Rect (5, 110, 130, 40), "Should Not Reward"))
					PsdkEventSystem.Instance.transform.gameObject.SendMessage ("OnRewardedAdShouldNotReward");
			
								if (GUI.Button (new Rect (140, 110, 70, 40), "Close")) {
									_adIsPlaying = false;
									PsdkEventSystem.Instance.transform.gameObject.SendMessage ("OnRewardedAdDidClose");
								}
						}
		}







	}
}
