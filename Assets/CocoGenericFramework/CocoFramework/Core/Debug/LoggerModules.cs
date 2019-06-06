using UnityEngine;
using System.Collections;

namespace TabTale
{
	public static class LoggerModules
	{
		public static readonly int SceneManager;
		public static readonly int SocialServices;
		public static readonly int AssetManager;
		public static readonly int GameApplication;
		public static readonly int Tasks;
		public static readonly int InputSystem;

		public static void Register()
		{
		}

		static LoggerModules()
		{
			SceneManager = CoreLogger.RegisterModule("SceneManager");
			SocialServices = CoreLogger.RegisterModule("SocialServices");
			AssetManager = CoreLogger.RegisterModule("AssetManager");
			GameApplication = CoreLogger.RegisterModule("GameApplication");
			Tasks = CoreLogger.RegisterModule("Tasks");
			InputSystem = CoreLogger.RegisterModule("InputSystem");
		}
	}
}
