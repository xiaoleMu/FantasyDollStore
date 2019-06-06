using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using strange.extensions.mediation.impl;

namespace TabTale
{
	public enum DebugCurrency 
	{
		cash,diamonds
	}
	public class DebugCanvas : View
	{
		[Inject]
		public  IGameDB gameDB {get;set;}

		[Inject]
		public CurrencyStateModel currencyStateModel { get; set; }

		System.Random random = new System.Random();

		private const string _keyPlayerId = "playerId";

		public GameObject debugPanel;
		public InputField ipInput;
		public InputField portInput;
		public InputField playerIdInput;

		public Text connectStatusText;

		public Text playerIdText;
		public Text selectedServer;
		public Text levelText;
		public Text xpText;
		public Text energyText;
		public Text cashText;
		public Text diamondsText;

		ServerLoggerProvider _loggerProvider;

		protected override void Start ()
		{
			base.Start ();

			debugPanel.SetActive (false);

			RefreshData();

		}

		public void RefreshData()
		{
            //cashText.text = "Cash: " + currencyStateModel.GetCurrencyAmount(DebugCurrency.cash);
            //diamondsText.text = "Diamonds: " + currencyStateModel.GetCurrencyAmount(DebugCurrency.diamonds);
		}

		public void ToggleDebugPanel ()
		{
			debugPanel.SetActive (!debugPanel.activeSelf);
		}

		public void Connect ()
		{
			/*
			if (_loggerProvider == null)
				return;


			connectStatusText.text = "Connecting...";

			int port;
			if (int.TryParse (portInput.text, out port)) {
				_loggerProvider.SetServerAddress (ipInput.text, port);
				_loggerProvider.Connect ();
			}

			if(_loggerProvider.IsConnected())
				connectStatusText.text = "Connected";
			else
				connectStatusText.text = "Disconnected";
			*/
		}

		public void SelectPlayerId()
		{
			playerIdInput.text = playerIdInput.text.Trim();
			Debug.Log ("Debug - set player ID to " + playerIdInput.text);
			long newId;
		}

		public void ChangeLevel(int delta)
		{

		}
		
		public void ChangeXp(int delta)
		{

		}

		public void GiveEnergy(int delta)
		{
			//playerStateModel.GiveEnergy(delta);
			//energyText.text = "Eenrgy: " + playerStateModel.GetState().energy.ToString() + "/50";
		}
		public void UseEnergy(int delta)
		{
            /*
			int energy = playerStateModel.GetState().energy;
			if (delta > energy)
				delta = energy;
			playerStateModel.UseEnergy(delta);
			energyText.text = "Eenrgy: " + playerStateModel.GetState().energy.ToString() + "/50";
            */         
		}

		/*
		public void IncreaseCash(int delta)
		{
		    var cost = new CurrencyData {{"cash", delta}};
		    currencyStateModel.IncreasePlayerCurrency(cost);
		    RefreshData();
		}
		public void DecreaseCash(float delta)
		{
		    float cash = currencyStateModel.GetState().currencyData.GetCurrencyAmount("cash");
			if (delta > cash)
				delta = cash;
			currencyStateModel.DecreasePlayerCurrency(new CurrencyData(){{"cash",delta}});

		    RefreshData();
		}
		public void IncreaseDiamonds(int delta)
		{
            var cost = new CurrencyData { { "diamonds", delta } };
            currencyStateModel.IncreasePlayerCurrency(cost);
		    RefreshData();
		}
		public void DecreaseDiamonds(float delta)
		{
            float diamonds = currencyStateModel.GetState().currencyData.GetCurrencyAmount("diamonds");
            if (delta > diamonds)
                delta = diamonds;
            currencyStateModel.DecreasePlayerCurrency(new CurrencyData() { { "diamonds", delta } });

            RefreshData();
		}

		public  void ToggleBanner()
		{

			GameObject go = GameObject.FindGameObjectWithTag("BannerManager");
			if(go==null)
				return;

			ShowBanner manager = go.GetComponent<ShowBanner>();
			if(manager!=null)
				manager.ToggleBanner();

		}*/

	}
}