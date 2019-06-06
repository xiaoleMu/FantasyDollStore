#if UNTIY_ANDROID && NOT_USED
using IntPtr = System.IntPtr;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;



namespace TabTale.Plugins.PSDK {

	public interface IPsdkAndroidJavaObject {
		 void Call(string methodName, params object[] args);
		 T Call<T>(string methodName, params object[] args);
		 T Get<T>(string methodName);
		 void Set<T>(string methodName, T t);
		 void SetStatic<T>(string methodName, T t);
		 T GetStatic<T>(string methodName);
		 T CallStatic<T>(string methodName, params object[] args);
		 void Dispose();
		 IntPtr GetRawClass();
		 IntPtr GetRawObject();
		IPsdkAndroidJavaObject GetStaticAndroidJavaObject(string methodName);
		IPsdkAndroidJavaObject CallStaticAndroidJavaObject(string methodName, params object[] args);
		IPsdkAndroidJavaObject CallAndroidJavaObject(string methodName, params object[] args);
		object GetAndroidObject();
	}

	public interface IPsdkAndroidJavaClass : IPsdkAndroidJavaObject {
	}
	
	public class PsdkAndroidJavaObjectStub :  IPsdkAndroidJavaObject {
		public PsdkAndroidJavaObjectStub(string className,params object[] values) {;}
		public void Call(string methodName, params object[] args) {;}
		public T Call<T>(string methodName, params object[] args) { return default(T);}
		public T Get<T>(string methodName) { return default(T);}
		public void Set<T>(string methodName, T t) { ;}
		public void SetStatic<T>(string methodName, T t) { ;}
		public T GetStatic<T>(string methodName) { return default(T);}
		public T CallStatic<T>(string methodName, params object[] args) { return default(T);}
		public void Dispose() {;}
		public IntPtr GetRawClass() {return new IntPtr();}
		public IntPtr GetRawObject() {return new IntPtr();}
		public IPsdkAndroidJavaObject GetStaticAndroidJavaObject(string methodName) {return null;}
		public IPsdkAndroidJavaObject CallStaticAndroidJavaObject(string methodName, params object[] args) {return null;}
		public IPsdkAndroidJavaObject CallAndroidJavaObject(string methodName, params object[] args) {return null;}
		public object GetAndroidObject() {return null;}
	}
	
	public class PsdkAndroidJavaClassStub : IPsdkAndroidJavaClass {
		public PsdkAndroidJavaClassStub(string className) {;}
		public void Call(string methodName, params object[] args) {;}
		public T Call<T>(string methodName, params object[] args) { return default(T);}
		public T Get<T>(string methodName) { return default(T);}
		public void Set<T>(string methodName, T t) { ;}
		public void SetStatic<T>(string methodName, T t) { ;}
		public T GetStatic<T>(string methodName) { return default(T);}
		public T CallStatic<T>(string methodName, params object[] args) { return default(T);}
		public void Dispose() {;}
		public IntPtr GetRawClass() {return new IntPtr();}
		public IntPtr GetRawObject() {return new IntPtr();}
		public IPsdkAndroidJavaObject GetStaticAndroidJavaObject(string methodName) {return null;}
		public IPsdkAndroidJavaObject CallStaticAndroidJavaObject(string methodName, params object[] args) {return null;}
		public IPsdkAndroidJavaObject CallAndroidJavaObject(string methodName, params object[] args) {return null;}
		public object GetAndroidObject() {return null;}
	}

	public class PsdkAndroidJavaObjectUtil {
		public static IPsdkAndroidJavaObject CreateObject(string methodName, params object[] args) {
			return CreateAndroidObject<IPsdkAndroidJavaObject>("PsdkAndroidJavaObject",methodName, args);
		}
		
		public static IPsdkAndroidJavaClass CreateClass(string methodName) {
			object[] args = new object[0];
			return CreateAndroidObject<IPsdkAndroidJavaClass>("PsdkAndroidJavaClass",methodName, args);
		}
		
		public static T CreateAndroidObject<T>(string className, string methodName, params object[] args) {
//				System.Type psdkMgrType = GetType();
//			if (psdkMgrType == null) {
//				Debug.LogError("psdkMgrType NULL !!");
//				return default(T);
//			}
//			System.Type serviceType = Types.GetType("TabTale.Plugins.PSDK." + className,psdkMgrType.Assembly.ToString());
//			//			Debug.Log("PSDKMgr::GettingServiceByReflection: " + className + " type " + serviceType + " ! assembly:" + psdkMgrType.Assembly.ToString());
//			if (serviceType == null) {
			System.Type serviceType = null;
				System.Type[] services = getTypeByName("TabTale.Plugins.PSDK." + className);
				if (services.Length > 0)
					serviceType = services[0];
//			} 
			if (serviceType == null) {
				Debug.Log("PSDK CreateAndroidObject: " + className + " type NULL !");
				return default(T);
			} 
			// try instantiating the class by reflection;
			System.Type[] types;
			if (className == "PsdkAndroidJavaObject") {
				object[] objArray = new object[1];
				types = new System.Type[2];
				types[0]  = typeof(System.String);
				types[1]  = objArray.GetType();
			}
			else {
				types = new System.Type[1];
				types[0]  = typeof(System.String);
			}
			ConstructorInfo ctor = serviceType.GetConstructor(types);
			if(ctor != null)
			{
				Debug.Log ("Found the construcotor of " + serviceType);
				object[] newArgs = new object[args.Length+1];
				newArgs[0] = methodName;
				for(int i=0; i < args.Length; ++i) {
					newArgs[i+1] = args[i];
				}

				object service = null;
				if (className == "PsdkAndroidJavaObject") {
					service = ctor.Invoke(new object[] { methodName, args });
				}
				else {
					service = ctor.Invoke(new object[] { methodName });
				}
				if (service is T) {
					return (T)service;
				} 
				else 
				{
					try {
						return (T)System.Convert.ChangeType(service, typeof(T));
					} catch (System.InvalidCastException) {
						return default(T);
					}
				}
			}
			
			return default(T);
		}

		private static System.Type[] getTypeByName(string classFullName)
		{
			List<System.Type> returnVal = new List<System.Type>();
			
			foreach (Assembly a in System.AppDomain.CurrentDomain.GetAssemblies())
			{
				System.Type[] assemblyTypes = a.GetTypes();
				for (int j = 0; j < assemblyTypes.Length; j++)
				{
					if (assemblyTypes[j].FullName == classFullName)
					{
						returnVal.Add(assemblyTypes[j]);
					}
				}
			}
			
			return returnVal.ToArray();
		}
	}

}
#endif