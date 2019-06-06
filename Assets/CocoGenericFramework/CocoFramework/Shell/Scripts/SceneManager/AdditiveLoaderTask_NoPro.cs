using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale.SceneManager
{
#if UNITY_PRO_LICENSE
#else

	/*
	public class AdditiveSceneLoaderTask : ITask
	{

		#region ITask implementation
		
		public event System.Action<TaskEnding> Done;
		
		public ITask Start (System.Action<TaskEnding> resultHandler, float timeout = -1f)
		{
			Done += resultHandler;
			return Start(timeout);
		}
		
		float _startTime;
		public ITask Start (float timeout = -1f)
		{
			_startTime = Time.realtimeSinceStartup;
			_taskFactory.FromEnumerableAction(Controller).Start(timeout);
			return this;
		}
		
		public void Cancel ()
		{
		}
		
		public TaskEnding Handle ()
		{
			IEnumerator controller = Controller();
			while(controller.MoveNext())
			{
			}
			
			Done(TaskEnding.Done);
			return TaskEnding.Done;
		}
		
		public TaskState State 
		{
			get { return TaskState.Running; }
		}
		
		public float StartTime 
		{
			get { return _startTime; }
		}

		float _progress = 0;
		public float Progress 
		{
			get { return _progress; }
		}
		
		public string Name 
		{
			get { return _name; }
			set { _name = value; }
		}
		
		#endregion
		
		ITaskFactory _taskFactory;
		string _sceneName;
		string _name;
		GameObject _sceneTransition;
		public AdditiveLoaderTask(string sceneName, ITaskFactory taskFactory, float additiveLoaderWatermark, GameObject sceneTransition)
		{
			_sceneTransition = sceneTransition;
			_taskFactory = taskFactory;
			_sceneName = sceneName;
			_name = string.Format("load scene {0}", sceneName);
		}

		IEnumerator Controller()
		{
			if(_sceneTransition != null)
			{
				GameObject newInstance = GameObject.Instantiate(_sceneTransition) as GameObject;
				GameObject.DontDestroyOnLoad(newInstance);
				
				IEnumerable<System.Func<IEnumerator>> transitionActions = newInstance.GetComponents<SceneTransition>().Select(st => st.ExitSequence);
				yield return GameApplication.Instance.StartCoroutine(EnumerableAction.Parallelize(transitionActions));
			}
			
			Application.LoadLevelAdditive(_sceneName);

			_progress = 1;

			yield return null;

			Done(TaskEnding.Done);
		}

	}
	*/

#endif

}
