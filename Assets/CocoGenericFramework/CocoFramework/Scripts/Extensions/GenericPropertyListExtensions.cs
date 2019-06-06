using System;
using System.Collections.Generic;

namespace TabTale
{
	public static class GenericPropertyListExtensions
	{
		#region Getters

		public static bool GetBoolProperty(this List<GenericPropertyData> properties, string id)
		{
			int index = properties.GetIndexOrCreate (id, PropertyType.Bool, default(bool).ToString());
			return properties[index].value == "true";
		}

		public static float GetIntProperty(this List<GenericPropertyData> properties, string id)
		{
			int index = properties.GetIndexOrCreate (id, PropertyType.Int, default(int).ToString());
			return float.Parse(properties[index].value);
		}

		public static float GetFloatProperty(this List<GenericPropertyData> properties, string id)
		{
			int index = properties.GetIndexOrCreate (id, PropertyType.Float, default(float).ToString());
			return float.Parse(properties[index].value);
		}

		public static string GetStringProperty(this List<GenericPropertyData> properties, string id)
		{
			int index = properties.GetIndexOrCreate (id, PropertyType.String, default(string));
			return properties[index].value;
		}

		public static string GetImageProperty(this List<GenericPropertyData> properties, string id)
		{
			return GetStringProperty(properties, id);
		}

		#endregion

		#region Setters
		public static void SetProperty(this List<GenericPropertyData> properties, string id, bool value)
		{
			string valueToSet = value ? "true" : "false";
			int index = properties.GetIndexOrCreate (id, PropertyType.Bool,valueToSet);
			properties[index].value = valueToSet;
		}
			
		public static void SetProperty(this List<GenericPropertyData> properties, string id, PropertyType pType, string value)
		{
			int index = properties.GetIndexOrCreate (id, pType,value);
			properties[index].value = value;
		}

		#endregion

		private static int GetIndexOrCreate(this ICollection<GenericPropertyData> properties, string id, PropertyType pType, string defaultVal)
		{
			int index = properties.FirstIndex(p => p.id == id);
			if(index == -1)
			{
				properties.Add(new GenericPropertyData(id, pType, defaultVal));
				index = properties.FirstIndex(p => p.id == id);
			}

			return index;
		}
	}
}

