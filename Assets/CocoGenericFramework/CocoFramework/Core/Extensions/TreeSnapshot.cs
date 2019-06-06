using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class TreeSnapshot
	{
		Transform _root;
		DefaultingDictionary<string, TreeSnapshot> _children;
		DefaultingDictionary<Transform, Transform> _allNodes = new DefaultingDictionary<Transform, Transform>();
		DefaultingDictionary<string, string> _allNodeNames = new DefaultingDictionary<string, string>();
		
		public bool Contains(Transform transform)
		{
			return _allNodes.ContainsKey(transform);
		}
		
		public bool Contains(string nodeName)
		{
			return _allNodeNames.ContainsKey(nodeName);
		}
		
		public TreeSnapshot(Transform transform)
		{
			_root = transform;
			
			_children = _root.GetChildren()
				.ToDefaultingDictionary<Transform, string, TreeSnapshot>(t => t.gameObject.name, t => new TreeSnapshot(t));
			
			AddChildNodes(this);
		}
		
		void AddChildNodes(TreeSnapshot tree)
		{
			_allNodes[tree._root] = tree._root;
			_allNodeNames[tree._root.gameObject.name] = tree._root.gameObject.name;
			
			foreach(TreeSnapshot child in tree._children.Values)
			{
				AddChildNodes(child);
			}
		}

		public IEnumerable<TreeSnapshot> Children
		{
			get { return _children.Values; }
		}
		
		public TreeSnapshot this[string key]
		{
			get { return _children[key]; }
		}
		
		public Transform Root
		{
			get { return _root; }
		}
		
		public override string ToString ()
		{
			return Print ("", this);
		}
		
		public GameObject GameObject
		{
			get { return _root.gameObject; }
		}
		
		public string Name
		{
			get { return _root.gameObject.name; }
		}
		
		private string Print(string tab, TreeSnapshot root)
		{
			string skeleton = string.Format("{0} {1}\n", tab, root.Name);
			string newTab = tab + "\t";
			foreach(TreeSnapshot child in root._children.Values)
			{
				skeleton += Print(newTab, child);
			}
			
			return skeleton;
		}
	}
}
