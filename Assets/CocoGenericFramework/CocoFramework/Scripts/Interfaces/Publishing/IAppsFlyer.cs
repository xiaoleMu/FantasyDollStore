using UnityEngine;
using System.Collections;

namespace TabTale 
{
	public interface IAppsFlyer
	{
		void ReportPurchase(string price, string currency);
	}
}

