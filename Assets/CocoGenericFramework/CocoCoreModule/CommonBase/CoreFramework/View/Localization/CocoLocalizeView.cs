using System;
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.signal.impl;
using TabTale;

namespace CocoPlay.Localization
{
    public class CocoLanguageSetSingal : Signal<Game.CocoLanguage> {}
    public class CocoLanguageUpdateSingal : Signal {}
    
    public class CocoLocalizeView : GameView
    {
        [Inject] public CocoLanguageUpdateSingal updateSignal { get; set; }
        
        public string key;

        /// reset image size base on sprite
        public bool resetImageSize = true;

        private bool mStarted = false;
        
        public string value 
        {
            set 
            {
                if (!string.IsNullOrEmpty (value)) 
                {
//					Debug.LogWarning ("location : " + value);
                    if (GetComponent<Text> () != null) 
                    {
                        Text txt = GetComponent<Text> ();
                        if (txt != null)
                            txt.text = value;
                    }
                    if (GetComponent<Image> () != null) 
                    {
                        Image img = GetComponent<Image> ();
                        if (img != null) {
                            var sprite = Resources.Load<Sprite> (value);
                            if (sprite != null) {
                                img.sprite = sprite;

                                if (resetImageSize) {
                                    img.SetNativeSize ();
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            updateSignal.AddListener(OnLocalize);
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            updateSignal.RemoveListener(OnLocalize);
        }

        protected override void OnEnable ()
        {
            base.OnEnable();
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            if (mStarted)
                OnLocalize ();
        }

        protected override void Start ()
        {
            base.Start();
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            mStarted = true;
            OnLocalize ();
        }

        /// <summary>
        /// This function is called by the Localization manager via a broadcast SendMessage.
        /// </summary>

        protected virtual void OnLocalize ()
        {
            // If no localization key has been specified, use the label's text as the key
            if (string.IsNullOrEmpty (key)) 
            {
                Text lbl = GetComponent<Text> ();
                if (lbl != null)
                    key = lbl.text;
            }

            // If we still don't have a key, leave the value as blank
            if (!string.IsNullOrEmpty (key))
                value = CocoLocalization.Get (key);
        }
    }
}

