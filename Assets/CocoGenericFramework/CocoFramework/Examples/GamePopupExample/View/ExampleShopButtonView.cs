using strange.extensions.signal.impl;
using UnityEngine.UI;
using System.Collections;

namespace TabTale
{
	public class ExampleShopButtonView : GameView
	{
		[Inject]
		public ExampleRequestOpenShopSignal openShopSignal { get; set; }
		
		[Inject]
		public UpdateCurrencySignal updateCashSignal { get; set; }
		
		[Inject]
		public SoundManager soundManager { get; set; }
		
		[Inject]
		public CurrencyStateModel currencyStateModel { get; set; }
		
		public void OnClick()
		{
			soundManager.PlaySound(SoundMapping.Map.GeneralButtonClick, SoundLayer.Main);
			openShopSignal.Dispatch();
		}
		
	}
	public class ExampleRequestOpenShopSignal : Signal
    {
        
    }
    
}
