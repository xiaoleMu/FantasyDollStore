using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;

namespace TabTale {

	public static class ReflectionUtils
	{
		public static object Create(string typeName)
		{
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type type = assembly.GetTypes().First(t => t.Name == typeName);
				if(type != null)
				{
					ConstructorInfo constructorInfo = type.GetConstructor(Type.EmptyTypes);
					if(constructorInfo != null)
					{
						return constructorInfo.Invoke(new object[] {});
					}
				}
			}
			
			return null;
		}
		
		public static ICollection<Type> FindTypes(string name)
		{
			IList<Type> types = new List<Type>();
			
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type type = assembly.GetTypes().First(t => t.Name == name);
				if(type != null)
				{
					types.Add(type);
				}
			}
			
			return types;
		}

		public static ICollection<Type> FindTypes(string name, Type baseType)
		{
			IList<Type> types = new List<Type>();
			
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type type = assembly.GetTypes().First(t => t.Name == name && (t.IsSubclassOf(baseType)));
				if(type != null)
				{
					types.Add(type);
				}
			}
			
			return types;
		}
	}
}