using UnityEngine;
using System.Collections;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TabTale
{
	public class NullPsdkCoreInitializer : NullService, IPsdkCoreInitializer
	{
		public bool Done { get { return true;} }

		private IPromise promise = new Promise();

		public IPromise Init() 
		{ 
			promise.Dispatch();
			return promise; 
		}
	}

	public class NullPsdkCoreServices : IPsdkCoreServices
	{
		#region IPsdkCoreServices implementation

		public bool OnBackPressed ()
		{
			return false;
		}

		#endregion
	}
}