using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;
using TabTale.Publishing;

namespace TabTale
{
    public class ShowBannersCommand : Command
    {
		[Inject]
		public IBillingService billingService { get; set; }
        [Inject]
        public SettingsStateModel settingsStateModel { get; set; }
		[Inject]
		public IBannerAds bannerAds { get; set; }


        public override void Execute ()
        {
			bool showingAds = settingsStateModel.ShowingAds();
			Debug.Log ("ShowBannersCommand - Banners Active: " + bannerAds.IsActive() + " ShowingAds: " + showingAds);

			if(bannerAds.IsActive() && showingAds)
			{
				bannerAds.Show();
			}

        }
    }
    
}