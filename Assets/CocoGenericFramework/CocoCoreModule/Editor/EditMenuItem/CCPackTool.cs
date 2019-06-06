#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using LitJson;
using System.IO;
using UnityEditor;

public class CCPackTool : MonoBehaviour
{
	[MenuItem ("Coco Common/CreateEditJson/Dress")]
	public static void Test()
	{
		JsonData baseInfo = new JsonData();
		baseInfo["category"] = "Basic_Body";
		baseInfo["iapConfigId"] = "com.cocoplay.cocosupermodel.allitems";

		JsonData items = new JsonData();
		string category = "basic";
		for(int i =0; i<=10; i++)
		{
			JsonData itemInfo = new JsonData();
			JsonData asset = new JsonData();
			string fileName = string.Format("{0}_{1:D3}", category, i);
			asset["model"] = fileName + ".FBX";
			asset["texture"] = fileName + ".tga";
			asset["icon"] = fileName + ".png";
			itemInfo["assetDatas"] = asset;
			items.Add(itemInfo);
		}

		JsonData allData = new JsonData();
		allData["base"] = baseInfo;
		allData["items"] = items;

		string file = string.Format("dress_model_{0}.json",category);
		string path = "Assets//DressAssets//Models//" + category + "//" + file;
		RwriteConfigFile(path, allData.ToJson());
	}

	public static void RwriteConfigFile(string path, string info)
	{
		if(File.Exists(path))
			File.Delete(path); 
		
		FileInfo t = new FileInfo(path);     
		StreamWriter sw = t.CreateText();
		sw.Write (info);
		sw.Close();
		sw.Dispose();
		Debug.Log ("write finish " + path);
	}
}
#endif
