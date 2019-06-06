using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace TabTale {

	public static class ReflectiveFactory<TInterface>
		where TInterface : class
	{
		static ReflectiveFactory()
		{
			if(!typeof(TInterface).IsInterface)
			{
				throw new ArgumentException("GetImplementros() only works on interfaces!");
			}
		}

		private static ICollection<Type> s_implementors;
		public static ICollection<Type> Implementors
		{
			get
			{
				if(s_implementors == null)
				{
					s_implementors = new List<Type>();
					foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
					{
						foreach(Type implementationCandidate in assembly.GetTypes())
						{
							Type interfaceImplementation = implementationCandidate.GetInterface(typeof(TInterface).Name);
							if(interfaceImplementation != null)
								s_implementors.Add(implementationCandidate);
						}
					}
				}

				return s_implementors;
			}
		}

		public static TInterface CreateProvider(string className, object[] parameters)
		{
			foreach(Type implementor in Implementors)
			{
				if(implementor.Name == className)
				{
					return CreateProvider(implementor, parameters);
				}
			}
			
			return null;
		}

		public static TInterface CreateProvider(object[] parameters)
		{
			foreach(Type implementor in Implementors)
			{
				TInterface provider = CreateProvider(implementor, parameters);
				if(provider != null)
					return provider;
			}

			return null;
		}

		public static TInterface CreateProvider(Type implementor, object[] parameters)
		{
			Type[] parameterTypes = new Type[parameters.Length];
			for(int i=0;i<parameters.Length;i++)
			{
				parameterTypes[i] = parameters[i].GetType();
			}

			ConstructorInfo constructor = implementor.GetConstructor(parameterTypes);
			if(constructor == null)
				return null;

			return constructor.Invoke(parameters) as TInterface;
		}

		public static TInterface FindProvider()
		{
			TInterface unityProvider = UnityEngine.Object.FindObjectOfType(typeof(TInterface)) as TInterface;
			if(unityProvider != null)
				return unityProvider;

			return CreateProvider(new object[] {});
		}
	}
}
