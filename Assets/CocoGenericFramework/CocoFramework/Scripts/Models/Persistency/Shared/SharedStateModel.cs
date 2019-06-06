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
	public class SharedStateModel<TSharedStateData> where TSharedStateData : class, ISharedData, new()
	{
		[Inject]
		public IGameDB _gameDB{get;set;}

		[Inject]
		public SyncSharedStateSignal syncSharedStateSignal{get;set;}

		//[Inject]
		//public ClearSharedStateSignal clearSharedStateSignal{get;set;}

		protected string _class;

		protected bool _shortLoadLog = false;

		protected ICollection<TSharedStateData> _sharedStateItems;

		protected string Tag 
		{
			get { return _class; }
		}

		[PostConstruct]
		public void Init(){
			_class = GetType().Name;

			TSharedStateData tempItem = new TSharedStateData();
			_gameDB.RegisterAndInitSharedState(tempItem.GetTableName());

			Load ();
			ValidateInit();

			syncSharedStateSignal.AddListener(OnSharedStateSync);
		}
		
		protected bool Load()
		{
			if (_sharedStateItems != null)
				_sharedStateItems.Clear();

			_sharedStateItems = _gameDB.LoadSharedState<TSharedStateData>();

			if (_shortLoadLog)
				Debug.Log ("LOAD SHAREDSTATE (" + _class + ") got " + _sharedStateItems.Count + " sharedStates.\n(Set _fullLoadLog=true to get full loading log)");
			else
			{
				string log = "LOAD SHAREDSTATE (" + _class + ") got " + _sharedStateItems.Count + " sharedStates:";
				foreach(TSharedStateData sharedStateItem in _sharedStateItems)
					log += ("\n" + sharedStateItem.ToLogString());
				Debug.Log(log);
			}

			return true;

		}
		
		protected virtual bool ValidateInit() { return true; }
		protected virtual bool ValidateAfterSync() { return true; }

		/// <summary>
		/// Save of shared state. Was enhanced to also reload data to the model after saving to the db
		/// </summary>
		/// <param name="shared">Shared.</param>
		protected bool Save(TSharedStateData shared)
		{
			bool updatedSuccessfully = false;
			updatedSuccessfully =_gameDB.UpdateSharedState<TSharedStateData>(shared);

			if(updatedSuccessfully)
				Load ();

			return updatedSuccessfully;
		}

		protected bool DeleteSharedState(TSharedStateData shared)
		{
			_sharedStateItems.Remove(shared);
			return _gameDB.DeleteSharedState<TSharedStateData>(shared);
		}
		
		protected void OnClearSharedState()
		{
			if (_sharedStateItems != null)
				_sharedStateItems.Clear();
			_gameDB.ClearSharedStateData<TSharedStateData>();
		}

		protected void OnSharedStateSync (ICollection<string> affectedTables)
		{
			string tableName = new TSharedStateData().GetTableName();
			if (affectedTables.Select(affectedTable => affectedTable.CompareTo(tableName) == 0).Count() > 0)
			{
			    Load ();
				ValidateAfterSync();
			}
		}

		public IEnumerable<TSharedStateData> GetAllSharedState() 
		{
			return _sharedStateItems.Select(item => item.Clone() as TSharedStateData);
		}
		public virtual TSharedStateData GetSharedStateItem(string id) 
		{
			TSharedStateData data = _sharedStateItems.FirstOrDefault(item => item.GetId() == id);
			//Debugger.Assert(data != null, _class + ".GetSharedStateItem(id) - Request for shared data with illegal id: " + id);
			return data == null ? null : data.Clone() as TSharedStateData;
		}
	}

	
	/*public class RelationSharedStateModel : SharedStateModel<RelationSharedStateData>
	{
		public RelationSharedStateData GetRelation(string id)
		{
			return _sharedItems.FirstOrDefault(config => (config as RelationSharedStateData).id == id).Clone() as RelationSharedStateData;
		}
		public IEnumerable<RelationSharedStateData> GetRelationsOfStatus(int status)
		{
			return _sharedItems.Where(item => (item as RelationSharedStateData).relationStatus == status).Select(item => item.Clone() as RelationSharedStateData);
		}
	}*/




}

