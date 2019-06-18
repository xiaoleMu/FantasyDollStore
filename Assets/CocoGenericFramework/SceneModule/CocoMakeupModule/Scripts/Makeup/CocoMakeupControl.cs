using UnityEngine;
using System.Collections.Generic;
using TabTale;
using System;

#if COCO_FAKE
using CocoRoleRendererID  = CocoPlay.Fake.CocoRoleRendererID;
#else
using CocoRoleRendererID = Game.CocoRoleRendererID;
#endif

namespace CocoPlay
{
	public class CocoMakeupControl : GameView
	{
		[Inject]
		public CocoMakeupPaintSignal makeupPaintSignal { get; set; }
		[Inject]
		public CocoMakeupPaintEndSignal makeupPaintEndSignal { get; set; }
		[Inject]
		public ICocoMakeupData MakeupData { get; set; }
		[Inject]
		public MakeupClearSignal makeupClearSignal { get; set;}


		Dictionary<CocoMakeupPaintLayer, CocoMakeupControlExecutorBase> m_ControlExecutorDic;
		protected CocoMakeupControlExecutorBase m_CurrControlExecutor;
		bool m_IsPainting;
		HashSet<CocoMakeupCategory> m_ValidPaintCategories;

		public List<string> m_MakeupHistory;

        public Action<string> OnPaintCallback;
		//	CCCharacterMakeupData m_CharacterMakeupData;

		#region Init and Clear

		protected override void AddListeners()
		{
			base.AddListeners();

			makeupPaintSignal.AddListener(OnPaint);
			makeupPaintEndSignal.AddListener(OnPaintEnd);
			makeupClearSignal.AddListener(OnClear);

		}

		protected override void RemoveListeners()
		{
			makeupPaintSignal.RemoveListener(OnPaint);
			makeupPaintEndSignal.RemoveListener(OnPaintEnd);
			makeupClearSignal.RemoveListener(OnClear);
			base.RemoveListeners();
		}

		protected override void Start()
		{
			base.Start();
		}

		public virtual void Init(CocoRoleBody roleBody)
		{
			foreach (CocoMakeupCategoryData categoryData in MakeupData.GetCategoryList())
			{
				// create paint executor
				if (!ControlExecutorDic.ContainsKey(categoryData.PaintLayer))
				{
					CocoMakeupControlExecutorBase executor = CreateExecutor(categoryData, roleBody);
					if (executor != null)
					{
						ControlExecutorDic.Add(categoryData.PaintLayer, executor);
					}
				}
			}

			m_CurrControlExecutor = null;
		}

		public void Clear()
		{
			foreach (CocoMakeupControlExecutorBase executor in ControlExecutorDic.Values)
			{
				executor.Clear();
			}

			ControlExecutorDic.Clear ();
			m_ControlExecutorDic.Clear ();
		}

		#endregion

		#region Category Change Listener

		protected CocoMakeupCategoryData m_CurrCategoryData;
		protected CocoMakeupItemData m_CurrItemData;
		public void SetCategoryData(CocoMakeupCategoryData categoryData)
		{
//			if (m_CurrCategoryData == pButton.MakeupCategoryData) return;

			m_CurrCategoryData = categoryData;
			m_CurrItemData = null;
			m_CurrControlExecutor = null;
			if (m_CurrCategoryData != null)
			{
				if (ControlExecutorDic.ContainsKey(m_CurrCategoryData.PaintLayer))
				{
					m_CurrControlExecutor = ControlExecutorDic[m_CurrCategoryData.PaintLayer];
				}
			}

			if(m_CurrCategoryData.PaintData is CocoMakeupCategoryPaintData_PaintTexture)
			{
				int layerID = ((CocoMakeupCategoryPaintData_PaintTexture)m_CurrCategoryData.PaintData).paintLayerId;
				m_CurrControlExecutor.PaintKit.SetCurrentCanvasLayer(layerID);
			}
		}

