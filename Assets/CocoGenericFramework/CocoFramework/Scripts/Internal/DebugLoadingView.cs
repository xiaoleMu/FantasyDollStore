using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TabTale;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DebugLoadingView : MonoBehaviour
{
	public static DebugLoadingView Instance;

	public Text text;
	public Text currentPageText;
	public GameObject debugPanel;
	public GameObject button;
	public ScrollRect scrollRect;
	public GameObject pageLeft, pageRight;

	public Text extraInfoText;

	public bool ScrollLockActive {
		get;
		set;
	}

	public bool ColorFilter {
		get;
		set;
	}

	string extraInfo;


	DateTime _firstLogDate;

	bool _refreshText = false;
	bool _debugActive = false;

	DateTime _firstExtraInfoDate;

	List<System.Text.StringBuilder> _debugDataList = new List<System.Text.StringBuilder>();
	public int maxLines = 100;
	const int MAX_PAGES = 8;
	int _lineCount;

	System.Text.StringBuilder _debugData;

	int _currentPage = -1;

	bool _isFiltering = false;
	bool _isPaging;

	string[] _filteredArr = null;

	string _visibleData;

	void Awake ()
	{
		DontDestroyOnLoad (this);
		Instance = this;
		ResetDebug ();

		//TabTale.Logger.MessageLogged += TabTale_Logger_MessageLogged;

#if UNITY_EDITOR
		_debugActive = true;
		button.SetActive (true);
		//LeanTween.delayedCall(1f, ToggleDebugPanel);
#endif
	}

	void Start ()
	{
		StartCoroutine (UpdateTextCoro ());
	}

	void OnDestroy(){
		//TabTale.Logger.MessageLogged -= TabTale_Logger_MessageLogged;
	}


	public static void SetText (string text)
	{
		SetText (text, Color.white);
	}

	public static void SetWarning (string text)
	{
		SetText (text, Color.yellow);
	}

	public static void SetError (string text)
	{
		SetText (text, Color.red);
	}

	public static void SetText (string text, Color color)
	{
		if (Instance == null)
			return;

		if (text.IsNullOrEmpty ())
			return;
		
		if (Instance._lineCount > Instance.maxLines){
			Instance.AddStringBuilder();
		}

		Instance.AppendLine(text, color);
	}

	public void AddStringBuilder(){

		_debugData = new System.Text.StringBuilder();

		_debugDataList.Add(_debugData);

		if (_debugDataList.Count > MAX_PAGES){
			_debugDataList.RemoveAt(0);
		}

		_lineCount = 0;
		scrollRect.verticalScrollbar.value = 0;

		if (!_isPaging)
			SetCurrentPage(_debugDataList.Count - 1);

		if (_debugDataList.Count > 1){
			pageLeft.SetActive(true);
			pageRight.SetActive(true);
			currentPageText.gameObject.SetActive(true);
		}
	}




	public void AppendLine(string text, Color color){

		TimeSpan diffFromFirstLog = DateTime.Now - _firstLogDate;

		string debugTimeTotal = string.Format ("{0:0.00}", diffFromFirstLog.TotalSeconds);

		_debugData.Append (debugTimeTotal);
		_debugData.Append (":");
		if (color != Color.white) {
			_debugData.Append ("<color=#" + ColorUtils.ColorToHex (color, false) + ">");
			_debugData.Append (text);
			_debugData.Append ("</color>");
		} else {
			_debugData.Append (text);
		}

		_debugData.Append ("\n");

		_lineCount++;

		_refreshText = true;
	}

	public static void SetExtraLoadingInfo (string text)
	{

		if (Instance == null)
			return;

		if (Instance.extraInfoText == null)
			return;

		if (!Instance._debugActive) {
			return;
		}

		Instance.extraInfo = text;
		Instance.extraInfoText.text = text;

		Instance._firstExtraInfoDate = DateTime.Now;
	}

	void UpdateTextUI ()
	{
		if (debugPanel.activeInHierarchy) {
			text.text = GetDefaultParams () + _visibleData;
		}
	}

	static string GetDefaultParams ()
	{
		return "";
		/*
		string clientVersion = GameApplication.ClientVersion;

		return "<color=#00ffffff>" + "version: " + clientVersion + "\n" +
		"server env: " + ConnectionHandler.GetShortServerName () + "\n" +
		"player id: " + PlayerId.Get () + "</color>" + "\n";
		*/
			 
	}

	public System.Text.StringBuilder CurrentDebugData{
		get{
			if (_isPaging)
				return _debugDataList[_currentPage];
			else
				return _debugData;
		}
	}

	#region paging

	public void PageLeft(){
		if (!_isPaging && _debugDataList.Count > 1){
			_isPaging = true;
			SetCurrentPage(_debugDataList.Count - 2);
			_refreshText = true;
		}else if (_currentPage > 0){
			SetCurrentPage(_currentPage - 1);
			_refreshText = true;
		}
	}

	public void PageRight(){
		if (_isPaging){
			if (_currentPage < _debugDataList.Count - 1){
				SetCurrentPage(_currentPage + 1);

				if (_currentPage == _debugDataList.Count - 1){ // last page, resume normal behaviour
					_isPaging = false;
				}
				_refreshText = true;
			}
		}
	}

	void SetCurrentPage(int index){
		_currentPage = index;
		currentPageText.text = (index + 1) + "/" + _debugDataList.Count;
		scrollRect.verticalScrollbar.value = 1;
	}	

	#endregion

	IEnumerator UpdateTextCoro ()
	{
		while (true) {
			if (_refreshText) {
				_refreshText = false;
				if (!_isFiltering) {
					_visibleData = CurrentDebugData.ToString ();

					UpdateTextUI ();
					if (ScrollLockActive)
						scrollRect.verticalScrollbar.value = 1;
					scrollRect.horizontalScrollbar.value = 0;
				}
			}

			if (extraInfoText != null && !extraInfo.IsNullOrEmpty ()) {

				TimeSpan diffFromFirstLog = DateTime.Now - _firstExtraInfoDate;
				
				string debugTimeTotal = string.Format (" {0:0.00}", diffFromFirstLog.TotalSeconds);

				extraInfoText.text = extraInfo + debugTimeTotal;

			}

			yield return new WaitForSeconds (0.5f);
		}
	}

	public void LoadInitScene ()
	{
		SetLoading ();
		SceneManager.LoadScene ("Init");
	}

	public void LoadSplashScene ()
	{
		SetLoading ();
		SceneManager.LoadScene ("SplashScreen");
	}

	void SetLoading ()
	{
		ResetDebug ();
	}


	void ResetDebug ()
	{
		_firstLogDate = DateTime.Now;
		//TabTale.Logger.ResetDebug ();
		_refreshText = true;
		AddStringBuilder();
		pageLeft.SetActive(false);
		pageRight.SetActive(false);
		currentPageText.gameObject.SetActive(false);
	}

	public void UpdateFilter (string filter)
	{

		if (filter.Length == 0) {
			_filteredArr = null;
			_isFiltering = false;
			_visibleData = CurrentDebugData.ToString ();

			UpdateTextUI ();
		} else {
			_isFiltering = true;

			if (_filteredArr == null) {
				_filteredArr = _debugData.ToString ().Split ('\n');
			}

			System.Text.StringBuilder filteredData = new System.Text.StringBuilder ();

			for (int i = 0; i < _filteredArr.Length; i++) {

				if (ColorFilter) {
					if (IsLineInFilter (i, filter)) {
						Color color = Color.yellow;
						string line = "<color=#" + ColorUtils.ColorToHex (color, false) + ">";
						line += _filteredArr [i];
						line += "</color>";
						filteredData.AppendLine (line);

					} else {
						filteredData.AppendLine (_filteredArr [i]);
					}
				} else {
					if (IsLineInFilter (i, filter))
						filteredData.AppendLine (_filteredArr [i]);
				}

			}

			_visibleData = filteredData.ToString ();
			UpdateTextUI ();
			if (!ColorFilter)
				scrollRect.verticalScrollbar.value = 1;
			scrollRect.horizontalScrollbar.value = 0;
		}
	}

	public bool IsLineInFilter (int i, string filter)
	{
		return _filteredArr [i].ToLower ().Contains (filter.ToLower ());
	}

	public void ToggleLogPanel ()
	{
		debugPanel.SetActive (!debugPanel.activeInHierarchy);

		button.SetActive (true);

		if (debugPanel.activeInHierarchy) {
			UpdateTextUI ();	
		}
	}

	#region callbacks

	static void TabTale_Logger_MessageLogged (string message, string stackTrace, LogType logType)
	{
		switch(logType){
			case LogType.Warning:
				SetWarning (message);
				break;
			case LogType.Error:
				int index = message.IndexOf (":Error:");
				if (index != -1)
					message = message.Substring (index);

				SetError (message + ": " + stackTrace);
				break;
			case LogType.Exception:
				SetError (message + ": " + stackTrace);
				break;
		}
			
	}

	#endregion
}