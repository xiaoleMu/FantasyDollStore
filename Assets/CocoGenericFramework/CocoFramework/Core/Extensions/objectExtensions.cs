using UnityEngine;
using System.Collections;

namespace TabTale
{
	public static class ObjectExtensions
	{
		public static bool IsNumber(this object obj)
		{
			if (Equals(obj, null))
			{
				return false;
			}
			
			System.Type objType = obj.GetType();
			objType = System.Nullable.GetUnderlyingType(objType) ?? objType;
			
			if (objType.IsPrimitive)
			{
				return objType != typeof(bool) && 
					objType != typeof(char) && 
						objType != typeof(System.IntPtr) && 
						objType != typeof(System.UIntPtr);
			}
			
			return objType == typeof(decimal);
		}
	}
}
