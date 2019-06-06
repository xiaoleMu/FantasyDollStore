using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

namespace TabTale.ProjectUpdater 
{	
	public class ProjectUpdaterDependenciesGUI : EditorWindow
	{
		private static int SCREEN_WIDTH = Screen.currentResolution.width;
		private static int SCREEN_HEIGHT = Screen.currentResolution.height;

		private Color logColor = new Color(0.5f,1.0f,1.0f);
		private Color errorColor = new Color(1.0f, 0.2f, 0.2f);

		private static Vector2 scrollPosition;
		public static List<string> updateScriptText = new List<string>();
		public static bool isDone = false;

		[MenuItem("TabTale/GSDK/Update Dependencies &u")]
		static void UpdateDependencies () 
		{
			ShowWindow();
			ProjectUpdater.StartUpdateProcess();
		}

		private static void ShowWindow()
		{
			isDone = false;
			EditorWindow.GetWindowWithRect<ProjectUpdaterDependenciesGUI>(new Rect(SCREEN_WIDTH/3,SCREEN_HEIGHT/2,SCREEN_WIDTH/2,SCREEN_HEIGHT/2),false, "Dependencies");
		}

		void OnGUI() 
		{
			FocusWindowIfItsOpen<ProjectUpdaterDependenciesGUI>();
			GUI.color = logColor;
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(0), GUILayout.Height(SCREEN_HEIGHT/2));
			if(updateScriptText != null)
			{
				foreach(string line in updateScriptText)
				{
					GUILayout.Label(line,EditorStyles.boldLabel);
				}
			}
			GUI.color = Color.white;
			if(isDone)
			{
				if (GUILayout.Button("Ok"))
				{
					this.Close();
				}
			}
			GUILayout.EndScrollView();
		}

		public void OnDestroy()
		{
			updateScriptText.Clear();
			AssetDatabase.Refresh();
		}

		public void Update()
		{
			Repaint();
		}

		public static void PrintLog(string log)
		{
			updateScriptText.Add(log);
		}
	}
}