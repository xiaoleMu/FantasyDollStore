using strange.extensions.signal.impl;
using System.Collections.Generic;
using System.Linq;

namespace TabTale
{
	public class CurrencyStateModel : StateModel<CurrencyStateData>
	{
		[Inject]
		public CurrencyDecreasingSignal currencyDecreasingSignal {get;set;}

		[Inject]
		public CurrencyIncreasingSignal currencyIncreasingSignal {get;set;}

		[Inject]
		public UpdateCurrencySignal updateCurrencySignal {get;set;}

		[Inject]
		public InsufficientCurrencySignal insufficientCurrencySignal {get;set;}

		#region Getters

		public GameElementData GetCurrency (System.Enum key)
		{
			int index = GetIndex(key.ToString());
			if (index == -1) 
			{
				return null;
			}

			return _data.currencies[index].Clone() as GameElementData;
		}

		public int GetCurrencyAmount (System.Enum key)
		{
			return(GetCurrencyAmount(key.ToString()));
		}

		public int GetIndex(string key)
		{
			return _data.currencies.FirstIndex( currency=>currency.key == key);
		}

		private int GetCurrencyAmount (string key)
		{
			if (GetIndex(key.ToString()) == -1) 
			{
				return 0;
			}
			return _data.currencies.FirstOrDefault( currency=>currency.key == key.ToString()).value;
		}

		private int GetIndex(GameElementData currency)
		{
			return _data.currencies.FirstIndex( data=>data.key == currency.key);
		}

		#endregion

		#region Increase/Decrease Currency

		public bool IncreaseCurrency (System.Enum key, int value)
		{
			return IncreaseCurrency (GameElementData.CreateCurrency(key,value));
		}

		public bool IncreaseCurrency (GameElementData amount)
		{
			currencyIncreasingSignal.Dispatch(amount);
			_data.currencies.Increase(amount);
			updateCurrencySignal.Dispatch(_data);
			return Save();

		}

		public bool IncreaseCurrency (IEnumerable<GameElementData> currencyAmounts)
		{
			foreach (var currency in currencyAmounts) 
			{
				IncreaseCurrency(currency);
			}
			return Save();
		}

		public bool DecreaseCurrency (IEnumerable<GameElementData> currencyAmounts)
		{
			foreach (var currency in currencyAmounts) 
			{
				DecreaseCurrency(currency);
			}
			return Save();
		}
		
		public bool DecreaseCurrency(System.Enum key, int value)
		{
			return DecreaseCurrency (GameElementData.CreateCurrency(key,value));
		}

		public bool DecreaseCurrency(GameElementData amount)
		{
			if(SufficientCurrency(amount))
			{
				int index = GetIndex(amount);
				currencyDecreasingSignal.Dispatch(amount);
				_data.currencies[index] -= amount;
				updateCurrencySignal.Dispatch(_data);
			}
			return Save();
		}

		#endregion

		#region Sufficiency

		public bool SufficientCurrency(IEnumerable<GameElementData> currencyAmounts)
		{
			foreach(GameElementData currency in currencyAmounts)
			{
				if(! SufficientCurrency(currency))
				{
					return false;
				}
			}
			return true;
		}

		public bool SufficientCurrency(GameElementData amount)
		{
			int index = GetIndex(amount);
			if ( index == -1) 
			{
				insufficientCurrencySignal.Dispatch(_data);
				return false;
			}
			else
			{
				return(_data.currencies[index].value >= amount.value);
			}
		}

		public bool SufficientCurrency(System.Enum key, int value)
		{
			return SufficientCurrency(GameElementData.CreateCurrency(key,value));
		}

		#endregion

		#region Restrictions

		public bool VerifyRestriction(GameElementData restriction)
		{
			return( GetCurrencyAmount(restriction.key) >= restriction.value);
		}

		#endregion
	}

	public class CurrencyDecreasingSignal : Signal<GameElementData> { }
	public class CurrencyIncreasingSignal : Signal<GameElementData> { }
	public class UpdateCurrencySignal : Signal<CurrencyStateData> { }
	public class InsufficientCurrencySignal : Signal<CurrencyStateData> { }
}