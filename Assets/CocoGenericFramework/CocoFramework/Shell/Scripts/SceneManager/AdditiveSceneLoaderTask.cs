using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale.SceneManager
{
#if UNITY_PRO_LICENSE

	public class AdditiveSceneLoaderTask : ControllerTask, ISceneLoaderTask
	{
		#region ITask implementation
		
		override public void Cancel ()
		{
			_cancel = true;
		}
		
		#endregion		

		#region ISceneLoaderTask implementation

		public string SceneName 
		{
			get { return _sceneName; }
		}

		#endregion

		string _sceneName;
		SceneTransition _sceneTransition;
		IEnumerable<GameObject> _removals;
#pragma warning disable 414
		float _delay = 0;
		float _additiveLoaderWatermark;
		bool _cancel = false;
#pragma warning restore 414
		SceneManager _sceneManager;
		public AdditiveSceneLoaderTask(SceneManager sceneManager, string sceneName, float additiveLoaderWatermark, SceneTransition sceneTransition, IEnumerable<GameObject> removals, float delay)
		{
			_sceneManager = sceneManager;
			_delay = delay;
			_sceneName = sceneName;
			_additiveLoaderWatermark = additiveLoaderWatermark;
			_name = string.Format("additive load scene {0}", sceneName);
			_sceneTransition = sceneTransition;
			_removals = removals;
		}	

		AsyncOperation _operation;

		void HandleRemovals()
		{
			if(_removals != null)
			{
				foreach(GameObject go in _removals)
				{
					GameObject.DestroyObject(go);
                }
            }
        }
       
#if UNITY_EDITOR

		override protected IEnumerator Controller()
		{
			ISceneLoaderTask pending = _sceneManager.PendingLoadTask;
			if(pending != null)
			{
				if(pending.SceneName == _sceneName)
				{
					CoreLogger.LogNotice(LoggerModules.SceneManager, string.Format("{0}: pending request for the same scene ({1}); ending operation", _name, _sceneName));
					InvokeDone(TaskEnding.Done);
					yield break;
				}
			}

			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("setting pending load to additive loader: {0}", _name));
			_sceneManager.PendingLoadTask = this;

			if(pending != null)
			{
				CoreLogger.LogNotice(LoggerModules.SceneManager, string.Format("{0}: waiting on loader {1}...", _name, pending.Name));
				yield return GameApplication.Instance.CoroutineFactory.Wait(pending);
				CoreLogger.LogNotice(LoggerModules.SceneManager, string.Format("{0}: waiting on loader {1} done!", _name, pending.Name));
            }
            
            SceneTransition sceneTransition = null;
			if(_sceneTransition != null)
			{
				sceneTransition = _sceneTransition.gameObject.GetComponent<SceneTransition>();
				if(sceneTransition != null)
				{
					GameObject newInstance = GameObject.Instantiate(_sceneTransition.gameObject) as GameObject;
					GameObject.DontDestroyOnLoad(newInstance);
					
					sceneTransition = newInstance.GetComponent<SceneTransition>();
					Debugger.Assert(sceneTransition != null, "sceneTransition cannot be null!");
					if(sceneTransition != null)
					{
						_sceneManager.HookTransition(sceneTransition);
						GameApplication.Instance.CoroutineFactory.StartCoroutine(sceneTransition.ExitSequence);

                        
						yield return GameApplication.Instance.CoroutineFactory.StartCoroutine(sceneTransition.WaitForFadeout);
                    }
                }
            }

			CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("starting additive sync load of scene {0}", _sceneName));
			Application.LoadLevelAdditive(_sceneName);
			CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("done additive load of scene {0}", _sceneName));

			HandleRemovals();

			if(sceneTransition != null)
			{
				sceneTransition.OnLevelWasLoaded(Application.loadedLevel);

				yield return GameApplication.Instance.CoroutineFactory.StartCoroutine(sceneTransition.WaitForFadein);
            }            

			if(_sceneManager.PendingLoadTask == this)
			{
				CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("resetting pending load from {0} to null", _name));
				_sceneManager.PendingLoadTask = null;
			}

			InvokeDone(TaskEnding.Done);
        }
        
