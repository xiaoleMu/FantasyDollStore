using UnityEngine;

namespace CocoPlay.ResourceManagement
{
	public class ResourceAsyncHelper : MonoBehaviour
	{
		#region Instance

		private static ResourceAsyncHelper _instance;

		public static ResourceAsyncHelper Instance {
			get {
				if (_instance == null) {
					_instance = CreateInstance ();
				}
				return _instance;
			}
		}

		private static ResourceAsyncHelper CreateInstance ()
		{
			var go = new GameObject ("_ResourceAsyncHelper");
			DontDestroyOnLoad (go);
			return go.AddComponent<ResourceAsyncHelper> ();
		}

		#endregion
	}
}