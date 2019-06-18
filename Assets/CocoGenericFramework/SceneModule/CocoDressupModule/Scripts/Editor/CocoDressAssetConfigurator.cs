using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CocoPlay
{
	public interface ICocoDressAssetConfiguratorOwner
	{
		CocoDressEditorConfigHolder EditorConfigHolder { get; }

		// start / end
		void OnConfigGenerationStarted ();
		void OnConfigGenerationEnded (CocoAssetConfigHolder assetConfigHolder);

		// random sorting
		void OnConfigAllRoleDressItemsBeforeRandomSorting (CocoAssetConfigHolder assetConfigHolder);
		void OnConfigAllRoleDressItemsAfterRandomSorting (CocoAssetConfigHolder assetConfigHolder);

		// allow change content after model content filled
		string OnItemModelContentFilled (CocoDressItemHolder itemHolder);

		// param: raw items, in category, in scene
		// return: reorganized items
		Dictionary<string, CocoDressItemHolder> OnConfigRawItemsCollected (Dictionary<string, CocoDressItemHolder> rawItems, string categoryId, string sceneId);
	}


	public class CocoDressAssetConfigurator
	{
		private ICocoDressAssetConfiguratorOwner _owner;
		private CocoDressEditorConfigHolder _editorConfigHolder;

		private string _rootFullDirectory;
		private string _assetConfigFullDirectory;

		private string GetRelativePath (string fullPath)
		{
			return CocoDressEditorHelper.GetRelativePath (fullPath, _rootFullDirectory);
		}


		#region Auto Generation

		public CocoAssetConfigHolder AutoGenerateConfigByOwner (ICocoDressAssetConfiguratorOwner owner)
		{
			if (owner == null) {
				return null;
			}

			_owner = owner;
			_editorConfigHolder = owner.EditorConfigHolder;
			if (_editorConfigHolder == null) {
				return null;
			}

			_rootFullDirectory = Path.Combine (Application.dataPath, _editorConfigHolder.originRootDirectory);
			_assetConfigFullDirectory = Path.Combine (_rootFullDirectory, _editorConfigHolder.configDirectory);

			return AutoGenerateAssetConfig ();
		}

		private CocoAssetConfigHolder AutoGenerateAssetConfig ()
		{
			CocoAssetConfigHolder assetConfigHolder;

			_owner.OnConfigGenerationStarted ();

			try {
				assetConfigHolder = GenerateAssetConfig ();
			}
			catch (Exception e) {
				Debug.LogErrorFormat ("AutoGenerateAssetConfig: Failed because exception [{0}]", e);
				assetConfigHolder = null;
			}

			if (assetConfigHolder != null) {
				assetConfigHolder.LoadSubConfigs (_rootFullDirectory);
				assetConfigHolder.LinkParent (null);

				// config item before sorting
				_owner.OnConfigAllRoleDressItemsBeforeRandomSorting (assetConfigHolder);

				// random sorting
				if (_editorConfigHolder.randomSorting) {
					RandomSortRoleDresses (assetConfigHolder);
				}

				// config item after sorting
				_owner.OnConfigAllRoleDressItemsAfterRandomSorting (assetConfigHolder);
			}

			_owner.OnConfigGenerationEnded (assetConfigHolder);

			return assetConfigHolder;
		}

		private CocoAssetConfigHolder GenerateAssetConfig ()
		{
			// create asset config holder
			var fullPath = Path.Combine (_assetConfigFullDirectory, _editorConfigHolder.globalConfigFileName);
			var assetConfigHolder = new CocoAssetConfigHolder () {
				id = Path.GetFileNameWithoutExtension (_editorConfigHolder.globalConfigFileName),
				assetPath = GetRelativePath (fullPath)
			};
			var allSceneDressItemIds = new Dictionary<string, Dictionary<string, HashSet<string>>> ();

			// create category holders
			var originAssetPath = Path.Combine (_rootFullDirectory, _editorConfigHolder.assetDirectory);
			originAssetPath = CocoDressEditorHelper.GetFullPath (originAssetPath);
			var categoryPaths = Directory.GetDirectories (originAssetPath);
			foreach (var path in categoryPaths) {
				Dictionary<string, HashSet<string>> sceneItemIds;
				var categoryHolder = GenerateCategory (path, out sceneItemIds);
				var categoryConfigHolder = GenerateCategoryConfig (categoryHolder);

				var tItemCount = 0;
				foreach (var item in sceneItemIds.Values) {
					tItemCount += item.Count;
					if (tItemCount > 0) {
						break;
					}
				}

				if (categoryHolder != null && tItemCount > 0) {
					assetConfigHolder.categoryConfigHolders.Add (categoryConfigHolder);
					CollectAllSceneDressItemIds (allSceneDressItemIds, sceneItemIds, categoryHolder.id);
				}
			}

			// create scene holders
			foreach (var kvpSceneDressItemIds in allSceneDressItemIds) {
				var sceneHolder = GenerateScene (kvpSceneDressItemIds.Key, kvpSceneDressItemIds.Value);
				var sceneConfigHolder = GenerateSceneConfig (sceneHolder);
				if (sceneConfigHolder != null) {
					assetConfigHolder.sceneConfigHolders.Add (sceneConfigHolder);
				}
			}

			// create role dress
			_editorConfigHolder.roleDressConfigHolders.ForEach (editorRoleDressConfigHolder => {
				var roleDressHolder = GenerateRoleDress (editorRoleDressConfigHolder, allSceneDressItemIds);
				if (roleDressHolder != null) {
					var roleDressConfigHolder = GenerateRoleDressConfig (roleDressHolder);
					if (roleDressConfigHolder != null) {
						assetConfigHolder.roleDressConfigHolders.Add (roleDressConfigHolder);
					}
				}
			});


			// create role body
			_editorConfigHolder.roleBodyConfigHolders.ForEach (editorRoleBodyConfigHolder => {
				var roleBodyHolder = GenerateRoleBody (editorRoleBodyConfigHolder);
				if (roleBodyHolder != null) {
					var roleBodyConfigHolder = GenerateRoleBodyConfig (roleBodyHolder);
					if (roleBodyConfigHolder != null) {
						assetConfigHolder.roleBodyConfigHolders.Add (roleBodyConfigHolder);
					}
				}
			});

			// create role
			_editorConfigHolder.roleConfigHolders.ForEach (editorRoleConfigHolder => {
				var roleHolder = GenerateRole (editorRoleConfigHolder);
				if (roleHolder != null) {
					var roleConfigHolder = GenerateRoleConfig (roleHolder);
					if (roleConfigHolder != null) {
						assetConfigHolder.roleConfigHolders.Add (roleConfigHolder);
					}
				}
			});

			return assetConfigHolder;
		}

		private CocoDressCategoryHolder GenerateCategory (string categoryPath, out Dictionary<string, HashSet<string>> sceneItemIds)
		{
			var categoryHolder = new CocoDressCategoryHolder { id = Path.GetFileName (categoryPath) };
			if (categoryHolder.id == null) {
				sceneItemIds = null;
				return null;
			}

			// match cover layer
			if (_editorConfigHolder.dressCategoryCoverLayerIds.ContainsKey (categoryHolder.id)) {
				categoryHolder.coverLayerId = _editorConfigHolder.dressCategoryCoverLayerIds [categoryHolder.id];
			}

			// main category
			if (_editorConfigHolder.dressMainCategoryIds.Contains (categoryHolder.id)) {
				categoryHolder.isMain = true;
			}

			var scenePaths = Directory.GetDirectories (categoryPath);
			sceneItemIds = new Dictionary<string, HashSet<string>> (scenePaths.Length);

			foreach (var scenePath in scenePaths) {
				var sceneId = Path.GetFileName (scenePath);
				if (string.IsNullOrEmpty (sceneId)) {
					continue;
				}

				var itemHolders = CollectDressItemsInScene (scenePath);
				var reorganizedItemHolders = _owner.OnConfigRawItemsCollected (itemHolders, categoryHolder.id, sceneId);
				if (reorganizedItemHolders != null && reorganizedItemHolders.Count > 0) {
					itemHolders = reorganizedItemHolders;
				}

				if (itemHolders != null) {
					categoryHolder.itemHolders.AddRange (itemHolders.Values);
					sceneItemIds.Add (sceneId, new HashSet<string> (itemHolders.Keys));
				}
			}

			return categoryHolder;
		}

		private TConfig GenerateConfig<TConfig, THolder> (THolder holder, string tag)
			where TConfig : CocoAssetJsonHolder<THolder>, new () where THolder : CocoHolderBase
		{
			var id = string.Format ("config_{0}_{1}", tag, holder.id);
			var path = Path.Combine (_assetConfigFullDirectory, id + ".json");
			if (!CocoData.SaveToJsonFile (holder, path, true)) {
				return null;
			}

			var categoryConfigHolder = new TConfig { id = id, assetPath = GetRelativePath (path) };
			return categoryConfigHolder;
		}

		private CocoDressCategoryConfigHolder GenerateCategoryConfig (CocoDressCategoryHolder categoryHolder)
		{
			return GenerateConfig<CocoDressCategoryConfigHolder, CocoDressCategoryHolder> (categoryHolder, "dress_category");
		}

		private CocoDressSceneHolder GenerateScene (string sceneId, Dictionary<string, HashSet<string>> categoryItemIdSet)
		{
			var sceneHolder = new CocoDressSceneHolder { id = sceneId };

			foreach (var kvpItemIds in categoryItemIdSet) {
				var categoryHolder = new CocoDressSceneCategoryHolder { id = kvpItemIds.Key };
				foreach (var itemId in kvpItemIds.Value) {
					var itemHolder = new CocoDressSceneItemHolder { id = itemId };
					categoryHolder.itemHolders.Add (itemHolder);
				}
				sceneHolder.categoryHolders.Add (categoryHolder);
			}

			return sceneHolder;
		}

		private CocoDressSceneConfigHolder GenerateSceneConfig (CocoDressSceneHolder sceneHolder)
		{
			return GenerateConfig<CocoDressSceneConfigHolder, CocoDressSceneHolder> (sceneHolder, "dress_scene");
		}

		private CocoRoleDressHolder GenerateRoleDress (CocoDressEditorRoleDressConfigHolder editorRoleDressConfigHolder,
			Dictionary<string, Dictionary<string, HashSet<string>>> allSceneDressItemIds)
		{
			var roleDressHolder = new CocoRoleDressHolder { id = editorRoleDressConfigHolder.dressId };

			// scene holders
			if (editorRoleDressConfigHolder.sceneConfigHolders.Count > 0) {
				editorRoleDressConfigHolder.sceneConfigHolders.ForEach (editorSceneConfigHolder => {
					if (!allSceneDressItemIds.ContainsKey (editorSceneConfigHolder.sceneId)) {
						return;
					}

					var sceneHolder = new CocoRoleDressSceneHolder { id = editorSceneConfigHolder.sceneId };
					roleDressHolder.sceneHolders.Add (sceneHolder);

					var sceneItemIds = allSceneDressItemIds [sceneHolder.id];
					foreach (var categoryItemIds in sceneItemIds.Values) {
						foreach (var sceneDressItemId in categoryItemIds) {
							if (editorSceneConfigHolder.itemIdPrefixs.Count > 0) {
								editorSceneConfigHolder.itemIdPrefixs.ForEach (itemIdPrefix => {
									if (sceneDressItemId.StartsWith (itemIdPrefix)) {
										var itemHolder = new CocoRoleDressItemHolder { id = sceneDressItemId, order = sceneHolder.itemHolders.Count };
										sceneHolder.itemHolders.Add (itemHolder);
									}
								});
							} else {
								// default contain all items
								var itemHolder = new CocoRoleDressItemHolder { id = sceneDressItemId, order = sceneHolder.itemHolders.Count };
								sceneHolder.itemHolders.Add (itemHolder);
							}
						}
					}
				});
			} else {
				// default contain all scenes
				foreach (var kvpCategoryItemIds in allSceneDressItemIds) {
					var sceneHolder = new CocoRoleDressSceneHolder { id = kvpCategoryItemIds.Key };
					roleDressHolder.sceneHolders.Add (sceneHolder);

					foreach (var sceneDressItemId in kvpCategoryItemIds.Value) {
						foreach (var itemId in sceneDressItemId.Value) {
							var itemHolder = new CocoRoleDressItemHolder { id = itemId, order = sceneHolder.itemHolders.Count };
							sceneHolder.itemHolders.Add (itemHolder);
						}
					}
				}
			}

			return roleDressHolder;
		}

		private CocoRoleDressConfigHolder GenerateRoleDressConfig (CocoRoleDressHolder roleDressHolder)
		{
			return GenerateConfig<CocoRoleDressConfigHolder, CocoRoleDressHolder> (roleDressHolder, "role_dress");
		}

		private CocoRoleBodyHolder GenerateRoleBody (CocoDressEditorRoleBodyConfigHolder editorRoleBodyConfigHolder)
		{
			var roleBodyHolder = new CocoRoleBodyHolder {
				id = editorRoleBodyConfigHolder.bodyId,
				bodyBones = editorRoleBodyConfigHolder.boneNames,
				bodyRenderers = editorRoleBodyConfigHolder.rendererNames
			};

			return roleBodyHolder;
		}

		private CocoRoleBodyConfigHolder GenerateRoleBodyConfig (CocoRoleBodyHolder roleBodyHolder)
		{
			return GenerateConfig<CocoRoleBodyConfigHolder, CocoRoleBodyHolder> (roleBodyHolder, "role_body");
		}

		private CocoRoleHolder GenerateRole (CocoDressEditorRoleConfigHolder editorRoleConfigHolder)
		{
			var roleHolder = new CocoRoleHolder {
				id = editorRoleConfigHolder.roleId,
				boneItemId = editorRoleConfigHolder.boneItemId,
				basicItemIds = editorRoleConfigHolder.basicItemIds,
				dressId = editorRoleConfigHolder.dressId,
				bodyId = editorRoleConfigHolder.bodyId,
				enableShadow = editorRoleConfigHolder.enableShadow
			};

			return roleHolder;
		}

		private CocoRoleConfigHolder GenerateRoleConfig (CocoRoleHolder roleHolder)
		{
			return GenerateConfig<CocoRoleConfigHolder, CocoRoleHolder> (roleHolder, "role_entity");
		}

		private void CollectAllSceneDressItemIds (Dictionary<string, Dictionary<string, HashSet<string>>> allSceneDressItemIds,
			Dictionary<string, HashSet<string>> sceneItemIds, string categoryId)
		{
			sceneItemIds.ForEach ((sceneId, itemIds) => {
				if (!allSceneDressItemIds.ContainsKey (sceneId)) {
					var categoryItemIds = new Dictionary<string, HashSet<string>> { { categoryId, itemIds } };
					allSceneDressItemIds.Add (sceneId, categoryItemIds);
					return;
				}

				var categoryDressItemIds = allSceneDressItemIds [sceneId];
				if (!categoryDressItemIds.ContainsKey (categoryId)) {
					categoryDressItemIds.Add (categoryId, itemIds);
					return;
				}

				foreach (var itemId in itemIds) {
					Debug.LogErrorFormat ("--- add item {0} to {1}<{2}> ({3})", itemId, sceneId, categoryId, categoryDressItemIds [categoryId].Count);
					if (!categoryDressItemIds [categoryId].Add (itemId)) {
						Debug.LogErrorFormat ("[{0}]->CollectAllSceneDressItemIds: item [{1}] in scene [{2}<{3}>] duplicated!",
							GetType ().Name, itemId, sceneId, categoryId);
					}
				}
			});
		}

		private Dictionary<string, CocoDressItemHolder> CollectDressItemsInScene (string scenePath)
		{
			// collect asset files
			var rootPath = Path.Combine (scenePath, "materials");
			var materialPaths = CollectFiles (rootPath, SearchOption.AllDirectories, "*.mat");
			rootPath = Path.Combine (scenePath, "models");
			var modelPaths = CollectFiles (rootPath, SearchOption.AllDirectories, "*.FBX", "*.prefab");
			rootPath = Path.Combine (scenePath, "icons");
			var spritePaths = CollectFiles (rootPath, SearchOption.AllDirectories, "*.png");
			var spriteMaterialPaths = CollectFiles (rootPath, SearchOption.AllDirectories, "*.mat");

			// generate items base on materials
			var itemHolders = GenerateItems (materialPaths);

			// fill contents
			itemHolders.ForEach (itemHolder => {
				FillModelContentsInItem (itemHolder, modelPaths);
				if (spritePaths.Count > 0) {
					FillSpriteContentsInItem (itemHolder, spritePaths, spriteMaterialPaths);
				}
			});

			// generate only model
			foreach (var kvpModelPath in modelPaths) {
				if (kvpModelPath.Value) {
					// used
					continue;
				}

				var modelPath = kvpModelPath.Key;
				var modelId = Path.GetFileNameWithoutExtension (modelPath);
				if (string.IsNullOrEmpty (modelId)) {
					continue;
				}
				if (itemHolders.ContainsKey (modelId)) {
					continue;
				}

				var itemHolder = GenerateItem (modelId);
				itemHolders.Add (itemHolder.id, itemHolder);

				// model
				var modelHolder = new CocoAssetModelHolder { id = modelId, assetPath = GetRelativePath (modelPath) };
				itemHolder.modelHolders.Add (modelHolder);
			}

			return itemHolders;
		}

		private CocoDressItemHolder GenerateItem (string itemId)
		{
			var itemHolder = new CocoDressItemHolder { id = itemId };

			// match cover layer
			_editorConfigHolder.dressItemPrefixCoverLayerIds.ForEach ((itemIdPrefix, coverLayerId) => {
				if (itemId.StartsWith (itemIdPrefix)) {
					itemHolder.coverLayerId = coverLayerId;
				}
			});

			return itemHolder;
		}

		private Dictionary<string, CocoDressItemHolder> GenerateItems (Dictionary<string, bool> materialPaths)
		{
			var itemHolders = new Dictionary<string, CocoDressItemHolder> ();

			foreach (var materialPath in materialPaths.Keys) {
				var materialId = Path.GetFileNameWithoutExtension (materialPath);
				// material
				var materialHolder = new CocoAssetMaterialHolder
					{ id = materialId, assetPath = GetRelativePath (materialPath) };

				var itemId = ExtractItemId (materialId);
				if (!itemHolders.ContainsKey (itemId)) {
					// create item
					var itemHolder = GenerateItem (itemId);
					itemHolders.Add (itemId, itemHolder);

					// model (only add material holder now, other content will fill later)
					var modelHolder = new CocoAssetModelHolder ();
					itemHolder.modelHolders.Add (modelHolder);
					modelHolder.materialHolders.Add (materialHolder);
				} else {
					// add multi material
					itemHolders [itemId].modelHolders [0].materialHolders.Add (materialHolder);
				}
			}

			return itemHolders;
		}

		private void FillModelContentsInItem (CocoDressItemHolder itemHolder, Dictionary<string, bool> modelPaths)
		{
			var usedPath = string.Empty;

			foreach (var kvpModelPath in modelPaths) {
				var modelPath = kvpModelPath.Key;
				var modelId = Path.GetFileNameWithoutExtension (modelPath);
				if (string.IsNullOrEmpty (modelId)) {
					continue;
				}
				if (!itemHolder.id.StartsWith (modelId)) {
					continue;
				}

				var modelHolder = itemHolder.modelHolders [0];
				modelHolder.id = modelId;
				modelHolder.assetPath = GetRelativePath (modelPath);
				usedPath = modelPath;
				break;
			}

			var changedPath = _owner.OnItemModelContentFilled (itemHolder);
			if (string.IsNullOrEmpty (usedPath) && !string.IsNullOrEmpty (changedPath)) {
				usedPath = changedPath;
			}

			if (string.IsNullOrEmpty (usedPath)) {
				Debug.LogWarningFormat ("[{0}]->FillModelContentsInItem: can NOT found model for item [{1}]!", GetType ().Name, itemHolder.id);
			} else {
				if (modelPaths.ContainsKey (usedPath)) {
					modelPaths [usedPath] = true;
				}
			}
		}

		private void FillSpriteContentsInItem (CocoDressItemHolder itemHolder, Dictionary<string, bool> spritePaths,
			Dictionary<string, bool> spriteMaterialPaths)
		{
			var usedPath = string.Empty;

			foreach (var kvpSpritePath in spritePaths) {
				var spritePath = kvpSpritePath.Key;
				var spriteId = Path.GetFileNameWithoutExtension (spritePath);
				if (string.IsNullOrEmpty (spriteId)) {
					continue;
				}
				if (!itemHolder.id.StartsWith (spriteId)) {
					continue;
				}

				var spriteHolder = new CocoAssetSpriteHolder { id = spriteId, assetPath = GetRelativePath (spritePath) };
				itemHolder.spriteHolders.Add (spriteHolder);

				FillMaterialContentsInSprite (spriteHolder, itemHolder.id, spriteMaterialPaths);
				usedPath = spritePath;
				break;
			}

			if (string.IsNullOrEmpty (usedPath)) {
				Debug.LogErrorFormat ("[{0}]->FillSpriteContentsInItem: can NOT found sprite for item [{1}]!", GetType ().Name, itemHolder.id);
			} else {
				spritePaths [usedPath] = true;
			}
		}

		private void FillMaterialContentsInSprite (CocoAssetSpriteHolder spriteHolder, string itemId, Dictionary<string, bool> materialPaths)
		{
			var usedPath = string.Empty;

			foreach (var kvpMaterialPath in materialPaths) {
				var materialPath = kvpMaterialPath.Key;
				var materialId = Path.GetFileNameWithoutExtension (materialPath);
				var expectItemId = ExtractItemId (materialId);
				if (expectItemId == itemId) {
					var materialHolder = new CocoAssetMaterialHolder
						{ id = materialId, assetPath = GetRelativePath (materialPath) };
					spriteHolder.materialHolder = materialHolder;
					usedPath = materialPath;
					break;
				}
			}


			if (string.IsNullOrEmpty (usedPath)) {
				Debug.LogFormat ("[{0}]->FillSpriteContentsInItem: can NOT found sprite material for sprite [{1}({2})]!", GetType ().Name,
					spriteHolder.id, itemId);
			} else {
				materialPaths [usedPath] = true;
			}
		}

		private Dictionary<string, bool> CollectFiles (string path, SearchOption searchOption, params string[] searchPatterns)
		{
			var files = new Dictionary<string, bool> ();

			if (!Directory.Exists (path)) {
				Debug.LogFormat ("[{0}]->CollectFiles: path [{1}] NOT exists!", GetType ().Name, path);
				return files;
			}

			if (Directory.Exists (path)) {
				foreach (var pattern in searchPatterns) {
					var filePaths = Directory.GetFiles (path, pattern, searchOption);
					foreach (var filePath in filePaths) {
						if (!files.ContainsKey (filePath)) {
							if (!filePath.Contains ("_extra"))
								files.Add (filePath, false);
						}
					}
				}
			}

			return files;
		}

		private string ExtractItemId (string materialId)
		{
			var key = "_mat";
			var strings = materialId.Split (new[] { key }, StringSplitOptions.RemoveEmptyEntries);
			if (strings.Length < 1) {
				return string.Empty;
			}

			// check if contain discolor
			var strId = -1;
			var colorIndex = -1;
			key = "_color";
			for (var i = 0; i < strings.Length; i++) {
				colorIndex = strings [i].IndexOf (key, StringComparison.Ordinal);
				if (colorIndex >= 0) {
					strId = i;
					break;
				}
			}

			// no discolor, use first part
			if (strId < 0) {
				return strings [0];
			}

			// discolor in first part, use first part (without color key) and color id
			if (strId == 0) {
				return strings [0].Remove (colorIndex, key.Length);
			}


			// discolor no in first part, use first part and color id
			colorIndex += key.Length;
			var colorStr = strings [strId];
			var colorId = colorIndex < colorStr.Length ? colorStr.Substring (colorIndex) : string.Empty;
			return strings [0] + colorId;
		}

		#endregion


		#region Random Sort

		private void RandomSortRoleDresses (CocoAssetConfigHolder assetConfigHolder)
		{
			// collect all basic items
			var basicItemIds = new HashSet<string> ();
			assetConfigHolder.RoleHolderDic.ForEach (roleHolder => {
				roleHolder.basicItemIds.ForEach (basicItemId => {
					if (!basicItemIds.Contains (basicItemId)) {
						basicItemIds.Add (basicItemId);
					}
				});
			});

			assetConfigHolder.RoleDressHolderDic.ForEach (roleDressHolder => {
				// reset random seed for each role
				if (ShouldResetRandomSeed (roleDressHolder.id)) {
					Random.InitState (0);
				}
				roleDressHolder.sceneHolders.ForEach (sceneHolder => { RandomSortSceneDress (sceneHolder, basicItemIds); });
			});
		}

		private bool ShouldResetRandomSeed (string dressId)
		{
			var roleDressConfigHolder =
				_editorConfigHolder.roleDressConfigHolders.Find (holder => holder.dressId == dressId);

			if (roleDressConfigHolder != null && roleDressConfigHolder.resetRandomSeed) {
				return true;
			}

			return false;
		}

		private void RandomSortSceneDress (CocoRoleDressSceneHolder roleSceneHolder, HashSet<string> basicItemIds)
		{
			var sceneDressItemHolders = new List<CocoRoleDressItemHolder> (roleSceneHolder.itemHolders.Count);
			roleSceneHolder.CategoryItemHolderDic.ForEach (roleDressItemHolders => {
				RandomSortRoleDressItems (ref roleDressItemHolders, basicItemIds);
				sceneDressItemHolders.AddRange (roleDressItemHolders);
			});
			roleSceneHolder.itemHolders = sceneDressItemHolders;
		}

		private void RandomSortRoleDressItems (ref List<CocoRoleDressItemHolder> roleDressItemHolders, HashSet<string> basicItemIds)
		{
			var sortedItemHolders = new List<CocoRoleDressItemHolder> (roleDressItemHolders.Count);

			// collect all items for model id
			var modelItemHolders = new Dictionary<string, List<CocoRoleDressItemHolder>> ();
			roleDressItemHolders.ForEach (itemHolder => {
				var modelId = itemHolder.LinkedDressItemHolder.modelHolders [0].id;

				List<CocoRoleDressItemHolder> itemHolders;
				if (modelItemHolders.ContainsKey (modelId)) {
					itemHolders = modelItemHolders [modelId];
				} else {
					itemHolders = new List<CocoRoleDressItemHolder> ();
					modelItemHolders.Add (modelId, itemHolders);
				}

				if (!basicItemIds.Contains (itemHolder.id)) {
					itemHolders.Add (itemHolder);
				} else {
					// priority basic item
					itemHolder.order = sortedItemHolders.Count;
					sortedItemHolders.Add (itemHolder);
				}
			});

			var lastModelId = string.Empty;
			while (modelItemHolders.Count > 0) {
				// random sort (select one per model)
				var modelIds = new List<string> (modelItemHolders.Keys);
				var isFirst = true;
				var modelId = string.Empty;

				while (modelIds.Count > 0) {
					var modelIndex = Random.Range (0, modelIds.Count);
					modelId = modelIds [modelIndex];

					// avoid same model id between first and last
					if (isFirst) {
						if (modelIds.Count > 1 && modelId == lastModelId) {
							modelIndex = (modelIndex + 1) % modelIds.Count;
							modelId = modelIds [modelIndex];
						}
						isFirst = false;
					}

					if (modelItemHolders.ContainsKey (modelId)) {
						// select one from the items in this model
						var itemHolders = modelItemHolders [modelId];
						if (itemHolders.Count > 0) {
							var itemIndex = Random.Range (0, itemHolders.Count);
							var itemHolder = itemHolders [itemIndex];
							itemHolder.order = sortedItemHolders.Count;
							sortedItemHolders.Add (itemHolder);
							itemHolders.RemoveAt (itemIndex);
						}

						// all already extracted, remove this from dictionary
						if (itemHolders.Count <= 0) {
							modelItemHolders.Remove (modelId);
						}
					}

					modelIds.RemoveAt (modelIndex);
				}

				// record last model id
				lastModelId = modelId;
			}

			//			string str = string.Empty;
			//			roleDressItemHolders.ForEach (itemHolder => str += itemHolder.id + "\n");
			//			str += "\nsort:\n";
			//			sortedItemHolders.ForEach (itemHolder => str += itemHolder.id + "\n");
			//			Debug.LogError (str);

			roleDressItemHolders = sortedItemHolders;
		}

		#endregion
	}
}