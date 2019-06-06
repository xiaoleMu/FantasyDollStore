using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TabTale.Plugins.PSDK {

	public interface IPsdkNativeCampaign : IPsdkService {
		bool ShowLocation(string location, RectTransform rectTransform, Camera camera);
		bool ShowLocation(string location, float xPosition, float yPosition, float width, float height, float angle);
        bool Show(string location = "homeScreen");

        bool HideLocation(string location);
        bool Hide(string location = "homeScreen");
        bool IsReady(string location);
	}

}

