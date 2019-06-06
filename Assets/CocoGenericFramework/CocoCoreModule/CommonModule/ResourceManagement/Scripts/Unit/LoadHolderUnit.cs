using System.Collections.Generic;

namespace CocoPlay.ResourceManagement
{
	public abstract class LoadHolderUnit<THolder, TRequest, TData> where THolder : LoadHolder<TRequest> where TRequest : LoadRequest where TData : HolderData
	{
		private readonly Dictionary<string, THolder> _loadHolderDic = new Dictionary<string, THolder> ();

		public THolder Load (TData data)
		{
			var holder = GetOrCreateHolder (data);
			if (holder == null) {
				return null;
			}

			var result = holder.Load ();
			if (!result) {
				ResourceDebug.Log ("{0}->Load: holder [{1}({2})] load failed.", GetType ().Name, data.Id, data.GetType ().Name);
			}

			return holder;
		}

		public TRequest LoadAsync (TData data)
		{
			var holder = GetOrCreateHolder (data);
			if (holder == null) {
				ResourceDebug.Log ("{0}->Load: holder [{1}({2})] NOT be supported.", GetType ().Name, data.Id, data.GetType ().Name);
				return null;
			}

			return holder.LoadAsync ();
		}

		public void Unload (TData data)
		{
			var holder = GetOrCreateHolder (data);
			if (holder == null) {
				ResourceDebug.Log ("{0}->Unload: holder [{1}({2})] NOT be supported.", GetType ().Name, data.Id, data.GetType ().Name);
				return;
			}

			holder.Unload ();
		}

		protected THolder GetOrCreateHolder (TData data)
		{
			if (_loadHolderDic.ContainsKey (data.Id)) {
				return _loadHolderDic [data.Id];
			}

			var holder = CreateHolder (data);
			if (holder == null) {
				ResourceDebug.Log ("{0}->GetOrCreateHolder: holder [{1}({2})] NOT be supported.", GetType ().Name, data.Id, data.GetType ().Name);
				return null;
			}

			_loadHolderDic.Add (data.Id, holder);
			return holder;
		}

		public THolder GetLoadedHolder (string id)
		{
			if (!_loadHolderDic.ContainsKey (id)) {
				ResourceDebug.Log ("{0}->GetLoadedHolder: holder [{1}] not be created.", GetType ().Name, id);
				return null;
			}

			var holder = _loadHolderDic [id];
			if (!holder.IsLoaded) {
				ResourceDebug.Log ("{0}->GetLoadedHolder: holder [{1}] not be loaded.", GetType ().Name, id);
				return null;
			}

			return holder;
		}

		protected abstract THolder CreateHolder (TData data);
	}
}