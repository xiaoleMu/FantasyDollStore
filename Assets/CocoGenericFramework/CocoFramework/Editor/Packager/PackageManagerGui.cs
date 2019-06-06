using UnityEngine;
using System.Collections;
using UnityEditor;

namespace TabTale.Packages {

	public class PackageManagerGui
	{
#if SIMULATION
		[MenuItem("TabTale/Framework/Packages/Build Framework Packages")]
		static void BuildAssetBundles () 
		{
			PackageManager.ExportPackages();
		}
#endif
	}
}
