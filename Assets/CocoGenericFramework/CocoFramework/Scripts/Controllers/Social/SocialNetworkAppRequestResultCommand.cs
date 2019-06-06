using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;

namespace TabTale
{
	public class SocialNetworkAppRequestResultCommand : Command
	{
		[Inject]
		public IGeneralDialogService generalDialog { get; set; }

		[Inject]
		public IModalityManager modalityManager { get; set; }

		[Inject]
		public bool actionSucceed { get; set; }

		[Inject]
		public string result { get; set; }


		public override void Execute ()
		{
			//ShowAppRequestDialog();
		}

		public void ShowAppRequestDialog ()
		{
			GeneralDialogData data;

			data = new GeneralDialogData ();
			if (actionSucceed){
				data.title = "Notification";
				data.message = result;
			}
			else{
				data.title = "Notification";
				data.message = result;
			}

			generalDialog.Show (data);//added support for generic stacked modals
		}
	}

}
