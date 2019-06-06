using UnityEngine;
using System.Collections;

namespace TabTale.SceneManager 
{
	public class SceneExit : MonoBehaviour, ISceneExit
	{
		public System.Func<IEnumerator> ExitSequence
		{
			get { return _ExitSequence; }
		}
		
		protected bool _active = false;

		protected bool _done = false;
		protected virtual IEnumerator _ExitSequence()
		{
			_active = true;
			OnActivate();

			while(!_done)
			{
				yield return null;
			}
		}

		protected virtual void OnActivate()
		{
		}
	}
}
