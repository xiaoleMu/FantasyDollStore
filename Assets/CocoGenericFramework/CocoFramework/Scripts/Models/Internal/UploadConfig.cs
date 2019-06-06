using UnityEngine;
using System.Collections.Generic;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.injector.impl;
using strange.extensions.injector.api;

namespace TabTale
{
	public class UploadConfig : GameView 
	{
		[Inject]
		public IGameDB gameDB { get; set; }

		[Inject]
		public ItemConfigModel itemConfigModel { get; set; }

		[Inject]
		public IAPConfigModel iapConfigModel { get; set; }

		[Inject]
		public AchievementsConfigModel achievementsConfigModel { get; set; }

		[Inject]
		public LeaderboardConfigModel leaderboardConfigModel { get; set; }

		[Inject]
		public GeneralParameterConfigModel generalParametersConfigModel { get; set; }

		[Inject]
		public CategoryConfigModel categoryConfigModel { get; set; }

        //[Inject]
        //public CinemaSeriesConfigModel cinemaSeriesConfigModel { get; set; }

        [Inject]
		public TextConfigModel textConfigModel { get; set; }

		[Inject]
		public ModelSyncService modelSyncService { get; set; }

		[Inject]
		public RankConfigModel rankConfigModel { get; set; }
		
		[Inject]
		public IInjectionBinder injectionBinder{ get; set; }

		void Start () 
		{
            //UploadConfigModel<TextConfigData>(textConfigModel);
            // UploadConfigModel<GeneralParameterConfigData>(generalParametersConfigModel);
            //UploadConfigModel<ItemConfigData>(itemConfigModel);
            //UploadConfigModel<IAPConfigData>(iapConfigModel);
            //UploadConfigModel<RankConfigData>(rankConfigModel);
            //UploadConfigModel<LevelConfigData>(levelConfigModel);
            //UploadConfigModel<AchievementsConfigData>(achievementsConfigModel);
            //UploadConfigModel<LeaderboardConfigData>(leaderboardConfigModel);
           // UploadConfigModel<GeneralParameterConfigData>(generalParametersConfigModel);
            //UploadConfigModel<ItemConfigData>(itemConfigModel);
            //UploadConfigModel<LevelConfigData>(levelConfigModel);
            //UploadConfigModel<CategoryConfigData>(categoryConfigModel);
            //UploadConfigModel<CinemaSeriesConfigData>(cinemaSeriesConfigModel);
        }

		private void UploadConfigModel<T> (ConfigModel<T> model) where T : class, IConfigData , new()
		{
			var configs = model.GetAllConfigs();
			foreach (var item in configs) 
			{
				StartCoroutine(modelSyncService.UploadConfig(item));
			}
		}

	}
}