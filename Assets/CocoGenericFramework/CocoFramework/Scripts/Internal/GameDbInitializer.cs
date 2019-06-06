using UnityEngine;
using System;
using System.Collections;

namespace TabTale
{
	public class GameDbInitializer : Module
	{
		private GameDB _gameDB;
		public GameDB GameDB { get { return _gameDB; } }

		/*
		private ServerTime _serverTime;
		public ServerTime ServerTime { get { return _serverTime; } }

		private ModelSyncService _modelSyncService;
		public ModelSyncService ModelSyncService { get { return _modelSyncService; } }
		*/

		protected override IEnumerator ModuleInitializer ()
		{
			Debug.Log("GameDbInitializer - ModuleInitializer start --------------------------------------------1");

			_gameDB = new GameDB();

			if(Application.loadedLevelName.Contains("Upload") && Application.isEditor)
				_gameDB.InitLocalDB();
			else
				_gameDB.InitDB();

			/*
			_serverTime = new ServerTime();

			_modelSyncService = new ModelSyncService();
			_modelSyncService.Init(_gameDB,this,_serverTime);
			//yield return StartCoroutine(_modelSyncService.PregameConnect());
			//StartCoroutine(_modelSyncService.AsyncLoop());

			_modelSyncService.Start();

			yield return StartCoroutine(_modelSyncService.WaitForState(ModelSyncService.SyncLoopState.Connected));
			*/

			CoreLogger.LogDebug("GameDbInitializer", "done with Init");
			
			Debug.Log("GameDbInitializer - ModuleInitializer end --------------------------------------------2");

			yield break;
		}	

		void OnApplicationPause(bool pauseStatus){
			CoreLogger.LogDebug("GameDbInitializer","OnApplicationPause "+pauseStatus);

			if(Application.platform.IsEditor())
				return;

			/*
			if(!pauseStatus && _modelSyncService!=null){
				_modelSyncService.RestartFlowAfterBackground();

			}
			*/
		}

		void OnDestroy()
		{
			if(_gameDB != null)
			{
				_gameDB.Terminate();
				_gameDB = null;
			}
		}

	}
}
