using UnityEngine;
using TabTale;
using CocoPlay;
using LitJson;

namespace Game
{
	public class GameStartModule : CocoStartModule
	{
		protected override void InitSignals ()
		{
			base.InitSignals ();

			Bind<SaveImageSignal> ();
			BindCommand<SaveImageSignal, SaveImageCommand> ();
		}

		protected override void InitDatas ()
		{
			UnityTypeBindings.Register ();

			base.InitDatas ();

			Bind<GameRoleStateModel> ();

			Bind<GameRecordStateModel> ();
			BindType<ICocoAudioData, GameAudioData> ();

			GameGlobalData globalData = new GameGlobalData ();
			BindValue<CocoGlobalData> (globalData);
			BindValue (globalData);
		}

		protected override void CleanDatas ()
		{
			Unbind<CocoGlobalData> ();
			Unbind<GameGlobalData> ();

			Unbind<GameRoleStateModel> ();
			Unbind<GameRecordStateModel> ();
			Unbind<ICocoAudioData> ();

			base.CleanDatas ();
		}

		protected override void InitSceneModuleDatas ()
		{
			AddSceneModuleData (new CocoSceneModuleData (CocoSceneID.CoverPage, "CoverPage"));
			AddSceneModuleData (new CocoSceneModuleData (CocoSceneID.Doll, "Doll", typeof(GameDollSceneModule)));
		}

		protected override void InitSubModules ()
		{
			base.InitSubModules ();

			CocoMainController.Instance.AddModule<CocoAssetModule> ();
		}

		protected override void CleanSubModules ()
		{
			CocoMainController.Instance.RemoveModule<CocoAssetModule> ();

			base.CleanSubModules ();
		}

		public override void StartGame ()
		{
			base.StartGame ();

			Input.multiTouchEnabled = false;

			InitAssetConfig ();
			InitRole ();
		}

		private void InitAssetConfig ()
		{
			var loadSignal = CocoRoot.GetInstance<CocoAssetLoadConfigHolderSignal> ();
			loadSignal.Dispatch (string.Empty);
		}

		private void InitRole ()
		{
			var gameGlobalData = CocoRoot.GetInstance<GameGlobalData> ();

		}

	}
}