using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace CocoPlay
{
	public class CocoObjectPool
	{
		#region UnityEngine.Object

		private static readonly Dictionary<Object, int> UnityObjectPool = new Dictionary<Object, int> ();


		#region Add / Remove

		public static void Add (Object obj)
		{
			if (Reference (obj)) {
				return;
			}

			UnityObjectPool.Add (obj, 1);
		}

		public static void Remove (Object obj)
		{
			UnReference (obj);
		}

		private static void TryDestroy (Object obj)
		{
			if (UnityObjectPool [obj] > 0) {
				return;
			}

			UnityObjectPool.Remove (obj);
			Object.Destroy (obj);
		}

		public static bool IsInPool (Object obj)
		{
			return UnityObjectPool.ContainsKey (obj);
		}

		#endregion


		#region Reference

		public static bool Reference (Object obj)
		{
			if (!UnityObjectPool.ContainsKey (obj)) {
				return false;
			}

			UnityObjectPool [obj]++;
			//UnityEngine.Debug.LogError ("Reference: " + obj.GetType () + ", " + obj.GetInstanceID () + ", " + UnityObjectPool [obj]);
			return true;
		}

		public static bool UnReference (Object obj)
		{
			if (!UnityObjectPool.ContainsKey (obj)) {
				return false;
			}

			UnityObjectPool [obj]--;
			//UnityEngine.Debug.LogError ("UnReference: " + obj.GetType () + ", " + obj.GetInstanceID () + ", " + UnityObjectPool [obj]);
			TryDestroy (obj);
			return true;
		}

		public static int GetReferenceCount (Object obj)
		{
			return !UnityObjectPool.ContainsKey (obj) ? 0 : UnityObjectPool [obj];
		}

		#endregion

		#endregion


		#region System.IDisposable

		private static readonly Dictionary<IDisposable, int> SysObjectPool = new Dictionary<IDisposable, int> ();


		#region Add / Remove

		public static void Add (IDisposable obj)
		{
			if (Reference (obj)) {
				return;
			}

			SysObjectPool.Add (obj, 1);
		}

		public static void Remove (IDisposable obj)
		{
			UnReference (obj);
		}

		private static void TryDestroy (IDisposable obj)
		{
			if (SysObjectPool [obj] > 0) {
				return;
			}

			SysObjectPool.Remove (obj);
			obj.Dispose ();
		}

		public static bool IsInPool (IDisposable obj)
		{
			return SysObjectPool.ContainsKey (obj);
		}

		#endregion


		#region Reference

		public static bool Reference (IDisposable obj)
		{
			if (!SysObjectPool.ContainsKey (obj)) {
				return false;
			}

			SysObjectPool [obj]++;
			//UnityEngine.Debug.LogError ("Reference: " + obj.GetType () + ", " + obj.GetHashCode () + ", " + SysObjectPool [obj]);
			return true;
		}

		public static bool UnReference (IDisposable obj)
		{
			if (!SysObjectPool.ContainsKey (obj)) {
				return false;
			}

			SysObjectPool [obj]--;
			//UnityEngine.Debug.LogError ("UnReference: " + obj.GetType () + ", " + obj.GetHashCode () + ", " + SysObjectPool [obj]);
			TryDestroy (obj);
			return true;
		}

		public static int GetReferenceCount (IDisposable obj)
		{
			return !SysObjectPool.ContainsKey (obj) ? 0 : SysObjectPool [obj];
		}

		#endregion

		#endregion
	}
}