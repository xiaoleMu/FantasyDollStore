using UnityEngine;
using System;
using System.Collections;

namespace TabTale
{
	public enum RequestType
	{
		invite, gift, askFor, failed
	}


	public enum RequestStatus
	{
		Sent=0, Received=1, Claimed=2, Expired=3
	}

	public class RequestStateData : ISharedData {

		public string id;
		public string sender;
		public string receiver;
		public string requestNetworkId;
		public RequestType type;
		public RequestStatus status;
		public GenericPropertyData[] data;
		public string createDate;

		#region ISharedData implementation

		public string GetTableName ()
		{
			return "request_state";
		}

		public string GetId ()
		{
			return id;
		}

		public string ToLogString ()
		{
			return string.Format ("[RelationshipScoreStateData: id={0}, sender={1}, receiver={2}, requestNetworkId={3}, type={4}, status={5}, data={6}, createDate={7}]", id, sender, receiver, requestNetworkId, type, status, data.ArrayString(), createDate);
		}

		public ISharedData Clone ()
		{
			RequestStateData clone = new RequestStateData();
			clone.id = this.id;
			clone.sender = this.sender;
			clone.receiver = this.receiver;
			clone.requestNetworkId = this.requestNetworkId;
			clone.type = this.type;
			clone.status = this.status;
			clone.createDate = this.createDate;
			return clone;
		}

		#endregion


	}
}
