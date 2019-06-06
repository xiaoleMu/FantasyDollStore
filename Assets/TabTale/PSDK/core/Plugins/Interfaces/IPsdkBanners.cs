using UnityEngine;
using System.Collections;

namespace TabTale.Plugins.PSDK 
{
	public interface IPsdkBanners : IPsdkService
	{
		void Hide();
		bool Show();
		float GetAdHeight();
		bool IsBlockingViewNeeded();
		bool IsActive();
		bool IsAlignedToTop();

		// Inititalization
		bool Setup();
		IPsdkBanners GetImplementation();
	}
}
