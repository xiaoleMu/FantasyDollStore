using TabTale;
using strange.extensions.command.impl;

namespace TabTale
{
	public class ShowRateUsCommand : Command
	{   
		[Inject]
		public IModalityManager modalityManager { get; set; }

		[Inject]
		public IRateUsService rateUsService { get; set; }

		public override void Execute ()
		{

			CoreLogger.LogDebug ("ShowRateUsCommand", "Execute");
			if (rateUsService.ShouldShowRateUs ()) {
				modalityManager.Add (new AppModalHandle ("GamePopups/RateUsModal", IModalMaskType.Masked), true);
			}
		}
		
		
	}
}
