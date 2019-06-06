using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TabTale
{
	public class RequestsManager
	{
		[Inject]
		public RequestStateModel requestStateModel { get; set; }

		[Inject]
		public IRoutineRunner routineRunner { get; set; }

		[Inject]
		public IGameDB _gameDB{get;set;}

		[Inject]
		public PlayerInfoService playerInfoService { get; set; }

		[Inject]
		public RelationshipStateModel relationshipStateModel { get; set; }

		protected ICollection<RequestStateData> _sharedStateItems;

		public int SendRequest(List<string> friendIds, RequestType requestType)
		{
			_sharedStateItems = _gameDB.LoadSharedState<RequestStateData>();

			List<string> ids = new List<string> ();

			if (friendIds.Count > 0) 
			{
				foreach (string id in friendIds) 
				{
					int requestsAlreadySent = _sharedStateItems.Count(r => r.sender == playerInfoService.PlayerId
					&& r.type == requestType
					&& r.receiver == id
					&& (r.status == RequestStatus.Sent || r.status == RequestStatus.Received));
	
					if ( requestsAlreadySent <= 0) 
					{
						ids.Add (id);
					}
				}

				if (ids.Count > 0) 
				{
					requestStateModel.CreateAppRequest (ids, requestType);
				}
		
			}
			return ids.Count;
		}

		public void UpdateRequestStatus(string friendId, RequestStatus newStatus)
		{
			requestStateModel.UpdateAppRequestStatus(friendId, newStatus);
		}

		public void SendAllFriendsRequest(RequestType type)
		{
			List<string> ids = new List<string>();
			foreach (RelationshipStateData _data in relationshipStateModel.GetAllSharedState())
			{
				ids.Add(_data.id);
			}
				
			SendRequest (ids, type);
		}
			

		public bool IsPlayerID(string friendID)
		{
			if (requestStateModel.IsPlayerID (friendID)) 
			{
				Debug.Log ("RequestManagerServiceView: " + "This is player id.");
				return true;
			} 
			else 
			{
				Debug.Log ("RequestManagerServiceView: " + "This is NOT player id.");
				return false;	
			}
		}

		public bool CheckStatus (string id, RequestStatus status)
		{
			if (requestStateModel.CheckRequestStatus (id,status))
				return true;
			else
				return false;		
		}

		//message types	(SEND)
		public List<string> RequestsReceivedMessages ()
		{
			List<string> ids = new List<string>();
			foreach (RequestStateData _data in requestStateModel.GetAllSharedState())
			{
				if (_data.status == RequestStatus.Received && _data.type == RequestType.askFor) {
					ids.Add (_data.sender);
				}
			}
			return ids;
		}

		// (ACCEPT)
		public List<string> RequestClaimMessages ()
		{
			List<string> ids = new List<string>();
			foreach (RequestStateData _data in requestStateModel.GetAllSharedState())
			{
				if (_data.status == RequestStatus.Received && _data.type == RequestType.gift) 
				{
					ids.Add (_data.sender);
				}
			}
			return ids;
		}
			

	}
}