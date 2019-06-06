using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public static class ListUtils
	{
		public static void Shuffle<T>(IList<T> list)  
		{  
			System.Random rng = new System.Random();  
			int n = list.Count;  
			while (n > 1) 
			{ 
				n--;  
				int k = rng.Next(n + 1);  
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}
	}
}
