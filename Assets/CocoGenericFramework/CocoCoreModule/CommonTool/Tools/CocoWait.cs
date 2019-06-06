using UnityEngine;
using System.Collections;

namespace CocoPlay
{
	public class CocoWait
	{
		public static IEnumerator WaitForTime (float time, System.Action action = null)
		{
			yield return new WaitForSeconds (time);

			if (action != null) {
				action ();
			}
		}
		
		public static IEnumerator WaitForRealTime (float time, System.Action action = null)
		{
			float startTime = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup - startTime < time) {
				yield return null;
			}

			if (action != null) {
				action ();
			}
		}

		public static IEnumerator WaitForFrame (int frameCount, System.Action action = null)
		{
			while (frameCount-- > 0) {
				yield return new WaitForEndOfFrame ();
			}

			if (action != null) {
				action ();
			}
		}

		public static IEnumerator WaitForFunc (System.Func<bool> waitFunc, System.Action action = null)
		{
			while (waitFunc ()) {
				yield return new WaitForEndOfFrame ();
			}

			if (action != null) {
				action ();
			}
		}

		public static IEnumerator WaitForYield (IEnumerator enumerator, System.Action action = null)
		{
			yield return enumerator;

			if (action != null) {
				action ();
			}
		}

		public static IEnumerator WaitForYield (YieldInstruction yieldInst, System.Action action = null)
		{
			yield return yieldInst;

			if (action != null) {
				action ();
			}
		}
	}
}
