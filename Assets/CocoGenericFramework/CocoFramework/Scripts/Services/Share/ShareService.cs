using UnityEngine;
using System.Collections;
using TabTale.Plugins.PSDK;

namespace TabTale
{
    public class ShareService : IShareService
    {
		private IPsdkShare _psdkShare;

		[PostConstruct]
        public void Init()
        {
            Debug.Log("ShareService : Init");
			_psdkShare = PSDKMgr.Instance.GetShare();
        }

        public void ShareWithImage(string shareSubject, string shareBodyText, string appStoreUrl, string shareHeadLine)
        {
			_psdkShare.ShareWithImage(shareSubject, shareBodyText, appStoreUrl, shareHeadLine);
        }
    }

}

