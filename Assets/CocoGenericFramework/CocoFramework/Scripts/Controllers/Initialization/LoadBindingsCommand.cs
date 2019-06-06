using System;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.injector.impl;
using strange.extensions.command.impl;
using strange.framework.impl;
using TabTale.Publishing;
using TabTale.Analytics;

namespace TabTale 
{
	public class LoadBindingsCommand : Command
	{
		[Inject]
		public IGameDB gameDB { get; set; }

		[Inject]
		public GSDKConfigModel gsdkConfigModel { get; set; }

		public override void Execute()
		{
			CoreLogger.LogDebug("LoadBindingsCommand","Execute");

			List<BindingData> bindings = gsdkConfigModel.GetBindings();

			foreach(BindingData binding in bindings)
			{
				try
				{
					string json = LitJson.JsonMapper.ToJson(new List<BindingData> { binding });
					injectionBinder.ConsumeBindings(json);

					Debug.Log ("BindingLoading - Successfully binded: " + binding.ToString());
				}
				catch(InjectionException ex)
				{
					Debug.Log ("BindingLoading - did not load binding: " + binding);
				}
				catch(BinderException ex)
				{
					Debug.Log ("BindingLoading - did not load binding: " + binding);
					Debug.LogWarning(ex.ToString());
				}
			}

			LoadNullBindings();
		}

		void LoadNullBindings()
		{
			LoadNullProvider<ILocationManager,NullLocations>();
			LoadNullProvider<IPsdkCoreInitializer,NullPsdkCoreInitializer>();
			LoadNullProvider<IPsdkCoreServices,NullPsdkCoreServices>();
			LoadNullProvider<IAppsFlyer,NullPsdkAppsFlyer>();
			LoadNullProvider<ISocialNetworkService,NullSocialNetworkProvider>();
			LoadNullProvider<ISocialService,NullSocialProvider>();
			LoadNullProvider<IRewardedAds,NullRewardedAds>();
			LoadNullProvider<IBannerAds,NullBannerAds>();
			LoadNullProvider<IBillingService,NullBillingService>();
			LoadNullProvider<IRateUsService,NullRateUsService>();
			LoadNullProvider<IAppInfo,NullAppInfoService>();
			LoadNullProvider<IShareService,NullShareServiceProvider>();
			LoadNullProvider<INotificationService,NullNotificationService>();
			LoadNullProvider<IAnalyticsService,NullAnalyticsProvider>();
			LoadNullProvider<IReceiptValidator,NullReceiptValidator>();
			LoadNullProvider<ICrashTools,NullPsdkCrashTools>();
		}

		public void LoadNullProvider<T,S>()
		{
			if(injectionBinder.GetBinding<T>() == null)
			{
				Debug.Log ("LoadBindingsCommand - no provider for " + typeof(T).Name + " loaded from gsdk_config json bindings, loading null provider " + typeof(S).Name);
				injectionBinder.Bind<T>().To<S>().ToSingleton().CrossContext();
			}
		}
	}
}

