using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale {

	public class WeakReference<T> : System.WeakReference
		where T : class
	{
		public WeakReference(T target)
			: base(target)
		{
		}

		public WeakReference(T target, bool trackResurrection)
			: base(target, trackResurrection)
		{
		}

		public new T Target
		{
			get { return (T)base.Target; }
			set { base.Target = value; }
		}
	};

	public class WeakDelegateEvent<TDelegate>
		where TDelegate : class
	{
		static WeakDelegateEvent()
		{
			if (!typeof(TDelegate).IsSubclassOf(typeof(System.Delegate)))
			{
				throw new System.InvalidOperationException(typeof(TDelegate).Name + " is not a delegate type");
			}
		}

		private IList<WeakReference<TDelegate>> _callbacks = new List<WeakReference<TDelegate>>();

		public void Add(TDelegate callback)
		{
		}

		public void Remove(TDelegate callback)
		{
			_callbacks.RemoveAll((c) => c.Target == callback);
		}

		public void Invoke()
		{
			_callbacks.RemoveAll((c) => !c.IsAlive);

			foreach(WeakReference<TDelegate> callback in _callbacks)
			{
				System.Delegate method = callback.Target as System.Delegate;
				if(method != null)
				{
					CoreLogger.LogInfo(string.Format("calling delegate {0}", callback.Target));
				}
			}
		}
	}
}
