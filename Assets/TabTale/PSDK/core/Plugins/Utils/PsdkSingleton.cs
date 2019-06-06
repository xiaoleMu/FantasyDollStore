using UnityEngine;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
namespace TabTale.Plugins.PSDK {

	public class PsdkSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;
		
		private static object _lock = new object();
		
		public static T Instance
		{
			get
			{
				if (applicationIsQuitting) {
					return null;
				}
				
				lock(_lock)
				{
					if (_instance == null)
					{
						_instance = (T) FindObjectOfType(typeof(T));
						
						if ( FindObjectsOfType(typeof(T)).Length > 1 )
						{
							Debug.LogError("[PsdkSingleton] Something went really wrong " +
							               " - there should never be more than 1 singleton!" +
							               " Reopenning the scene might fix it.");
							return _instance;
						}
						
						if (_instance == null)
						{
							GameObject singleton = new GameObject();
							_instance = singleton.AddComponent<T>();
							string[] delimited = typeof(T).ToString().Split(new char[]{'.'});
							singleton.name = delimited[delimited.Length-1];
							
							DontDestroyOnLoad(singleton);
						} 
					}
					
					return _instance;
				}
			}
		}
		
		private static bool applicationIsQuitting = false;
		/// <summary>
		/// When Unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when application quits.
		/// If any script calls Instance after it have been destroyed, 
		///   it will create a buggy ghost object that will stay on the Editor scene
		///   even after stopping playing the Application. Really bad!
		/// So, this was made to be sure we're not creating that buggy ghost object.
		/// <para>
		/// IMPORTANT: <br/>
		/// When you override OnDestroy, you must call base.OnDestroy()<br/>
		/// at the end of youe inhertued derived OnDestroy.<br/>
		/// </para>
		/// </summary>
		protected virtual void OnDestroy () {
			applicationIsQuitting = true;
		}

		protected virtual void Awake()
		{
			if(_instance){
				DestroyImmediate(gameObject);
				applicationIsQuitting = false;
			}
			else
			{
				DontDestroyOnLoad(gameObject);
				_instance = gameObject.GetComponent<T>();
			}
		}

	}
}
