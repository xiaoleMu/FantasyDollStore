using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class LCAFinder
	{
		IList<int> _eulerianPath = new List<int>();
		IList<Transform> _nodes = new List<Transform>();

		public IEnumerable<Transform> EulerianPath
		{
			get 
			{ 
				foreach(int i in _eulerianPath)
				{
					yield return _nodes[i];
				}
			}
		}

		Transform _root;
		public LCAFinder(Transform root)
		{
			_root = root;
			GetEulerianPath();
		}

		class NodeRegistry
		{
			public Transform transform;
			public int firstVisitIndex;
		}

		DefaultingDictionary<Transform, NodeRegistry> _nodeIndex = new DefaultingDictionary<Transform, NodeRegistry>();

		void EulerTour(int rootIndex)
		{
			Transform node = _nodes[rootIndex];

			_nodeIndex[node] = new NodeRegistry() { transform = node, firstVisitIndex = _eulerianPath.Count };
			_eulerianPath.Add(rootIndex);
			
			for(int i=0;i<node.childCount;i++)
			{
				_nodes.Add(node.GetChild(i));
				EulerTour(_nodes.Count - 1);
				_eulerianPath.Add(rootIndex);
			}
		}
		
		void GetEulerianPath()
		{
			_nodes.Add(_root);
			EulerTour(0);
		}

		public Transform FindLCA(Transform[] nodes)
		{
			if(nodes.Length == 0)
				return null;

			Transform lca = nodes[0];

			for(int i=1;i<nodes.Length;i++)
			{
				lca = FindLCA(lca, nodes[i]);
			}

			return lca;
		}

		public Transform FindLCA(Transform a, Transform b)
		{
			NodeRegistry nodeA = _nodeIndex[a];
			if(nodeA == null)
				return null;
			NodeRegistry nodeB = _nodeIndex[b];
			if(nodeB == null)
				return null;

			if(a == b)
				return a;

			int indexA = nodeA.firstVisitIndex;
			int indexB = nodeB.firstVisitIndex;
			if (indexB < indexA)
			{
				int temp = indexA;
				indexA = indexB;
				indexB = temp;
			} else if(indexA == indexB)
			{
				return _nodes[indexA];
			}

			//Find the lowest value.
			int lowest = int.MaxValue;
			for (int i = indexA; i < indexB; i++)
			{
				if (_eulerianPath[i] < lowest)
				{
					lowest = _eulerianPath[i];
				}
			}

			if(lowest >= _nodes.Count)
				return null;

			return _nodes[lowest];
		}
	}
}
