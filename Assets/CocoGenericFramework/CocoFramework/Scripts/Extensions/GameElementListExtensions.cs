using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale 
{	
	public static class GameElementListExtensions
	{
		public static void Increase(this List<GameElementData> _this, GameElementData amount)
		{
			int gameElementIndex = _this.FirstIndex(element => element.key == amount.key && element.type == amount.type);
			if(gameElementIndex == -1)
			{
				_this.Add(amount);
				Debug.Log ("<color=yellow>GameElementListExtensions - first</color> " + amount.value.ToString());
			}
			else
			{
				_this[gameElementIndex].value += amount.value;
				Debug.Log ("<color=yellow>GameElementListExtensions</color> " + amount.value.ToString());
			}
		}

		public static void Set(this List<GameElementData> _this, GameElementData amount)
		{
			int gameElementIndex = _this.FirstIndex(element => element.key == amount.key && element.type == amount.type);
			if(gameElementIndex == -1)
			{
				_this.Add(amount);
			}
			else
			{
				_this[gameElementIndex].value = amount.value;
			}
		}

		public static int GetIndex(this List<GameElementData> _this,GameElementData gameElementData)
		{
			return _this.FirstIndex( element => element.key == gameElementData.key);
		}
		
		public static int GetIndex(this List<GameElementData> _this,string key)
		{
			return _this.FirstIndex( element => element.key == key);
		}
	
	}
}