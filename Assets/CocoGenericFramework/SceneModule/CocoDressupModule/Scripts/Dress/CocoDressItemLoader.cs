using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using CocoPlay.ResourceManagement;
using TabTale;

namespace CocoPlay
{
	public class CocoDressItemLoader : GameView
	{
		#region Listen

		protected override void AddListeners ()
		{
			base.AddListeners ();
			ListenRequestFinish ();
		}


		protected override void RemoveListeners ()
		{
			CancelListenRequestFinish ();
			base.RemoveListeners ();
		}

		private bool m_RequestFinishIsListening;

		private void ListenRequestFinish ()
		{
			if (!m_RequestFinishIsListening) {
				dressRequestItemHolderFinishSignal.AddListener (OnDressRequestItemFinish);
				m_RequestFinishIsListening = true;
			}
		}

		private void CancelListenRequestFinish ()
		{
			if (m_RequestFinishIsListening) {
				dressRequestItemHolderFinishSignal.RemoveListener (OnDressRequestItemFinish);
			}
		}

		#endregion


		#region Processing Request

		private Dictionary<string, Action<Dictionary<CocoDressItemHolder, List<GameObject>>>> m_ProcessingRequestDic;

		private Dictionary<string, Action<Dictionary<CocoDressItemHolder, List<GameObject>>>> ProcessingRequestDic {
			get {
				if (m_ProcessingRequestDic == null) {
					m_ProcessingRequestDic = new Dictionary<string, Action<Dictionary<CocoDressItemHolder, List<GameObject>>>> ();
				}
				return m_ProcessingRequestDic;
			}
		}

		public bool HasProcessingRequest {
			get { return ProcessingRequestDic.Count > 0; }
		}

		public bool RequestIsProcessing (string requestId)
		{
			return ProcessingRequestDic.ContainsKey (requestId);
		}

		private static int m_AnonymousRequestId = -1;

		public int NextAnonymousRequestId {
			get { return ++m_AnonymousRequestId; }
		}

		#endregion


		#region Request

		[Inject]
		public CocoAssetRequestDressItemHolderSignal dressRequestItemHolderSignal { get; set; }

		[Inject]
		public CocoAssetRequestDressItemHolderFinishSignal dressRequestItemHolderFinishSignal { get; set; }

		[Inject]
		public CocoAssetControl dressHolderControl { get; set; }

		public string LoadItemDict (List<string> itemIds, Action<Dictionary<CocoDressItemHolder, List<GameObject>>> finishAction, string requestId = "")
		{
			var itemHolders = dressHolderControl.ConfigHolder.GetItemHolders (itemIds);
			return LoadItemDict (itemHolders, finishAction, requestId);
		}

		public string LoadItemDict (List<CocoDressItemHolder> itemHolders, Action<Dictionary<CocoDressItemHolder, List<GameObject>>> finishAction,
			string requestId = "")
		{
			if (itemHolders == null || itemHolders.Count <= 0) {
				Debug.LogErrorFormat ("[{0}<{1}>]->LoadItems: item holders is EMPTY !", name, GetType ().Name);
				if (finishAction != null) {
					finishAction (new Dictionary<CocoDressItemHolder, List<GameObject>> ());
				}
				return string.Empty;
			}

			if (string.IsNullOrEmpty (requestId)) {
				requestId = NextAnonymousRequestId.ToString ();
			}
			if (RequestIsProcessing (requestId)) {
				Debug.LogErrorFormat ("[{0}<{1}>]->LoadItems: request id [{2}] ALREADY exists!", name, GetType ().Name, requestId);
				if (finishAction != null) {
					finishAction (new Dictionary<CocoDressItemHolder, List<GameObject>> ());
				}
				return string.Empty;
			}

			ListenRequestFinish ();

			ProcessingRequestDic.Add (requestId, finishAction);
			dressRequestItemHolderSignal.Dispatch (itemHolders, requestId);

			return requestId;
		}

		public string LoadItemModelList (string itemId, Action<List<GameObject>> finishAction, string requestId = "")
		{
			var itemIds = new List<string> (1) { itemId };

			requestId = LoadItemDict (itemIds, itemModelsDic => {
				if (finishAction != null) {
					var models = itemModelsDic.Count > 0 ? itemModelsDic.FirstOrDefault ().Value : null;
					finishAction (models);
				}
			}, requestId);

			return requestId;
		}

