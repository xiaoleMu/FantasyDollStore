using UnityEngine;
using UnityEditor;

namespace CocoPlay
{
	public class CocoFindMissingScriptsRecursively
	{
		private static int _goCount, _componentCount, _missingCount;

		[MenuItem ("CocoPlay/Scripts/Find Missing Scripts Recursively", false, 65)]
		private static void Find ()
		{
			GameObject[] gos = Selection.gameObjects;

			if (gos == null || gos.Length <= 0) {
				// no selection, collect from active scene
				UnityEngine.SceneManagement.Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene ();
				gos = activeScene.GetRootGameObjects ();
			}

			_goCount = 0;
			_componentCount = 0;
			_missingCount = 0;
			foreach (GameObject g in gos) {
				FindInGo (g);
			}
			Debug.LogErrorFormat ("Searched {0} GameObjects, {1} components, found {2} missing", _goCount, _componentCount, _missingCount);
		}

		private static void FindInGo (GameObject go)
		{
			_goCount++;
			Component[] components = go.GetComponents<Component> ();
			for (int i = 0; i < components.Length; i++) {
				_componentCount++;
				if (components [i] == null) {
					_missingCount++;
					string s = go.name;
					Transform t = go.transform;
					while (t.parent != null) {
						s = t.parent.name + "/" + s;
						t = t.parent;
					}
					Debug.LogWarningFormat (go, "{0} has an empty script attached in position [{1}]", s,  i);
				}
			}
			// Now recurse through each child GO (if there are any):
			foreach (Transform childT in go.transform) {
				//Debug.Log("Searching " + childT.name  + " " );
				FindInGo (childT.gameObject);
			}
		}
	}
}