using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using TabTale;
#if !COCO_FAKE
using CocoSceneID = Game.CocoSceneID;
#else
using CocoSceneID = CocoPlay.Fake.CocoSceneID;
#endif
using SceneManager = UnityEngine.SceneManagement.SceneManager;


namespace CocoPlay
{
	public abstract class CocoStartModule : CocoModuleBase
	{
		#region Signals

		protected override void InitSignals ()
		{
			base.InitSignals ();
			Bind<CocoPopupShowSignal> ();
			Bind<CocoPopupClosedSignal> ();
			Bind<CocoRemovePopupSignal> ();

			// loading 界面结束时发送
			Bind<CocoSceneLoadingFinishSignal> ();
			// 用于需要在场景初始化需要多帧完成时，在开始初始化时发送false，结束初始化时发送true。
			Bind<CocoSceneInitFinishSignal> ();
		    Bind<CocoSceneLoadingStartSignal> ();

			BindCommand<PauseGameMusicSignal, CocoAudioMuteCommand> ();
		}

		protected override void CleanSignals ()
		{
			Unbind<CocoPopupShowSignal> ();
			Unbind<CocoPopupClosedSignal> ();
			Unbind<CocoRemovePopupSignal> ();
			Unbind<CocoSceneLoadingFinishSignal> ();
			Unbind<CocoSceneInitFinishSignal> ();
		    Unbind<CocoSceneLoadingStartSignal> ();

			base.CleanSignals ();
		}

		#endregion


		#region Datas

		protected override void InitDatas ()
		{
			base.InitDatas ();

			//			Bind <CocoGlobalRecordModel> ();
			InitSceneModuleDatas ();
		}

		protected override void CleanDatas ()
		{
			//			Unbind <CocoGlobalRecordModel> ();

			base.CleanDatas ();
		}

		#endregion


		protected override void InitSubModules ()
		{
			base.InitSubModules ();
			BindSubModule<CocoPlay.Localization.CocoLocalizationModule> ();
		}

		protected override void CleanSubModules ()
		{
			base.CleanSubModules ();
			UnbindSubModule<CocoPlay.Localization.CocoLocalizationModule> ();
		}


		#region Scene Module

		const string EMPTY_SCENE_NAME = "Empty";

		Dictionary<CocoSceneID, CocoSceneModuleData> m_ConfigDataDic = new Dictionary<CocoSceneID, CocoSceneModuleData> ();

		public Dictionary<CocoSceneID, CocoSceneModuleData> SceneConfigDataDic {
			get { return m_ConfigDataDic; }
		}

		CocoSceneModuleBase m_CurrSceneModule = null;

		protected abstract void InitSceneModuleDatas ();

		protected void AddSceneModuleData (CocoSceneModuleData moduleData)
		{
			m_ConfigDataDic.Add (moduleData.sceneId, moduleData);
		}

		public CocoSceneModuleData GetSceneModuleData (CocoSceneID sceneId)
		{
			if (!m_ConfigDataDic.ContainsKey (sceneId)) {
				return null;
			}

			return m_ConfigDataDic [sceneId];
		}

		public bool GetSceneWaitInit(CocoSceneID sceneId)
		{
			CocoSceneModuleData moduleData = GetSceneModuleData (sceneId);
			return moduleData != null && moduleData.waitInit;
		}

		bool LoadSceneModule (CocoSceneID sceneId)
		{
			CocoSceneModuleData moduleData = GetSceneModuleData (sceneId);
			if (moduleData == null) {
				Debug.LogErrorFormat ("{0}->AddSceneModule: can NOT found scene module for scene [{1}]!", GetType ().Name, sceneId);
				return false;
			}

			m_CurrSceneModule = CocoMainController.Instance.AddModule<CocoSceneModuleBase> (moduleData.moduleType, moduleData.moduleAssetPath);
			if (m_CurrSceneModule == null) {
				return false;
			}

			m_CurrSceneModule.Data = moduleData;
			return true;
		}

		bool UnloadCurrSceneModule ()
		{
			if (m_CurrSceneModule == null) {
				return false;
			}
			CocoMainController.Instance.RemoveModule<CocoSceneModuleBase> ();
			m_CurrSceneModule = null;
			return true;
		}

		#endregion


		#region Load/Unload Scene

		public void LoadScene (CocoSceneID sceneId)
		{
			if (!LoadSceneModule (sceneId)) {
				return;
			}

			SceneManager.LoadScene (m_CurrSceneModule.Data.sceneName);
		}

		public IEnumerator LoadSceneAsync (CocoSceneID sceneId)
		{
			if (!LoadSceneModule (sceneId)) {
				yield break;
			}

			yield return SceneManager.LoadSceneAsync (m_CurrSceneModule.Data.sceneName);
		}

		public void UnloadCurrScene ()
		{
			SceneManager.LoadScene (EMPTY_SCENE_NAME);
			UnloadCurrSceneModule ();
		}

		public IEnumerator UnloadCurrSceneAsync ()
		{
			yield return SceneManager.LoadSceneAsync (EMPTY_SCENE_NAME);
			UnloadCurrSceneModule ();
		}

		public IEnumerator ClearCurrSceneAsync ()
		{
			if (m_CurrSceneModule == null) {
				yield break;
			}

			if (m_CurrSceneModule.SceneManager != null) {
				m_CurrSceneModule.SceneManager.Clear ();
			}

			if (m_CurrSceneModule.Data != null) {
				yield return SceneManager.UnloadSceneAsync (m_CurrSceneModule.Data.sceneName);
			}
		}

		#endregion


		#region Game Start

		public virtual void StartGame ()
		{
			LoadFirstSceneModule ();
		}

		void LoadFirstSceneModule ()
		{
#if UNITY_EDITOR
			CocoSceneModuleData startModuleData = m_ConfigDataDic.Values.FirstOrDefault (moduleData => {
				return moduleData.sceneName == GameApplication.RequestingScene;
			});
#else
			CocoSceneModuleData startModuleData = m_ConfigDataDic.Values.FirstOrDefault ();
			#endif

			LoadSceneModule (startModuleData.sceneId);
			Debug.LogFormat ("---- START FIRST SCENE: [{0}<{1}>] --------------------------", startModuleData.sceneId, startModuleData.sceneName);
		}

		#endregion
	}
}