		public void SetItemData(CocoMakeupItemData itemData)
		{
			if (m_CurrItemData == itemData) return;

			m_CurrItemData = itemData;
			m_CurrControlExecutor.OnCategoryItemChanged(m_CurrItemData);

			//			m_IsPainting = false;
			//			CocoMakeupPaintType paintType = CocoMakeupPaintType.ChangeTexture;
			//
			//			if (m_CurrItemData != null)
			//			{
			//				CocoMakeupCategoryData categoryData = MakeupData.GetCategoryData(m_CurrItemData.Category);
			//
			//				bool faceToCamera = true;
			//				if (categoryData.PaintData is CocoMakeupCategoryPaintData_PaintTexture)
			//				{
			//					CocoMakeupCategoryPaintData_PaintTexture paintTextureData = (CocoMakeupCategoryPaintData_PaintTexture)categoryData.PaintData;
			//					faceToCamera = paintTextureData.faceToCameraWhenPaint;
			//				}
			//				if (faceToCamera)
			//				{
			////					MakeCharacterFaceToCamera();
			//				}
			//			}
		}

		#endregion

		#region Paint Listener

		private void OnPaint(Vector2 pScreenPos, CocoMakeupCategory pCategory)
		{
			if (m_CurrControlExecutor == null) return;

			bool inPainting = m_CurrControlExecutor.OnPaint(pScreenPos);

			if(inPainting)
			{
				PlayPaintEffect();
                if (OnPaintCallback != null)
                {
                    if (m_CurrItemData == null)
                        return;
                    string itemname = m_CurrItemData.Category.ToString() + "_" + m_CurrItemData.m_Index.ToString();
                    OnPaintCallback(itemname);
                }

			}
		}

		private void OnPaintEnd(CocoMakeupCategory pCategory)
		{
			if (m_CurrControlExecutor == null) return;

			if(m_CurrControlExecutor.OnPaintEnd())
			{
                PlayPaintEndEffect();
			}
		}


		void OnPaint(Vector2 screenPos)
		{
			if (m_CurrControlExecutor == null)
				return;

			if (!m_IsPainting)
			{
				m_IsPainting = true;
				//PlayCharacterAnimation (m_CurrCategoryData.paintStartAnim);
			}

			bool inPainting = m_CurrControlExecutor.OnPaint(screenPos);
			//			if (inPainting)
			//			{
			//				m_GuideControl.StopFinger();
			//			}
		}

		void OnPaintEnd(string _name)
		{
			if (m_CurrControlExecutor == null)
				return;
			m_IsPainting = false;
			//PlayCharacterAnimation (m_CurrCategoryData.paintEndAnim);
			if (m_CurrControlExecutor.OnPaintEnd())
			{
				//			if (m_CurrCategoryData != null)
				//			{
				//				AddValidCategory(m_CurrCategoryData.type);
				//
				//			}

				//			if (!m_MakeupHistory.Contains(_name))
				//			{
				//				m_MakeupHistory.Add(_name);
				//				int star = 25;
				//				ButtonDropStarType temp = SceneItemData.Instance.GetDropTypeWithName(_name);
				//				//Debug.LogError(temp.ToString());
				//				switch (temp)
				//				{
				//					case ButtonDropStarType.Free:
				//						star = GameData.Instance.GetSceneStar(GameSceneID.MakeUp);
				//						break;
				//					case ButtonDropStarType.Rv:
				//					case ButtonDropStarType.Iap:
				//						star = GameData.Instance.GetSceneStar(GameSceneID.MakeUp, SceneStarState.RV);
				//						break;
				//				}
				//
				//				//Debug.LogError(star);
				//				GameSignal.Instance.ProcessBarAdd(star);
				//			}

				//			m_CharacterMakeupData.SetItem(m_CurrItemData.belongCategoryData.type, m_CurrItemData.textureIndex);
				//			m_CharacterManager.SaveCharacterDress(m_CharacterManager.MainCharacter);
			}
			//		m_MainCtrl.SetMakeupAchievements(_name);
		}

		protected virtual void OnClear(CocoMakeupCategory _category)
		{
			if(_category == CocoMakeupCategory.Eyebrow || _category == CocoMakeupCategory.Eyecolor)
			{
				CocoMakeupControlChange temp =(CocoMakeupControlChange)m_CurrControlExecutor;
				temp.ResetTexture();
			}
			else
				m_CurrControlExecutor.ClearPatting();
			PlayFaceEffect();

			//m_CharacterMakeupData.RemoveItem(m_CurrControlExecutor.ItemData.belongCategoryData.type);
			//m_CharacterManager.SaveCharacterDress(m_CharacterManager.MainCharacter);
		}

