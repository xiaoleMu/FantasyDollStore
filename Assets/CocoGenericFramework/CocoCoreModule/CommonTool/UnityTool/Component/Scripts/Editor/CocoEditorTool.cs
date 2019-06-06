#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AddChild : ScriptableObject
{
//	[MenuItem ("CocoEditorTool/Transform/当前选择对象")]  
	static Transform GetCurrentSelect ()
	{ 
		Transform tParentTrant = Selection.activeTransform;
		return tParentTrant;
	}

	[MenuItem ("CocoEditorTool/Transform/当前选择对象路径")]  
	static void GetCurrentSelectPath ()
	{
		Transform tTrans = GetCurrentSelect ();
		string tPath = string.Empty;
		if (tTrans != null) {
			tPath = tTrans.gameObject.name;
			while (tTrans.parent != null) {
				tPath = tTrans.parent.name + "/" + tPath;
				tTrans = tTrans.parent;
			}
		} 
		Debug.LogError (tPath);
	}

	[MenuItem ("CocoEditorTool/Transform/当前对象下添加Image")]  
	static void AddImage ()
	{
		Transform tTrans = GetCurrentSelect ();
		if (tTrans != null) {
			GameObject tImage = new GameObject ("Image");
			tImage.AddComponent<Image> ();
			tImage.transform.SetParent (tTrans);
			tImage.transform.localPosition = Vector3.zero;
			tImage.transform.localEulerAngles = Vector3.zero;
			tImage.transform.localScale = Vector3.one;
		}
	}
}
#endif