using UnityEngine;
using System.Collections;
using TabTale;
using TabTale.Publishing;

public class AdjustToBannersHeightView : GameView {
	[Inject]
	public IBannerAds bannerAds { get; set; }
	[Inject]
	public BannerAdsShownSignal bannerAdsShownSignal { get; set; }
	[Inject]
	public BannerAdsHiddenSignal bannerAdsHiddenSignal { get; set; }

	private float originalY;

	private void PrintPosition(Vector3 position, string extra)
	{
		if(Debug.isDebugBuild) Debug.Log("adjust to banner - " + gameObject.name + " ; position " + extra + " : (" + position.x + "," + position.y + "," + position.z + ")");
	}
	
	private void UpdatePosition()
	{
		Vector3 position = gameObject.transform.position;
		PrintPosition(position, "before");
		if (bannerAds.IsShowing)
		{
			gameObject.transform.position = new Vector3(position.x, originalY + bannerAds.GetAdHeightInPercentage() * Screen.height, position.z);
		}
		else
		{
			gameObject.transform.position = new Vector3(position.x, originalY, position.z);
		}
		PrintPosition(gameObject.transform.position, "after");
	}

	protected override void Start()
	{
		base.Start();
		originalY = gameObject.transform.position.y;
		PrintPosition(gameObject.transform.position, "original");
		UpdatePosition();

		bannerAdsShownSignal.AddListener(UpdatePosition);
		bannerAdsHiddenSignal.AddListener(UpdatePosition);
	}

	protected override void OnDestroy ()
	{
		bannerAdsShownSignal.RemoveListener(UpdatePosition);
		bannerAdsHiddenSignal.RemoveListener(UpdatePosition);
		base.OnDestroy();
	}
}
