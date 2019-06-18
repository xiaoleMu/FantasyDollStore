using UnityEngine;
using System.Collections.Generic;
using LitJson;
using TabTale;

namespace CocoPlay
{
	public class CocoDressStateData : CocoLockableStateData
	{
		#region implemented abstract members of CocoLockableStateDataBase

		public override string GetStateName ()
		{
			return "cocoDressState";
		}

		public override CocoLockableStateData Create ()
		{
			return new CocoDressStateData ();
		}

		public override string ToLogString ()
		{
			return string.Format ("{0}, roleDressItemIds [{1}]", base.ToLogString (), roleDressItemIds.Count);
		}

		#endregion


		#region Init

		public override void CloneContent (CocoLockableStateData source)
		{
			base.CloneContent (source);

			CocoDressStateData sourceData = (CocoDressStateData)source;
			roleDressItemIds = new Dictionary<string, List<string>> ();
			sourceData.roleDressItemIds.ForEach ((roleId, dressItems) => {
				roleDressItemIds.Add (roleId, dressItems);
			});
		}

		#endregion


		#region Role Dress

		public Dictionary<string, List<string>> roleDressItemIds = new Dictionary<string, List<string>> ();

		public void UpdateRoleDressItems (string roleId, List<string> dressItemIds)
		{
			if (!roleDressItemIds.ContainsKey (roleId)) {
				roleDressItemIds.Add (roleId, dressItemIds);
				return;
			}

			roleDressItemIds [roleId] = dressItemIds;
		}

		public List<string> GetRoleDressItems (string roleId)
		{
			if (!roleDressItemIds.ContainsKey (roleId)) {
				return null;
			}

			return roleDressItemIds [roleId];
		}

		#endregion
	}
}
