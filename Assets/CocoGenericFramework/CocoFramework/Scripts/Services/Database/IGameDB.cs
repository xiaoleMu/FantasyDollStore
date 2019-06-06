using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System;
using System.IO;
using System.Data;
using System.Reflection;
using LitJson;

namespace TabTale  {

    public enum StateId { Connection=1, Player, Building, Avatar, Currency };

    public enum SyncStatus { Updated=0, Outdated, InProgress, NoChange };

    public interface IGameDB 
    {
        void Terminate();

        void InitDB ();

		void InitLocalDB ();

        bool RegisterAndInitSharedState(string tableName);

        string LoadSyncInfo(string key);

        bool SaveSyncInfo(string key, string value);

        TStateData LoadState<TStateData>() where TStateData : new();

		bool SetStateDataAsOutdated();

        bool UpdateState(string stateId, string data);

        bool SyncState(string stateId, string data);

        ICollection<TConfigData> LoadConfig<TConfigData>() where TConfigData : new();

        bool SaveConfig(string configTable, string configId, string configData);

        ICollection<TSharedStateData> LoadSharedState<TSharedStateData>() where TSharedStateData : new();

        bool UpdateSharedState<TSharedStateData>(TSharedStateData shared) where TSharedStateData : ISharedData;

        bool SyncSharedState<TSharedStateData>(TSharedStateData shared) where TSharedStateData : ISharedData;

        bool SaveSharedState(string shared, string sharedId, string sharedData);

        int SetSyncStatus(SyncStatus oldStatus, SyncStatus newStatus);

        SyncUpdates GetOutdatedData();

        int GetOutdatedData(ref Dictionary<string, string> outdated);

		bool DeleteSharedState<TSharedStateData>(TSharedStateData shared) where TSharedStateData : ISharedData;

		bool ClearSharedStateData<TSharedStateData>() where TSharedStateData : new();

		List<string> GetAllConfigTableNames();
    }


}
