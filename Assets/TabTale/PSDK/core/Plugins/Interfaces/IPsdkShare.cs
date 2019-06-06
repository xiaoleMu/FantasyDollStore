using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK {

	public interface IPsdkShare  : IPsdkService {
		
		IPsdkShare GetImplementation();

		void ShareWithImage(string shareSubject, string shareBodyText, string appStoreUrl, string shareHeadLine);

		void ShareScreenshot();
		void ShareVideo(string pathToVideo);
		void ShareImage(string pathToImage);
		void ShareLink(string link);
		void ShareAppLink();
	}
}

