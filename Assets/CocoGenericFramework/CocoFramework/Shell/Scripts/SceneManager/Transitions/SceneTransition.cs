using UnityEngine;
using System.Collections;

namespace TabTale.SceneManager 
{
	public class SceneTransition : SceneExit
	{
		public float transitionOutMinimalDuration = 0f;
		public float transitionInMinimalDuration = 3f;
		public string transitionName;

		protected enum Stage
		{
			Inactive, 
			Out, 
			Limbo, 
			In, 
			Done
		}

		protected Stage _stage = Stage.Inactive;

		protected float _transitionStartTime;

		void Awake()
		{
			if(transitionName == "")
				transitionName = GetType().Name;
		}

		protected override void OnActivate()
		{
			Debugger.Assert(_stage == Stage.Inactive, "Activated twice!");

			_now = Time.time;
			CoreLogger.LogDebug(LoggerModules.SceneManager, string.Format("{0} activated!", name));

			base.OnActivate ();
			
			DontDestroyOnLoad(this);			

			_stage = Stage.Out;
			
			StartCoroutine("TransitionOut");
		}

		protected virtual void OnStartTransitionIn()
		{
		}

		protected virtual void OnEndTransitionIn()
		{
		}

		protected virtual void OnStartTransitionOut()
		{
		}

		protected virtual void OnEndTransitionOut()
		{
		}

		public event System.Action<SceneTransition> StartTransitionIn = st => {};
		public event System.Action<SceneTransition> EndTransitionIn = st => {};
		public event System.Action<SceneTransition> StartTransitionOut = st => {};
		public event System.Action<SceneTransition> EndTransitionOut = st => {};

		public System.Func<IEnumerator> WaitForFadeout
		{
			get { return _WaitForFadeout; }
		}

		public System.Func<IEnumerator> WaitForFadein
		{
			get { return _WaitForFadein; }
		}

		IEnumerator _WaitForFadeout()
		{
			while(!_outDone)
				yield return null;
		}

		IEnumerator _WaitForFadein()
		{
			while(!_inDone)
				yield return null;
		}

		protected float _now;
		bool _outDone = false;
		IEnumerator TransitionOut()
		{
			CoreLogger.LogInfo(LoggerModules.SceneManager, "starting transition out...");

			StartTransitionOut(this);
			OnStartTransitionOut();

			_transitionStartTime = Time.time;
			float elapsed = 0f;
			while((elapsed < transitionOutMinimalDuration) && (_stage == Stage.Out))
			{
				HandleOut(elapsed);
				yield return null;
				_now = Time.time;
				elapsed = _now - _transitionStartTime;
			}

			if(_stage == Stage.In)
			{
				CoreLogger.LogInfo(LoggerModules.SceneManager, "breaking from transition out - timeout...");
			} else
			{
				CoreLogger.LogInfo(LoggerModules.SceneManager, "breaking from transition out - stage changed...");
			}

			_stage = Stage.Limbo;

			CoreLogger.LogInfo(LoggerModules.SceneManager, "ending transition out...");
			EndTransitionOut(this);
			OnEndTransitionOut();
			CoreLogger.LogInfo(LoggerModules.SceneManager, "transition out ended");


			//need to verify we don't go down with the sinking ship that is our parent...
			transform.parent = null;

			_outDone = true;
			yield return null;

			_done = true;
		}	

		public void OnLevelWasLoaded(int level)
		{
			CoreLogger.LogInfo(LoggerModules.SceneManager, "Received Level Loaded event - stopping fade out if still running");

			//never activated
			if(_stage == Stage.Inactive)
				return;

			//stage can be either Limbo, signaling we are done, or Out,
			//signaling we had a timeout (the scene manager didn't want 
			//to wait for us)
			_stage = Stage.In;

			//verify we are not fading out anymore
			if(!_outDone)
			{
				StopCoroutine("TransitionOut");
				_outDone = true;
			}

			//start fading in
			StartCoroutine("TransitionIn");
		}

		bool _inDone = false;
		IEnumerator TransitionIn()
		{
			CoreLogger.LogInfo(LoggerModules.SceneManager, "staring transition in...");
			StartTransitionIn(this);
			OnStartTransitionIn();
			CoreLogger.LogInfo(LoggerModules.SceneManager, "transition in started");

			_transitionStartTime = Time.time;
			float elapsed = 0f;
			while(elapsed < transitionInMinimalDuration)
			{
				HandleIn(elapsed);
				yield return null;
				_now = Time.time;
				elapsed = Time.time - _transitionStartTime;
			}

			_stage = Stage.Done;

			CoreLogger.LogInfo(LoggerModules.SceneManager, "ending transition in...");
			EndTransitionIn(this);
			OnEndTransitionIn();
			CoreLogger.LogInfo(LoggerModules.SceneManager, "transition in ended");

			_inDone = true;
			yield return null;

			DestroyObject(gameObject);
		}

		protected float _progress = 0f;

		void HandleOut(float elapsedTime)
		{
			_progress = Mathf.Clamp(elapsedTime / transitionOutMinimalDuration, 0f, 1f);
		}

		void HandleIn(float elapsedTime)
		{
			_progress = 1f - Mathf.Clamp(elapsedTime / transitionInMinimalDuration, 0f, 1f);
		}
	}

}


