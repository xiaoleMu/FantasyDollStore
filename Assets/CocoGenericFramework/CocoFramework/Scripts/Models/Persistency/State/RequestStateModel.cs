using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using strange.extensions.signal.impl;

namespace TabTale
{
	public class RequestStateModel : SharedStateModel <RequestStateData> {

		[Inject]
		public ServerTime serverTime { get; set;}

		[Inject]
		public RequestReceivedSignal requestReceivedSignal { get; set;}

		[Inject]
		public RequestStateModel requestStateModel { get; set;}

		[Inject]
		public ModelSyncService modelSyncService { get; set;}

		[Inject]
		public ISocialNetworkService socialNetworkService { get; set;}

		[Inject]
		public RelationshipStateModel relationshipStateModel { get; set;}

		[Inject]
		public GeneralParameterConfigModel generalParameterConfigModel { get; set; }

		[Inject]
		public PlayerInfoService playerInfoService { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		private const string AskForHelpMessage = "AskForHelpMessage";
		private const string SendGiftMessage = "SendGiftMessage";
		private const string InviteMessage = "InviteMessage";
		private const string SendGiftTitle = "SendGiftTitle";
		private const string InviteTitle = "InviteTitle";
		private const string AskForHelpTitle = "AskForHelpTitle";

		public bool HasNewRequests
		{
			get
			{
				return NewRequestsCount > 0;
			}
		}

		public int NewRequestsCount
		{
			get
			{
				return _sharedStateItems.Count(r => r.status == 0 && r.receiver == playerInfoService.PlayerId); 
			}
		}

		public RequestStateData GetAppRequestData(string id)
		{
			RequestStateData _data = _sharedStateItems.FirstOrDefault(data => data.id == id);
			if (!RequestExists(id,_data)) return null;
			else return _data.Clone () as RequestStateData;
		}

		public string GetSender (string id)
		{
			RequestStateData _data = _sharedStateItems.FirstOrDefault(data => data.id == id);
			if (!RequestExists(id,_data)) return null;
			else return _data.sender.Clone ().ToString();
	
		}

		public string GetReceiver (string id)
		{
			RequestStateData _data = _sharedStateItems.FirstOrDefault(data => data.id == id);

			if (!RequestExists(id,_data)) return null;
			else return _data.receiver;
		}

		public string GetType (string id)
		{
			RequestStateData _data = _sharedStateItems.FirstOrDefault(data => data.id == id);
			if (!RequestExists(id,_data)) return null;
			else return _data.type.ToString();
		}

		public RequestStatus GetStatus (string id)
		{
			RequestStateData _data = _sharedStateItems.FirstOrDefault(data => data.id == id);
			if (!RequestExists(id,_data)) return RequestStatus.Expired;
			else return _data.status;
		}

		public string GetCreateDate (string id)
		{
			RequestStateData _data = _sharedStateItems.FirstOrDefault(data => data.id == id);
			if (!RequestExists(id,_data)) return null;
			else return _data.createDate.ToString();
		}
			
		public bool CreateAppRequest(List<string>receiverIds, RequestType type)
		{
			string playerID = playerInfoService.PlayerId;

			List<string> fbNotificationsList = new List<string> ();

			foreach(string id in receiverIds)
			{
				RequestStateData _data = new RequestStateData ();

				_data.id = string.Format("{0:yyyyMMdd_hhmmss}_{1}_{2}",DateTime.Now,type,UnityEngine.Random.Range(100,999));
				_data.sender = playerID;
				_data.receiver = id;
				_data.requestNetworkId = socialNetworkService.UserId;
				_data.type = type;
				_data.status = RequestStatus.Sent;
				_data.data = null;
				_data.createDate = serverTime.GetLocalTime ().ToString();

				string fbID = relationshipStateModel.GetFriendFBNetworkId(id);//receivers fb id

				fbNotificationsList.Add(fbID);

				Debug.Log ("RequestStateModel - " + "AppReques of type " + type + " is sent from: " + playerID  +" to: " + id);
				Save (_data);
			}
				
			Debug.Log (fbNotificationsList.Count ().ToString ());
			SendFBNotification (type,fbNotificationsList);
			return true;
		}
			
		private void SendFBNotification(RequestType _type, List<string> recepients)
		{
			string _message="";
			string _title="";

			switch (_type) 
			{
			case RequestType.askFor:
				_message = generalParameterConfigModel.GetString (AskForHelpMessage,"Ask your friends for lives!");
				_title = generalParameterConfigModel.GetString (AskForHelpTitle,"Ask friends");
				break;
			case RequestType.gift:
				_message = generalParameterConfigModel.GetString (SendGiftMessage,"Send gift to friends!");
				_title = generalParameterConfigModel.GetString (SendGiftTitle, "Send gift");
				break;
			case RequestType.invite:
				_message = generalParameterConfigModel.GetString (InviteMessage, "Invite friends to play together!");
				_title = generalParameterConfigModel.GetString (InviteTitle, "Invite friends");
				break;
			}
			//send test data moved to socialnetwork
			socialNetworkService.FacebookNotificationRequest(_message, _title, null, recepients);
		}

		public bool UpdateAppRequestStatus(string id, RequestStatus newStatus)
		{
			RequestStateData _data = _sharedStateItems.FirstOrDefault (data => data.id == id);

			if (RequestExists (id, _data)) 
			{
				_data.status = newStatus;
				Debug.Log ("RequestStateModel - " + "AppRequest status update sent to: " + id + ". New status: " + newStatus);
			}

			return Save (_data);
		}

		public bool CheckRequestStatus (string id, RequestStatus status)
		{
			RequestStateData _data = _sharedStateItems.FirstOrDefault (data => data.id == id);

			if (!RequestExists (id, _data)) return true; //Set true but will show error log with data info. 
			else
			{
				if (_data.status == status)
				{
					return true;
				} 
				else
				{
					return false;
				}
			}
		}

		public RequestType GetRequestType (string id)
		{
			RequestStateData _data = _sharedStateItems.FirstOrDefault (data => data.sender == id);

			if (!RequestExists (id, _data)) return RequestType.failed;
			else 
			{
				return _data.type;
			}
		}

		protected override bool ValidateAfterSync()
		{
			base.ValidateAfterSync();

			Debug.Log ("RequestStateModel: " + "Dispatched RequestReceivedSignal");

			List<RequestStateData> newRequests = _sharedStateItems.Where(r => r.receiver == playerInfoService.PlayerId && r.status == 0).ToList();

			if (newRequests.Count > 0) 
			{
				requestReceivedSignal.Dispatch (newRequests);
			}

			return true;
		}

		public bool RequestExists (string id, RequestStateData data)
		{
			if (data != null) 
			{
				return true;
			} 
			else 
			{
				Debug.LogError ("RequestStateModel: " + "RequestStateData for ID: " + id + " is does not exist!");
				return false;
			}
		}

		public bool IsPlayerID(string friendID)
		{
			string playerID = playerInfoService.PlayerId;
			Debug.Log ("RequestStateModel - " + "Comparing friendID: " + friendID + " to playerID: " + playerID +"! ");

			if (friendID == playerID) 
			{
				return true;
			} 
			else 
			{
				return false;
			}
		}

		public void MarkReceivedRequest(string id)
		{
			RequestStateData _data = _sharedStateItems.FirstOrDefault (data => data.id == id);

			_data.status = RequestStatus.Received;

			Save (_data);
		}

		public void ClaimReceivedRequest(string id)
		{
			RequestStateData _data = _sharedStateItems.FirstOrDefault (data => data.id == id);

			_data.status = RequestStatus.Claimed;

			Save (_data);
		}
			
		public bool HasRequestExpired (string id)
		{
			RequestStateData _data = _sharedStateItems.FirstOrDefault (data => data.id == id);
			int expirationTimer = generalParameterConfigModel.GetInt("RequestExpirationTime");
			DateTime now = serverTime.GetLocalTime();
			DateTime created = DateTime.Parse (_data.createDate);
			TimeSpan duration = now - created;

			if (duration.TotalHours > expirationTimer) 
			{
				_data.status = RequestStatus.Expired;
				Save (_data);
				return true;
			}
			return false;
		}



	}
}