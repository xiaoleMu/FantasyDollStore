using UnityEngine;
using System.Collections;

namespace TabTale {

	public static class ComponentUtils
	{
		public static TComponent CreateComponentObject<TComponent>(bool dontDestroy = true, string name = "")
			where TComponent : Component
		{
			GameObject go = new GameObject(name == "" ? typeof(TComponent).Name : name);
			TComponent component = go.AddComponent<TComponent>();
			if(dontDestroy)
				GameObject.DontDestroyOnLoad(go);

			return component;
		}
	}
}
