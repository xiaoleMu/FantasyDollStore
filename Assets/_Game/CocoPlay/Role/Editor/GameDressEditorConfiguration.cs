using UnityEditor;
using System.Collections.Generic;
using CocoPlay;

namespace Game
{
	[InitializeOnLoad]
	public class GameDressEditorConfiguration
	{
		static GameDressEditorConfiguration ()
		{
			CocoDressEditWindow.onConfigGenerationStarted += OnConfigGenerationStarted;
			CocoDressEditWindow.onConfigGenerationEnded += OnConfigGenerationEnd;
			CocoDressEditWindow.onConfigRoleDressItemBeforeRandomSorting += OnConfigRoleDressItemBeforeRandomSorting;
			CocoDressEditWindow.onConfigRoleDressItemAfterRandomSorting += OnConfigRoleDressItemAfterRandomSorting;
		}

		public static void OnConfigGenerationStarted ()
		{
		}

		public static void OnConfigGenerationEnd (CocoAssetConfigHolder AssetHolder){
			foreach (CocoDressItemHolder itemHolder in AssetHolder.ItemHolderDic.Values){
				if (itemHolder.id.Contains ("body")){
					itemHolder.modelHolders[0].assetPath = "role/body/common/models/body_001.FBX";
					itemHolder.modelHolders[0].id = itemHolder.id;
				}

//				if (itemHolder.id.Contains ("arm")){
//					itemHolder.modelHolders[0].assetPath = "role/arm/common/models/arm_001.FBX";
//					itemHolder.modelHolders[0].id = itemHolder.id;
//				}
//
//				if (itemHolder.id.Contains ("head")){
//					itemHolder.modelHolders[0].assetPath = "role/head/common/models/head_001.FBX";
//					itemHolder.modelHolders[0].id = itemHolder.id;
//				}
//
//				if (itemHolder.id.Contains ("leg")){
//					itemHolder.modelHolders[0].assetPath = "role/leg/common/models/leg_001.FBX";
//					itemHolder.modelHolders[0].id = itemHolder.id;
//				}
			}
		}

		public static void OnConfigRoleDressItemBeforeRandomSorting (CocoRoleDressItemHolder itemHolder)
		{
//			if (itemHolder.)
		}

		public static void OnConfigRoleDressItemAfterRandomSorting (CocoRoleDressItemHolder itemHolder)
		{
		}
			
	}
}