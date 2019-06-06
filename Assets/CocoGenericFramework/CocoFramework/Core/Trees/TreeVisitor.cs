using UnityEngine;
using System.Collections;

namespace TabTale
{
	public static class TreeVisitor
	{
		public static void Visit(Transform node, System.Func<Transform, bool> visitor)
		{
			visitor(node);
			for(int i=0;i<node.childCount;i++)
			{
				Transform child = node.GetChild(i);
				Visit (child, visitor);
			}
		}
	}
}
