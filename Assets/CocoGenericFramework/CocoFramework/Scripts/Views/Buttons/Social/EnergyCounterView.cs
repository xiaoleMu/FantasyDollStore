using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace TabTale 
	{
		public class EnergyCounterView : MainView 
		{

		[Inject]
		public EnergySystemService energySystemService { get; set; }

		[Inject]
		public EnergyChargeCompleteSignal energyChargeCompleteSignal { get; set; }

		[Inject]
		public EnergyConsumedCompleteSignal energyConsumedCompleteSignal { get; set; }

		[Inject]
		public TimerLoopCompleteSignal timerLoopCompleteSignal { get; set; }

		[Inject]
		public GeneralParameterConfigModel generalParameterConfigModel { get; set; }

		[Inject]
		public CurrencyStateModel currencyStateModel { get; set; }

		[Inject]
		public TickerService tickerService { get; set; }

		public  GameObject[] partialEnergyList;

		public GameObject[] fullEnergyList;

		public GameObject[] emptyEnergyList;

		public Text energyCounter;
		public Text timerCounter;
		private string _maxCooldownTimer;

		protected override void OnRegister ()
		{
			base.OnRegister();
		}

		protected override void OnEnable ()
		{
			logger.Log(Tag, "OnEnable - Updating UI & Restarting counter");
			GetEnergyAndHandleUI ();

			StartCoroutine(UpdateTimeLeftCoro());
		}

		protected override void AddListeners()
		{
			energyChargeCompleteSignal.AddListener (OnEnergyChargeComplete);
			energyConsumedCompleteSignal.AddListener (OnEnergyConsumedComplete);
		}

		protected override void RemoveListeners()
		{
			energyChargeCompleteSignal.RemoveListener (OnEnergyChargeComplete);
			energyConsumedCompleteSignal.RemoveListener (OnEnergyConsumedComplete);
		}

		private void OnEnergyChargeComplete()
		{
			GetEnergyAndHandleUI ();
		}

		private void OnEnergyConsumedComplete()
		{
			GetEnergyAndHandleUI ();
		}

		private void GetEnergyAndHandleUI()
		{
			if (energySystemService.IsEnergyEmpty) 
			{
				energyCounter.text = energySystemService.EnergyAmount.ToString ();

				for (int i = 0; i < fullEnergyList.Length; i++)
				{
					fullEnergyList [i].SetActive (false);
				}

				for (int i = 0; i < partialEnergyList.Length; i++)
				{
					partialEnergyList [i].SetActive (false);
				}
	
				for (int i = 0; i < emptyEnergyList.Length; i++)
				{
					emptyEnergyList [i].SetActive (true);
				}
			}	

			if (energySystemService.IsEnergyFull) 
			{	
				energyCounter.text = energySystemService.EnergyAmount.ToString ();

				timerCounter.text = "Full";
				
				for (int i = 0; i < fullEnergyList.Length; i++)
				{
					fullEnergyList [i].SetActive (true);
				}

				for (int i = 0; i < partialEnergyList.Length; i++)
				{
					partialEnergyList [i].SetActive (false);
				}

				for (int i = 0; i < emptyEnergyList.Length; i++)
				{
					emptyEnergyList [i].SetActive (false);
				}
			}	

			if (energySystemService.IsEnergyPartial) 
			{
				energyCounter.text = energySystemService.EnergyAmount.ToString ();

				for (int i = 0; i < fullEnergyList.Length; i++)
				{
					fullEnergyList [i].SetActive (false);
				}

				for (int i = 0; i < partialEnergyList.Length; i++)
				{
					partialEnergyList [i].SetActive (true);
				}

				for (int i = 0; i < emptyEnergyList.Length; i++)
				{
					emptyEnergyList [i].SetActive (false);
				}
			}	
		}
			
		IEnumerator UpdateTimeLeftCoro() 
		{
			while(energySystemService != null)
			{
				timerCounter.text = energySystemService.IsEnergyFull ? "Full" : energySystemService.TimeLeftUntilNextCharge ();

				yield return new WaitForSeconds(1);
			}
		}
			
	}
}