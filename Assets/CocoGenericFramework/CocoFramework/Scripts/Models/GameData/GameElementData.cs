using UnityEngine;
using System.Collections;
using System;

namespace TabTale
{
	public enum GameElementType
	{
		Currency,
		Item,
		State
	}

	public class GameElementData : ICloneable
	{
		public GameElementType type;
		
		public string key;

		public int value;

		#region ICloneable implementation
		
		public object Clone ()
		{
			GameElementData c = new GameElementData (type,key,value);
			return c;
		}
		
		#endregion

		[Obsolete("for json mapper only",true)]
		public GameElementData ()
		{
			
		}

		public GameElementData (GameElementType type, string key, int value)
		{
			this.type = type;
			this.key = key;
			this.value = value;
		
		}

		public static GameElementData CreateCurrency(System.Enum key, int value)
		{
			return new GameElementData(GameElementType.Currency,key.ToString(),value);
		}

		public static GameElementData CreateItem(string key, int value)
		{
			return new GameElementData(GameElementType.Item,key,value);
		}

		public static GameElementData CreateState(string key, int value)
		{
			return new GameElementData(GameElementType.State,key,value);
		}
		
		public override string ToString ()
		{
			return string.Format ("[GameElementData: Type={0}, Key={1}, Value = {2}]", type, key, value);
		}
		
		public static bool operator == (GameElementData data1, GameElementData data2)
		{
			// Allow checking equality with null
			if(object.ReferenceEquals(data1, null))
			{
				return object.ReferenceEquals(data2, null);
			}

			return data1.type == data2.type && data1.key == data2.key;
		}
		
		public static bool operator != (GameElementData data1, GameElementData data2)
		{
			if(object.ReferenceEquals(data1, null))
			{
				return !object.ReferenceEquals(data2, null);
			}

			return !(data1.type == data2.type && data1.key == data2.key);
		}

		public static GameElementData operator + (GameElementData data1, GameElementData data2)
		{
			if (data1.type != data2.type) 
			{
				Debug.LogError("Tried to modify GameElementData of different types.");
				return data1;
			}
			if (data2.value < 0) 
			{
				Debug.LogError("Value cannot be negative");
				return data1;
			}
			
			data1.value += data2.value;
			
			return data1;
		}
		
		public static GameElementData operator - (GameElementData data1, GameElementData data2)
		{
			if (data1.type != data2.type) 
			{
				Debug.LogError("Tried to modify GameElementData of different types.");
				return data1;
			}			
			
			if (data2.value < 0) 
			{
				Debug.LogError("Value cannot be negative");
				return data1;
			}

			if (data1.value < data2.value) 
			{
				Debug.LogError("Not enough " +data1.key+ " of type "+data1.type);
				return data1;
			}
			
			
			data1.value -= data2.value;
			return data1;
		}
	}	
}