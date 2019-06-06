#undef UNITY_EDITOR
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

using System.Collections.Generic;

//[MenuItem("Coco Common/Unibill/Set Platform")]

//using Newtonsoft.Json;


public class CCEditMenuItem : MonoBehaviour 
{
	//	[MenuItem("Coco Common/Unibill/Set Platform")]
	//	public static void SetPlatform()
	//	{
	//		UnibillConfiguration pConfig;
	//		using (TextReader reader = File.OpenText(InventoryPostProcessor.UNIBILL_JSON_INVENTORY_PATH)) {
	//			pConfig = new UnibillConfiguration(reader.ReadToEnd(), Application.platform, new Uniject.Impl.UnityLogger());
	//		}
	//
	//		pConfig.AndroidBillingPlatform = BillingPlatform.AmazonAppstore;
	//
	//		using (StreamWriter o = new StreamWriter(InventoryPostProcessor.UNIBILL_JSON_INVENTORY_PATH)) {
	//			var json = JsonConvert.SerializeObject(pConfig.Serialize(), Newtonsoft.Json.Formatting.Indented);
	//			o.Write(json);
	//		}
	//	}
	[MenuItem("Coco Common/Shader/Set BanTou Shader")]
	static public void SetHalfShader()
	{
		Object[] SelectedAsset = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);
		foreach(GameObject obj in SelectedAsset)
		{
			MeshRenderer render = obj.GetComponent<MeshRenderer>();
			render.sharedMaterial.shader = Shader.Find("CUSTOM/Toon/Transparent (Queue Deferred)");
		}
	}

	[MenuItem("Coco Common/Shader/Set CutTou Shader")]
	static public void SetAllShader()
	{
		Object[] SelectedAsset = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);
		foreach(GameObject obj in SelectedAsset)
		{
			MeshRenderer render = obj.GetComponent<MeshRenderer>();
			render.sharedMaterial.shader = Shader.Find("CUSTOM/Toon/Transparent_Cutout");
		}
	}

	[MenuItem("Coco Common/Build/dress prefabs")]
	static public void BuildDressPrefabs()
	{
		Object[] SelectedAsset = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);
		foreach(GameObject obj in SelectedAsset)
		{
			string name = obj.name;
			int index = name.IndexOf('_');
			if(index < 0)
				Debug.LogError("wrong name = " + obj.name);

			string category = name.Substring(0, index);

			CreateDressPrefab(obj, category, name);
		}
	}

	static public Object CreatePrefab(GameObject go, string sPath, string name)
	{
		if(!Directory.Exists(sPath))
		{
			Directory.CreateDirectory(sPath);
		}
		
		//先创建一个空的预制物体
		//预制物体保存在工程中路径，可以修改("Assets/" + name + ".prefab");
		string filePath = sPath + name + ".prefab";
		if(File.Exists(filePath))
			File.Delete(filePath); 
		
		Object tempPrefab = EditorUtility.CreateEmptyPrefab(sPath + "/" + name + ".prefab");
		//然后拿我们场景中的物体替换空的预制物体
		tempPrefab = EditorUtility.ReplacePrefab(go, tempPrefab);
		//返回创建后的预制物体
		return tempPrefab;
	}

	static Object CreateDressPrefab(GameObject go, string category, string name)
	{
		string sPath = "Assets/_Game/Models/Dress/Resources/" + category;
		if(!Directory.Exists(sPath))
		{
			Directory.CreateDirectory(sPath);
		}

		//先创建一个空的预制物体
		//预制物体保存在工程中路径，可以修改("Assets/" + name + ".prefab");
		string filePath = sPath + name + ".prefab";
		if(File.Exists(filePath))
			File.Delete(filePath); 

		Object tempPrefab = EditorUtility.CreateEmptyPrefab(sPath + "/" + name + ".prefab");
		//然后拿我们场景中的物体替换空的预制物体
		tempPrefab = EditorUtility.ReplacePrefab(go, tempPrefab);
		//返回创建后的预制物体
		return tempPrefab;
	}

	[MenuItem("Coco Common/Build/dress prefabs with mat")]
	static public void BuildDressPrefabsWithMat()
	{

		Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
		List<GameObject> t_FBX_List = new List<GameObject>();
		List<Material> t_Mat_List = new List<Material>();

		foreach(Object obj in SelectedAsset)
		{
			if(obj is Material)
				t_Mat_List.Add((Material)obj);
			
			if(obj is GameObject)
				t_FBX_List.Add((GameObject)obj);
		}
			
		string tColorConfig = string.Empty;

		foreach(var go in t_FBX_List)
		{
			for(int i = 1; i < 6; i++)
			{
				string name = string.Format("{0}_color_{1:D3}", go.name, i);
				GameObject tGo = Instantiate(go) as GameObject;
				SkinnedMeshRenderer tRender = tGo.GetComponentInChildren<SkinnedMeshRenderer>();
				Material[] mats = new Material[tRender.sharedMaterials.Length];
				for(int j = 0; j < mats.Length; j++)
				{
					string matName = string.Format("{0}_part_{1:D2}_colour_{2:D2}", go.name, j + 1, i);
					foreach(var ttMat in t_Mat_List)
					{
						if(ttMat.name == matName)
						{
							mats[j] = ttMat as Material;
							break;
						}
					}
				}
				tRender.sharedMaterials = mats;
				string category = go.name.Substring(0, go.name.IndexOf('_'));
				CreateDressPrefab(tGo, category, name);
				DestroyImmediate(tGo);
			}
		}
		EditorApplication.SaveAssets();
	}

	[MenuItem("Coco Common/Build/home")]
	static public void BuildHome()
	{
		Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
		List<GameObject> t_FBX_List = new List<GameObject>();
		Dictionary<string, Material> t_Mat_Dict = new Dictionary<string, Material>();
		
		foreach(Object obj in SelectedAsset)
		{
			string name = obj.name;
			if(obj is Material)
			{
				Material mat = obj as Material;
				t_Mat_Dict.Add(mat.name, mat);
			}
			
			if(obj is GameObject)
				t_FBX_List.Add((GameObject)obj);
		}

		foreach(var fbxObj in t_FBX_List)
		{
			string assetPath =  EditorUtility.GetAssetPath(fbxObj);
			string DirectoryPath = SubStringFromEnd(assetPath, '/');
			DirectoryPath =  SubStringFromEnd(DirectoryPath, '/');
			DirectoryPath += "/Resources";

			int colorcount = 3;
			if(fbxObj.name.Contains("wallpaper") || fbxObj.name.Contains("floor"))
				colorcount = 1;

			for(int i = 1; i <= colorcount; i++)
			{
				GameObject tGo = Instantiate(fbxObj) as GameObject;
				MeshRenderer mesh = tGo.GetComponent<MeshRenderer>();
				int count = mesh.sharedMaterials.Length;

				Material[] mats = new Material[mesh.sharedMaterials.Length];
				for(int j = 0; j < mats.Length; j++)
				{
					Material enableMaterial;
					string matNameEnable = string.Format("{0}_part_{1:D2}_colour_{2:D2}", fbxObj.name, j + 1, 1);
					string matName = string.Format("{0}_part_{1:D2}_colour_{2:D2}", fbxObj.name, j + 1, i);

					if(t_Mat_Dict.ContainsKey(matName))
						mats[j] = t_Mat_Dict[matName];
					else if(t_Mat_Dict.ContainsKey(matNameEnable))
						mats[j] = t_Mat_Dict[matNameEnable];
					else
						continue;
				}
				mesh.sharedMaterials = mats;

				string PrefabName = GetHomeFileName(DirectoryPath, fbxObj.name, i);
				CreatePrefab(tGo, DirectoryPath, PrefabName);
				DestroyImmediate(tGo);
			}
		}
		EditorApplication.SaveAssets();
	}

	static public string SubStringFromEnd(string str, char ch)
	{
		return str.Substring(0,str.LastIndexOf('/', str.Length-1));
	}

	static string GetHomeFileName(string path, string objName, int colorIndex)
	{
		#region homename
		string homeName = "";
		if(path.Contains("home_01"))
		{
			homeName = "home_01";
		}
		else if(path.Contains("home_02"))
		{
			homeName = "home_02";
		}
		else if(path.Contains("home_03"))
		{
			homeName = "home_03";
		}
		else if(path.Contains("home_04"))
		{
			homeName = "home_04";
		}
		#endregion

		#region filename
		string middle = "";
		string[] fileSplit = objName.Split('_');
		if(fileSplit.Length == 2)
		{
			middle = string.Format("{0}_01", fileSplit[0]);
		}
		else
		{
			int index = int.Parse(fileSplit[fileSplit.Length-1]);
			middle = string.Format("{0}_{1:D2}", fileSplit[0], index);
		}

		#endregion
		string colorName = string.Format("color_{0:D2}", colorIndex);

		if(objName.Contains("wallpaper") || objName.Contains("floor"))
		{
			string fileName = string.Format("{0}_{1}", homeName, middle);
			return fileName;
		}
		else
		{
			string fileName = string.Format("{0}_{1}_{2}", homeName, middle, colorName);
			return fileName;
		}
	}
}
#endif
