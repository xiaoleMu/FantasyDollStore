using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public static class ComponentExtensions
	{
		public static IEnumerable<TComponent> GetComponentsInSubtree<TComponent>(this Component _this)
			where TComponent : Component
		{
			return _this.transform.GetComponentnsInSubtree<TComponent>();
		}

		public static TComponent DontDestroyOnLoad<TComponent>(this TComponent _this)
			where TComponent : Component
		{
			GameObject.DontDestroyOnLoad(_this.gameObject);
			return _this;
		}
	}
}