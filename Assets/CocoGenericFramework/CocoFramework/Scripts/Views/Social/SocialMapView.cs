using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
namespace TabTale
{
	public interface IMapImagesDisplayer
	{
		void DisplayImageForLevel(int level, Texture2D image);
	}

    public class SocialMapView : GameView
    {
        [Inject]
        public MatchEndSignal matchEndSignal { get; set; }

        [Inject]
        public LevelStateModel levelStateModel { get; set; }

        [Inject]
        public MatchStateModel matchStateModel { get; set; }

        [Inject]
        public SocialNetworkLoginCompletedSignal socialLoginCompletedSignal { get; set; }

        [Inject]
        public ISocialNetworkService socialNetworkService { get; set; }

        [Inject]
        public RemoteImageService remoteImageService { get; set; }

        [Inject]
        public ProgressStateModel progressStateModel { get; set; }

		[Inject]
		public SocialStateModel socialStateModel { get; set;}

		[Inject]
		public RelationshipStateModel relationshipStateModel { get; set; }

		[Inject]
		public RelationshipsUpdateSignal relationshipsUpdateSignal { get; set; }

		[Inject]
		public SocialImageReadySignal socialImageReadySignal { get; set; }

		[Inject]
		public ISyncService syncService { get; set; }

		IMapImagesDisplayer _mapUI;

		protected override void OnRegister ()
		{
			base.OnRegister();

			_mapUI = gameObject.GetComponentOrInterface<IMapImagesDisplayer>();

			if(_mapUI == null)
			{
				Debug.LogError("Cannot find a behaviour implementing the map images displayer interface");
			}

			ShowImagesOnMap();
		}

        protected override void AddListeners()
        {
			relationshipsUpdateSignal.AddListener (OnRelationshipsUpdated);
			socialImageReadySignal.AddListener(OnSocialImageReady);
        }

        protected override void RemoveListeners()
        {
			relationshipsUpdateSignal.RemoveListener (OnRelationshipsUpdated);
			socialImageReadySignal.RemoveListener(OnSocialImageReady);
		}

		private void OnRelationshipsUpdated()
		{
			Debug.Log("MapView.OnRelationshipsUpdated triggered - Attempt to display friends images");
			ShowFriendsImages();
		}

		private void OnSocialImageReady(string photoUrl)
		{
			// Temporarily disabled since it causes the player's image to by shown twice (due to the signal being sent twice during social state sync process)
			//Debug.Log("MapView.OnSocialImageReady triggered - Attempt to display player's image");
			//ShowPlayerImage();
		}

		private void ShowImagesOnMap()
		{
			ShowPlayerImage();
			ShowFriendsImages();
		}

		private void ShowPlayerImage()
		{
			int levelReached = syncService.GetMaxProgress();

			if(! socialStateModel.HasProfileImage())
				return;

			string photoUrl = socialStateModel.GetProfileImage().value;

			StartCoroutine(ShowPlayerImageForLevel(photoUrl,levelReached));

		}

		public void ShowFriendsImages()
		{
			List<string> ids = new List<string>();
			foreach (RelationshipStateData _data in relationshipStateModel.GetAllSharedState())
			{
				ids.Add(_data.id);
			}
			
			int friendsCount = relationshipStateModel.GetFriendCount();
			
			Debug.LogWarning("MapPlayerView: Getting list of friends. Friend count " + friendsCount);
			
			StartCoroutine(ShowFriendsImagesCoro(ids));
		}

		private IEnumerator ShowFriendsImagesCoro(List<string> ids)
		{
			Debug.LogWarning("MapPlayerView: Showing image thumbnails of friends per level");
			for (int i = 0; i < ids.Count; i++)
			{
				string photoUrl = relationshipStateModel.GetFriendImage(ids[i]);
				int levelReached = relationshipStateModel.GetFriendMaxLevel(ids[i]);

				StartCoroutine(ShowPlayerImageForLevel(photoUrl,levelReached));
				
			}
			yield return null;
		}
		
		private IEnumerator ShowPlayerImageForLevel(string photoUrl, int levelReached)
        {
			if(string.IsNullOrEmpty(photoUrl))
			{
				yield break;
			}

			WWW www = remoteImageService.getCachedWWW(photoUrl);
			yield return www;

			Texture2D tex = www.texture;

			if (tex != null && levelReached > 0)
            {
				_mapUI.DisplayImageForLevel(levelReached, tex);
            }
        }

    }
}