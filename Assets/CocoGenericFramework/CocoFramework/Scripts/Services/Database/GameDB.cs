using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System;
using System.IO;
using System.Data;
using System.Reflection;
using LitJson;
using System.Text;
using TabTale.Analytics;

namespace TabTale
{
    public class GameDB : IGameDB
    {
        private const string _dbName = "game.db";
        private string _connectionString = "";
        private IDbConnection _dbcon = null;
        IDbCommand _dbcmd = null;
        IDataReader _dbReader = null;
        object _dbAccessLock = new object();

        private const string _configKeyForceReset = "forceReset";
        private const string _configKeyDebugQueries = "debugQueries";
        private const string _sqliteSystemTable = "sqlite_master";

        private List<string> _listStateTables = new List<string>();

        private string dbFolder;
        private string dbPath;
        private string dbStreamingAsset;
        private bool _debugQueries;


        public void Terminate()
        {
            Debug.Log("GameDB.Terminate()");
            lock (_dbAccessLock)
            {
                DisconnectDB();
            }
        }

        public void InitDB()
        {

            bool runOnAndroid = (Application.platform == RuntimePlatform.Android);

            dbFolder = Application.persistentDataPath + "/DB/";
            dbPath = dbFolder + _dbName;
            dbStreamingAsset = Application.streamingAssetsPath + "/DB/" + _dbName;

            int dbVersion = 0, dbAssetVersion = 0;

            if (runOnAndroid)
            {
                string tempAndroidDbFolder = dbFolder + "AndroidTemp/";
                string tempAndroidDb = tempAndroidDbFolder + _dbName;

                if (!Directory.Exists(tempAndroidDbFolder))
                    Directory.CreateDirectory(tempAndroidDbFolder);

                string relativeDBPath = "DB/" + _dbName;
                if (AssetUtils.CopyStreamingAssetsFile(relativeDBPath, tempAndroidDb))
                {
                    dbStreamingAsset = tempAndroidDb;
                    Debug.Log("GameDB Android: Copied DB from streaming assets to temp folder:" + dbStreamingAsset);
                }
                else
                    Debug.Log("GameDB Android: Faield to copy streaing asset to temp folde:" + dbStreamingAsset);

            }

            if (File.Exists(dbStreamingAsset))
            {
                dbAssetVersion = CheckDBVersion(dbStreamingAsset);
                Debug.Log("GameDB Found streaming asset: " + dbStreamingAsset + " version=" + dbAssetVersion);
            }
            else
                Debug.Log("GameDB Not found streaing asset:" + dbStreamingAsset);

            bool isNewDBVersionFound = false;
            if (File.Exists(dbPath))
            {
                dbVersion = CheckDBVersion(dbPath);
                Debug.Log("GameDB Found db: " + dbPath + " vesion =" + dbVersion);
                if ((dbVersion == 0) || ((dbVersion > 0) && (dbAssetVersion > dbVersion)))
                    isNewDBVersionFound = true;
            }
            else
                Debug.Log("GameDB Not found db:" + dbPath);


            if (!Directory.Exists(dbFolder))
                Directory.CreateDirectory(dbFolder);

            // Allow force DB reset by local config
            Data.DataElement dbConfig = GameApplication.GetConfiguration("database");
            bool forceReset = (dbConfig.IsNull || !dbConfig.ContainsKey(_configKeyForceReset)) ? false : (bool)dbConfig[_configKeyForceReset];
            if (forceReset)
                Debug.Log("GameDB Forcing database reset by local config setting");
            _debugQueries = (dbConfig.IsNull || !dbConfig.ContainsKey(_configKeyDebugQueries)) ? false : (bool)dbConfig[_configKeyDebugQueries];

            // If new version detected (or reset required in config) force DB reset
            if (isNewDBVersionFound || forceReset)
            {
                Debug.Log("GameDB Found newer version of database in assest, storing current state and updating new configs");
                TableCopyFlow(dbStreamingAsset, dbPath);

            }
            if (!File.Exists(dbPath))
            {
                if (File.Exists(dbStreamingAsset))
                {
                    File.Copy(dbStreamingAsset, dbPath);
                    Debug.Log("GameDB No local DB copy over the source db");

                }
                else
                    DownloadDefaultDB();
            }

            SetupDbConnectionAndStatus();

        }

        private void ExecuteDBCommand (IDbConnection dbConn, string sql)
        {
            IDbCommand dbcmd = dbConn.CreateCommand ();
            dbcmd.CommandText = sql;
            IDataReader reader = dbcmd.ExecuteReader ();
            Debug.LogFormat ("GameDB - {0}", sql);

            dbcmd.Dispose ();
            dbcmd = null;
            reader.Close ();
            reader = null;
        }

