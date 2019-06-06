using UnityEngine;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using System.Collections;
using System;

namespace TabTale
{
	public class EnergySystemService
	{
		[Inject]
		public CurrencyStateModel currencyStateModel { get; set; }

		[Inject]
		public ProgressStateModel progressStateModel { get; set; }

		[Inject]
		public EnergyStateModel energyStateModel { get; set; }

		[Inject]
		public EnergyChargeCompleteSignal energyChargeCompleteSignal { get; set; }

		[Inject]
		public EnergyConsumedCompleteSignal energyConsumedCompleteSignal { get; set; }

		[Inject]
		public GeneralParameterConfigModel generalParameterConfigModel { get; set; }

		[Inject]
		public ServerTime serverTime { get; set; }

		[Inject]
		public IRoutineRunner routineRunner { get; set; }

		[Inject]
		public NetworkCheck networkCheck { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		private string Tag
		{
			get { return "EnergySystemService".Colored(Colors.green); }
		}

		public int MaxEnergy
		{
			get { return generalParameterConfigModel.GetInt ("EnergySystemMaxCharge"); }
		}
			
		public int MaxCooldownTimer
		{
			//TODO: Change to int (requires updating client config versions)
			get { return int.Parse(generalParameterConfigModel.GetString ("EnergySystemRechargeTime","300")); }
		}

		public int EnergyAmount
		{
			get 
			{
				int energyCurrencyIndex = currencyStateModel.GetIndex(CurrenciesType.energy.ToString());

				// If the player has no energy defined then return max energy (will happen on first game session), otherwise return the amount
				if (energyCurrencyIndex == -1) {
					currencyStateModel.IncreaseCurrency(CurrenciesType.energy, MaxEnergy);
					energyChargeCompleteSignal.Dispatch ();
					return MaxEnergy;
				}
				return currencyStateModel.GetCurrencyAmount(CurrenciesType.energy);
			}
		}

		public bool IsEnergyEmpty
		{
			get { return EnergyAmount == 0; }
		}

		public bool IsEnergyFull
		{
			get { return EnergyAmount == MaxEnergy; }
		}

		public bool IsEnergyPartial
		{
			get { return !(IsEnergyFull || IsEnergyEmpty); } 
		}

		[PostConstruct]
		public void Init()
		{
			AdjustForShiftInServerTime();
		}

		private void AdjustForShiftInServerTime()
		{
			// If we had a big shift in time, adjust the last charge time
			// This can happen since if we dont have the current server time, the server time service will return 
			// the timespan since the app started (instead of current time).
			// When we first receive the server time there will be a big jump
			// This scenario is supposed to happen only once in the application lifetime
			TimeSpan timeSinceLastCharge = CalcTimeSinceLastCharge();
			if(timeSinceLastCharge.TotalSeconds > 3600*24*365)
			{
				logger.Log(Tag,"AdjustForShiftInServerTime - shift in time since last charge");

				try
				{
					DateTime lastCharge = serverTime.ToLocal (energyStateModel.LastChargeTime);
					DateTime serverTimeWhenLastWentToBackground = serverTime.GetPersistedServerTime();

					TimeSpan difference = serverTimeWhenLastWentToBackground - lastCharge;

					if(difference.TotalSeconds > 0)
					{
						logger.Log(Tag,"Restting last charge time due to shift in server time");

						energyStateModel.LastChargeTime = serverTime.GetLocalTime().Subtract(difference).ToString();

						logger.Log(Tag,String.Format("Adjusted for shift in server time - lastCharge:{0}, " +
							"serverTimeWhenLastWentToBackground:{1} " +
							"difference:{2} " +
							"setting last charge time to: {3}", 
							lastCharge, serverTimeWhenLastWentToBackground, difference, energyStateModel.LastChargeTime));
					}
				}
				catch(Exception e)
				{
					logger.LogError(Tag,"ERROR - Could not adjust last charge for shift in server time. " + e.ToString());
				}
			}
		}

		public void ConsumeEnergy(int energyToConsume)
		{
			int currentEnergy = currencyStateModel.GetCurrencyAmount (CurrenciesType.energy);

			if (currentEnergy >= energyToConsume) 
			{
				currencyStateModel.DecreaseCurrency (CurrenciesType.energy, energyToConsume);
				logger.Log (Tag,"Consuming energy by " + energyToConsume.ToString());
				energyConsumedCompleteSignal.Dispatch ();
			}
		}

		public void ChargeEnergy(int energyToCharge = 1, bool allowOvercharge = false)
		{
			if(IsEnergyFull)
			{
				logger.Log(Tag, "Energy full - skipping charge");
				return;
			}

            int currentEnergy = currencyStateModel.GetCurrencyAmount(CurrenciesType.energy);

            int energyToIncrease = allowOvercharge ? energyToCharge : Mathf.Min(energyToCharge, MaxEnergy - currentEnergy);

			energyToIncrease = Math.Max(energyToIncrease,0);

			energyStateModel.LastChargeTime = serverTime.GetLocalTime().ToString();

			if(FirstTimeRun())
			{
				logger.Log(Tag,"First time run - skipping charge");
				return;
			}

            logger.Log(Tag, "Charging energy by " + energyToIncrease.ToString());

            currencyStateModel.IncreaseCurrency(CurrenciesType.energy, energyToIncrease);
            energyChargeCompleteSignal.Dispatch();
        }

		private Func<int,string> DefaultTimeUntilNextChargeStringfier = secondsLeft => {

			float minutes = Mathf.Floor(secondsLeft / 60);
			float seconds = Mathf.Ceil(secondsLeft % 60);

			string timeLeftString = string.Format("{0:0}:{1:00}", minutes, seconds);
			return timeLeftString;
		};

		public string TimeLeftUntilNextCharge(Func<int, string> timeLeftStringifer = null)
		{
			if(timeLeftStringifer == null)
			{
				timeLeftStringifer = DefaultTimeUntilNextChargeStringfier;
			}

			AdjustForShiftInServerTime();
			TimeSpan timeSinceLastCharge = CalcTimeSinceLastCharge();

			// Sanity check - time since last charge should not be negative, 
			// but this situation can happen during debugging when manipulating time, 
			// and if so it is fine, we just have to reset the time.
			if(timeSinceLastCharge < TimeSpan.Zero)
			{
				logger.Log(Tag, "TimeLeftUntilNextCharge - possible time manipulation, time since last charge : " + timeSinceLastCharge.Seconds);

				energyStateModel.LastChargeTime = serverTime.GetLocalTime().ToString();
			}

			if(timeSinceLastCharge.TotalSeconds > MaxCooldownTimer)
			{
				if (! IsEnergyFull) 
				{
					int amountToCharge = (int)timeSinceLastCharge.TotalSeconds / MaxCooldownTimer;
					ChargeEnergy (amountToCharge);
					timeSinceLastCharge = CalcTimeSinceLastCharge();
				}
			}

			int timeLeftInSeconds = MaxCooldownTimer - (int)timeSinceLastCharge.TotalSeconds;

			// Sanity check:
			// If time left is below zero at this point then it is a real issue:
			if(timeLeftInSeconds < 0)
			{
				logger.LogError(Tag, "TimeLeftUntilNextCharge ERROR : " + timeSinceLastCharge.Seconds);
				timeLeftInSeconds = 300;
			}

			return timeLeftStringifer(timeLeftInSeconds);
		}

		private TimeSpan CalcTimeSinceLastCharge()
		{
			DateTime lastCharge = serverTime.ToLocal (energyStateModel.LastChargeTime);
			DateTime time = serverTime.GetLocalTime ();

			TimeSpan difference = time - lastCharge;

			return difference;
		}

		private bool FirstTimeRun()
		{
			if (TTPlayerPrefs.HasKey("firstTimer"))
				return false;
			else
			{
				TTPlayerPrefs.SetValue("firstTimer", true);
				return true;
			}
		}
   }
}
