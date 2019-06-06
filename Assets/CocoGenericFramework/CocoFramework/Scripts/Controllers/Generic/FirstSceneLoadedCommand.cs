using System;
using UnityEngine;
using strange.extensions.command.impl;
using TabTale.Analytics;
using System.Collections.Generic;

namespace TabTale
{
	public class FirstSceneLoadedCommand : GameCommand
	{
		[Inject]
		public string sceneName { get; set; }

		[Inject]
		public AnalyticsService analyticsService { get; set; }

		[Inject]
		public IAppInfo appInfoService { get; set; }

		private enum PlatformType
		{
			IOS_MOBILE,
			IOS_TABLET,
			ANDROID,
			ANDROID_MOBILE,
			ANDROID_TABLET
		}

		private PlatformType Platform 
		{
			get 
			{
				#if UNITY_IPHONE
				if(SystemInfo.deviceModel.Contains("iPad"))
					return PlatformType.IOS_TABLET;
				else
					return PlatformType.IOS_MOBILE;
				#else
					return PlatformType.ANDROID;
				#endif
			}	
		}

		public override void Execute()
		{
			logger.Log("FirstSceneLoadedCommand - Scene name :" +  sceneName);

			if(Application.platform.IsStandalone())
				return;
			
			ReportLoadingTime();
		}

		private void ReportLoadingTime()
		{
			int loadingTime = (int) Time.realtimeSinceStartup;
			logger.Log(Tag, "ReportLoadingTime - " + loadingTime + "s");

			IDictionary<string, object> eventParams = new Dictionary<string, object > ();
			eventParams ["clientVersion"] = appInfoService.BundleVersion;
			eventParams ["loadingTime"] = loadingTime;
			eventParams ["platform"] = Platform.ToString();

			analyticsService.LogEvent (AnalyticsTargets.ANALYTICS_TARGET_DELTA_DNA, "loadingTime", eventParams, false);
		}
	}
}