        /* New FLOW */
        /* ********************* */
        void TableCopyFlow(string source, string target)
        {
            string sourceDB = "URI=file:" + source; //Path to streaming database.
            string targetDB = "URI=file:" + target; //Path to local database.

            string tempSourceCopy = Application.persistentDataPath + "/DB/tempDBCopy.db";
            string tempDB = "URI=file:" + tempSourceCopy; //Path to local database.
            IDbConnection dbconn;

            if (File.Exists(tempSourceCopy))
            {
                Debug.Log("GameDB - temp DB copy found - deleting");
                File.Delete(tempSourceCopy);

            }

            File.Copy(source, tempSourceCopy);
            Debug.Log("GameDB DB temp copy");

            dbconn = (IDbConnection)new SqliteConnection(tempDB);
            dbconn.Open(); //Open connection to the database.

//            IDbCommand dbcmd0 = dbconn.CreateCommand();
//            string sqlQueryi = "DELETE FROM state";
//            dbcmd0.CommandText = sqlQueryi;
//            IDataReader reader0 = dbcmd0.ExecuteReader();
//            Debug.Log("GameDB - DELETE * FROM state");
//            reader0.Close();
//            reader0 = null;
//            dbcmd0.Dispose();
//            dbcmd0 = null;
//
//            //sqlite doesnt support multiple commands in query - after each, exec shoud be called and then disposed
//            IDbCommand dbcmd = dbconn.CreateCommand();
//            sqlQueryi = "ATTACH DATABASE '" + target + "' AS test";
//            dbcmd.CommandText = sqlQueryi;
//            IDataReader reader = dbcmd.ExecuteReader();
//            Debug.Log("GameDB - ATTACH DATABASE AS test");
//            reader.Close();
//            reader = null;
//            dbcmd.Dispose();
//            dbcmd = null;
//
//
//            IDbCommand dbcmd1 = dbconn.CreateCommand();
//            sqlQueryi = "INSERT INTO state SELECT * FROM test.state";
//            dbcmd1.CommandText = sqlQueryi;
//            IDataReader reader1 = dbcmd1.ExecuteReader();
//            Debug.Log("GameDB - INSERT INTO state SELECT * FROM test.state");
//            reader1.Close();
//            reader1 = null;
//            dbcmd1.Dispose();
//            dbcmd1 = null;
//
//            IDbCommand dbcmd2 = dbconn.CreateCommand();
//            sqlQueryi = "DETACH DATABASE 'test'";
//            dbcmd2 = dbconn.CreateCommand();
//            dbcmd2.CommandText = sqlQueryi;
//            IDataReader reader2 = dbcmd2.ExecuteReader();
//            Debug.Log("GameDB - DETACH test DATABASE ");
//            reader2.Close();
//            reader2 = null;
//            dbcmd2.Dispose();
//            dbcmd2 = null;


            //sqlite doesnt support multiple commands in query - after each, exec shoud be called and then disposed
            try {
                // attach state from target to temp
                var sql = "ATTACH DATABASE '" + target + "' AS test";
                ExecuteDBCommand (dbconn, sql);

                // delete state from temp if exists in target
                sql = "DELETE FROM state WHERE state_id IN ( SELECT state_id FROM test.state )";
                ExecuteDBCommand (dbconn, sql);

                // copy state from target to temp
                sql = "INSERT INTO state SELECT * FROM test.state";
                ExecuteDBCommand (dbconn, sql);

                // detach test in temp
                sql = "DETACH DATABASE 'test'";
                ExecuteDBCommand (dbconn, sql);
            }
            catch (Exception e) {
                Debug.LogError ("GameDB - init failed:" + e.Message);
            }
            finally {
                dbconn.Close ();
                dbconn = null;
            }

            if (File.Exists(target))
            {
                Debug.Log("GameDB - Local DB copy exists - overriding");
                File.Delete(target);
                File.Copy(tempSourceCopy, target);
            }
            else
            {
                if (File.Exists(tempSourceCopy))
                {
                    Debug.Log("GameDB - Local DB copy doesnt exist - copying over.");
                    File.Copy(tempSourceCopy, target);
                }
            }

            if (File.Exists(tempSourceCopy))
            {
                File.Delete(tempSourceCopy);
                Debug.Log("GameDB - Deleting temp DB Copy");
            }

            Debug.Log("GameDB - DB UPDATE DONE to Version " + CheckDBVersion(dbStreamingAsset));
        }
        /* ******************* */

        /// <summary>
        /// Initializes the database in Local mode for use in the editor:
        /// </summary>
        public void InitLocalDB()
        {
            Debugger.Assert(Application.isEditor, "Local db can only be used in Unity Editor");

            dbFolder = Application.streamingAssetsPath + "/DB/";
            dbPath = dbFolder + _dbName;
            SetupDbConnectionAndStatus();
        }

        private void SetupDbConnectionAndStatus()
        {
            _connectionString = "URI=file:" + dbPath + ";Timeout=1000;DefaultTimeout=1000";
            Debug.Log(_connectionString);

            _listStateTables = GetAllStateTableNames();

            SetSyncStatus(SyncStatus.InProgress, SyncStatus.Outdated);
        }

