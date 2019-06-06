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

			GameGlobalData globalData = new GameGlobalData ();
			BindValue<CocoGlobalData> (globalData);
			BindValue (globalData);

			Bind<GameRecordStateModel> ();
			BindType<ICocoAudioData, GameAudioData> ();
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

		}

		protected override void InitSubModules ()
		{
			base.InitSubModules ();
		}

		protected override void CleanSubModules ()
		{

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
		}

		private void InitRole ()
		{
			var gameGlobalData = CocoRoot.GetInstance<GameGlobalData> ();

		}

	}
}