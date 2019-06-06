using UnityEngine;
using System.Collections;

namespace TabTale
{
	public partial class SocialStateModel : StateModel<SocialStateData>
	{
		[Inject]
		public SocialImageReadySignal socialImageReadySignal { get; set; }

		public bool WaitingForSocialLogin
		{
			get { return TTPlayerPrefs.GetValue("WaitingForSocialLogin", false);}
			set { TTPlayerPrefs.SetValue("WaitingForSocialLogin", value); }
		}

		public bool HasProfileImage()
		{
			return !string.IsNullOrEmpty(_data.profileImage.value);
		}

		public AssetData GetProfileImage()
		{
			return _data.profileImage.Clone() as AssetData;
		}

		public bool SetProfileImage(AssetData img)
		{
			if (img.assetType == AssetType.Image) {
				_data.profileImage = img;
				return Save();
			}

			Debug.LogError("ProfileImage needs to be of type Image.");
			return false;
		}

		public string GetPlayerId()
		{
			return _data.playerId;
		}
		
		public bool SetPlayerId(string val)
		{
			_data.playerId = val;
			return Save();
		}
		
		
		public string GetPlayerName()
		{
			return _data.playerName;
		}
		
		public bool SetPlayerName(string val)
		{
			_data.playerName = val;
			return Save();
		}
		
		
		public string GetFacebookId()
		{
			return _data.facebookId;
		}
		
		public bool SetFacebookId(string val)
		{
			_data.facebookId = val;
			return Save();
		}
		
		
		public string GetFacebookName()
		{
			return _data.facebookName;
		}
		
		public bool SetFacebookName(string val)
		{
			_data.facebookName = val;
			return Save();
		}
		
		
		public string GetGameCenterId()
		{
			return _data.gameCenterId;
		}
		
		public bool SetGameCenterId(string val)
		{
			_data.gameCenterId = val;
			return Save();
		}
		
		
		public string GetGooglePlayId()
		{
			return _data.googlePlayId;
		}
		
		public bool SetGooglePlayId(string val)
		{
			_data.googlePlayId = val;
			return Save();
		}

		public string GetEmail()
		{
			return _data.email;
		}
		
		public bool SetEmail(string val)
		{
			_data.email = val;
			return Save();
		}

		public string GetNetworkUserId(SocialNetwork network)
		{
			switch (network){
			case SocialNetwork.Facebook:
				return _data.facebookId;
			case SocialNetwork.GameCenter:
				return _data.gameCenterId;
			case SocialNetwork.PlayServices:
				return _data.googlePlayId;
			}
			return "";
		}
		public bool SetNetworkUserId(SocialNetwork network, string userId)
		{
			switch (network){
			case SocialNetwork.Facebook:
				return SetFacebookId(userId);
			case SocialNetwork.GameCenter:
				return SetGameCenterId(userId);
			case SocialNetwork.PlayServices:
				return SetGooglePlayId(userId);
			}
			return false;
		}

		protected override void PerformAfterSync()
		{
			CheckIfSocialImageReady();
		}

		private void CheckIfSocialImageReady()
		{
			string photoUrl = GetProfileImage().value;
			if(!string.IsNullOrEmpty(photoUrl))
			{
				socialImageReadySignal.Dispatch(photoUrl);
			}
		}
	}
}