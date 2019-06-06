using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TabTale
{
	public class RankConfigData : IConfigData
	{
		public string id;
		public int index;
		public string name;
		public int xp;
		public string rankUpTextId;
		public List<GameElementData> restrictions 	= new List<GameElementData>();
		public List<GenericPropertyData> properties = new List<GenericPropertyData>();


		#region IConfigData implementation
		public string GetTableName ()
		{
			return "rank_config";
		}
		public string GetId ()
		{
			return id;
		}
		public string ToLogString ()
		{
			string res = 
				@"RankConfigData: Id:{0}, Name: {1}, Xp:{2}, RankUpTextId:{3}, Index:{4},Restrictions:{5},Properties:{6}";
			return string.Format(res, id,name,xp,rankUpTextId,index,restrictions.ArrayString(),properties.ArrayString());
		}

		public bool IsBlob()
		{
			return false;
		}

		public IConfigData Clone ()
		{
			RankConfigData c = new RankConfigData();
			c.id = id;
			c.name = name;
			c.xp = xp;
			c.rankUpTextId = rankUpTextId;
			c.index = index;
			c.restrictions = new List<GameElementData>(restrictions.Clone());
			c.properties = new List<GenericPropertyData>(properties.Clone());

			return c;
		}
		#endregion
	

	}
}
