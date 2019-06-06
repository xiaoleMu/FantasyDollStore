using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

public class InspectorContextMenu
{
	public GenericMenu menu;

	private List<AttributeData> attributeList;

	public InspectorContextMenu()
	{
		LoadCustomContextMenuItem();
		CreateContextMenuItem();
	}

	public void ShowMenu()
	{
		menu.ShowAsContext();
	}

	void LoadCustomContextMenuItem()
	{
		if (attributeList != null)
			return;
		attributeList = new List<AttributeData>();
		Assembly assembly = GetType().Assembly;
		if (assembly != null)
		{
			System.Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				if (!types[i].IsClass)
					continue;
				//获取静态方法
				MethodInfo[] methods =
					types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				for (int j = 0; j < methods.Length; j++)
				{
					var attributes = methods[j].GetCustomAttributes(typeof (InspectorContextMenuItemAttribute), false);
					foreach (var attribute in attributes)
					{
						attributeList.Add(new AttributeData(methods[j], (InspectorContextMenuItemAttribute) attribute));
					}
				}
			}
		}
		//通过优先级排序
		attributeList.Sort(Comparison);
	}

	void CreateContextMenuItem()
	{
		if (menu != null)
			return;
		if (attributeList == null)
			return;
		menu = new GenericMenu();
		int level = 1;
		for (int i = 0; i < attributeList.Count; i++)
		{
			//每1000优先级为一组，不同组之间用横线隔开
			if (i > 0 && attributeList[i].attribute.priority - level * 1000 >= 0)
			{
				string line = "";
				int id = attributeList[i].attribute.menuItem.LastIndexOf('/');
				if (id >= 0)
					line = attributeList[i].attribute.menuItem.Substring(0, id);
				menu.AddSeparator(line + "/");
				level += 1;
			}
			menu.AddItem(new GUIContent(attributeList[i].attribute.menuItem), false, attributeList[i].Invoke);
		}
	}

	int Comparison(AttributeData A, AttributeData B)
	{
		return A.attribute.priority - B.attribute.priority;
	}

	class AttributeData
	{
		public MethodInfo method;
		public InspectorContextMenuItemAttribute attribute;

		public AttributeData(MethodInfo method, InspectorContextMenuItemAttribute attribute)
		{
			this.method = method;
			this.attribute = attribute;
		}

		public void Invoke()
		{
			if (method != null)
				method.Invoke(null, null);
		}
	}
}