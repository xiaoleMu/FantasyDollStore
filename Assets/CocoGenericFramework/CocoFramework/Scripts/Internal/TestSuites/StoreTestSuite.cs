using System;
using TabTale.Publishing;
using UnityEngine;

namespace TabTale
{
	public class StoreTestSuite : TestSuite
	{
		[Inject]
		public IStoreManager storeManager { get; set; }

		[PostConstruct]
		public void Init()
		{
			AddTest(TestAnyItemAvailableToBuy, "Any Item Available To Buy", "Returns True/False");
		}

		private void TestAnyItemAvailableToBuy (Action<TestCaseResult> resultCallback)
		{
			bool anyItemAvailable = storeManager.AnyItemAvailableToBuy();

			resultCallback(TestCaseResult.Create(TestCaseResultCode.Inconclusive, anyItemAvailable.ToString()));
		}
			
	}
}

