using UnityEngine;
using System.Collections;

namespace TabTale
{
    public class NullShareServiceProvider : IShareService
    {
        public void Init()
        {
            Debug.Log("NullShareServiceProvider - Initialisaion complete");
        }

		public void ShareWithImage(string shareSubject, string shareBodyText, string appStoreUrl, string shareHeadLine)
        {
            Debug.Log("NullShareServiceProvider - Sharing an image");
        }
    }
}

