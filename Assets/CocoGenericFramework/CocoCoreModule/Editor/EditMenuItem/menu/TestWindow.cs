using UnityEngine;
using UnityEditor;
using System.Collections;

public class TestWindow : EditorWindow
{
	private InspectorContextMenu menu
	{
		get
		{
			if (m_Menu == null)
				m_Menu = new InspectorContextMenu();
			return m_Menu;
		}
	}

	private InspectorContextMenu m_Menu;

	[MenuItem("Test/TestWindow")]
	static void Init()
	{
		TestWindow window = EditorWindow.GetWindow<TestWindow>();
	}

	void OnGUI()
	{
		if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
		{
			menu.ShowMenu();
			Event.current.Use();
		}
	}

	[InspectorContextMenuItem("TestButtonA")]
	static void TestButtonA()
	{
		Debug.Log("ClickTestButtonA");
	}

	[InspectorContextMenuItem("TestButtonB", 1000)]
	static void TestButtonB()
	{
		Debug.Log("ClickTestButtonB");
	}

	[InspectorContextMenuItem("TestButtonC", 1001)]
	static void TestButtonC()
	{
		Debug.Log("ClickTestButtonC");
	}

	[InspectorContextMenuItem("TestButtonD")]
	static void TestButtonD()
	{
		Debug.Log("ClickTestButtonD");
	}
}
