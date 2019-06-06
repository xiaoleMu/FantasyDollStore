using CocoPlay.ResourceManagement;
using UnityEngine;

namespace CocoPlay
{
	public class CocoLoad
	{

		#region Get/Add Component

		public static T GetOrAddComponent<T> (GameObject go) where T : Component
		{
			T com = go.GetComponent<T> ();
			if (com == null) {
				com = go.AddComponent<T> ();
			}

			return com;
		}

		public static T GetOrAddComponent<T> (Component component) where T : Component
		{
			return GetOrAddComponent<T> (component.gameObject);
		}

		public static Component GetOrAddComponent (System.Type type, GameObject go)
		{
			Component com = go.GetComponent (type);
			if (com == null) {
				com = go.AddComponent (type);
				if (com == null) {
					Debug.LogError ("CocoLoad->GetOrAddComponent: component [" + type.Name + "] can NOT be added.");
				}
			}

			return com;
		}

		public static Component GetOrAddComponent (System.Type type, Component component)
		{
			return GetOrAddComponent (type, component.gameObject);
		}

		public static void RemoveComponent<T>(GameObject obj) where T : Component
		{
			T component = obj.GetComponent<T> ();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}

		public static void RemoveComponent<T>(Component com) where T : Component
		{
			T component = com.GetComponent<T> ();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}

		#endregion


		#region Get/Add Component In Children

		public static T GetOrAddComponentInChildren<T> (GameObject go, bool includeInactive = false) where T : Component
		{
			T com = go.GetComponentInChildren<T> (includeInactive);
			if (com == null) {
				com = go.AddComponent<T> ();
			}

			return com;
		}

		public static T GetOrAddComponentInChildren<T> (Component component, bool includeInactive = false) where T : Component
		{
			return GetOrAddComponentInChildren<T> (component.gameObject, includeInactive);
		}

		public static Component GetOrAddComponentInChildren (System.Type type, GameObject go, bool includeInactive = false)
		{
			Component com = go.GetComponentInChildren (type, includeInactive);
			if (com == null) {
				com = go.AddComponent (type);
				if (com == null) {
					Debug.LogError ("CocoLoad->GetOrAddComponent: component [" + type.Name + "] can NOT be added.");
				}
			}

			return com;
		}

		public static Component GetOrAddComponentInChildren (System.Type type, Component component, bool includeInactive = false)
		{
			return GetOrAddComponentInChildren (type, component.gameObject, includeInactive);
		}

		#endregion


		#region Set Parent

		public enum TransStayOption
		{
			Non,
			Local,
			World
		}

		public static void SetParent (GameObject go, Transform parent, TransStayOption stayOption = TransStayOption.Non)
		{
			if (go == null) {
				return;
			}

			SetParent (go.transform, parent, stayOption);
		}

		public static void SetParent (Component component, Transform parent, TransStayOption stayOption = TransStayOption.Non)
		{
			if (component == null) {
				return;
			}

			Transform trans = component.transform;

			switch (stayOption) {
			case TransStayOption.Local:
				trans.SetParent (parent, false);
				break;
			case TransStayOption.World:
				trans.SetParent (parent);
				break;
			default:
				trans.SetParent (parent, false);
				trans.localPosition = Vector3.zero;
				trans.localRotation = Quaternion.identity;
				trans.localScale = Vector3.one;
				break;
			}
		}

		#endregion


		#region Instantiate GameObject/Component (maybe null)

		public static GameObject Instantiate (GameObject prefab, Transform parent = null, TransStayOption stayOption = TransStayOption.Non)
		{
			if (prefab == null) {
				return null;
			}

			GameObject go = Object.Instantiate (prefab);
			SetParent (go, parent, stayOption);
			return go;
		}

		public static GameObject Instantiate (string path, Transform parent = null, TransStayOption stayOption = TransStayOption.Non)
		{
			if (string.IsNullOrEmpty (path)) {
				return null;
			}

			GameObject prefab = ResourceManager.Load<GameObject> (path);
			return Instantiate (prefab, parent, stayOption);
		}

