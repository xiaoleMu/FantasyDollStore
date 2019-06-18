using UnityEngine;
using System.Collections.Generic;
using LitJson;
using TabTale;

namespace CocoPlay
{
	public class CocoDressStateModel : CocoLockableStateModel<CocoDressStateData>
	{
		#region Role Dress

		public void UpdateRoleDressItems (string roleId, List<string> dressItemIds)
		{
			_data.UpdateRoleDressItems (roleId, dressItemIds);
			Save ();
		}

		public List<string> GetRoleDressItems (string roleId)
		{
			return _data.GetRoleDressItems (roleId);
		}

		#endregion
	}
}
