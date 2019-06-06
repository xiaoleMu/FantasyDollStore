using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale 
{
	public static class IListExtensions
	{
		public static void RemoveAll<T>(this IList<T> list, System.Func<T, bool> predicate)
		{
			for(int i = list.Count - 1; i >= 0; i--)
			{
				if(predicate(list[i]))
					list.RemoveAt(i);
			}
		}

        public static T GetRandomItem<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

		public static IEnumerable<T> Shuffle<T>(this IList<T> _this)  
		{  
			IList<T> newList = _this.ToList();
			ListUtils.Shuffle(newList);
			return newList;
		}
	}
}
