using System.Collections.Generic;
using UnityEngine;

namespace CocoPlay.ResourceManagement
{
	public class AssetBundleHolder : LoadHolder<AssetBundleLoadRequest>
	{
		public AssetBundleHolder (string name, List<AssetBundleHolder> dependencies, LocationHolder inLocation)
		{
			Name = name;
			Dependencies = dependencies;
			_inLocation = inLocation;
		}

		public string Name { get; private set; }

		public List<AssetBundleHolder> Dependencies { get; private set; }

		private readonly LocationHolder _inLocation;


		#region Load

		public event System.Action OnUnload;

		public override bool IsSyncLoadSupported {
			get { return _inLocation.IsAssetBundleSyncLoadSupported; }
		}

		protected override bool LoadProcess ()
		{
			LoadDependencies ();

			Entity = _inLocation.LoadAssetBundle (this);
			InitReference ();
			return Entity != null;
		}

		private void LoadDependencies ()
		{
			if (Dependencies == null) {
				return;
			}

			foreach (var dependency in Dependencies) {
				dependency.Load ();
			}
		}

		private void UnloadDependencies ()
		{
			if (Dependencies == null) {
				return;
			}

			foreach (var dependency in Dependencies) {
				dependency.Unload ();
			}
		}

		protected override bool UnloadProcess ()
		{
			if (!HasReference) {
				return true;
			}

			UnloadDependencies ();

			UnReference ();
			if (HasReference) {
				return false;
			}

			if (Entity != null) {
				Entity.Unload (true);
				Entity = null;
			}

			if (OnUnload != null) {
				OnUnload ();
			}

			return true;
		}

		#endregion


		#region Request

		protected override AssetBundleLoadRequest CreateLoadRequest ()
		{
			return _inLocation.LoadAssetBundleAsync (this);
		}

		protected override void OnLoadRequestReceived (AssetBundleLoadRequest loadRequest)
		{
			Entity = loadRequest.Asset;
			InitReference ();
		}

		#endregion


		#region Reference

		public AssetBundle Entity { get; private set; }

		private int ReferenceCount { get; set; }

		public bool HasReference {
			get { return ReferenceCount > 0; }
		}

		private void InitReference ()
		{
			if (Entity != null) {
				ReferenceCount = 1;
			}
		}

		public void Reference ()
		{
			if (Entity == null) {
				return;
			}

			ReferenceCount++;

			ResourceDebug.Log ("{0}->Reference: [{1}] count ({2})", GetType ().Name, Name, ReferenceCount);
		}

		public void UnReference ()
		{
			if (Entity == null) {
				return;
			}
			if (ReferenceCount <= 0) {
				return;
			}

			ReferenceCount--;
			ResourceDebug.Log ("{0}->UnReference: [{1}] count ({2})", GetType ().Name, Name, ReferenceCount);
		}

		#endregion
	}
}