using UnityEngine;
using strange.extensions.command.impl;

namespace TabTale
{
    public class PurchaseCurrencyCommand : Command
    {
        [Inject]
        public CurrencyStateModel currencyStateModel { get; set; }
        [Inject]
        public ItemConfigData itemConfigData { get; set; }

        public override void Execute()
        {
            if (itemConfigData.costs.Count > 0)
            {
                Debug.Log("PurchaseCurrencyCommand of " + itemConfigData);

                currencyStateModel.IncreaseCurrency(itemConfigData.costs);
            }
        }
    }
}