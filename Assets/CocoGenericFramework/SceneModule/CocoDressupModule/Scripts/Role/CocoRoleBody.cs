using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if !COCO_FAKE
using CocoRoleBoneID = Game.CocoRoleBoneID;
using CocoRoleRendererID = Game.CocoRoleRendererID;

#else
using CocoRoleBoneID = CocoPlay.Fake.CocoRoleBoneID;
using CocoRoleRendererID = CocoPlay.Fake.CocoRoleRendererID;
#endif

namespace CocoPlay
{
	public class CocoRoleBody : CocoRoleUnitBase
	{
		CocoRoleBodyHolder m_BodyHolder = null;
		ICocoRoleBodyProvider m_BodyProvider = null;

		public override void Init (CocoRoleEntity owner)
		{
			base.Init (owner);

			m_BodyHolder = Owner.RoleHolder.BodyHolder;
			m_BodyProvider = Owner.Dress;
			if (m_BodyHolder == null || m_BodyProvider == null) {
				Debug.LogErrorFormat ("[{0}<{1}>]->Init: body holder or provider NOT exists!", name, GetType ().Name);
			}

			InitBones ();
			InitRenderers ();
		}

		#region Bone

		Dictionary <CocoRoleBoneID, Transform> m_BoneDic = null;

		void InitBones ()
		{
			m_BoneDic = new Dictionary<CocoRoleBoneID, Transform> ();
			m_BodyHolder.bodyBones.ForEach ((boneKey, boneValue) => {
				CocoRoleBoneID boneId;
				if (!boneKey.ToEnum (out boneId)) {
					Debug.LogErrorFormat ("[{0}<{1}>]->InitBones: bone id [{2}] NOT exists!", name, GetType ().Name, boneKey);
					return;
				}

				Transform bone = m_BodyProvider.GetBone (boneValue);
                if (bone != null) {
                    m_BoneDic.Add (boneId, bone);
                } else {
                    Debug.LogErrorFormat ("[{0}<{1}>]->InitBones: bone name [{2}] NOT exists!", name, GetType ().Name, boneValue);
                }
			});
		}

		public Transform GetBone (CocoRoleBoneID boneId)
		{
			return m_BoneDic.GetValue (boneId);
		}

		#endregion


		#region Render

		Dictionary<CocoRoleRendererID, SkinnedMeshRenderer> m_RendererDic = null;
		private Dictionary<string, CocoRoleRendererID> m_RendererNameIdDic = null;

		void InitRenderers ()
		{
			int count = m_BodyHolder.bodyRenderers.Count;
			m_RendererNameIdDic = new Dictionary<string, CocoRoleRendererID> (count);
			m_RendererDic = new Dictionary<CocoRoleRendererID, SkinnedMeshRenderer> (count);

			m_BodyHolder.bodyRenderers.ForEach ((rendererKey, rendererValue) => {
				CocoRoleRendererID rendererId;
				if (!rendererKey.ToEnum (out rendererId)) {
					Debug.LogErrorFormat ("[{0}<{1}>]->InitRenderers: renderer id [{2}] NOT exists!", name, GetType ().Name, rendererKey);
					return;
				}

				if (m_RendererNameIdDic.ContainsKey (rendererValue)) {
					Debug.LogErrorFormat ("[{0}<{1}>]->InitRenderers: renderer name [{2}] ALREADY exists!", name, GetType ().Name, rendererValue);
					return;
				}

				m_RendererNameIdDic.Add (rendererValue, rendererId);

				var smr = m_BodyProvider.GetRenderer (rendererValue);
                m_RendererDic.Add (rendererId, smr);
			});

			m_BodyProvider.OnRendererChanged += OnRendererChanged;
		}

		public SkinnedMeshRenderer GetRenderer (CocoRoleRendererID renderId)
		{
			return m_RendererDic.GetValue (renderId);
		}

		private void OnRendererChanged (SkinnedMeshRenderer smr, bool add)
		{
			if (smr == null || !m_RendererNameIdDic.ContainsKey (smr.name)) {
				return;
			}

			var rendererId = m_RendererNameIdDic [smr.name];
			m_RendererDic [rendererId] = add ? smr : null;
		}

		#endregion
	}
}
