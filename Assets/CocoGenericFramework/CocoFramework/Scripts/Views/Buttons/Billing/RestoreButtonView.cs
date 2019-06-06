using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

namespace TabTale {
	
	public class RestoreButtonView : MainView
	{
		[Inject]
		public RequestRestoreSignal requestRestoreSignal {get; set;}

		[Inject]
		public NetworkCheck networkCheck { get; set; }

		[Inject]
		public PurchasesRestoredSignal purchasesRestoredSignal { get; set;}

	    [Inject]
	    public SoundManager soundManager { get; set; }

		public bool hideOnAndroid = true;

		override protected void Awake()
		{
			if(hideOnAndroid && Application.platform == RuntimePlatform.Android)
			{
				gameObject.SetActive(false);
				return;
			}

			// Check if already restored purchases
			bool purchasesAlreadyRestored = PlayerPrefs.GetInt("restoredPurchases", 0) == 1;

			if(purchasesAlreadyRestored)
				OnPurchasesRestored(true);

		}

		protected override void OnRegister()
		{
			base.OnRegister();
			AddListeners();
		}
		
		protected override  void AddListeners()
		{
			purchasesRestoredSignal.AddListener(OnPurchasesRestored);
		}
		
		protected override  void RemoveListeners()
		{
			purchasesRestoredSignal.RemoveListener(OnPurchasesRestored);
		}
		
		public void OnClick()
		{
            soundManager.PlaySound(SoundMapping.Map.GeneralButtonClick, SoundLayer.Main);

			if(networkCheck.HasInternetConnection())
			{
				requestRestoreSignal.Dispatch();
			}
			else
			{
				networkCheck.ShowNoConnectionPopup();
			}
		}

		/// <summary>
		/// Disable button when restored purchases
		/// </summary>
		public void OnPurchasesRestored(bool success)
		{
			if(success)
			{
				gameObject.GetComponent<Button>().interactable = false;
			}
		}

		protected override void OnUnRegister()
		{
			RemoveListeners();
			base.OnUnRegister();
		}
	}
	
}