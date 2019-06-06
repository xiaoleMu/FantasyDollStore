using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;

namespace TabTale
{

	public class HandleSocialServerVerificationCompleteCommand : Command{

		[Inject]
		public IGeneralDialogService generalDialog { get; set; }
		
		//[Inject]
		//public TextConfigModel textConfigModel { get; set; }

		[Inject]
		public SocialNetwork socialNetwork {get; set;}

		[Inject]
		public SocialStateModel socialStateModel { get; set; }

		//[Inject]
		//public ToggleLoadingDialogSignal toggleLoadingDialogSignal {get; set;}
		
		const string CONNECTED_DIALOG_TITLE_KEY = "";
		const string CONNECTED_DIALOG_MESSAGE_KEY = "";

		public override void Execute ()
		{
			RemoveLoadingDialog();

			ShowConnectedDialog();
		}

		void RemoveLoadingDialog ()
		{
			//toggleLoadingDialogSignal.Dispatch(false);
		}

		public void ShowConnectedDialog ()
		{
			/*
			GeneralDialogData data;
			data = new GeneralDialogData ();
			data.title = textConfigModel.GetText(CONNECTED_DIALOG_TITLE_KEY, "Success!");
			//data.message = "user id: " + socialNetworkService.UserId + "\napp id:" + socialNetworkService.AppId;

			data.message = textConfigModel.GetText(CONNECTED_DIALOG_TITLE_KEY, "You're now connected to " + GetSocialNetworkName() + "!");
			data.buttons.Add(new BasicDialogButtonData("Ok"));
			generalDialog.Show (data);
			*/
		}

		string GetSocialNetworkName ()
		{
			switch(socialNetwork){
				case SocialNetwork.Facebook:
					return "facebook";
				case SocialNetwork.GameCenter:
					return "Game Center";
				case SocialNetwork.PlayServices:
					return "Google Play";
			}

			return "No Network";
		}
	}
}
