using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale
{
	public static class TransformExtensions
	{
		public static Transform GetRoot(this Transform _this)
		{
			Transform current = _this;
			Transform parent = current.parent;
			while(parent != null)
			{
				current = parent;
				parent = current.parent;
			}

			return current;
		}

		public static IEnumerable<Transform> GetChildren(this Transform _this)
		{
			for(int i=0;i<_this.childCount;i++)
			{
				yield return _this.GetChild(i);
			}
		}

		public static Transform FindNode(this Transform _this, string name, bool recursive = false)
		{
			if(_this.name == name)
				return _this;

			if(recursive)
			{
				for(int i=0;i<_this.childCount;i++)
				{
					Transform node = FindNode(_this.GetChild(i), name, recursive);
					if(node != null)
						return node;
				}
			}

			return null;
		}

		public static string GetPath(this Transform _this)
		{
			string pathString = "";

			Stack<Transform> path = new Stack<Transform>();
			path.Push(_this);
			Transform parent = _this;

			while(parent.parent != null)
			{
				parent = parent.parent;
				path.Push(parent);
			}
			string tab = "";
			while(path.Count > 0)
			{
				Transform t = path.Pop();
				pathString += string.Format("{0}{1}\n", tab, t.gameObject.name);
				tab += "\t";
			}

			return pathString;
		}

		public static IEnumerable<TComponent> GetComponentnsInSubtree<TComponent>(this Transform _this)
			where TComponent : Component
		{
			Stack<Transform> stack = new Stack<Transform>();
			stack.Push(_this);
			
			while(stack.Count > 0)
			{
				Transform u = stack.Pop();

				TComponent[] childComponents = u.GetComponents<TComponent>();
				for(int i=0;i<childComponents.Length;i++)
				{
					yield return childComponents[i];
				}

				foreach(Transform t in u.GetChildren())
				{
					stack.Push(t);
				}
			}
		}

		public static IEnumerable<Transform> SelectSubtree(this Transform _this)
		{
			Stack<Transform> stack = new Stack<Transform>();
			stack.Push(_this);
			
			while(stack.Count > 0)
			{
				Transform u = stack.Pop();
				
				yield return u;
				
				foreach(Transform t in u.GetChildren())
				{
					stack.Push(t);
				}
			}
		}

		public static Transform RemoveAllChildren(this Transform transform)
		{
			foreach (Transform child in transform) {
				GameObject.Destroy(child.gameObject);
			}
			return transform;
		}

		public static void AlignLocalTo(this Transform _this, Transform other)
		{
			Stack<Transform> thisStack = new Stack<Transform>();
			thisStack.Push(_this);
			Stack<Transform> otherStack = new Stack<Transform>();
			otherStack.Push(other);
			
			while(thisStack.Count > 0)
			{
				Transform t = thisStack.Pop();
				Transform u = otherStack.Pop();
				
				t.localPosition = u.localPosition;
				t.localRotation = u.localRotation;
				t.localScale = u.localScale;
				
				if(t.childCount == u.childCount)
				{
					for(int i=0;i<t.childCount;i++)
					{
						thisStack.Push(t.GetChild(i));
						otherStack.Push(u.GetChild(i));
					}
				}
			}
		}

		public static IEnumerable<Transform> DFS(this Transform _this)
		{
			Stack<Transform> stack = new Stack<Transform>();
			stack.Push(_this);

			while(stack.Count > 0)
			{
				Transform t = stack.Pop();
				yield return t;

				for(int i=0;i<t.childCount;i++)
				{
					stack.Push(t);
				}
			}
		}
	}
}
