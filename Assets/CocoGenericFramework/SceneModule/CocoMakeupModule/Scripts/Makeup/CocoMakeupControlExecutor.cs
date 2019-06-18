using UnityEngine;
using System.Collections;
using TabTale;

namespace CocoPlay{
	public abstract class CocoMakeupControlExecutorBase 
	{
		CocoMakeupItemData m_ItemData;
		protected SkinnedMeshRenderer m_Renderer;
		protected MeshCollider m_MeshCollider;
		bool m_ColliderIsOwned = false;
		protected CocoMakeupPaintKit m_PaintKit;
		public CocoMakeupPaintKit PaintKit{get{return m_PaintKit;}}
		public CocoMakeupItemData ItemData{get{return m_ItemData;}}

		Mesh m_OriginMesh = null;
		bool m_BakeMesh = false;
		public CocoMakeupControlExecutorBase (GameObject target, string materialProperty, bool bakeMesh)
		{
			m_Renderer = target.GetComponentInChildren<SkinnedMeshRenderer> ();

			// collider
			m_MeshCollider = m_Renderer.GetComponent<MeshCollider> ();
			if (m_MeshCollider == null) {
				m_MeshCollider = m_Renderer.gameObject.AddComponent<MeshCollider> ();
				m_ColliderIsOwned = true;
			}

			// bake mesh
			m_OriginMesh = m_MeshCollider.sharedMesh;
			m_BakeMesh = bakeMesh;
			if (m_BakeMesh) {
				if (m_MeshCollider.sharedMesh == null || m_MeshCollider.sharedMesh == m_Renderer.sharedMesh) {
					Mesh mesh = new Mesh ();
					m_Renderer.BakeMesh (mesh);
					m_MeshCollider.sharedMesh = mesh;
				} else {
					m_BakeMesh = false;
				}
			} else {
				m_MeshCollider.sharedMesh = m_Renderer.sharedMesh;
			}
		}

		public virtual void Clear ()
		{
			if (m_MeshCollider != null) {
				if (m_BakeMesh) {
					Object.Destroy (m_MeshCollider.sharedMesh);
					m_BakeMesh = false;
				}
				m_MeshCollider.sharedMesh = m_OriginMesh;
			}

			if (m_ColliderIsOwned) {
				Object.Destroy (m_MeshCollider);
				m_ColliderIsOwned = false;
			}
			m_MeshCollider = null;

			m_Renderer = null;
		}

		public void OnCategoryItemChanged (CocoMakeupItemData data)
		{
			if (data == m_ItemData)
				return;

			SwitchItem (m_ItemData, data);
			m_ItemData = data;
		}

		protected abstract void SwitchItem (CocoMakeupItemData oldData, CocoMakeupItemData newData);

		public abstract void PaintDirectly ();

		public abstract bool OnPaint (Vector2 screenPos);

		public abstract bool OnPaintEnd ();


		public void ClearPatting()
		{
			m_PaintKit.ResetCurrentLayer();
		}
	}

	public class CocoMakeupControlPaint : CocoMakeupControlExecutorBase
	{
		bool m_UseUV2 = false;

		bool m_PaintValid;

		public CocoMakeupControlPaint (GameObject target, string materialProperty, bool bakeMesh, Texture2D sampleTexture, bool useUV2) : base (target, materialProperty, bakeMesh)
		{
			// bake mesh
			m_UseUV2 = useUV2;
		
			// use sample texture
			if (sampleTexture != null) {
//				Texture2D texture = new Texture2D (sampleTexture.width, sampleTexture.height, TextureFormat.ARGB32, false);
//				texture.SetPixels32 (sampleTexture.GetPixels32 ());
//				texture.Apply ();
				if (string.IsNullOrEmpty (materialProperty)) {
					m_Renderer.material.mainTexture = sampleTexture;
				} else {
					m_Renderer.material.SetTexture (materialProperty, sampleTexture);
				}
			}

			// paint kit
			m_PaintKit = new CocoMakeupPaintKit (m_MeshCollider.gameObject, materialProperty);
			m_PaintValid = false;
		}
			