		protected virtual void PlayFaceEffect ()
		{
			//		Transform parent = m_CharacterManager.MainCharacter.Dress.body.Head.transform;
			//		GameObject effect = CCLoad.LoadGameObject(PlayerKey.Spa_Particle_Face, parent);
			//		effect.transform.localPosition = new Vector3(0, 1.105f, 0.075f);
			//		CCSound.Play(GameAudioType.Button_Open);
		}

		public virtual void PlayPaintEffect()
		{
			
		}

		public virtual void PlayPaintEndEffect()
		{

		}
		#endregion


		#region Executor

		protected Dictionary<CocoMakeupPaintLayer, CocoMakeupControlExecutorBase> ControlExecutorDic
		{
			get
			{
				if (m_ControlExecutorDic == null)
					m_ControlExecutorDic = new Dictionary<CocoMakeupPaintLayer, CocoMakeupControlExecutorBase>();
				return m_ControlExecutorDic;
			}
		}

		protected virtual CocoMakeupControlExecutorBase CreateExecutor(CocoMakeupCategoryData categoryData, CocoRoleBody body)
		{
			GameObject target = null;
			string materialProperty = string.Empty;
			Texture2D sampleTexture = null;
			bool useUV2 = false;
			bool bakeMashEnable = true;
			CCPaintMode paintmode = CCPaintMode.MixOrigin;
			materialProperty = MakeupData.GetMaterialProperty(categoryData.PaintLayer);

			switch (categoryData.PaintLayer)
			{
				case CocoMakeupPaintLayer.Eye:
					target = body.GetRenderer(CocoRoleRendererID.EyeBall).gameObject;
					break;

				case CocoMakeupPaintLayer.EyeBrow:
					target = body.GetRenderer(CocoRoleRendererID.EyeBrow).gameObject;
					break;

				case CocoMakeupPaintLayer.EyeLash:
					target = body.GetRenderer(CocoRoleRendererID.Eyelash).gameObject;
					bakeMashEnable = false;
					paintmode = CCPaintMode.NonMixOrigin;
					break;

				case CocoMakeupPaintLayer.Head:
					target = body.GetRenderer(CocoRoleRendererID.Head).gameObject;
					break;

				case CocoMakeupPaintLayer.Head_Layer1:
					target = body.GetRenderer(CocoRoleRendererID.Head).gameObject;
					break;

				case CocoMakeupPaintLayer.Head_Layer2:
					target = body.GetRenderer(CocoRoleRendererID.Head).gameObject;
					useUV2 = true;
					break;

				default:
					target = body.GetRenderer(CocoRoleRendererID.Head).gameObject;
					break;
			}

			CocoMakeupControlExecutorBase executor = null;
			switch (categoryData.PaintData.paintType)
			{
				case CocoMakeupPaintType.PaintTexture:
					CocoMakeupCategoryPaintData_PaintTexture paintData = (CocoMakeupCategoryPaintData_PaintTexture)categoryData.PaintData;
					if (paintData.paintBlendColor)
					{
						executor = new CocoMakeupControlPaintBlendColor(target, materialProperty, bakeMashEnable, sampleTexture, useUV2);
					}
					else
					{
						executor = new CocoMakeupControlPaint(target, materialProperty, bakeMashEnable, sampleTexture, useUV2);
					}

					executor.PaintKit.SetMixMode(paintmode);
					break;
				default:
					executor = new CocoMakeupControlChange(target, materialProperty, bakeMashEnable);
					break;
			}

			return executor;
		}

		#endregion


		#region Valid Category

		//	HashSet<CcocoMakeupCategory> ValidPaintCategories
		//	{
		//		get
		//		{
		//			if (m_ValidPaintCategories == null)
		//				m_ValidPaintCategories = new HashSet<CcocoMakeupCategory>();
		//			return m_ValidPaintCategories;
		//		}
		//	}

		void AddValidCategory(CocoMakeupCategory categoryType)
		{
			//		if (!ValidPaintCategories.Contains(categoryType))
			//		{
			//			ValidPaintCategories.Add(categoryType);
			//		}
		}

		//	public float ValidCateogryProgress
		//	{
		//		get
		//		{
		//			float percent = ValidPaintCategories.Count * 1f / m_MakeupData.CategoryList.Count;
		//			if (percent > 1)
		//				percent = 1;
		//			return percent;
		//		}
		//	}

		#endregion


	}
}