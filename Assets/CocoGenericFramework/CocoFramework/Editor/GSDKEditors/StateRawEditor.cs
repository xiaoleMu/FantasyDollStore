using UnityEngine;
using UnityEditor;
using TabTale;


public class StateRawEditor : EditorWindow
{
	private Color _contentColor;
	private Color _backgroundColor;
	private bool _dirty;

	// State data Vars
	private StateRawData _stateData;

	private int _selectedIndex = -1;
	private int _newNameSufix;


	#region Unity Editor Methods

	[MenuItem ("TabTale/DB Editors/State Raw Editor", false, 0)]
	public static void ShowWindow ()
	{
		if (EditorApplication.isPlaying)
			EditorUtility.DisplayDialog ("Play Mode", "Dood! The game is running -.-", "OK");
		else if (EditorInstances.liveInstance != null) {
			EditorUtility.DisplayDialog ("Another Editor is opened", "Another Editor is opened. Please Close it first.", "OK");
		} else {
			GetWindow (typeof(StateRawEditor));
		}
	}

	private bool _inited;

	private void Init (bool forceLoad = false)
	{
		EditorInstances.liveInstance = this;
		if (!_inited || forceLoad) {
			titleContent.text = "State Editor";
			_inited = true;
			LoadData ();
		}
	}

	private void Update ()
	{
		if (EditorApplication.isPlayingOrWillChangePlaymode) {
			if (_dirty) {
				EditorApplication.isPlaying = false;
				Focus ();
				focusedWindow.ShowNotification (new GUIContent ("Game stopped because there is an open editor (" + titleContent.text + ")" +
				                                                "with unsaved changes!"));
			} else
				Close ();
		}
	}

	private void OnDestroy ()
	{
		_inited = false;
		if (_dirty && EditorUtility.DisplayDialog ("There are unsaved changes that will be lost!",
			    "Are you sure you want to close the window and lose usaved changes?", "Save it!", "Forget it...")) {
			Save ();
		}
		EditorInstances.liveInstance = null;
		EditorHelpers.ClearData ();
	}

	private Vector2 _scrollPos;

	public void OnGUI ()
	{
		_contentColor = GUI.contentColor;
		_backgroundColor = GUI.backgroundColor;

		Init ();
		_scrollPos = EditorGUILayout.BeginScrollView (_scrollPos);
		if (DrawLoadService ()) {
			GUILayout.Space (30);
			DrawItem ();
		}
		EditorGUILayout.EndScrollView ();

		GUI.contentColor = _contentColor;
		GUI.backgroundColor = _backgroundColor;
	}

	#endregion


	#region Utility Methods

	private void LoadData ()
	{
		_stateData = null;
		_selectedIndex = -1;

		EditorHelpers.LoadStateRawDatas ();
	}

	private void Save ()
	{
		bool canSave = true;
		string notificationMsg = string.Empty;

		// check if names are valid
		foreach (var d in EditorHelpers.AllStates) {
			if (d.ID.Contains ("New State") || d.ID.Contains ("Clone") || d.ID.Length == 0) {
				canSave = false;
				notificationMsg = "State (" + d.ID + ") has invalid name!\n";
				break;
			}
		}

		if (!canSave) {
			ShowNotification (new GUIContent (notificationMsg + "\nState is not saved!"));
		} else {
			if (EditorHelpers.AllStates.Count > 0)
				EditorHelpers.gameDB.DeleteAllStateRawDatas (EditorHelpers.AllStates [0].GetTableName ());
			foreach (var p in EditorHelpers.AllStates) {
				EditorHelpers.gameDB.SaveStateRawData (p.GetTableName (), p);
				p.OriginalId = p.ID;
			}
			EditorHelpers.gameDB.IncrementLocalDBVersion ();
			EditorHelpers.InitStateNames ();
			_dirty = false;
		}
	}

	#endregion


	#region Services