        public bool RegisterAndInitSharedState(string tableName)
        {
            if (_listStateTables.Contains(tableName))
                return false;
            _listStateTables.Add(tableName);

            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    string setOutdatedQuery = string.Format("UPDATE {0} SET {0}_sync={1} WHERE {0}_sync={2}", tableName, (int)SyncStatus.Outdated, (int)SyncStatus.InProgress);
                    ExecuteQueryWithLockCheck(setOutdatedQuery);
                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                }
            }
            return true;
        }


        private int CheckDBVersion(string path)
        {
            int version = 0;

            lock (_dbAccessLock)
            {
                _connectionString = "URI=file:" + path + ";";
                _dbcon = (IDbConnection)new SqliteConnection(_connectionString);
                try
                {
                    _dbcon.Open();
                    _dbcmd = _dbcon.CreateCommand();
                    _dbcmd.CommandText = "SELECT db_version_version FROM db_version";
                    _dbReader = _dbcmd.ExecuteReader();
                    _dbReader.Read();
                    version = _dbReader.GetInt32(0);

                }
                catch (Exception e)
                {
                    Debug.Log("GameDB CheckDBVersion - no db, assuming version 0.\n" + e.Message);
                }
                if (_dbcmd != null)
                {
                    _dbcmd.Cancel();
                    _dbcmd.Dispose();
                }
                if (_dbReader != null)
                    _dbReader.Close();
                if (_dbcon != null)
                    _dbcon.Close();

                _dbcmd = null;
                _dbReader = null;
                _dbcon = null;
            }
            return version;
        }

        public void IncrementLocalDBVersion()
        {
            Debugger.Assert(Application.isEditor, "Local db can only be incremented in Unity Editor");

            int version = 0;

            lock (_dbAccessLock)
            {
                _dbcon = (IDbConnection)new SqliteConnection(_connectionString);
                try
                {
                    _dbcon.Open();
                    _dbcmd = _dbcon.CreateCommand();
                    _dbcmd.CommandText = "SELECT db_version_version FROM db_version";
                    _dbReader = _dbcmd.ExecuteReader();
                    _dbReader.Read();
                    version = _dbReader.GetInt32(0);
                    _dbReader.Close();

                    _dbcmd.CommandText = "UPDATE db_version SET db_version_version = " + ++version;
                    _dbReader = _dbcmd.ExecuteReader();
                    _dbReader.Close();
                }
                catch (Exception e)
                {
                    Debug.Log("GameDB IncrementLocalDBVersion - no db, assuming version 0.\n" + e.Message);
                }
                if (_dbcmd != null)
                {
                    _dbcmd.Cancel();
                    _dbcmd.Dispose();
                }
                if (_dbReader != null)
                    _dbReader.Close();
                if (_dbcon != null)
                    _dbcon.Close();

                _dbcmd = null;
                _dbReader = null;
                _dbcon = null;
            }
        }

        private void ConnectDB()
        {
            _dbcon = (IDbConnection)new SqliteConnection(_connectionString);

            try
            {
                _dbcon.Open();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw e;
            }

            _dbcmd = _dbcon.CreateCommand();
        }
        private void DisconnectDB()
        {
            try
            {
                if (_dbcmd != null)
                {
                    _dbcmd.Cancel();
                    _dbcmd.Dispose();
                }
                if (_dbReader != null)
                    _dbReader.Close();
                if (_dbcon != null)
                    _dbcon.Close();
            }
            catch (System.Exception e)
            {
                Debug.Log("GameDB Error in GameDB.Disconnect - " + e.Message);
            }

            _dbcmd = null;
            _dbReader = null;
            _dbcon = null;
        }

        private void PrintException(Exception e)
        {
            Debug.Log("Caught exception: " + e.GetType());
            Debug.Log("Message: " + e.Message);
            Debug.Log("InnerException: " + e.InnerException);
            Debug.Log("Source: " + e.Source);
            Debug.Log("Data: " + e.Data);
            Debug.Log("StackTrace: " + e.StackTrace);
        }

        private void DownloadDefaultDB()
        {
            // TODO - optional - if default DB is missing from assets, download from server
            throw new UnityException(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name + "() - not supported.");
        }


        private int ExecuteQueryWithLockCheck(string query)
        {
            // Important - only call this from WITHIN lock(_dbAccessLock), after connecting to DB
            int affectedRows = 0;
            bool updateSuccess = false;
            int updateTriesCount = 0;

            lock (_dbAccessLock)
            {
                while (!updateSuccess)
                {
                    try
                    {
                        updateTriesCount++;
                        if (updateTriesCount == 3)
                        {
                            Debug.Log("GameDB Failed to perform query 3 times. Canceling query.\n");// + query);
                            break;
                        }

                        _dbcmd.CommandText = query;
                        affectedRows = _dbcmd.ExecuteNonQuery();
                        updateSuccess = true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("GameDB Query failed with message: " + e.Message + "\n" + query);
                        DisconnectDB();
                        if (e.Message.StartsWith("The database file is locked"))
                        {
                            Debug.Log("GameDB Query failed due locked database, attempting to fix #" + updateTriesCount);
                            string dbPath2 = dbPath + "2";
                            File.Copy(dbPath, dbPath2);
                            File.Delete(dbPath);
                            File.Copy(dbPath2, dbPath);
                        }
                        else
                        {
                            Debug.LogError("GameDB Query failed due error (not fixed): " + e.Message + "\n" + query);
                        }
                        ConnectDB();
                    }
                }
            }

            if (_debugQueries)
                Debug.Log("GameDB Activated query (affected " + affectedRows + " rows):\n" + query);
            return affectedRows;
        }

        public string LoadSyncInfo(string key)
        {
            string value = "";
            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    string loadStateQuery = "SELECT sync_info_value FROM sync_info WHERE sync_info_key = \'" + key + "\'";

                    if (_debugQueries)
                        Debug.Log("GameDB Activating query: " + loadStateQuery);
                    _dbcmd.CommandText = loadStateQuery;
                    _dbReader = _dbcmd.ExecuteReader();
                    bool readSuccessful = _dbReader.Read();
                    if (readSuccessful)
                    {
                        value = _dbReader.GetString(0);
                    }
                    else
                    {
                        Debug.LogError("Missing key in sync_info table :" + key);
                    }
                    if (_debugQueries)
                        Debug.Log("GameDB Loaded sync info: \"" + key + "\"=\"" + value + "\"");

                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                }
            }
            return value;
        }
        public bool SaveSyncInfo(string key, string value)
        {
            int rowsAffected = 0;
            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    string updateStateQuery = "UPDATE sync_info SET sync_info_value=\'" + value + "\' WHERE sync_info_key = \'" + key + "\'";

                    _dbcmd.CommandText = updateStateQuery;
                    rowsAffected = _dbcmd.ExecuteNonQuery();

                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                    return false;
                }
            }
            return (rowsAffected > 0);
        }

        public TStateData LoadState<TStateData>() where TStateData : new()
        {
            IStateData tempState = new TStateData() as IStateData;
            TStateData state = default(TStateData);
            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    string loadStateQuery = "SELECT state_data, state_sync FROM state WHERE state_id = \'" + tempState.GetStateName() + "\'";

                    if (_debugQueries)
                        Debug.Log("GameDB Activating query: " + loadStateQuery);
                    _dbcmd.CommandText = loadStateQuery;
                    _dbReader = _dbcmd.ExecuteReader();
                    _dbReader.Read();
                    string data = _dbReader.GetString(0);
                    data = String.IsNullOrEmpty(data) ? "{}" : data;
                    state = JsonMapper.ToObject<TStateData>(data);
                    //Debug.Log ("GameDB Data=" + data + " State = " + ((state == null) ? "null" : (state as IStateData).ToLogString()));

                    DisconnectDB();
                    return state;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogError("GameDB failed loading state: " + tempState.GetStateName());
                    DisconnectDB();
                    return default(TStateData);
                }
            }
        }

        // Called by models to update states from UI actions
        public bool UpdateState(string stateId, string data)
        {
            return SaveState(stateId, data, SyncStatus.Outdated);
        }
        // Called by sync manager to sync states from server
        public bool SyncState(string stateId, string data)
        {
            return SaveState(stateId, data, SyncStatus.Updated);
        }
        private bool SaveState(string stateId, string data, SyncStatus stateSync)
        {
            int rowsAffected = 0;
            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();

                    data = data.Replace("'", "''");
                    string updateStateQuery = "UPDATE state SET state_data=\'" + data + "\', state_sync=" + (int)stateSync + " WHERE state_id = \'" + stateId + "\'";

                    rowsAffected = ExecuteQueryWithLockCheck(updateStateQuery);

                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                    return false;
                }
            }
            return (rowsAffected > 0);
        }


        public ICollection<TConfigData> LoadConfig<TConfigData>() where TConfigData : new()
        {
            int recordCount = 0;
            ICollection<TConfigData> allConfigs = new List<TConfigData>();
            StringBuilder jsonSb = new StringBuilder();

            jsonSb.Append("[");

            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    IConfigData tempConfig = new TConfigData() as IConfigData;
                    string tableName = tempConfig.GetTableName(); //ConfigTables[(int)stateId];
                    string loadConfigQuery = "SELECT " + tableName + "_data FROM " + tableName;
                    string data = "";

                    _dbcmd.CommandText = loadConfigQuery;
                    _dbReader = _dbcmd.ExecuteReader();

                    while (_dbReader.Read())
                    {
                        data = _dbReader.GetString(0);

                        if (recordCount > 0)
                        {
                            jsonSb.Append(",");
                        }

                        jsonSb.Append(data);

                        recordCount++;
                    }

                    jsonSb.Append("]");

                    if (_debugQueries)
                        Debug.Log("GameDB LoadConfig (got " + recordCount + " records): " + loadConfigQuery);
                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                    return allConfigs;
                }
            }

            try
            {
                string jsonData = jsonSb.ToString();
                allConfigs = JsonMapper.ToObject<List<TConfigData>>(jsonData);
            }
            catch (Exception e)
            {
                Debug.LogError(typeof(TConfigData).ToString() + ":GameDB Loading config item failed by model exception:\nException message: " + e.Message);// + "\nSkipped item: " + data;
            }

            return allConfigs;
        }

        public bool SaveConfig(string configTable, string configId, string configData)
        {
            int rowsAffected = 0;

            if (!TableExists(configTable))
            {
                Debug.LogWarning("GameDB - Skipping save of non existing config table: " + configTable);
                return false;
            }

            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    configData = configData.Replace("'", "''");
                    configId = configId.Replace("'", "''");
                    string updateConfigQuery = String.Format("UPDATE {0} SET {0}_data=\'{1}\' WHERE {0}_id=\'{2}\'", configTable, configData, configId);
                    string insertConfigQuery = String.Format("INSERT INTO {0} ({0}_id, {0}_data) VALUES (\'{1}\', \'{2}\')", configTable, configId, configData);

                    rowsAffected = ExecuteQueryWithLockCheck(updateConfigQuery);

                    if (rowsAffected == 0) // id not exist in database
                    {
                        if (_debugQueries)
                            Debug.Log("GameDB Update failed (new id), inserting new row");
                        rowsAffected = ExecuteQueryWithLockCheck(insertConfigQuery);
                    }


                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                    return false;
                }
            }
            return (rowsAffected > 0);
        }

        public bool DeleteConfig(string configTable, string configId)
        {
            Debugger.Assert(Application.isEditor, "DeleteConfig can only be used in Unity Editor");

            int rowsAffected = 0;

            if (!TableExists(configTable))
            {
                Debug.LogWarning("GameDB - Skipping delete from non existing config table: " + configTable);
                return false;
            }

            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    configId = configId.Replace("'", "''");
                    string deleteConfigQuery = String.Format("DELETE FROM {0} WHERE {0}_id = \'{1}\'", configTable, configId);

                    rowsAffected = ExecuteQueryWithLockCheck(deleteConfigQuery);

                    if (rowsAffected == 0) // id not exist in database
                        Debug.Log(string.Format("GameDB Delete failed ({0}), config already deleted or did not exist", configId));

                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                    return false;
                }
            }
            return (rowsAffected > 0);
        }

        public bool DeleteAllConfigs(string configTable)
        {
            Debugger.Assert(Application.isEditor, "DeleteAllConfigs can only be used in Unity Editor");

            int rowsAffected = 0;

            if (!TableExists(configTable))
            {
                Debug.LogWarning("GameDB - Skipping delete from non existing config table: " + configTable);
                return false;
            }

            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    string deleteConfigQuery = String.Format("DELETE FROM {0}", configTable);

                    rowsAffected = ExecuteQueryWithLockCheck(deleteConfigQuery);

                    if (rowsAffected == 0) // id not exist in database
                        Debug.Log("GameDB Delete failed, table already empty");

                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                    return false;
                }
            }
            return (rowsAffected > 0);
        }

        public ICollection<TSharedStateData> LoadSharedState<TSharedStateData>() where TSharedStateData : new()
        {
            int recordCount = 0;
            ICollection<TSharedStateData> allSharedStateItems = new List<TSharedStateData>();
            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    ISharedData tempSharedState = new TSharedStateData() as ISharedData;
                    string tableName = tempSharedState.GetTableName();
                    string loadSharedStateQuery = "SELECT " + tableName + "_data FROM " + tableName;
                    string data = "";

                    _dbcmd.CommandText = loadSharedStateQuery;
                    _dbReader = _dbcmd.ExecuteReader();

                    while (_dbReader.Read())
                    {
                        data = _dbReader.GetString(0);
                        try
                        {
                            TSharedStateData shared = new TSharedStateData();
                            //Debug.Log ("Converting type " + shared.GetType().ToString() + " to json, from data=" + data);
                            shared = JsonMapper.ToObject<TSharedStateData>(data);
                            allSharedStateItems.Add(shared);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("GameDB Loading shared item failed by  exception:\nException message: " + e.Message + "\nSkipped item: " + data);
                        }
                        recordCount++;
                    }
                    if (_debugQueries)
                        Debug.Log("GameDB LoadSharedState (got " + recordCount + " records): " + loadSharedStateQuery);
                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                    return allSharedStateItems;
                }
            }
            return allSharedStateItems;
        }

        // Called by models to update shared data from UI actions
        public bool UpdateSharedState<TSharedStateData>(TSharedStateData shared) where TSharedStateData : ISharedData
        {
            return SaveSharedState<TSharedStateData>(shared, SyncStatus.Outdated);
        }
        // Called by sync manager to sync shared data from server
        public bool SyncSharedState<TSharedStateData>(TSharedStateData shared) where TSharedStateData : ISharedData
        {
            return SaveSharedState<TSharedStateData>(shared, SyncStatus.Updated);
        }
        private bool SaveSharedState<TSharedStateData>(TSharedStateData shared, SyncStatus sharedSync) where TSharedStateData : ISharedData
        {
            int rowsAffected = 0;
            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    string sharedData = JsonMapper.ToJson(shared).Replace("'", "''");
                    string findSharedQuery = String.Format("SELECT {0}_id FROM {0} WHERE {0}_id=\'{1}\'", shared.GetTableName(), shared.GetId());
                    string updateSharedQuery = string.Format("UPDATE {0} SET {0}_data=\'{1}\', {0}_sync={2} WHERE {0}_id = \'{3}\'",
                                                             shared.GetTableName(), sharedData, (int)sharedSync, shared.GetId());
                    string insertSharedQuery = String.Format("INSERT INTO {0} ({0}_id, {0}_data, {0}_sync) VALUES (\'{1}\', \'{2}\', {3})",
                                                              shared.GetTableName(), shared.GetId(), sharedData, (int)sharedSync);

                    _dbcmd.CommandText = findSharedQuery;
                    _dbReader = _dbcmd.ExecuteReader();

                    bool sharedFound = false;
                    while (_dbReader.Read())
                    {
                        sharedFound = true;
                    }
                    _dbReader.Close();

                    if (sharedFound)
                        rowsAffected = ExecuteQueryWithLockCheck(updateSharedQuery);
                    else
                        rowsAffected = ExecuteQueryWithLockCheck(insertSharedQuery);


                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    DisconnectDB();
                    return false;
                }
            }
            return (rowsAffected > 0);
        }

        public bool SaveSharedState(string shared, string sharedId, string sharedData)
        {
            int rowsAffected = 0;
            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();

                    string updateSharedStateQuery = String.Format("UPDATE {0} SET {0}_data=\'{1}\' WHERE {0}_id=\'{2}\'", shared, sharedData, sharedId);
                    string insertSharedStateQuery = String.Format("INSERT INTO {0} ({0}_id, {0}_data) VALUES (\'{1}\', \'{2}\')", shared, sharedId, sharedData);

                    rowsAffected = ExecuteQueryWithLockCheck(updateSharedStateQuery);

                    if (rowsAffected == 0) // id not exist in database
                    {
                        if (_debugQueries)
                            Debug.Log("GameDB Update failed (new id), inserting new row");
                        rowsAffected = ExecuteQueryWithLockCheck(insertSharedStateQuery);
                    }


                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                    return false;
                }
            }
            return (rowsAffected > 0);
        }
        /*public int InitSyncStatus()
		{
			int rowsAffected = 0;
			lock(_dbAccessLock)
			{
				try
				{
					ConnectDB();
					string setOutdatedQuery = "UPDATE state SET state_sync=" + (int)SyncStatus.Outdated + " WHERE state_sync = " + (int)SyncStatus.InProgress;

					rowsAffected = ExecuteQueryWithLockCheck(setOutdatedQuery);

					DisconnectDB ();
				}
				catch(Exception e)
				{
					Debug.LogException(e);
				}
			}
			return rowsAffected;
		}*/
        public int SetSyncStatus(SyncStatus oldStatus, SyncStatus newStatus)
        {
            int rowsAffected = 0;
            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    string setOutdatedQuery = "UPDATE state SET state_sync=" + (int)newStatus + " WHERE state_sync = " + (int)oldStatus;

                    rowsAffected = ExecuteQueryWithLockCheck(setOutdatedQuery);
                    foreach (string tableName in _listStateTables)
                    {
                        setOutdatedQuery = string.Format("UPDATE {0} SET {0}_sync={1} WHERE {0}_sync={2}", tableName, (int)newStatus, (int)oldStatus);
                        rowsAffected = ExecuteQueryWithLockCheck(setOutdatedQuery);
                    }

                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                }
            }
            return rowsAffected;
        }

        public bool SetStateDataAsOutdated()
        {
            bool success = false;
            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    string setStatesAsOutdatedQuery = "UPDATE state SET state_sync=" + (int)SyncStatus.Outdated + " WHERE state_sync = " + (int)SyncStatus.Updated;
                    ExecuteQueryWithLockCheck(setStatesAsOutdatedQuery);

                    Debug.Log("GameDB - set all updated states as outdated to force sync of all states");
                    _dbReader.Close();
                    success = true;

                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                }

                DisconnectDB();
            }
            return success;
        }

        public SyncUpdates GetOutdatedData()
        {
            int countUpdates = 0;
            SyncUpdates updates = new SyncUpdates();
            updates.playerData = new Dictionary<string, string>();
            updates.listStateData = new Dictionary<string, List<string>>();

            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    string setStateInProgressQuery = "UPDATE state SET state_sync=" + (int)SyncStatus.InProgress + " WHERE state_sync = " + (int)SyncStatus.Outdated;
                    string getOutdatedStatesQuery = "SELECT state_id, state_data FROM state WHERE state_sync=" + (int)SyncStatus.Outdated;

                    _dbcmd.CommandText = getOutdatedStatesQuery;
                    _dbReader = _dbcmd.ExecuteReader();

                    while (_dbReader.Read())
                    {
                        string stateId = _dbReader.GetString(0);
                        string stateData = _dbReader.GetString(1);

                        //JsonData statesJson = JsonMapper.ToObject (stateData);

                        updates.playerData.Add(stateId, stateData);
                        countUpdates++;
                        if (_debugQueries)
                            Debug.Log("GameDB Syncing state to server: id=" + stateId + " data=" + stateData);
                    }
                    _dbReader.Close();

                    if (_debugQueries)
                        Debug.Log("GameDB GetOutdatedData (got " + countUpdates + "state records): " + getOutdatedStatesQuery);
                    if (countUpdates > 0)
                    {
                        ExecuteQueryWithLockCheck(setStateInProgressQuery);
                    }

                    foreach (string state_table in _listStateTables)
                    {
                        string setListStateInProgressQuery = string.Format("UPDATE {0} SET {0}_sync={1} WHERE {0}_sync={2}", state_table, (int)SyncStatus.InProgress, (int)SyncStatus.Outdated);
                        string getOutdatedListStateQuery = string.Format("SELECT {0}_id, {0}_data FROM {0} WHERE {0}_sync={1}", state_table, (int)SyncStatus.Outdated);

                        _dbcmd.CommandText = getOutdatedListStateQuery;
                        _dbReader = _dbcmd.ExecuteReader();

                        countUpdates = 0;
                        List<string> listStateUpdates = new List<string>();

                        while (_dbReader.Read())
                        {
                            string listStateId = _dbReader.GetString(0);
                            string listStateData = _dbReader.GetString(1);
                            //JsonData sharedJson = JsonMapper.ToObject (sharedData);
                            listStateUpdates.Add(listStateData);
                            countUpdates++;
                            Debug.Log("GameDB Syncing shared to server: id=" + listStateId + " data=" + listStateData);
                        }
                        _dbReader.Close();
                        if (_debugQueries)
                            Debug.Log("GameDB GetOutdatedData (got " + countUpdates + "shared records): " + getOutdatedListStateQuery);

                        if (countUpdates > 0)
                        {
                            ExecuteQueryWithLockCheck(setListStateInProgressQuery);
                            updates.listStateData.Add(state_table, listStateUpdates);
                        }

                    }

                    DisconnectDB();

                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                }
            }

            return updates;
        }

        public int GetOutdatedData(ref Dictionary<string, string> outdatedStates)
        {
            int rowsAffected = 0;
            int countStates = 0;
            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    string setInProgressQuery = "UPDATE state SET state_sync=" + (int)SyncStatus.InProgress + " WHERE state_sync = " + (int)SyncStatus.Outdated;
                    string getOutdatedStatesQuery = "SELECT state_id, state_data FROM state WHERE state_sync=" + (int)SyncStatus.Outdated;


                    _dbcmd.CommandText = getOutdatedStatesQuery;
                    _dbReader = _dbcmd.ExecuteReader();

                    while (_dbReader.Read())
                    {
                        countStates++;
                        string stateId = _dbReader.GetString(0);
                        string stateData = _dbReader.GetString(1);
                        outdatedStates.Add(stateId, stateData);
                        if (_debugQueries)
                            Debug.Log("GameDB Syncing state to server: id=" + stateId + " data=" + stateData);
                    }
                    _dbReader.Close();

                    if (countStates > 0)
                    {
                        rowsAffected = ExecuteQueryWithLockCheck(setInProgressQuery);
                    }

                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                }
            }
            return rowsAffected;
        }

        public List<string> GetAllStateTableNames()
        {
            return GetAllTableNamesWithPostfix("_state");
        }

        public List<string> GetAllConfigTableNames()
        {
            return GetAllTableNamesWithPostfix("_config");
        }

        public List<string> GetAllTableNamesWithPostfix(string postfix)
        {
            int recordCount = 0;
            List<string> tableNames = new List<string>();

            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    string tableName = _sqliteSystemTable;
                    string findTablesWithPostfixQuery = "SELECT tbl_name FROM " + tableName + " WHERE type=\'table\' AND tbl_name LIKE \'%" + postfix + "\'";
                    string data = "";

                    _dbcmd.CommandText = findTablesWithPostfixQuery;
                    _dbReader = _dbcmd.ExecuteReader();

                    while (_dbReader.Read())
                    {
                        data = _dbReader.GetString(0);
                        try
                        {
                            tableNames.Add(data);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Gamedb loading tables name failed by exception:\nException message: " + e.Message + "\nSkipped item: " + data);
                        }
                        recordCount++;
                    }

                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                    return tableNames;
                }
            }

            return tableNames;
        }

        public bool DeleteSharedState<TSharedStateData>(TSharedStateData shared) where TSharedStateData : ISharedData
        {
            int rowsAffected = 0;
            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    string deleteSharedStateQuery = string.Format("DELETE FROM {0} WHERE {0}_id = \'{1}\'", shared.GetTableName(), shared.GetId());
                    rowsAffected = ExecuteQueryWithLockCheck(deleteSharedStateQuery);
                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    DisconnectDB();
                    return false;
                }
            }
            return (rowsAffected > 0);
        }

        // Called ONLY for replacing player - via social-network-player-switch, or debug menu change player/server
        public bool ClearSharedStateData<TSharedStateData>() where TSharedStateData : new()
        {
            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    ISharedData tempSharedState = new TSharedStateData() as ISharedData;

                    string clearSharedStateQuery = String.Format("DELETE FROM {0}", tempSharedState.GetTableName());
                    int rows = ExecuteQueryWithLockCheck(clearSharedStateQuery);
                    Debug.Log("Clear shared table \"" + tempSharedState.GetTableName() + "\" - " + rows + " rows");

                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    DisconnectDB();
                    return false;
                }
            }
            return true;
        }

        private bool TableExists(string tableName)
        {
            bool result = false;
            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();

                    string checkTableExistsQuery = String.Format("SELECT name FROM sqlite_master WHERE type=\'table\' AND name=\'{0}\'", tableName);

                    _dbcmd.CommandText = checkTableExistsQuery;
                    _dbReader = _dbcmd.ExecuteReader();

                    string data = "";
                    while (_dbReader.Read())
                    {
                        data = _dbReader.GetString(0);
                        if (data != null)
                            result = true;
                    }
                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                }

            }

            return result;
        }


        #region State Raw Data

        public ICollection<StateRawData> LoadStateRawData ()
        {
            int recordCount = 0;
            ICollection<StateRawData> allDatas = new List<StateRawData> ();

            lock (_dbAccessLock) {
                try {
                    ConnectDB ();

                    StateRawData tempData = new StateRawData ("");
                    string tableName = tempData.GetTableName ();
                    string query = string.Format ("SELECT {0}_id, {0}_data, {0}_sync FROM {0}", tableName);

                    _dbcmd.CommandText = query;
                    _dbReader = _dbcmd.ExecuteReader ();


                    while (_dbReader.Read ()) {
                        StateRawData data = new StateRawData (_dbReader.GetString (0));
                        data.RawData = _dbReader.GetString (1);
                        int? sync = _dbReader [2] as int?;
                        data.Sync = sync != null ? (SyncStatus)sync : SyncStatus.Updated;

                        allDatas.Add (data);
                    }

                    if (_debugQueries)
                        Debug.Log ("GameDB LoadConfig (got " + recordCount + " records): " + query);
                    DisconnectDB ();
                }
                catch (Exception e) {
                    Debug.LogException (e);
                    DisconnectDB ();
                    return allDatas;
                }
            }

            return allDatas;
        }


        public bool SaveStateRawData (string stateTable, StateRawData stateRawData)
        {
            int rowsAffected = 0;

            if (!TableExists(stateTable))
            {
                Debug.LogWarning("GameDB - Skipping save of non existing state table: " + stateTable);
                return false;
            }

            lock (_dbAccessLock)
            {
                try
                {
                    ConnectDB();
                    stateRawData.ID = stateRawData.ID.Replace("'", "''");
                    stateRawData.RawData = stateRawData.RawData.Replace("'", "''");
                    string updateStateQuery = String.Format("UPDATE {0} SET {0}_id=\'{1}\', {0}_data=\'{2}\', {0}_sync={3} WHERE {0}_id=\'{4}\'",
                        stateTable, stateRawData.ID, stateRawData.RawData, (int)stateRawData.Sync, stateRawData.OriginalId);
                    string insertStateQuery = String.Format("INSERT INTO {0} ({0}_id, {0}_data, {0}_sync) VALUES (\'{1}\', \'{2}\', {3})",
                        stateTable, stateRawData.ID, stateRawData.RawData, (int)stateRawData.Sync);

                    rowsAffected = ExecuteQueryWithLockCheck(updateStateQuery);

                    if (rowsAffected == 0) // id not exist in database
                    {
                        if (_debugQueries)
                            Debug.Log("GameDB Update failed (new id), inserting new row");
                        rowsAffected = ExecuteQueryWithLockCheck(insertStateQuery);
                    }


                    DisconnectDB();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    DisconnectDB();
                    return false;
                }
            }
            return (rowsAffected > 0);
        }

        public bool DeleteStateRawData (string stateTable, string stateId)
        {
            return DeleteConfig (stateTable, stateId);
        }

        public bool DeleteAllStateRawDatas(string stateTable)
        {
            return DeleteAllConfigs (stateTable);
        }

        #endregion
    }
}