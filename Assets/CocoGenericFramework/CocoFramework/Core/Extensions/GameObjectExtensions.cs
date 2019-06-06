using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale {

	public static class GameObjectExtensions
	{
		public static void MoveToLayer(this GameObject _this, int layer)
		{
			Stack<GameObject> stack = new Stack<GameObject>();
			stack.Push(_this);

			while(stack.Count > 0)
			{
				GameObject current = stack.Pop();
				current.layer = layer;

				foreach(GameObject child in current.transform.GetChildren().Select(t => t.gameObject))
				{
					stack.Push(child);
				}
			}

		}
		public static GameObject DontDestroyOnLoad(this GameObject go)
		{
			GameObject.DontDestroyOnLoad(go);
			return go;
		}

		public static TComponent AddMissingComponent<TComponent> (this GameObject go) 
			where TComponent : Component
		{
#if UNITY_FLASH
			object comp = go.GetComponent<T>();
#else
			TComponent comp = go.GetComponent<TComponent>();
#endif
			if (comp == null)
			{
#if UNITY_EDITOR
				if (!Application.isPlaying)
					UnityEditor.Undo.RecordObject(go, "Add " + typeof(TComponent));
					UnityEditor.EditorUtility.SetDirty(go);
#endif
				comp = go.AddComponent<TComponent>();
			}
#if UNITY_FLASH
			return (T)comp;
#else
			return comp;
#endif
		}

		public static TComponent GetComponentOrInterface<TComponent> (this GameObject go) {
			return go.GetComponents<MonoBehaviour> ()
				.Where (c => c.GetType ().GetInterfaces ().Contains (typeof(TComponent)) || typeof(TComponent).IsAssignableFrom (c.GetType ())).Cast<TComponent> ()
					.FirstOrDefault ();
		}

		public static TComponent[] GetComponentsInImmediateChildren<TComponent> (this GameObject go) where TComponent:Component {
			
			List<TComponent> ret=new List<TComponent>();
			
			for (int i = 0; i < go.transform.childCount; i++) {
				TComponent component = go.transform.GetChild(i).GetComponent<TComponent>();
				if(component!=null)
					ret.Add(component);
			}
			return ret.ToArray();
		}

		public static TComponent GetComponentInImmediateChildren<TComponent> (this GameObject go) where TComponent:Component {
			TComponent[] list = GetComponentsInImmediateChildren<TComponent>(go);
			if (list.Length == 0){
				return null;
			}
			return list[0];
		}


	}


}