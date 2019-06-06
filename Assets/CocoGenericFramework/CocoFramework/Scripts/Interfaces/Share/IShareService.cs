using UnityEngine;
using System.Collections;

namespace TabTale
{
    public interface IShareService
	{
		void Init();

		void ShareWithImage(string shareSubject, string shareBodyText, string appStoreUrl, string shareHeadLine);
	}
}

