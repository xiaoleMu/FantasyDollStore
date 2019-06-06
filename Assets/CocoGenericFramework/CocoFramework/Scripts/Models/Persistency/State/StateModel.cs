using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using LitJson;
using strange.extensions.signal.impl;
using System.Linq;

namespace TabTale
{
	public class StateModel<TStateData> where TStateData : class, IStateData, new()
	{
		[Inject]
		public IGameDB _gameDB{get;set;}
		
		[Inject]
		public SyncStatesSignal syncStateSignal{get;set;}
		
		protected string _class;

		protected TStateData _data;

#if UNITY_2017_1_OR_NEWER
		protected ILogger logger = Debug.unityLogger;
#else
		protected ILogger logger = Debug.logger;
#endif

		[PostConstruct]
		public virtual void Init()
		{
			_class = GetType().Name;
            Debug.Log("Name is " + _class);
			Load ();
			ValidateInit ();
			
			syncStateSignal.AddListener(OnStateSync);
		}
		
		protected bool Load()
		{
			try
			{
				_data = _gameDB.LoadState<TStateData>();

                if(_data == null)
                    Debug.Log("Loaded empty data for : " + _class);
                else
				    Debug.Log("LOAD STATE (" + _class + ") " + _data.ToString());
			}
			catch (System.Exception e)
			{
				Debug.LogError("Error in " + _class + ".Load: - " + e.Message);
				return false;
			}

			return true;
		}
		
		protected virtual bool ValidateInit() { return true; }

		protected bool Save()
		{
			return _gameDB.UpdateState(_data.GetStateName(), JsonMapper.ToJson(_data));
		}
		
		protected void OnStateSync (ICollection<string> affectedStateIds)
		{
			string stateId = new TStateData().GetStateName();
			if (affectedStateIds.Select(affectedStateId => affectedStateId.CompareTo(stateId) == 0).Count() > 0)
			{
				Load ();
				PerformAfterSync();
			}
		}

		protected virtual void PerformAfterSync()
		{
			
		}

		public TStateData GetState() 
		{
			return _data.Clone() as TStateData;
		}
	}
}
