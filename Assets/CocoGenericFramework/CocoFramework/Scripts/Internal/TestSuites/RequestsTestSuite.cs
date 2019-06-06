using System;

namespace TabTale
{
	public class RequestsTestSuite : TestSuite
	{
		[Inject]
		public RequestsManager requestsManager { get; set;}

		[PostConstruct]
		public void Init()
		{
			AddTest(TestSendRequest);
			AddTest(TestFailingRequest);
		}

		private void TestSendRequest(Action<TestCaseResult> resultCallback)
		{
			logger.Log("Dummy test");

			resultCallback(TestCaseResult.Create(TestCaseResultCode.Success, "Success!"));
		}

		private void TestFailingRequest(Action<TestCaseResult> resultCallback)
		{
			logger.Log("Dummy failing test");

			resultCallback(TestCaseResult.Create(TestCaseResultCode.Failure, "Failed"));
		}
	}
}

