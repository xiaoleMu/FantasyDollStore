using UnityEngine;
using System.Collections;

namespace TabTale
{
	public static class Progress
	{
		public static IProgress FromAsyncOperation(AsyncOperation operation)
		{
			return new AsyncOperationProgress(operation);
		}

		class AsyncOperationProgress : IProgress
		{
			AsyncOperation _operation;
			public AsyncOperationProgress(AsyncOperation operation)
			{
				_operation = operation;
			}

			public float Progress
			{
				get { return _operation.progress; }
			}
		}
	}
}
