using UnityEngine;
using System.Collections;
using System.Linq;

namespace TabTale.SceneManager
{
	public class ResourcesLoader : MonoBehaviour, ISceneLoadAction
	{
		public StringListAsset[] resourceLists;
		public StringListAsset[] bundleLists;

		#region ISceneLoadAction implementation

		public System.Func<IEnumerator> Action 
		{
			get { return Load; }
		}

		public float Progress
		{
			get { return _progress; }
		}

		public string Name
		{
			get { return name; }
		}

		#endregion

		float _progress = 0f;

		IEnumerator Load()
		{
			_progress = 0f;

			System.Func<IEnumerator> resourceCacher = GameApplication.Instance.AssetManager.GetResourceCacher(resourceLists.SelectMany(r => r.strings));
			System.Func<IEnumerator> bundleCacher = GameApplication.Instance.AssetManager.GetBundleCacher(bundleLists.SelectMany(r => r.strings));

			_progress = 1f;

			return EnumerableAction.Concat(bundleCacher, resourceCacher)();
		}
	}
}
