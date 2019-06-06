using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

namespace TabTale
{
	public class PsdkAudienceProvider : IAudience
	{
		private IPsdkAudience _psdkAudience;

		[PostConstruct]
		public void Init()
		{
			_psdkAudience = PSDKMgr.Instance.GetAudience();
		}

		#region IPsdkAudienceProvider implementation

		public void SetBirthYear (int birthYear)
		{
			_psdkAudience.SetBirthYear(birthYear);
		}

		public int GetAge ()
		{
			return _psdkAudience.GetAge();
		}

		public PSDKAudienceMode GetAudienceMode ()
		{
			return _psdkAudience.GetAudienceMode();
		}

		#endregion
	}
}
