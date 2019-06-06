using UnityEngine;
using System.Collections;

namespace TabTale 
{
	public class Pair<TFirst, TSecond>
	{
		public TFirst First;
		public TSecond Second;

		public Pair(TFirst first, TSecond second)
		{
			this.First = first;
			this.Second = second;
		}

		public Pair()
		{

		}
	}
	
	[System.Serializable]
	/// <summary>
	/// A pair of strings is constantly used for mapping values, and is provided here
	/// without a Generic to minimize ios AOT problems.
	/// </summary>
	public class StringPair
	{
		public string First;
		public string Second;

		public StringPair()
		{
		}

		public StringPair(string first, string second)
		{
			this.First =  first;
			this.Second = second;
		}

		public override int GetHashCode ()
		{
			return First.GetHashCode() ^ Second.GetHashCode();
		}
	}
}
