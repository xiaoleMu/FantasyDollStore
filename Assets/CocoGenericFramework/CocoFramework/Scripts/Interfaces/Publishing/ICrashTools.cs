using System;

namespace TabTale
{
	public interface ICrashTools
	{
		void AddBreadCrumb(string crumb); 

		void ClearAllBreadCrumbs();
	}
}