		public string LoadItemModel (string itemId, Action<GameObject> finishAction, string requestId = "")
		{
			var itemIds = new List<string> (1) { itemId };

			requestId = LoadItemDict (itemIds, (itemModelsDic) => {
				if (finishAction != null) {
					var models = itemModelsDic.Count > 0 ? itemModelsDic.FirstOrDefault ().Value : null;
					if (models == null)
						finishAction (null);
					else
						finishAction (models [0]);
				}
			}, requestId);

			return requestId;
		}

		public void CancelRequest (string requestId)
		{
			ProcessingRequestDic.Remove (requestId);
		}

		private void OnDressRequestItemFinish (List<CocoDressItemHolder> itemHolders, string requestId)
		{
			if (!RequestIsProcessing (requestId)) {
				return;
			}

			var itemModelsDic = new Dictionary<CocoDressItemHolder, List<GameObject>> ();
			itemHolders.ForEach (itemHolder => {
				var models = LoadItemModels (itemHolder);
				itemHolder.UnloadAsset ();
				itemModelsDic.Add (itemHolder, models);
			});

			var finishAction = ProcessingRequestDic [requestId];
			ProcessingRequestDic.Remove (requestId);
			if (finishAction != null) {
				finishAction (itemModelsDic);
			}
		}

		#endregion


		#region Load Models

		private List<GameObject> LoadItemModels (CocoDressItemHolder itemHolder)
		{
			var modelHolders = itemHolder.modelHolders;
			var modelGos = new List<GameObject> (modelHolders.Count);

			for (var i = 0; i < modelHolders.Count; i++) {
				var go = LoadModel (modelHolders [i]);
				go.name = itemHolder.id + "_" + i;
				modelGos.Add (go);
				//Debug.LogError (itemHolder.id + ":" + go.name);
			}

			return modelGos;
		}

		private GameObject LoadModel (CocoAssetModelHolder modelHolder)
		{
			var go = CocoLoad.Instantiate (modelHolder.AssetEntity, transform);
			LoadModelMaterials (go, modelHolder);
			return go;
		}

		private void LoadModelMaterials (GameObject go, CocoAssetModelHolder modelHolder)
		{
			var renderers = go.GetComponentsInChildren<Renderer> ();
			if (renderers.Length <= 0) {
				return;
			}

			var materialHolders = modelHolder.materialHolders;
			if (materialHolders.Count <= 0) {
				return;
			}

			var materials = new List<Material> (materialHolders.Count);
			materialHolders.ForEach (holder => {
				var material = holder.AssetEntity;
				if (material == null) {
					return;
				}

				materials.Add (holder.AssetEntity);

				if (Application.isEditor) {
					ResourceManager.FixShaderOnEditor (holder.AssetEntity);
				}
			});
			if (materials.Count <= 0) {
				return;
			}

			// update materials of renderers
			var availableMaterials = materials.ToArray ();
			var targetRendererNames = modelHolder.TargetSmrSet;
			renderers.ForEach (modelRenderer => {
				if (targetRendererNames.Count <= 0 || targetRendererNames.Contains (modelRenderer.name)) {
					//renderer.sharedMaterials = availableMaterials;
					UpdateRendererMaterials (modelRenderer, availableMaterials);
				} else {
					Destroy (modelRenderer.gameObject);
				}
			});
		}

		private void UpdateRendererMaterials (Renderer modelRenderer, Material[] materials)
		{
			// Mesh mesh = null;
			// var smr = modelRenderer as SkinnedMeshRenderer;
			// if (smr != null) {
			// 	mesh = smr.sharedMesh;
			// } else if (modelRenderer is MeshRenderer) {
			// 	var filter = modelRenderer.GetComponent<MeshFilter> ();
			// 	if (filter != null) {
			// 		mesh = filter.sharedMesh;
			// 	}
			// }

			// if (mesh != null && mesh.subMeshCount < materials.Length) {
			// 	Material[] availableMaterials = new Material[mesh.subMeshCount];
			// 	for (int i = 0; i < availableMaterials.Length; i++) {
			// 		availableMaterials [i] = materials [i];
			// 	}
			// 	renderer.sharedMaterials = availableMaterials;
			// 	return;
			// }

			modelRenderer.sharedMaterials = materials;
		}

		#endregion
	}
}