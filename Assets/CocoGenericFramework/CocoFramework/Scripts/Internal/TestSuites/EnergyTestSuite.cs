using System;
using UnityEngine;

namespace TabTale
{
	public class EnergyTestSuite : TestSuite
	{
		[Inject]
		public EnergySystemService energySystemService { get; set;}

		[Inject]
		public ILogger logger { get; set;}

		[PostConstruct]
		public void Init()
		{
			AddTest(TestConsumeEnergy, "Consume Energy");
			AddTest(TestChargeEnergy, "Charge Energy");
		}

		private void TestConsumeEnergy(Action<TestCaseResult> resultCallback)
		{
			energySystemService.ConsumeEnergy(1);

			resultCallback(TestCaseResult.Create(TestCaseResultCode.Success, "Success!"));
		}

		private void TestChargeEnergy(Action<TestCaseResult> resultCallback)
		{
			energySystemService.ChargeEnergy(1);

			resultCallback(TestCaseResult.Create(TestCaseResultCode.Success, "Success!"));
		}
	}
}

