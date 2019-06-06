using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class CoroutineFactory : MonoDispatcher, ICoroutineFactory
	{
		void Awake()
		{
			if(s_instance == null)
				s_instance = this;
		}

		IEnumerator _Concat(Coroutine routine1, Coroutine routine2)
		{
			yield return routine1;
			yield return routine2;
		}

		public Coroutine Concat(Coroutine routine1, Coroutine routine2)
		{
			return StartCoroutine(_Concat(routine1, routine2));
		}

		public Coroutine StartCoroutine(System.Func<IEnumerator> serialAction)
		{
			Debugger.Assert(serialAction != null, "serialAction parameter cannot be null!");
			return StartCoroutine(serialAction());
		}

		public Coroutine StartCoroutine(IEnumerableAction enumerableAction)
		{
			return StartCoroutine(() => enumerableAction.Run());
		}

		IEnumerator Wait(System.Func<bool> predicate, float timeout)
		{
			float deadline = Time.realtimeSinceStartup + timeout;
			while(!predicate() && (Time.realtimeSinceStartup < deadline))
			{
				yield return null;
			}
		}

		IEnumerator Wait(System.Func<bool> predicate)
		{
			while(!predicate())
			{
				yield return null;
			}
		}

		public Coroutine Wait(ITask task)
		{
			return StartCoroutine(Wait (() => task.State.IsDone()));
		}

		public Coroutine Wait(ITask task, float timeout)
		{
			return StartCoroutine(Wait(() => task.State.IsDone(), timeout));
		}

		Coroutine StartWait(System.Func<bool> predicate, float timeout)
		{
			return StartCoroutine(Wait(predicate, timeout));
		}

		Coroutine StartWait(System.Func<bool> predicate)
		{
			return StartCoroutine(Wait(predicate));
		}		

		public Coroutine StartWait(ITask task)
		{
			bool done = false;
			Coroutine routine = StartWait(() => done);
			task.Start(result => { done = true; });
			return routine;
		}

		public Coroutine StartWait(ITask task, float timeout)
		{
			bool done = false;
			Coroutine routine = StartWait(() => done);
			task.Start(result => { done = true; }, timeout);
			return routine;
		}

		static CoroutineFactory s_instance;
		public static CoroutineFactory Instance
		{
			get
			{
				if(s_instance == null)
				{
					GameObject go = new GameObject();
					DontDestroyOnLoad(go);
					s_instance = go.AddComponent<CoroutineFactory>();
				}

				return s_instance;
			}
		}
	}
}