	private bool DrawLoadService ()
	{
		EditorGUILayout.BeginHorizontal ();
		GUI.backgroundColor = EditorHelpers.orangeColor;
		if (GUILayout.Button ("Load", GUILayout.Width (60))) {
			LoadData ();
		}

		GUI.backgroundColor = EditorHelpers.yellowColor;
		if (GUILayout.Button ("New", GUILayout.Width (60))) {
			var newData = new StateRawData ("New State " + _newNameSufix);
			++_newNameSufix;
			EditorHelpers.AllStates.Insert (0, newData);
			EditorHelpers.InitStateNames ();
			_selectedIndex = 0;
			_stateData = newData;
			ShowNotification (new GUIContent ("New State added."));
			_dirty = true;
		}
		GUI.backgroundColor = EditorHelpers.greenColor;
		if (GUILayout.Button ("Save", GUILayout.Width (60))) {
			Save ();
		}
		GUI.backgroundColor = _backgroundColor;
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Separator ();
		EditorGUILayout.Separator ();
		EditorGUILayout.Separator ();
		if (EditorHelpers.AllStates == null) {
			EditorGUILayout.HelpBox ("It seems that there is no data... try reopening the editor.", MessageType.Error);
			return false;
		}
		if (EditorHelpers.AllStates.Count > 0) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("States:", GUILayout.Width (100));
			int oldIntValue = _selectedIndex;
			_selectedIndex = EditorGUILayout.Popup (oldIntValue, EditorHelpers.StateNames, GUILayout.Width (250));
			if (oldIntValue != _selectedIndex) {
				_stateData = EditorHelpers.AllStates [_selectedIndex];
			}
			if (_stateData != null) {
				GUI.backgroundColor = EditorHelpers.blueColor;
				if (GUILayout.Button ("Duplicate", GUILayout.Width (70))) {
					var newData = _stateData.Clone ();
					if (newData != null) {
						newData.ID = string.Format ("{0}(Clone)", _stateData.ID);
						EditorHelpers.AllStates.Insert (0, newData);
						EditorHelpers.InitStateNames ();
						_selectedIndex = 0;
						_stateData = newData;
					}
					ShowNotification (new GUIContent ("State duplicated."));
					_dirty = true;
				}

				GUI.backgroundColor = EditorHelpers.redColor;
				if (GUILayout.Button ("Delete", GUILayout.Width (70))) {
					if (EditorUtility.DisplayDialog ("Deleting State!", "Are you sure you want to delete State'" + _stateData.ID + "'?", "Yes, Delete it.", "No!")) {
						EditorHelpers.gameDB.DeleteStateRawData (_stateData.GetTableName (), _stateData.ID);
						EditorHelpers.AllStates.Remove (_stateData);
						EditorHelpers.InitStateNames ();
						_selectedIndex = -1;
						_stateData = null;
						ShowNotification (new GUIContent ("State deleted."));
						return false;
					}
				}
				GUI.backgroundColor = _backgroundColor;
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
		}

		return (_stateData != null);
	}

	#endregion


	#region Draw Item

	private void DrawItem ()
	{
		_contentColor = GUI.contentColor;
		_backgroundColor = GUI.backgroundColor;

		string oldStringValue;
		int oldIntValue;

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("id:", GUILayout.Width (100));
		oldStringValue = _stateData.ID;
		_stateData.ID = EditorGUILayout.TextField (_stateData.ID, GUILayout.Width (250));
		if (_stateData.ID != oldStringValue)
			_dirty = true;
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("raw data:", GUILayout.Width (100));
		oldStringValue = _stateData.RawData;
		EditorStyles.textArea.wordWrap = true;
		_stateData.RawData = EditorGUILayout.TextArea (_stateData.RawData, EditorStyles.textArea, GUILayout.Width (400), GUILayout.Height (100));
		if (_stateData.RawData != oldStringValue)
			_dirty = true;
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("sync status:", GUILayout.Width (100));
		oldIntValue = (int)_stateData.Sync;
		_stateData.Sync = (SyncStatus)EditorGUILayout.EnumPopup (_stateData.Sync, GUILayout.Width (250));
		if ((int)_stateData.Sync != oldIntValue)
			_dirty = true;
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.Separator ();
		
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("original id:", GUILayout.Width (100));
		EditorGUILayout.LabelField (_stateData.ID, GUILayout.Width (250));
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Separator ();
		EditorGUILayout.Separator ();
		EditorGUILayout.Separator ();
	}

	#endregion
}