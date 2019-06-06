using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;

namespace TabTale
{
	public class HandleRelationshipsUpdateCommand : Command
	{	
		[Inject]
		public RemoteImageService remoteImageService { get; set; }

		[Inject]
		public RelationshipStateModel relationshipStateModel { get; set; }
		
		public override void Execute ()
		{
			Debug.Log ("HandleRelationshipsUpdageCommand.Execute - Updating all images");

			foreach(RelationshipStateData relationshipData in relationshipStateModel.GetAllSharedState())
			{
				string imageUrl = relationshipStateModel.GetFriendImage(relationshipData.id);
				if(string.IsNullOrEmpty(imageUrl))
					continue;

				Debug.Log ("HandleRelationshipsUpdateCommand - Adding image to cache...");
				remoteImageService.getCachedWWW(imageUrl);
			}
		}
	}
}