using System;
using UnityEngine;

namespace CocoPlay
{
    public class CocoUIDataButton<Button, Data> : CocoUINormalButton where Button : CocoUIDataButton<Button, Data>
    {
        [SerializeField]
        protected Data m_Data;
        
        public Data ButtonData
        {
            get { return m_Data; }
            set { m_Data = value; }
        }

        public System.Action<Button> OnClickAction;

        protected override void OnClick()
        {
            base.OnClick();
            if(OnClickAction != null)
                OnClickAction(this as Button);
        }
    }
}
