#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;


public class EditMatTool : MonoBehaviour 
{
	[MenuItem("Tools/Mat_HSL_info")]
	static void BuildDress()
	{
		Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
		Dictionary<string, Material> t_Mat_Dict = new Dictionary<string, Material>();

		foreach(Object obj in SelectedAsset)
		{
			if(obj is Material)
			{
				t_Mat_Dict.Add(obj.name, (Material)obj);
			}
		}

		string info = string.Empty;
		for(int i=0; i<8; i++)
		{
			info += string.Format("Dictionary<int, Color> color_{0:D2} = new Dictionary<int, Color>();\n", i+1);

			for(int j=0; j<4; j++)
			{
				string name = string.Format("cakedesign_decoration_2d_{0:D3}_{1:D3}", i+1, j+1);  
				if(t_Mat_Dict.ContainsKey(name))
				{
					Material mat = t_Mat_Dict[name];
					float H = mat.GetFloat("_Hue");
					float S = mat.GetFloat("_Saturation");
					float L = mat.GetFloat("_Lightness");
					float F = mat.GetFloat("_MixingFactor");
					info += string.Format("color_{4:D2}.Add({5}, new Color({0}f, {1}f, {2}f, {3}f));\n", H, S, L, F, i+1, j+1);
				}
			}
			info += "\n\n";
		}
		Debug.LogError(info);
	}
}

#endif
