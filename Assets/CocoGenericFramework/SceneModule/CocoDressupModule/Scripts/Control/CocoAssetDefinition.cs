using System.Collections.Generic;
using strange.extensions.signal.impl;

namespace CocoPlay
{
	/// <summary>
	/// string: config file path (in resource)
	/// </summary>
	public class CocoAssetLoadConfigHolderSignal : Signal<string>
	{
	}

	public class CocoAssetLoadConfigHolderFinishSignal : Signal<bool>
	{
	}


	public class CocoAssetRequestDressItemHolderSignal : Signal<List<CocoDressItemHolder>, string>
	{
	}


	public class CocoAssetRequestDressItemHolderFinishSignal : Signal<List<CocoDressItemHolder>, string>
	{
	}


	public enum CocoAssetBundleLocation
	{
		Embedded = 0,
		Server = 1,
		ODR = 2
	}
}