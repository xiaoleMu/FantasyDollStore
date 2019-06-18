namespace CocoPlay
{
	public class CocoGodButton : CocoUINormalButton
	{
		[Inject]
		public CocoStoreControl StoreControl { get; set; }

		protected override void OnRegister ()
		{
			base.OnRegister ();

			if (!CocoDebugSettingsData.Instance.IsGodModeEnabled) {
				gameObject.SetActive (false);
			}
		}

		protected override void OnClick ()
		{
			base.OnClick ();

			if (CocoDebugSettingsData.Instance.IsGodModeEnabled) {
				StoreControl.EnableGodMode ();
			}
		}
	}
}