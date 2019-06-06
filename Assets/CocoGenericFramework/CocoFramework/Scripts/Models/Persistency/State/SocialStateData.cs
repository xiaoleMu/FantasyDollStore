using System.Collections.Generic;

namespace TabTale
{
	public class SocialStateData : IStateData
	{
		public string playerId;
		public string playerName;
		public string facebookId;
		public string facebookName;
		public string gameCenterId;
		public string googlePlayId;
		public string email;
		public AssetData profileImage = new AssetData("", AssetType.String, "");
		public List<GenericPropertyData> properties = new List<GenericPropertyData>();

		public string GetNetworkUserId(SocialNetwork network)
		{
			switch (network){
			case SocialNetwork.Facebook:
				return facebookId;
			case SocialNetwork.GameCenter:
				return gameCenterId;
			case SocialNetwork.PlayServices:
				return googlePlayId;
			}
			return "";
		}
		public static SocialNetwork GetNetworkFromString(string network)
		{
			if (network.CompareTo(SocialNetwork.Facebook.ToString()) == 0)
				return SocialNetwork.Facebook;
			if (network.CompareTo(SocialNetwork.GameCenter.ToString()) == 0)
				return SocialNetwork.GameCenter;
			if (network.CompareTo(SocialNetwork.PlayServices.ToString()) == 0)
				return SocialNetwork.PlayServices;
			
			return SocialNetwork.NoNetwork;
		}

		#region IStateData implementation
		
		public string GetStateName ()
		{
			return "socialState";
		}
		
		public string ToLogString ()
		{
			return string.Format("SocialStateData: PlayerID: {0}, PlayerName: {1}, FacebookId: {2}, FacebookName: {3}, GameCenterId: {4}, GooglePlayeId:{5}, Properties: {6}",
			                     playerId,playerName,facebookId,gameCenterId,googlePlayId,email,properties.ArrayString());
		}
		
		public IStateData Clone ()
		{
			SocialStateData c = new SocialStateData();
			c.playerId = playerId;
			c.playerName = playerName;
			c.facebookId = facebookId;
			c.facebookName = facebookName;
			c.gameCenterId = gameCenterId;
			c.googlePlayId = googlePlayId;
			c.email = email;
			c.profileImage = (AssetData)profileImage.Clone();
			c.properties = properties == null ? new List<GenericPropertyData>() : new List<GenericPropertyData>(properties.Clone());

			return c;
		}
		
		#endregion
	}
}
