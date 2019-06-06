using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TabTale
{
	public class EventCurentView : MainView
	{
		[Inject]
		public EventSystemService eventSystemService { get; set; }

		[Inject]
		public IModalityManager modalityManager { get; set; }

		[Inject]
		public RemoteImageService remoteImageService { get; set; }

		List<EventStateData> currentEventData = new List<EventStateData>();
		private EventStateData _currentEventData = null;

		List<EventConfigData> availableEventData = new List<EventConfigData>();
		private EventConfigData _availableEventData = null;

		public RawImage bannerImage;
		public string eventCounter;

		protected override void Awake()
		{
			base.Awake();
			currentEventData = eventSystemService.GetCurrentEvents ();
			availableEventData = eventSystemService.GetAvailableEvents ();
		}

		protected override void Start()
		{
			base.Start();
			_currentEventData = currentEventData [0];
			_availableEventData = availableEventData [0];
			SetBannerForEvent ();
		}

		public IEnumerator SetBannerForEvent()
		{
			WWW www = remoteImageService.getCachedWWW(_availableEventData.assets[0].value); //To-do - add field in eventState for banerr image url
			yield return www;

			Texture2D tex = www.texture;
			if (tex != null)
			{
				Rect rec = new Rect (0, 0, tex.width, tex.height);
				bannerImage.texture = tex;
			}
		}

		public void Go()
		{
			//To-do - add field in eventState for playerCurrentLevel
			string[] levelsToLoad = eventSystemService.eventConfigModel.GetLevels (_currentEventData.id).ToArray ();
		}


	}
}
