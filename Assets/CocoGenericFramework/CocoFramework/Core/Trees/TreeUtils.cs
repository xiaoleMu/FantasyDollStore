using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public static class TreeUtils
	{
		static void EulerTour(IList<Transform> path, Transform root)
		{
			path.Add(root);

			for(int i=0;i<root.childCount;i++)
			{
				EulerTour(path, root.GetChild(i));
				path.Add(root);
			}
		}

		public static IList<Transform> GetEulerianPath(Transform root)
		{
			IList<Transform> path = new List<Transform>();

			EulerTour(path, root);

			return path;
		}
	}
}