		protected override void SwitchItem (CocoMakeupItemData oldData, CocoMakeupItemData newData)
		{
			if (newData != null) {
				Texture2D texture;

				CocoMakeupCategoryData categoryData = CocoRoot.GetInstance <ICocoMakeupData>().GetCategoryData(newData.Category);
				if(categoryData == null){return;}
				if (oldData == null || oldData.Category != newData.Category) {
					CocoMakeupCategoryPaintData_PaintTexture paintData = (CocoMakeupCategoryPaintData_PaintTexture)categoryData.PaintData;
					// brush
					texture = Resources.Load<Texture2D> (paintData.paintBrushPath);
					m_PaintKit.CurrentBrush = new CocoMakeupBrush (texture);
					//Resources.UnloadAsset (texture);
					// mask
	
					if(!string.IsNullOrEmpty(paintData.paintMaskPath))
					{
						texture = Resources.Load<Texture2D> (paintData.paintMaskPath);
						m_PaintKit.CurrentPaintMask = new CocoMakeupMask (texture);
						//Resources.UnloadAsset (texture);
					}
					else
					{
						m_PaintKit.CurrentPaintMask = new CocoMakeupMask (Texture2D.whiteTexture);
					}
	
					// layer
					m_PaintKit.SetCurrentCanvasLayer (paintData.paintLayerId);
					m_PaintKit.CurrentBrush.Size = paintData.BrushSize;
				}
	
				if (oldData == null || oldData.m_TexturePath != newData.m_TexturePath) {
					// pattern
					texture = Resources.Load<Texture2D> (newData.m_TexturePath);
					m_PaintKit.CurrentPattern = new CocoMakeupPattern (texture, CocoMakeupPattern.FillMode.CanvasArea);
					//Resources.UnloadAsset (texture);
				}

				Resources.UnloadUnusedAssets();
			}
		}

		public override void PaintDirectly ()
		{
			m_PaintKit.CurrentBrush.Size = new Vector2 (2048, 2048);
			m_PaintKit.Paint (Vector2.zero);
			m_PaintKit.PaintEnd ();
		}

