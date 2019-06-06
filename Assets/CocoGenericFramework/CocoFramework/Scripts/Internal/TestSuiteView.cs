using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace TabTale
{
	public class TestSuiteView : BaseModalView, IModalDataReceiver<TestSuite>, IBackButtonListener
	{
		public GameObject layout;
		public Button closeButton;

		public TestSuite _testSuite;

		protected override void OnModalOpenComplete()
		{
			base.OnModalOpenComplete();

			LoadTestCaseViews();
		}

		public void LoadTestCaseViews()
		{
			closeButton.onClick.AddListener (_appModalView.Close);

			foreach(TestCase test in _testSuite.tests)
			{
				var go = Instantiate(Resources.Load("Debug/TestSuites/DebugTest")) as GameObject;

				PlaceInLayout(go);

				SetupTest(test, go);
			}
		}

		#region IModalDataReceiver implementation
		public void SetData (TestSuite data)
		{
			_testSuite = data;
		}
		#endregion

		private void PlaceInLayout(GameObject go)
		{
			go.transform.SetParent(layout.transform);
			go.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
			go.transform.localPosition = new Vector3 (go.transform.localPosition.x, go.transform.localPosition.y, 0f);
			go.transform.localScale = new Vector3 (1f, 1f, 1f);
		}

		private void SetupTest(TestCase test, GameObject go)
		{
			TestCaseView testView = go.GetComponent<TestCaseView>();

			testView.ActButton.onClick.AddListener(delegate{OnTestClicked(test, go);});

			testView.ActButton.GetComponentInChildren<Text>().text = test.Description;

			testView.details.text = test.Details;
		}

		private void OnTestClicked(TestCase test, GameObject go)
		{
			foreach(TestPrecondition preCondition in test.preconditions)
			{	
				if(! preCondition.preCondition())
				{
					HandleTestResult(TestCaseResult.Create(TestCaseResultCode.CannotArrange, preCondition.notMetDetails), test, go.GetComponent<TestCaseView> ());
					return;
				}
			}

			test.Test(r => HandleTestResult (r, test, go.GetComponent<TestCaseView> ()));
		}

		private void HandleTestResult(TestCaseResult result, TestCase test, TestCaseView testView)
		{
			switch(result.Result)
			{
			case TestCaseResultCode.Success:
				testView.resultImage.color = Color.green;
				break;
			case TestCaseResultCode.Failure:
				testView.resultImage.color = Color.red;
				break;
			case TestCaseResultCode.Inconclusive:
				testView.resultImage.color = Color.yellow;
				break;
			case TestCaseResultCode.CannotArrange:
				testView.resultImage.color = Color.magenta;
				break;
			default:
				break;
			}

			if(! result.Details.IsNullOrEmpty())
			{
				testView.details.text = result.Details;
			}
		}
			
	}
}