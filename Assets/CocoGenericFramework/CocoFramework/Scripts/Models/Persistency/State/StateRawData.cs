namespace TabTale
{
	public class StateRawData
	{
		public string ID;
		public string RawData;
		public SyncStatus Sync;
		public string OriginalId;

		public StateRawData (string id)
		{
			ID = id;
			RawData = "{}";
			Sync = SyncStatus.Updated;
			OriginalId = ID;
		}

		public string GetTableName ()
		{
			return "state";
		}

		public StateRawData Clone ()
		{
			var c = new StateRawData (ID) {
				RawData = RawData,
				Sync = Sync
			};

			return c;
		}
	}
}