using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TabTale;

namespace CocoPlay
{
	public partial class CocoRoleControl : GameView
	{
		[Inject]
		public CocoAssetControl assetControl { get; set; }

		[Inject]
		public CocoDressStateModel DressStateModel { get; set; }

		Dictionary<string, CocoRoleEntity> m_Roles;

		/// <summary>
		/// Gets the or create main role.
		/// 建立主要角色 (全局存在，如果已存在，则返回存在的那个)
		/// </summary>
		/// <returns>The main role.</returns>
		/// <param name="roleId">Role identifier.</param>
		/// <param name="roleName">Role name.</param>
		public CocoRoleEntity GetOrCreateMainRole (string roleId, string roleName, bool recordDress = true)
		{
			if (m_Roles == null)
				m_Roles = new Dictionary<string, CocoRoleEntity> ();

			string tRoleKey = GetRoleKey (roleId, roleName);
			if (m_Roles.ContainsKey (tRoleKey)) {
				return m_Roles [tRoleKey];
			}

			CocoRoleEntity role = CreateTempRole (roleId, roleName, transform, recordDress);
			m_Roles.Add (tRoleKey, role);
			return role;
		}

		public bool DestoryMainRole (string roleId, string roleName)
		{
			string tRoleKey = GetRoleKey (roleId, roleName);
			return DestoryMainRole (tRoleKey);
		}

		public bool DestoryMainRole (string roleKey)
		{
			if (m_Roles.ContainsKey (roleKey)) {
				CocoRoleEntity role = m_Roles [roleKey];
				GameObject.Destroy (role.gameObject);
				m_Roles.Remove (roleKey);
				return true;
			}

			return false;
		}

		public CocoRoleEntity CurRole { get; set; }

		/// <summary>
		/// Creates the temp role.
		/// 创建一个临时的角色，不放在Control下，你需要自己删除
		/// </summary>
		/// <returns>The temp role.</returns>
		/// <param name="roleId">Role identifier.</param>
		/// <param name="roleName">Role name.</param>
		/// <param name="parent">parent transform</param>
		/// <param name="recordDress">record dress</param>
		public CocoRoleEntity CreateTempRole (string roleId, string roleName, Transform parent = null, bool recordDress = false)
		{
			CocoRoleHolder roleHolder = assetControl.ConfigHolder.GetRoleHolder (roleId);
			if (roleHolder == null) {
				Debug.LogErrorFormat ("{0}->CreateTempRole: can NOT found role holder for role id {1}", GetType ().Name, roleId);
				return null;
			}

			CocoRoleEntity role = CocoLoad.InstantiateOrCreate<CocoRoleEntity> (string.Empty, parent);
			role.name = roleName;

			role.DressRecordKey = GetRoleKey (roleId, roleName);
			role.IsDressRecordActive = recordDress;

			role.Init (roleHolder);

			return role;
		}

		public static string GetRoleKey (string roleId, string roleName)
		{
			return string.Format ("{0}|{1}", roleId, roleName);
		}


		#region Dress

		public List<string> GetRoleInitDressItemIds (string roleId, string roleName)
		{
			// record
			var roleKey = GetRoleKey (roleId, roleName);
			var dressItemIds = DressStateModel.GetRoleDressItems (roleKey);
			if (dressItemIds != null && dressItemIds.Count > 0) {
				return dressItemIds;
			}

			// default
			var roleHolder = assetControl.ConfigHolder.GetRoleHolder (roleId);
			if (roleHolder != null) {
				return roleHolder.basicItemIds;
			}

			return null;
		}

		#endregion
	}
}