		public override bool OnPaint (Vector2 screenPos)
		{
			Ray ray = Camera.main.ScreenPointToRay (screenPos);
			RaycastHit hit;
			if (m_MeshCollider.Raycast (ray, out hit, Camera.main.farClipPlane)) {
				if (m_UseUV2) {
					m_PaintKit.Paint (hit.textureCoord2);
				} else {
					m_PaintKit.Paint (hit.textureCoord);
				}
				m_PaintValid = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		public override bool OnPaintEnd ()
		{
			m_PaintKit.PaintEnd ();
			if (m_PaintValid) {
				m_PaintValid = false;
				return true;
			}

			return false;
		}

	}

	public class CocoMakeupControlPaintBlendColor : CocoMakeupControlPaint
	{
		Color32 m_BlendColor;

		public CocoMakeupControlPaintBlendColor (GameObject target, string materialProperty, bool bakeMesh, Texture2D sampleTexture, bool useUV2) : base (target, materialProperty, bakeMesh, sampleTexture, useUV2)
		{
			m_PaintKit.PaintPixelFunc = PaintPixelBlendColor;
			m_BlendColor = new Color32 (255, 255, 255, 255);
			m_MeshCollider.sharedMesh = m_Renderer.sharedMesh;
		}

		protected override void SwitchItem (CocoMakeupItemData oldData, CocoMakeupItemData newData)
		{
			base.SwitchItem (oldData, newData);

//			if (newData != null && newData is MakeupItemData_ToolColor) {
//				MakeupItemData_ToolColor colorData = (MakeupItemData_ToolColor)newData;
//				m_BlendColor = colorData.BlendColor;
//			}
		}

		private Color32 PaintPixelBlendColor (Color32 pLayerColor, byte pMaskAlpha, byte pBrushAlpha, Color32 pBrushMixColor, Color32 pPatternColor)
		{
			if (pMaskAlpha == 0)
				return pLayerColor;
			if (pBrushAlpha == 0)
				return pLayerColor;

			// blend color
			int colorR = pPatternColor.r * m_BlendColor.r / 255;
			int colorG = pPatternColor.g * m_BlendColor.g / 255;
			int colorB = pPatternColor.b * m_BlendColor.b / 255;

			// update layer color
			int layerAlpha = pLayerColor.a + pBrushAlpha;
			switch (m_PaintKit.CurrentPattern.PatternFillMode) {
			case CocoMakeupPattern.FillMode.BrushArea:
				if (layerAlpha > 255)
					layerAlpha = 255;
				break;
			default:
				if (layerAlpha > pPatternColor.a)
					layerAlpha = pPatternColor.a;
				break;
			}
			pLayerColor.r = (byte)(colorR < 128 ? (2 * colorR * pBrushMixColor.r / 255) : (255 - 2 * (255 - colorR) * (255 - pBrushMixColor.r) / 255));
			pLayerColor.g = (byte)(colorG < 128 ? (2 * colorG * pBrushMixColor.g / 255) : (255 - 2 * (255 - colorG) * (255 - pBrushMixColor.g) / 255));
			pLayerColor.b = (byte)(colorB < 128 ? (2 * colorB * pBrushMixColor.b / 255) : (255 - 2 * (255 - colorB) * (255 - pBrushMixColor.b) / 255));
			pLayerColor.a = (byte)layerAlpha;

			return pLayerColor;
		}
	}

	public class CocoMakeupControlChange : CocoMakeupControlExecutorBase
	{
		Material m_Material;
		string m_MaterialProperty;
		Texture m_Texture;
		Vector2 m_ScreenPos;
		Texture m_OriginTexture;

		bool m_PaintValid;
		public CocoMakeupControlChange (GameObject target, string materialProperty, bool bakeMesh) : base (target, materialProperty, bakeMesh)
		{
			m_Material = m_Renderer.material;
			m_MaterialProperty = materialProperty;
			m_OriginTexture = m_Material.mainTexture;

			m_PaintValid = false;
		}

		protected override void SwitchItem (CocoMakeupItemData oldData, CocoMakeupItemData newData)
		{
		}

		public override void PaintDirectly ()
		{
			if (m_Texture != null) {
				//Resources.UnloadAsset (m_Texture);
				GameObject.Destroy (m_Texture);
			}
			m_Texture = ItemData != null ? GameObject.Instantiate(Resources.Load<Texture> (ItemData.m_TexturePath)) as Texture : null;
			//m_Texture = ItemData != null ? Resources.Load<Texture> (ItemData.m_TexturePath) : null;

			if (string.IsNullOrEmpty (m_MaterialProperty)) {
				m_Material.mainTexture = m_Texture;
			} else {
				m_Material.SetTexture (m_MaterialProperty, m_Texture);
			}
		}

		public override bool OnPaint (Vector2 screenPos)
		{
			Ray ray = Camera.main.ScreenPointToRay (screenPos);
			RaycastHit hit;
			if (m_MeshCollider.Raycast (ray, out hit, Camera.main.farClipPlane)) {
				PaintDirectly ();
				m_PaintValid = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		public override bool OnPaintEnd ()
		{
			//			Ray ray = Camera.main.ScreenPointToRay (m_ScreenPos);
			//			RaycastHit hit;
			//			if (m_MeshCollider.Raycast (ray, out hit, Camera.main.farClipPlane)) {
			//				PaintDirectly ();
			//				return true;
			//			}

			if (m_PaintValid)
			{
				m_PaintValid = false;
				return true;
			}

			return false;
		}

		public void ResetTexture()
		{
			if (string.IsNullOrEmpty(m_MaterialProperty))
			{
				m_Material.mainTexture = m_OriginTexture;
			}
			else
			{
				m_Material.SetTexture(m_MaterialProperty, m_OriginTexture);
			}
		}

	}
}