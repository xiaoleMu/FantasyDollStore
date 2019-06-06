using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Enum)]  
public class EnumDesc : System.Attribute  
{  
	public List<string> DescList;
	public string Desc { get; private set; }  
//	public EnumDesc(string arg)
//	{
//		this.Desc = arg;
//	}

	public EnumDesc(params string[] strArray)
	{
		DescList = new List<string> ();
		foreach (string str in strArray)
		{
			DescList.Add(str);
		}
	}
} 
public class CCEnum  
{  
	private static Dictionary<System.Type, Dictionary<string, List<string>>> cache = new Dictionary<System.Type, Dictionary<string,  List<string>>>();  
	
	public static string Get(object p, int index = 0)  
	{  
		var type = p.GetType();  
		if (!cache.ContainsKey(type))  
		{  
			Cache(type);  
		}  
		var fieldNameToDesc = cache[type];  
		var fieldName = p.ToString();
		if(!fieldNameToDesc.ContainsKey(fieldName))
		{
			Debug.LogError(string.Format("Can not found such desc for field `{0}` in type `{1}`", fieldName, type.Name));
			return "";
		}
		return fieldNameToDesc[fieldName][index];
	}  
	
	private static void Cache(System.Type type)  
	{  
		var dict = new Dictionary<string,  List<string>>();  
		cache.Add(type, dict);  
		var fields = type.GetFields();  
		foreach (var field in fields)  
		{  
			var objs = field.GetCustomAttributes(typeof(EnumDesc), true);  
			if (objs.Length > 0)  
			{
				EnumDesc pDesc = (EnumDesc)objs[0];
				List<string> descList = new List<string>(pDesc.DescList.ToArray());
				dict.Add(field.Name, descList);  
			}  
		}  
	}  
}  
