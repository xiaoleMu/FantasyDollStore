#if UNITY_ANDROID 
using UnityEngine;
using System.Collections;
using System.Reflection;
using TabTale.Plugins.PSDK;

public class UnitySyncDelegate : AndroidJavaProxy {

	public UnitySyncDelegate() : base("com.tabtale.publishingsdk.services.UnitySyncDelegate") { }

	public void sendSyncMessage(string methodName,string message)
	{
		Debug.Log("UnitySyncDelegate::sendSyncMessage - " + methodName + " - " + message);
		if(methodName != null){
			MethodInfo mi = PsdkEventSystem.Instance.GetType ().GetMethod (methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if(mi != null){
				mi.Invoke (PsdkEventSystem.Instance,message.Length > 0 ? new object[]{ message } : null);
			}
		}
	}

	public string sendSyncMessageWithReturn(string methodName,string message)
	{
		Debug.Log("UnitySyncDelegate::sendSyncMessageWithReturn - " + methodName + " - " + message);
		if(methodName != null){
			MethodInfo mi = PsdkEventSystem.Instance.GetType ().GetMethod (methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if(mi != null){
				try {
					object retObj = mi.Invoke (PsdkEventSystem.Instance,message.Length > 0 ? new object[]{ message } : null);
					if(retObj != null && retObj.GetType() == typeof(string)){
						return (string)retObj;
					}
					else {
						Debug.Log("UnitySyncDelegate::sendSyncMessageWithReturn - return object is not of type string or null.");
					}
				}
				catch (System.Exception e){
					Debug.Log("UnitySyncDelegate::sendSyncMessageWithReturn - invocation failed. Exception - " + e.Message + ", " + e.StackTrace);
				}
			}
			else {
				Debug.Log("UnitySyncDelegate::sendSyncMessageWithReturn - method info is null.");
			}

		}
		else {
			Debug.Log("UnitySyncDelegate::sendSyncMessageWithReturn - methodName is null.");
		}
		return null;
	}
}
#endif
