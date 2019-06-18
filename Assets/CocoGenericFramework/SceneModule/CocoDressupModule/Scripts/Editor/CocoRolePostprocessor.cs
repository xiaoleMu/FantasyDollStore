using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace CocoPlay
{
	public class CocoRolePostprocessor : AssetPostprocessor
	{
		private static string m_RoleAssetPath;

		private string RoleAssetPath {
			get {
				if (m_RoleAssetPath == null) {
					var rootDirectory = CocoDressEditWindow.ConfigHolder.originRootDirectory;
					m_RoleAssetPath = Path.Combine ("Assets", rootDirectory);
				}
				return m_RoleAssetPath;
			}
		}


		#region Model

		private static HashSet<string> m_RoleBoneModelNames;

		private HashSet<string> RoleBoneModelNames {
			get {
				if (m_RoleBoneModelNames == null) {
					m_RoleBoneModelNames = new HashSet<string> ();
					var roleConfigHolders = CocoDressEditWindow.ConfigHolder.roleConfigHolders;
					roleConfigHolders.ForEach (holder => {
						if (!m_RoleBoneModelNames.Contains (holder.boneItemId)) {
							m_RoleBoneModelNames.Add (holder.boneItemId);
						}
					});
				}
				return m_RoleBoneModelNames;
			}
		}

		private void OnPreprocessModel ()
		{
			if (!assetImporter.assetPath.StartsWith (RoleAssetPath))
				return;

			var modelImporter = (ModelImporter)assetImporter;

			// don't import materials
			modelImporter.importMaterials = false;
			// default animation type to Generic
			modelImporter.animationType = ModelImporterAnimationType.Generic;

			var modelName = Path.GetFileNameWithoutExtension (modelImporter.assetPath);
			if (string.IsNullOrEmpty (modelName)) {
				return;
			}

			if (!modelName.Contains ("@")) {
				// don't import animations if only model
				modelImporter.importAnimation = false;
			}
		}

		private void OnPostprocessModel (GameObject model)
		{
			if (!assetImporter.assetPath.StartsWith (RoleAssetPath))
				return;

			var modelName = model.name;
			if (modelName.Contains ("@") || RoleBoneModelNames.Contains (modelName)) {
				// for animation models, destroy all renderer objects and meshes
				DestroySubObjects<SkinnedMeshRenderer> (model, com => ((SkinnedMeshRenderer)com).sharedMesh);
				DestroySubObjects<MeshFilter> (model, com => ((MeshFilter)com).sharedMesh);
			} else {
				// for simple models, destroy animators and avatars
				DestroySubObjects<Animator> (model, (com) => ((Animator)com).avatar, false);
			}
		}


		private delegate Object GetObjectFunc (Component com);


		private void DestroySubObjects<T> (GameObject rootGo, GetObjectFunc getObjFunc = null, bool destroyGo = true) where T : Component
		{
			var renderers = rootGo.GetComponentsInChildren<T> ();
			foreach (var component in renderers) {
				if (getObjFunc != null) {
					var obj = getObjFunc (component);
					if (obj != null) {
						Object.DestroyImmediate (obj);
					}
				}
				if (destroyGo) {
					Object.DestroyImmediate (component.gameObject);
				} else {
					Object.DestroyImmediate (component);
				}
			}
		}

		#endregion


		#region Texture

		private void OnPreprocessTexture ()
		{
			if (!assetImporter.assetPath.StartsWith (RoleAssetPath))
				return;

			var textureImporter = (TextureImporter)assetImporter;

			if (assetImporter.assetPath.Contains ("icons/")) {
				textureImporter.textureType = TextureImporterType.Sprite;
			}
		}

		#endregion
	}
}