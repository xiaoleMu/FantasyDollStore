using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TabTale;
using System;
using CocoPlay.Localization;

#if !COCO_FAKE
using CocoSceneID = Game.CocoSceneID;
using CocoStoreKey = Game.CocoStoreKey;
using CocoStoreID = Game.CocoStoreID;
#else
using CocoSceneID = CocoPlay.Fake.CocoSceneID;
using CocoStoreKey = CocoPlay.Fake.CocoStoreKey;
using CocoStoreID = CocoPlay.Fake.CocoStoreID;
#endif

namespace CocoPlay {

	public class CocoStoreItemButton : CocoUINormalButton
	{
		[Inject]
		public CocoStoreControl m_StoreControl { get; set; }
	    [Inject]
		public CocoStoreUpdateStateSignal m_UpdateStateSingal { get; set; }
        [Inject]
        public StoreClickSignal storeClickSignal{ get; set;}

	    [SerializeField]
		protected Text price;
	    [SerializeField]
		protected CocoStoreID m_StoreID;



		public CocoStoreID StoreID { get { return m_StoreID; } }

	    [SerializeField]
	    bool m_AnimationEnable = false;

		[SerializeField]
		private bool m_AnimIgnoreTimeScale = true;

		[SerializeField]
		private float m_animTime = 0.8f;
		[SerializeField]
		private CocoFloatRange m_range = new CocoFloatRange(0.97f,1f);

	    Vector3 DefaultScale;

	    protected override void AddListeners()
	    {
	        base.AddListeners();
	        m_UpdateStateSingal.AddListener(UpdatePrice);
	    }

	    protected override void RemoveListeners()
	    {
	        base.RemoveListeners();
	        m_UpdateStateSingal.RemoveListener(UpdatePrice);
	    }

	    protected override void Start()
	    {
	        base.Start();
	        price.text = "";
	        UpdatePrice();
	    }

	    protected override void OnEnable()
	    {
	        base.OnEnable();
	        if (m_AnimationEnable)
	        {
				DefaultScale = transform.localScale;
	            PlayPingpong();
	        }
	    }

		protected override void OnDisable ()
		{
			base.OnDisable ();
			if (m_AnimationEnable)
			{
				LeanTween.cancel(gameObject);
				transform.localScale = DefaultScale;
			}
		}

	    public void PlayPingpong()
	    {
	        LeanTween.cancel(gameObject);
	        transform.localScale = DefaultScale * m_range.From;
			LeanTween.scale(gameObject, DefaultScale*m_range.To, m_animTime).setLoopPingPong().setIgnoreTimeScale (m_AnimIgnoreTimeScale);
	    }

	    protected override void OnButtonPress(bool press)
	    {
	        if (m_AnimationEnable)
	        {
				if (!IsTouchEnabled)
	                return;
	            float scale = press ? 1f : 0.8f;
	            transform.localScale = DefaultScale * scale;
	            LeanTween.cancel(gameObject);
				if (!press) {
					PlayPingpong ();
				} else {
						if (clickAudio.FirstUsed) {
							CocoAudio.PlaySound (clickAudio.First);
						} else {
							CocoAudio.PlaySound (clickAudio.Second);
						}
				}
	        }
	        else
	        {
	            base.OnButtonPress(press);
	        }
	    }

		protected override void OnClick()
	    {
			base.OnClick ();
	        if (IsPurchased())
	        {
	            Debug.LogWarning("Is Already Buy Item");
	        }
	        else
	        {
				m_StoreControl.Buy(m_StoreID);
	        }
            storeClickSignal.Dispatch(m_StoreID.ToString());
	    }

		protected bool IsPurchased()
	    {
			return m_StoreControl.IsPurchased(m_StoreID);
	    }

		public bool useISOCurrencySymbol = false;

		protected virtual void UpdatePrice()
	    {
	        if (IsPurchased())
	        {
	            //price.resizeTextForBestFit = false;
				price.text = CocoLocalization.Get(CocoStoreKey.Store_txt_Unlocked);
	        }
	        else
	        {
				string itemPrice = m_StoreControl.GetPriceString(m_StoreID, useISOCurrencySymbol);
//	            itemPrice = itemPrice.Replace(" ", "");
	            if (!string.IsNullOrEmpty(itemPrice))
	            {
	                //price.resizeTextForBestFit = false;
	                price.text = itemPrice;
	            }
	        }
	    }
	}
}
