using UnityEngine;
using System.Collections;

namespace TabTale.SceneManager {

	public class SceneEntry : MonoBehaviour, ISceneEntry
	{
		protected bool _done = false;
		public bool Done
		{
			get { return _done; }
		}

		protected bool _active = false;
	}

}
