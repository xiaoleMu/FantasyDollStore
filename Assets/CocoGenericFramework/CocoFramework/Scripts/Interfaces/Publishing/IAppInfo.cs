using UnityEngine;
using System.Collections;

namespace TabTale
{
	public interface IAppInfo 
	{
		string ApplicationId { get; }
		string BundleIdentifier { get; }
		string BundleVersion { get; }
	}
}
