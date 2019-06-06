using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale {

	public static class ICollectionExtensions
	{

		public static ICollection<T> Clone<T>(this ICollection<T> collection) where T : System.ICloneable
		{
			return collection.Select(item => (T)item.Clone()).ToList();
		}

		public static string ArrayString<T>(this ICollection<T> collection)
		{
			if (collection == null) {
				return "null";
			}
			string res = "[";
			foreach (var item in collection) {
				res = res + item.ToString()+",";
			}
			res = res.Substring(0,res.Length-1)+"]"; //remove last comma.
			return res;
		}

		public static ICollection<S> Select<T, S>(this ICollection<T> collection, System.Func<T, S> selector)
		{
			IList<S> result = new List<S>();
			
			foreach(T t in collection)
			{
				result.Add(selector(t));
			}
			
			return result;
		}
		
		public static ICollection<S> OfType<T, S>(this ICollection<T> collection)
			where T : class
			where S : class
			{
				ICollection<S> result = new List<S>();
				
				foreach(T t in collection)
				{
					S s = t as S;
					if(s != null)
					{
						result.Add(s);
					}
				}
				
				return result;
			}

	}
}
