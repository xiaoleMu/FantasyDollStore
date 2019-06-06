using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using strange.extensions.signal.impl;

namespace TabTale
{
    public class RelationshipStateModel : SharedStateModel<RelationshipStateData>
    {
		[Inject]
		public ProgressStateModel progressStateModel { get; set; }

		[Inject]
		public RelationshipsUpdateSignal relationshipsUpdateSignal { get; set; }

        public int GetFriendCount()
        {
            return _sharedStateItems.Count;
        }

        public string GetFriendName(string id)
        {
            return _sharedStateItems.FirstOrDefault(x => x.GetId() == id).name;
        }

        public int GetFriendMaxLevel(string id) {

            return _sharedStateItems.FirstOrDefault(x => x.GetId() == id).progress.maxLevel;
        }

        public string GetFriendImage(string id)
        {
			AssetData assetData = _sharedStateItems.FirstOrDefault(x => x.GetId() == id).image;
			return (assetData == null) ? "" : assetData.value;

        }

        public int GetFriendTopScore(string id)
        {
            return _sharedStateItems.FirstOrDefault(x => x.GetId() == id).progress.topScore;
        }

		public RelationshipStateData GetNextFriendByScore()
		{
			int myTopScore = progressStateModel.GetTopScore();

			IEnumerable<RelationshipStateData> friends = _sharedStateItems
				.Where(r => r.progress.topScore >= myTopScore).OrderByDescending(s => s.progress.topScore);

			RelationshipStateData friend = null;
			if(friends != null)
			{
				friend = friends.LastOrDefault();
			}

			if(friend == null)
			{
				friend = _sharedStateItems.Where(r => r.progress.topScore < myTopScore).OrderByDescending(s => s.progress.topScore).First ();
			}
			
			if(friend == null)
			{
				Debug.Log ("RelationshipStateModel.GetNextFriendByScore - Could not find any friends");
				return null;
			}

			return friend;
		}

		protected override bool ValidateAfterSync()
		{
			base.ValidateAfterSync();

			Debug.Log ("Received new relationship data");
			relationshipsUpdateSignal.Dispatch();

			return true;
		}


		public string GetFriendFBNetworkId(string id) 
		{
			RelationshipStateData friendState = _sharedStateItems.FirstOrDefault(x => x.GetId() == id);
//			if (friendState != null) 
//			{
				//if (friendState.socialNetworks[0].type == SocialNetwork.Facebook) 
//					{
			Debug.Log("Friend State " +  friendState.socialNetworks[0].id);
			return friendState.socialNetworks[0].id;
//					}
//			}
//			return null;
		}
	}
}
