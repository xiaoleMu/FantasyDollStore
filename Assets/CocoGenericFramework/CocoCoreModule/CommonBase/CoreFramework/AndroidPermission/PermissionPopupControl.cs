using UnityEngine;
using System.Collections;
using CocoPlay;

public class PermissionPopupControl : CocoGenericPopupBase {

    [SerializeField]
    private CocoUINormalButton defaultButton;

    protected override bool OnBackButtonPress()
    {
        if (m_MainController.TouchEnable == false)
            return true;

        uiButtonClickSignal.Dispatch(defaultButton);
        CloseBtnClick();
        return true;
    }

    protected override void OnButtonClickWithButtonName(string buttonName)
    {
        if (buttonName.Equals("ShowAlertViewOne") || buttonName.Equals("ShowAlertViewTwo"))
        {
            CloseBtnClick();
        }
    }

}
