using System;
using System.Collections;
using CocoPlay.ResourceManagement;
using TabTale;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public class GameAssetPreInitSample : MonoBehaviour, IApplicationPreInit
	{
		public ITask GetInitializer (IServiceResolver moduleContainer)
		{
			return moduleContainer.TaskFactory.FromEnumerableAction (InitAsync);
		}

		public event Action<bool> Done = done => { };

		public void Init ()
		{
			StartCoroutine (InitAsync ());
		}

		private IEnumerator InitAsync ()
		{
			var startProgress = 0f;

			// init config
			var stepProgress = 0.1f;
			yield return GameAssetHelperSample.Instance.Init ();
			if (!GameAssetHelperSample.Instance.IsInited) {
				ResourceDebug.Log ("{0}->InitAsync: init failed.", GetType ().Name);
				Done (false);
				yield break;
			}
			ResourceDebug.Log ("{0}->InitAsync: init success.", GetType ().Name);

			startProgress += stepProgress;
			UpdateLoadingProgress (startProgress);

			// load locations
			stepProgress = 0.6f;
			var request = GameAssetHelperSample.Instance.LoadAllLocationsAsync ();
			yield return RunLoadRequest (request, startProgress, stepProgress);
			ResourceDebug.Log ("{0}->InitAsync: location load finish: error [{1}]", GetType ().Name, request.Error);
			startProgress += stepProgress;

			// load asset bundles
			stepProgress = 0.3f;
			request = GameAssetHelperSample.Instance.LoadAllAssetBundlesAsync ();
			yield return RunLoadRequest (request, startProgress, stepProgress);
			ResourceDebug.Log ("{0}->InitAsync: asset bundle load finish: error [{1}]", GetType ().Name, request.Error);

			UpdateLoadingProgress (1f);

			Done (true);
		}

		private IEnumerator RunLoadRequest (LoadRequest request, float startProgress, float stepProgress)
		{
			StartCoroutine (request);
			while (!request.IsDone) {
				yield return new WaitForEndOfFrame ();
				UpdateLoadingProgress (request.Progress, startProgress, stepProgress);
			}

			UpdateLoadingProgress (startProgress + stepProgress);
			AddLoadingInfo (request.Error);
		}


		#region Loading Bar

		[SerializeField]
		private Image _loadingBar;

		[SerializeField]
		private Text _loadingInfo;

		private void UpdateLoadingProgress (float progress)
		{
			if (_loadingBar == null) {
				return;
			}

			_loadingBar.fillAmount = progress;
		}

		private void UpdateLoadingProgress (float progress, float startProgress, float stepProgress)
		{
			UpdateLoadingProgress (startProgress + progress * stepProgress);
		}

		private void AddLoadingInfo (string info)
		{
			if (_loadingInfo == null || string.IsNullOrEmpty (info)) {
				return;
			}

			_loadingInfo.text += info + "\n";
		}

		#endregion
	}
}