using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale {
	
	public sealed class ApplicationLocation
	{
		public static readonly ApplicationLocation AppLoaded;
		public static readonly ApplicationLocation MoreApps;
		public static readonly ApplicationLocation MoreAppsInSeries;
		public static readonly ApplicationLocation Terminated;
		public static readonly ApplicationLocation SceneStart;
		public static readonly ApplicationLocation SceneEnd;
		public static readonly ApplicationLocation FocusGained;
		public static readonly ApplicationLocation FocusLost;
		public static readonly ApplicationLocation GameOver;
		public static readonly ApplicationLocation LevelStart;
		public static readonly ApplicationLocation LevelEnd;
		public static readonly ApplicationLocation GameStart;

		public static readonly ApplicationLocation RewardedAds;
		public static readonly ApplicationLocation Interstitial;
		public static readonly ApplicationLocation BannerAds;
		public static readonly ApplicationLocation SceneTransitions;

		private readonly string _name;

		private static IDictionary<string, ApplicationLocation> s_nameToEvent = new Dictionary<string, ApplicationLocation>();

		static ApplicationLocation()
		{
			AppLoaded = NameToEvent("sessionStart");
			MoreApps = NameToEvent("moreApps");
			MoreAppsInSeries = NameToEvent("MoreAppsInSeries");
			Terminated = NameToEvent("SessionEnded");
			SceneStart = NameToEvent("SceneStart");
			SceneEnd = NameToEvent("SceneEnd");
			FocusGained = NameToEvent("FocusGained");
			FocusLost = NameToEvent("FocusLost");
			GameOver = NameToEvent("GameOver");
			LevelStart = NameToEvent("LevelStart");
			LevelEnd = NameToEvent("LevelEnd");
			GameStart = NameToEvent("GameStart");

			// Tempoarry placehoders until PSDK implement the location manager:
			RewardedAds = NameToEvent("RewardedAds");
			Interstitial = NameToEvent("interstitial");
			BannerAds = NameToEvent ("BannerAds");
			SceneTransitions = NameToEvent("sceneTransitions");
		}

		private ApplicationLocation(string name)
		{
			this._name = name;
		}
		
		public override string ToString()
		{
			return _name;
		}		

		public string Name
		{
			get { return _name; }
		}

		public static ICollection<ApplicationLocation> Events
		{
			get { return s_nameToEvent.Values; }
		}

		public static ApplicationLocation NameToEvent(string name)
		{
			ApplicationLocation applicationEvent;
			if(s_nameToEvent.TryGetValue(name, out applicationEvent))
				return applicationEvent;

			applicationEvent = new ApplicationLocation(name);

			s_nameToEvent[name] = applicationEvent;

			return applicationEvent;
		}
	}
}
