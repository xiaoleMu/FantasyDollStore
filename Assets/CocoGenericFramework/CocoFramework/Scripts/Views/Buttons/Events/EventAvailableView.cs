using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace TabTale
{
	public class EventAvailableView : MainView
	{
		[Inject]
		public EventSystemService eventSystemService { get; set; }

		[Inject]
		public IModalityManager modalityManager { get; set; }

		[Inject]
		public TickerService tickerService { get; set; }

		[Inject]
		public NetworkCheck networkCheck { get; set; }

		[Inject]
		public RemoteImageService remoteImageService { get; set; }

		[Inject]
		public ISocialNetworkService socialNetwork{ get; set; }

		[Inject]
		public ServerTime serverTime{ get; set; }

		[Inject]
		public EventConfigModel eventConfigModel { get; set; }

		protected EventConfigData eventData;

		public RawImage bannerImage;
		public RawImage prizeImage;
		public Text eventCounter;

		protected override void Start()
		{
			base.Start();

			eventData = eventSystemService.GetAvailableEvents().First();

			List<AssetData> assetData = eventConfigModel.GetAssets (eventData.id);

			StartCoroutine(SetBanner(assetData[0], bannerImage));
			StartCoroutine(SetBanner(assetData[1], prizeImage));
		}
	
		protected IEnumerator SetBanner(AssetData asset, RawImage targetImage)
		{
			WWW www = remoteImageService.LoadRemoteImage(asset);
			yield return www;

			Texture2D tex = www.texture;
			if (tex != null)
			{
				Rect rec = new Rect (0, 0, tex.width, tex.height);
				targetImage.texture = tex;
			}
		}
			
		protected virtual void RegisterToEvent()
		{
			
		}
	}
}
