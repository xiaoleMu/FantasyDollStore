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
			CocoDressEditWindow.onConfigRoleDressItemBeforeRandomSorting += OnConfigRoleDressItemBeforeRandomSorting;
			CocoDressEditWindow.onConfigRoleDressItemAfterRandomSorting += OnConfigRoleDressItemAfterRandomSorting;

		}

		public static void OnConfigGenerationStarted ()
		{
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