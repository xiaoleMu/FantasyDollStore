using UnityEngine;
using UnityEditor;

namespace TabTale
{
	[InitializeOnLoad]
	static class ProjectValidation
	{
		static ProjectValidation()
		{
			ValidateApiCompatibility();
		}

		static void ValidateApiCompatibility()
		{
			if(PlayerSettings.apiCompatibilityLevel == ApiCompatibilityLevel.NET_2_0_Subset)
			{
				Debug.LogError("TABTALE VALIDATION: The Gaming SDK requires the API Compatbility be set to .Net 2.0, Please change your settings accordingly (In Player Settings -> Other)");
			}
		}
	}

}