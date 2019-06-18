namespace CocoPlay
{
	public class CocoAssetModule : CocoModuleBase
	{
		#region Signal

		protected override void InitSignals ()
		{
			base.InitSignals ();
			Bind<CocoAssetLoadConfigHolderSignal> ();
			Bind<CocoAssetLoadConfigHolderFinishSignal> ();
			Bind<CocoAssetRequestDressItemHolderSignal> ();
			Bind<CocoAssetRequestDressItemHolderFinishSignal> ();
		}

		protected override void CleanSignals ()
		{
			Unbind<CocoAssetLoadConfigHolderSignal> ();
			Unbind<CocoAssetLoadConfigHolderFinishSignal> ();
			Unbind<CocoAssetRequestDressItemHolderSignal> ();
			Unbind<CocoAssetRequestDressItemHolderFinishSignal> ();
			base.CleanSignals ();
		}

		#endregion


		#region Data

		protected override void InitDatas ()
		{
			base.InitDatas ();
			Bind<CocoDressStateModel> ();
		}

		protected override void CleanDatas ()
		{
			Unbind<CocoDressStateModel> ();
			base.CleanDatas ();
		}

		#endregion


		#region Object

		private CocoAssetControl m_AssetControl;

		public CocoAssetControl AssetControl {
			get { return m_AssetControl; }
		}

		private CocoRoleControl m_RoleControl;

		public CocoRoleControl RoleControl {
			get { return m_RoleControl; }
		}

		protected override void InitObjects ()
		{
			base.InitObjects ();

			m_AssetControl = CocoLoad.GetOrAddComponent<CocoAssetControl> (gameObject);
			BindValue (m_AssetControl);

			m_RoleControl = CocoLoad.GetOrAddComponent<CocoRoleControl> (gameObject);
			BindValue (m_RoleControl);
		}

		protected override void CleanObjects ()
		{
			Unbind<CocoAssetControl> ();
			base.CleanObjects ();
		}

		#endregion
	}
}