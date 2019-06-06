namespace CocoPlay.ResourceManagement
{
	public class LocationUnit : LoadHolderUnit<LocationHolder, LocationLoadRequest, LocationData>
	{
		protected override LocationHolder CreateHolder (LocationData data)
		{
			switch (data.Location) {
			case LocationType.Streaming:
				var streamingPath = ResourceStreamingLocation.RuntimeAssetBundleFullRootPath;
				return new StreamingLocationHolder (streamingPath);

			case LocationType.Server:
				var serverUri = ResourceServerLocation.AssetBundleFullRootUri;
				return new ServerLocationHolder (serverUri);

			case LocationType.ODR:
				var odrTag = data.Id;
				return new ODRLocationHolder (odrTag);
			}

			return null;
		}
	}
}