#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using LitJson;
using System.IO;

public class EditeSpriteAnimation
{
	[MenuItem("Coco Common/SpritePlayer/BuildText")]
	static public void BuildText()
	{
		Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
		List<Texture> _List = new List<Texture>();

		foreach(Object obj in SelectedAsset)
		{
			if(obj is Texture)
			{
				_List.Add((Texture)obj);
			}
		}
		_List.Sort(delegate(Texture x, Texture y)
			{
				var array_x  = x.name.Split('_');
				int index_x = int.Parse(array_x[array_x.Length -1]);

				var array_y  = x.name.Split('_');
				int index_y = int.Parse(array_x[array_y.Length -1]);
				int index = index_y - index_x;
				return x.name.CompareTo(y.name);
				return index_y - index_x;
			});

		if(_List.Count == 0)
			return;
		
		var sprite_01 = _List[0];
		string filePath = EditorUtility.GetAssetPath(sprite_01);
		var path_array = filePath.Split('/');

		int Resources_Index = 0;
		for(int i=0; i<path_array.Length; i++)
		{
			string key = path_array[i];
			if(key == "Resources")
			{
				Resources_Index = i;
				break;
			}
		}

		string path = "";
		for(int i=Resources_Index; i<path_array.Length-1; i++)
		{
			string key = path_array[i];
			path = key + "/";
		}

		JsonData jsonInfo = new JsonData();
		foreach(var obj in _List)
		{
			jsonInfo.Add(obj.name);
		}

		string fileName = sprite_01.name.Substring(0, sprite_01.name.LastIndexOf("_")) + ".json";
		string savePath = filePath.Substring(0, filePath.LastIndexOf("/"));
		savePath = savePath.Substring(0, savePath.LastIndexOf("/"));
		savePath = savePath.Substring(0, savePath.LastIndexOf("/"));
		CCPackTool.RwriteConfigFile(savePath, fileName);
	}
}
#endif

