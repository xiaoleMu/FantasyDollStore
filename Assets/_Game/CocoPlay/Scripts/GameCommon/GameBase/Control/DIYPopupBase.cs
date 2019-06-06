using System;
using System.Collections.Generic;
using System.Linq;
using CocoPlay;
using TabTale;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class DIYPopupBase : CocoGenericPopupBase
    {
        protected override void Awake()
        {
            base.Awake();
            InitButton();
        }

        protected override void OnUIButtonClick (CocoUINormalButton button)
        {
           
        }
        
        protected virtual void OnButtonClick (CocoUINormalButton button)
        {
            if (! gameObject.activeInHierarchy) return;
            if (button.UseButtonID) {
                OnButtonClickWithButtonId (button.ButtonID);
            } else {
                OnButtonClickWithButtonName (button.ButtonKey);
            }
        }

        protected void InitButton()
        {
            var buttonArray = GetComponentsInChildren<CocoUINormalButton>(true);
            foreach (var button in buttonArray)
            {
                button.OnClickEvent += OnButtonClick;
            }
        }
    }
}