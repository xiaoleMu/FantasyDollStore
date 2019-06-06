using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using CocoPlay;
using TabTale;

namespace Test
{
	public class CocoShaderChecker
	{
		[MenuItem ("CocoPlay/Shader/Find Duplicating Shaders", false, 51)]
		private static void FindDuplications ()
		{
			var allShaderPaths = CollectFiles (Application.dataPath, SearchOption.AllDirectories, "*.shader");
			var shaderDic = CreateShaderDic (allShaderPaths);

			shaderDic.ForEach ((shaderName, shaderPaths) => {
				Debug.LogWarningFormat ("---- {0} count [{1}]", shaderName, shaderPaths.Count);
				if (shaderPaths.Count > 1) {
					var pathString = string.Empty;
					shaderPaths.ForEach (path => pathString = string.Format ("{0} [{1}]", pathString, path));
					Debug.LogErrorFormat ("Duplicated: shader [{0}] -> {{{1}}}", shaderName, pathString);
				}
			});
		}

		private static List<string> CollectFiles (string path, SearchOption searchOption, params string[] searchPatterns)
		{
			var files = new List<string> ();

			if (Directory.Exists (path)) {
				searchPatterns.ForEach (pattern => {
					var filePaths = Directory.GetFiles (path, pattern, searchOption);
					files.AddRange (filePaths);
				});
			}

			return files;
		}

		private static Dictionary<string, List<string>> CreateShaderDic (List<string> shaderPaths)
		{
			var shaderDic = new Dictionary<string, List<string>> ();
			var rootPath = Path.GetDirectoryName (Application.dataPath);
			if (rootPath != null) {
				shaderPaths.ForEach (shaderPath => {
					var shader = AssetDatabase.LoadAssetAtPath<Shader> (shaderPath.Remove (0, rootPath.Length + 1));
					if (shaderDic.ContainsKey (shader.name)) {
						shaderDic [shader.name].Add (shaderPath);
					} else {
						shaderDic.Add (shader.name, new List<string> { shaderPath });
					}
				});
			}

			return shaderDic;
		}
	}
}
