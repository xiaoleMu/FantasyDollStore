using UnityEngine;
using System.Collections;
using TabTale;
using strange.extensions.signal.impl;
using CocoSceneID = Game.CocoSceneID;

namespace CocoPlay
{
	public class CocoSceneLoadingFinishSignal : Signal
	{
	}


	public class CocoSceneLoadingStartSignal : Signal<CocoSceneID>
	{
	}


	public class CocoSceneInitFinishSignal : Signal<bool>
	{
	}


	public class CocoSceneSwitchControl : GameView
	{
		private const string DEFAULT_TRANSITION_ASSET_PATH = "UI Scene Transition";


#if !COCO_FAKE
		[Inject]
		public Game.GameGlobalData GlobalData { get; set; }
#else
		public CocoGlobalData GlobalData {get; set;}
		#endif

		[Inject]
		public CocoSceneInitFinishSignal SceneInitFinishSignal { get; set; }

		[Inject]
		public CocoSceneLoadingFinishSignal SceneLoadingFinishSignal { get; set; }

		[Inject]
		public CocoSceneLoadingStartSignal SceneLoadingStartSignal { get; set; }

		[Inject]
		public CocoStartModule StartModule { get; set; }

		// [Inject]
		// public LocationShownSignal LocationShowSignal { get; set; }


		protected bool m_SceneInitFinished = true;

		protected static CocoSceneID m_EnterSceneID;

		protected CocoSceneTransitionBase m_Transition;

		public bool unloadAssetsAfterLoadEmptyScene;

		/// <summary>
		/// Create the specified ID.
		/// </summary>
		/// <param name="id">I.</param>
		/// <param name="transitionAssetPath"></param>
		public static CocoSceneSwitchControl Create (CocoSceneID id, string transitionAssetPath = null)
		{
			if (string.IsNullOrEmpty (transitionAssetPath)) {
				transitionAssetPath = DEFAULT_TRANSITION_ASSET_PATH;
			}
			CocoSceneSwitchControl control = CocoLoad.InstantiateOrCreate<CocoSceneSwitchControl> (transitionAssetPath);
			m_EnterSceneID = id;
			return control;
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start ()
		{
			base.Start ();
			m_Transition = GetComponentInChildren<CocoSceneTransitionBase> ();
			PlayLoadScene ();
		}

		protected virtual void PlayLoadScene ()
		{
			StartCoroutine (LoadScene ());
		}

		/// <summary>
		/// Adds the listeners.
		/// </summary>
		protected override void AddListeners ()
		{
			base.AddListeners ();

			SceneInitFinishSignal.AddListener (SceneInitFinish);
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected override void RemoveListeners ()
		{
			SceneInitFinishSignal.RemoveListener (SceneInitFinish);

			base.RemoveListeners ();
		}

		/// <summary>
		/// Loads the scene.
		/// </summary>
		/// <returns>The scene.</returns>
		private IEnumerator LoadScene ()
		{
			//重置状态
			DontDestroyOnLoad (gameObject);
			CocoMainController.Instance.TouchEnable = false;
			CocoMainController.Canvas_UI = null;
			CocoMainController.UICamera = null;

			CocoGlobalData.EnterSceneID = m_EnterSceneID;
			SceneLoadingStartSignal.Dispatch (m_EnterSceneID);

			var waitFrame = new WaitForEndOfFrame ();

			// show transition in
			if (m_Transition != null) {
				yield return m_Transition.TransitionInAsync ();
			}

			//卸载场景
			yield return waitFrame;
			yield return StartModule.ClearCurrSceneAsync ();
			yield return waitFrame;

			//加载空场景
			StartModule.UnloadCurrScene ();

			if (unloadAssetsAfterLoadEmptyScene) {
				yield return waitFrame;
				Resources.UnloadUnusedAssets ();
				System.GC.Collect ();
				yield return waitFrame;
			}

			Time.timeScale = 1;
			yield return new WaitForSeconds (0.5f);

			//加载新场景
			GlobalData.CurSceneID = m_EnterSceneID;
			StartModule.LoadScene (m_EnterSceneID);
			yield return waitFrame;

			m_SceneInitFinished = !StartModule.GetSceneWaitInit (m_EnterSceneID);
			//等待场景初始化
			while (!m_SceneInitFinished)
				yield return waitFrame;

			//加载Interstitial
			// bool canShowIntertial = false;
			// bool haveShowIntertial = false;
			//
			// LocationShowSignal.AddListener (OnLocationShow);
			// if (GlobalData.CanShowInterstitialBetweenScene (GlobalData.FrontSceneID, m_EnterSceneID)) {
			// 	yield return CocoMainController.AdsControl.showInterstitial ((a, b) => {
			// 		canShowIntertial = a;
			// 		haveShowIntertial = b;
			// 	});
			// }
			//
			// // show transition out
			// if (!(canShowIntertial && haveShowIntertial)) {
			// 	if (transform.childCount > 0) {
			// 		transform.GetChild (0).localScale = Vector3.one;
			// 	}
			//
			// 	if (m_Transition != null) {
			// 		yield return m_Transition.TransitionOutAsync ();
			// 	}
			// }
			// LocationShowSignal.RemoveListener (OnLocationShow);

			var isInterstitialDone = true;
			var isInterstitialShown = false;
			if (GlobalData.CanShowInterstitialBetweenScene (GlobalData.FrontSceneID, m_EnterSceneID)) {
				isInterstitialDone = false;
				StartCoroutine (CocoMainController.AdsControl.ShowInterstitial (
					() => isInterstitialShown = true,
					() => isInterstitialDone = true));

				// wait for interstitial show
				while (!isInterstitialShown && !isInterstitialDone) {
					yield return waitFrame;
				}
			}

			// show transition out
			if (m_Transition != null) {
				if (isInterstitialShown) {
					yield return new WaitForSeconds (0.5f);
				}
				yield return m_Transition.TransitionOutAsync ();
				// hide transition
				if (transform.childCount > 0) {
					transform.GetChild (0).localScale = Vector3.zero;
				}
			}

			// wait for interstitial done
			while (!isInterstitialDone) {
				yield return waitFrame;
			}

			// clean
			CocoMainController.Instance.TouchEnable = true;
			SceneLoadingFinishSignal.Dispatch ();
			CocoMainController.sceneSwitchControl = null;
			CocoMainController.CleanResources ();
			Destroy (gameObject);
		}

		// protected void OnLocationShow (LocationResult result)
		// {
		// 	Debug.LogError (result.sourceAssigned + "----" + result.success + "-----" + result.playsMusic);
		// 	if (result.location != ApplicationLocation.SceneTransitions) {
		// 		return;
		// 	}
		//
		// 	if (result.sourceAssigned) {
		// 		StartCoroutine (HidePanel ());
		// 	}
		// }

		// private IEnumerator HidePanel ()
		// {
		// 	yield return new WaitForSeconds (0.5f);
		// 	transform.GetChild (0).localScale = Vector3.zero;
		// }


		/// <summary>
		/// Scenes the init finish.
		/// </summary>
		/// <param name="pValue">If set to <c>true</c> p value.</param>
		private void SceneInitFinish (bool pValue)
		{
			m_SceneInitFinished = pValue;
		}
	}
}