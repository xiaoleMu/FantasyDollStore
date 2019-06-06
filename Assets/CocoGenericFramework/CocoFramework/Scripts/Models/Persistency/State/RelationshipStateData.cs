using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class RelationshipStateData : ISharedData
	{
		public string id;
		public List<SocialNetworkData> socialNetworks;
		public string name;
		public ProgressStateData progress;
		public AssetData image;
		public int requestCount;
		public string timeFromLastRequest;
		
		#region IStateData implementation
		
		public string GetTableName ()
		{
			return "relationship_state";
		}
		
		public string GetId()
		{
			return id;
		}
		
		public string ToLogString ()
		{
			return string.Format ("[RelationshipStateData: id={0}, socialNetworks={1}, name={2}, progress={3}, image={4}, requestCount={5}, timeFromLastRequest={6}]", 
			                      id, socialNetworks.ArrayString(), name, progress.ToLogString(), image.ToString(), requestCount, timeFromLastRequest);
		}
		
		public ISharedData Clone ()
		{
			RelationshipStateData clone = new RelationshipStateData();
			clone.id = this.id;
			clone.socialNetworks = new List<SocialNetworkData>(socialNetworks.Clone());
			clone.name = this.name;
			clone.progress = this.progress.Clone() as ProgressStateData;
			clone.image = this.image.Clone() as AssetData;
			clone.requestCount = this.requestCount;
			clone.timeFromLastRequest = this.timeFromLastRequest;
			
			return clone;
		}
		
		#endregion
	}
}