#else
		float _lastProgress = 0;
        override protected IEnumerator Controller()
		{
			ISceneLoaderTask pending = _sceneManager.PendingLoadTask;
			if(pending != null)
			{
				if(pending.SceneName == _sceneName)
				{
					CoreLogger.LogNotice(LoggerModules.SceneManager, string.Format("{0}: pending request for the same scene ({1}); ending operation", _name, _sceneName));
					InvokeDone(TaskEnding.Done);
					yield break;
				}
			}

			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("setting pending load to additive loader: {0}", _name));
			_sceneManager.PendingLoadTask = this;

			if(pending != null)
			{
				CoreLogger.LogNotice(LoggerModules.SceneManager, string.Format("{0}: waiting on loader {1}...", _name, pending.Name));
				yield return GameApplication.Instance.CoroutineFactory.Wait(pending);
				CoreLogger.LogNotice(LoggerModules.SceneManager, string.Format("{0}: waiting on loader {1} done!", _name, pending.Name));
            }

			CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("starting additive async load of scene {0}", _sceneName));
            _operation = Application.LoadLevelAdditiveAsync(_sceneName);
			_operation.allowSceneActivation = false;
			_operation.priority = 0;

			while(_progress < _additiveLoaderWatermark)
			{
				if(_cancel)
				{
					InvokeDone(TaskEnding.Cancelled);
					yield break;
				}
				
				yield return null;

				_lastProgress = _progress;
				_progress = _operation.progress;

				if(_progress < _lastProgress)
				{
					//time travel - this can happen if we came back from background
					CoreLogger.LogNotice(LoggerModules.SceneManager, "time travel in scene loader - loading the next scene and calling done");
				}
			}

			_progress = 1;
			yield return null;
			yield return new WaitForSeconds(_delay);

			SceneTransition sceneTransition = null;
			if(_sceneTransition != null)
			{
				sceneTransition = _sceneTransition.gameObject.GetComponent<SceneTransition>();
				if(sceneTransition != null)
				{
					GameObject newInstance = GameObject.Instantiate(_sceneTransition.gameObject) as GameObject;					
					GameObject.DontDestroyOnLoad(newInstance);
					
					sceneTransition = newInstance.GetComponent<SceneTransition>();
					Debugger.Assert(sceneTransition != null, "sceneTransition cannot be null!");
					if(sceneTransition != null)
					{
						GameApplication.Instance.CoroutineFactory.StartCoroutine(sceneTransition.ExitSequence);
                        
						yield return GameApplication.Instance.CoroutineFactory.StartCoroutine(sceneTransition.WaitForFadeout);
					}
				}
			}

			CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("allowing scene additive load operation to complete"));

			_operation.allowSceneActivation = true;
			yield return _operation;

			CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("done additive load of scene {0}", _sceneName));

			HandleRemovals();

			if(sceneTransition != null)
			{
				sceneTransition.OnLevelWasLoaded(Application.loadedLevel);
				CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("additive load of scene {0} will now handle transition {1}", _sceneName, sceneTransition.name));
				yield return GameApplication.Instance.CoroutineFactory.StartCoroutine(sceneTransition.WaitForFadein);
			}

			if(_sceneManager.PendingLoadTask == this)
			{
				CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("resetting pending load from {0} to null", _name));
				_sceneManager.PendingLoadTask = null;
            } else
			{
				if(_sceneManager.PendingLoadTask == null)
				{
					CoreLogger.LogError(LoggerModules.SceneManager, string.Format("{0}: pending load is null!", _name));
				} else
				{
					CoreLogger.LogError(LoggerModules.SceneManager, string.Format("{0}: no need to reset pending since it is already somebody else: {1}", _name, _sceneManager.PendingLoadTask.Name));
				}
			}

			CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("additive load of scene {0} will now invoke done", _sceneName));
			InvokeDone(TaskEnding.Done);
			CoreLogger.LogInfo(LoggerModules.SceneManager, string.Format("additive load of scene {0} has invoked done", _sceneName));
		}

#endif

	}

#endif
}
