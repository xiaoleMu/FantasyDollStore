using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class MonoDispatcher : MonoBehaviour, IDispatcher
	{
		#region IDispatcher implementation

		public void Post (System.Action action, int minFrameDelay = 1)
		{
			_news.Enqueue(new Entry(action, minFrameDelay));
		}

		public void Send (System.Action action)
		{
			action();
		}

		#endregion

		class Entry
		{
			public System.Action action;
			public int delay;

			public Entry(System.Action action, int delay)
			{
				this.action = action;
				this.delay = delay;
			}
		}

		IList<Entry> _post = new List<Entry>();
		Queue<Entry> _news = new Queue<Entry>();

		public int actionsPerIteration = -1;

		void OnDestroy()
		{
			CoreLogger.LogDebug(LoggerModules.Tasks, string.Format("dispatcher {0} going down", name));
		}

		IEnumerator Loop()
		{
			CoreLogger.LogDebug(LoggerModules.Tasks, string.Format("dispatcher {0} starting up!", name));

			while(true)
			{
				int count = 0;
				while(_news.Count > 0 && (count < actionsPerIteration || actionsPerIteration < 0))
				{
					_post.Add(_news.Dequeue());
					count++;
				}

				foreach(Entry entry in _post)
				{
					entry.delay--;
					if(entry.delay == 0)
					{
						try
						{
							entry.action();
						} catch(System.Exception ex)
						{
							CoreLogger.LogError(LoggerModules.Tasks, string.Format("Error in Dispatcher Action: {0}\n Stack Trace:{1}", ex.Message, ex.StackTrace));
						}
					}
				}

				_post.RemoveAll(e => e.delay == 0);

				yield return null;
			}
		}

		protected virtual void Awake()
		{
			StartCoroutine(Loop ());
		}
	}
}