		public static T Instantiate<T> (GameObject prefab, Transform parent = null, TransStayOption stayOption = TransStayOption.Non) where T : Component
		{
			GameObject go = Instantiate (prefab, parent, stayOption);
			if (go == null) {
				return null;
			}

			return go.GetComponent<T> ();
		}

		public static T Instantiate<T> (string path, Transform parent = null, TransStayOption stayOption = TransStayOption.Non) where T : Component
		{
			if (string.IsNullOrEmpty (path)) {
				return null;
			}

			GameObject prefab = ResourceManager.Load<GameObject> (path);
			return Instantiate<T> (prefab, parent, stayOption);
		}

		#endregion


		#region Instantiate/Create GameObject/Component

		public static GameObject InstantiateOrCreate (GameObject prefab, Transform parent = null, TransStayOption stayOption = TransStayOption.Non)
		{
			GameObject go = prefab != null ? Object.Instantiate (prefab) : new GameObject ();
			SetParent (go, parent, stayOption);
			return go;
		}

		public static GameObject InstantiateOrCreate (string path, Transform parent = null, TransStayOption stayOption = TransStayOption.Non)
		{
			GameObject prefab = ResourceManager.Load<GameObject> (path);
			return InstantiateOrCreate (prefab, parent, stayOption);
		}

		public static T InstantiateOrCreate<T> (GameObject prefab, Transform parent = null, TransStayOption stayOption = TransStayOption.Non) where T : Component
		{
			GameObject go = InstantiateOrCreate (prefab, parent, stayOption);
			return GetOrAddComponent<T> (go);
		}

		public static T InstantiateOrCreate<T> (string path, Transform parent = null, TransStayOption stayOption = TransStayOption.Non) where T : Component
		{
			GameObject prefab = ResourceManager.Load<GameObject> (path);
			return InstantiateOrCreate<T> (prefab, parent, stayOption);
		}

		public static Component InstantiateOrCreate (System.Type type, GameObject prefab, Transform parent = null, TransStayOption stayOption = TransStayOption.Non)
		{
			GameObject go = InstantiateOrCreate (prefab, parent, stayOption);
			return GetOrAddComponent (type, go);
		}

		public static Component InstantiateOrCreate (System.Type type, string path, Transform parent = null, TransStayOption stayOption = TransStayOption.Non)
		{
			GameObject prefab = ResourceManager.Load<GameObject> (path);
			return InstantiateOrCreate (type, prefab, parent, stayOption);
		}

		#endregion


		#region Instantiate/Create GameObject/Component

		public static T InstantiateOrCreateInChildren<T> (GameObject prefab, Transform parent = null, TransStayOption stayOption = TransStayOption.Non, bool includeInactive = false) where T : Component
		{
			GameObject go = InstantiateOrCreate (prefab, parent, stayOption);
			return GetOrAddComponentInChildren<T> (go, includeInactive);
		}

		public static T InstantiateOrCreateInChildren<T> (string path, Transform parent = null, TransStayOption stayOption = TransStayOption.Non, bool includeInactive = false) where T : Component
		{
			GameObject prefab = ResourceManager.Load<GameObject> (path);
			return InstantiateOrCreateInChildren<T> (prefab, parent, stayOption, includeInactive);
		}

		public static Component InstantiateOrCreateInChildren (System.Type type, GameObject prefab, Transform parent = null, TransStayOption stayOption = TransStayOption.Non, bool includeInactive = false)
		{
			GameObject go = InstantiateOrCreate (prefab, parent, stayOption);
			return GetOrAddComponentInChildren (type, go, includeInactive);
		}

		public static Component InstantiateOrCreateInChildren (System.Type type, string path, Transform parent = null, TransStayOption stayOption = TransStayOption.Non, bool includeInactive = false)
		{
			GameObject prefab = ResourceManager.Load<GameObject> (path);
			return InstantiateOrCreateInChildren (type, prefab, parent, stayOption, includeInactive);
		}



		#endregion

        #region add assets

        public static T InstantiateOrCreateAssets<T>(string path) where T:Object{

            T target = ResourceManager.Load<T>(path);
            if (target == null)
            {
                Debug.LogError(string.Format("path is error or can not find \"{0}\" target", typeof(T)));
                return null;
            }

            return Object.Instantiate(target);
        }
        #endregion

	}
}
