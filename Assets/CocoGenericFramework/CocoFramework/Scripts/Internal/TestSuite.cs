using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RSG;
using System;
using strange.extensions.injector.api;

namespace TabTale
{
	public enum TestCaseResultCode
	{
		Success,
		Failure,
		CannotArrange,
		Inconclusive
	}

	public class TestCaseResult
	{
		public TestCaseResultCode Result;
		public string Details;

		public static TestCaseResult Create(TestCaseResultCode result, string details = "")
		{
			return new TestCaseResult(result,details);
		}

		public static TestCaseResult CreateFromCondition(bool success, string successDetails, string failureDetails)
		{
			if(success)
			{
				return new TestCaseResult(TestCaseResultCode.Success,successDetails);
			}
			else
			{
				return new TestCaseResult(TestCaseResultCode.Failure, failureDetails);
			}
		}

		private TestCaseResult(TestCaseResultCode result, string details = "")
		{
			this.Result = result;
			this.Details = details;
		}
	}

	public delegate void TestAction(Action<TestCaseResult> resultCallBack);

	public struct TestPrecondition
	{
		public Func<bool> preCondition;
		public string notMetDetails;

		public TestPrecondition(Func<bool> preCondition, string details = "")
		{
			this.preCondition = preCondition;
			this.notMetDetails = details;
		}

		public static TestPrecondition Create(Func<bool> preCondition, string details = "")
		{
			return new TestPrecondition(preCondition, details);
		}
	}

	public class TestCase
	{
		public TestAction Test;

		public string Description;

		public string Details;

		public List<TestPrecondition> preconditions;
	}

	public class TestSuite
	{
		[Inject]
		public IInjector injector { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		public List<TestCase> tests = new List<TestCase>();

		public void AddTest(TestAction test, string description = "", string details = "", bool isPreconditionMet = true, string preconditionDescription = "")
		{
			AddTest(test, description, details, new List<TestPrecondition> { new TestPrecondition(() => isPreconditionMet, preconditionDescription) } );
		}

		public void AddTest(TestAction test, string description, string details, List<TestPrecondition> preconditions)
		{
			TestCase testCase = injector.binder.GetInstance<TestCase>();
			testCase.Test = test;
			testCase.Description = description;
			testCase.Details = details;
			testCase.preconditions = preconditions ?? new List<TestPrecondition>();

			tests.Add(testCase);
		}

		public void AddTest(TestCase testCase)
		{
			tests.Add(testCase);
		}
	}
}