using UnityEngine;
using System.Collections;
using System.IO;
using TabTale;

public class ShareButtonView : MainView 
{
    [Inject]
    public SoundManager soundManager { get; set; }

    [Inject]
    public IShareService shareService { get; set; }

    [Inject]
	public GeneralParameterConfigModel generalParameterConfigModel { get; set; }

	[Inject]
	public NetworkCheck networkCheck { get; set; }

    public void OnClick()
    {
		logger.Log(Tag, "OnClick");

		if(networkCheck.HasInternetConnection())
		{
			string shareSubject = generalParameterConfigModel.GetString("ShareSubject");
			string shareBodyText = generalParameterConfigModel.GetString("ShareBodyText");
			
			string storeUrlConfigKey = Application.platform == RuntimePlatform.IPhonePlayer ? "iOSAppStoreUrl" : "GooglePlayAppStoreUrl";
			string storeAppUrl =  generalParameterConfigModel.GetString(storeUrlConfigKey);
			string shareHeadLine =  generalParameterConfigModel.GetString("ShareHeadLine");
			
			shareService.ShareWithImage(shareSubject, shareBodyText, storeAppUrl, shareHeadLine);
		}
		else
		{
			networkCheck.ShowNoConnectionPopup();
		}

    }

}