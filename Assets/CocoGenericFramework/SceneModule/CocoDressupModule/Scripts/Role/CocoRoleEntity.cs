using System;
using UnityEngine;
using System.Collections.Generic;
using TabTale;

#if !COCO_FAKE
using CocoRoleBoneID = Game.CocoRoleBoneID;

#else
using CocoRoleBoneID = CocoPlay.Fake.CocoRoleBoneID;
#endif

namespace CocoPlay
{
	public partial class CocoRoleEntity : GameView
	{
		#region Init

		[Inject]
		public CocoRoleControl roleControl { get; set; }

		public bool IsReady { get; private set; }

		public void Init (CocoRoleHolder roleHolder)
		{
			RoleHolder = roleHolder;

			// dress
			Dress = AddUnit<CocoRoleDress> ();

			StartCoroutine (CocoWait.WaitForFunc (() => {
				return !Dress.IsReady;
			}, () => {
				// body
				Body = AddUnit<CocoRoleBody> ();

				// shadow
				if (RoleHolder.enableShadow) {
					Shadow = CocoLoad.Instantiate<CocoRoleShadow> ("role/role_shadow", transform, CocoLoad.TransStayOption.Local);
					Shadow.FollowTarget = Body.GetBone (CocoRoleBoneID.Root);
				}

				// layer
				if (transform.parent != null) {
					transform.SetSelfAndChildLayer (transform.parent.gameObject.layer);
				}

				IsReady = true;
			}));
		}

		#endregion


		#region Unit

		private Dictionary<string, CocoRoleUnitBase> m_UnitDic = new Dictionary<string, CocoRoleUnitBase> ();

		public T AddUnit<T> () where T : CocoRoleUnitBase
		{
			string unitId = typeof(T).Name;
			if (m_UnitDic.ContainsKey (unitId)) {
				return m_UnitDic [unitId] as T;
			}

			T unit = CocoLoad.GetOrAddComponent<T> (gameObject);
			unit.Init (this);

			m_UnitDic.Add (unitId, unit);
			return unit;
		}

		public void RemoveUnit<T> () where T : CocoRoleUnitBase
		{
			string unitId = typeof(T).Name;
			if (!m_UnitDic.ContainsKey (unitId)) {
				return;
			}

			CocoRoleUnitBase unit = m_UnitDic [unitId];
			m_UnitDic.Remove (unitId);

			Destroy (unit);
		}

		public T GetUnit<T> () where T : CocoRoleUnitBase
		{
			string unitId = typeof(T).Name;
			if (!m_UnitDic.ContainsKey (unitId)) {
				return null;
			}

			return m_UnitDic [unitId] as T;
		}

		#endregion


		#region Unit References

		public CocoRoleDress Dress { get; private set; }

		[Obsolete ("This property is deprecated because of typo error, please use \"DressRecordKey\" instead.", true)]
		public string DressRecrodKey {
			get { return DressRecordKey; }
			set { DressRecordKey = value; }
		}

		public CocoRoleBody Body { get; private set; }

		public string DressRecordKey { get; set; }

		public bool IsDressRecordActive { get; set; }

		#endregion


		#region Animation

		private CCCharacterAnimation m_Animation;

		public CCCharacterAnimation Animation {
			get {
				if (m_Animation == null) {
					Animator animator = GetComponentInChildren<Animator> ();
					m_Animation = CocoLoad.GetOrAddComponent<CCCharacterAnimation> (animator.gameObject);
				}
				return m_Animation;
			}
		}

		#endregion


		#region Shadow

		public CocoRoleShadow Shadow { get; private set; }

		#endregion


		#region Data

		public CocoRoleHolder RoleHolder { get; private set; }

		public ICocoRoleCustomData CustomData { get; set; }

		#endregion
	}
}
