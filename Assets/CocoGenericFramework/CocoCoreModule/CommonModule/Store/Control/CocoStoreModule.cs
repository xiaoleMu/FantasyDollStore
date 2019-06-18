using System;
using strange.extensions.signal.impl;

namespace CocoPlay
{
	public class StoreClickSignal : Signal<string>{}
	public class CocoStoreModule : CocoModuleBase
	{
		#region Signals

		protected override void InitSignals ()
		{
			base.InitSignals ();

			Bind<CocoStoreUpdateStateSignal> ();
			Bind<StoreClickSignal> ();

		}

		protected override void CleanSignals ()
		{
			Unbind<CocoStoreUpdateStateSignal> ();
			Unbind<StoreClickSignal> ();
			base.CleanSignals ();
		}

		protected override void InitDatas ()
		{
			base.InitDatas ();

			var configDataType = Type.GetType ("Game.StoreConfigData");
			if (configDataType != null) {
				BindType<ICocoStoreConfigData> (configDataType);
			}
		}

		protected override void CleanDatas ()
		{
			Unbind <ICocoStoreConfigData> ();

			base.CleanDatas ();
		}

		protected override void StuffDatas ()
		{
			base.StuffDatas ();

			StuffBindType<ICocoStoreConfigData, CocoStoreConfigData> ();
		}

		#endregion


		private CocoStoreControl m_StoreControl;

		protected override void InitObjects ()
		{
			base.InitObjects ();

			m_StoreControl = CocoLoad.GetOrAddComponent<CocoStoreControl> (gameObject);
			BindValue (m_StoreControl);
		}

		protected override void CleanObjects ()
		{
			Unbind <CocoStoreControl>();

			base.CleanObjects ();
		}
	}
}
