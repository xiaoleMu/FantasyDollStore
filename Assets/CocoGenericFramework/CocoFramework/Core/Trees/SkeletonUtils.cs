using UnityEngine;
using System.Collections;

namespace TabTale
{
	public static class SkeletonUtils
	{
		public static string PrintSkeleton(Transform root)
		{
			return PrintSkeleton("", root);
		}

		private static string PrintSkeleton(string tab, Transform root)
		{
			string skeleton = string.Format("{0} {1}\n", tab, root.gameObject.name);
			string newTab = tab + "\t";
			foreach(Transform child in root.GetChildren())
			{
				skeleton += PrintSkeleton(newTab, child);
			}

			return skeleton;
		}
	}
}
