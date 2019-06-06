using UnityEngine;
using System.Collections;

namespace CocoPlay
{
	public abstract class CocoSceneTransitionBase : MonoBehaviour
	{
		public abstract IEnumerator TransitionInAsync ();

		public abstract IEnumerator TransitionOutAsync ();

		public void TransitionIn (System.Action endAction = null)
		{
			StartCoroutine (CocoWait.WaitForYield (TransitionInAsync (), endAction));
		}

		public void TransitionOut (System.Action endAction = null)
		{
			StartCoroutine (CocoWait.WaitForYield (TransitionOutAsync (), endAction));
		}
	}
}
