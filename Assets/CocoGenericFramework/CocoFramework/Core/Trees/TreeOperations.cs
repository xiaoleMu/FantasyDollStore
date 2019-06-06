using UnityEngine;
using System.Collections;

namespace TabTale
{
	public static class TreeOperations
	{
		public static Transform Clone(this Transform _this, bool keepName = false)
		{
			if(_this == null)
				return null;

			GameObject go = GameObject.Instantiate(_this.gameObject) as GameObject;

			if(keepName)
				go.name = _this.name;

			return go.transform;
		}
	}
}