using System;
using System.Collections;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace CocoPlay.VCSInfo
{
	public class CocoVCSInfoDisplayer : MonoBehaviour
	{
		#region Unity Event Function

		[SerializeField]
		private bool _showOnStart;

		private void Start ()
		{
			InitDetect ();

			if (_showOnStart) {
				DisplayVCSInfo (true);
			}
		}

		private void Update ()
		{
			DetectTouch ();
		}

		private void OnGUI ()
		{
			DisplayVCSInfoOnGUI ();
		}

		#endregion


		#region Info Load

		private enum LoadStatus
		{
			None,
			Loading,
			Loaded,
			LoadFailed
		}

		private const string INFO_FILE_NAME = "coco_vcsinfo.json";
		public const string INFO_TIME_FORMAT = "dd-MM-yyyy HH:mm";

		public static string InfoFilePath {
			get { return Path.Combine (Application.streamingAssetsPath, INFO_FILE_NAME); }
		}

		private LoadStatus _loadStatus = LoadStatus.None;
		private CocoVCSInfo _vcsInfo;

		private IEnumerator LoadVCSInfoAsync ()
		{
			string path = LocalPathProtocolHeader + InfoFilePath;
			using (WWW www = new WWW (path)) {
				_loadStatus = LoadStatus.Loading;

				yield return www;
				if (!string.IsNullOrEmpty (www.error)) {
					Debug.LogWarningFormat ("{0}->LoadVCSInfoAsync: load vcs info FAILED! [{1}]", GetType ().Name, www.error);
					yield break;
				}

				_vcsInfo = JsonUtility.FromJson<CocoVCSInfo> (www.text);
				_loadStatus = _vcsInfo != null && !string.IsNullOrEmpty (_vcsInfo.CommitId) ? LoadStatus.Loaded : LoadStatus.LoadFailed;
			}
		}

		private static string LocalPathProtocolHeader {
			get {
				switch (Application.platform) {
				case RuntimePlatform.Android:
					return string.Empty;
				}

				return "file://";
			}
		}

		#endregion


		#region Display Content

		private bool _displayContentInited;
		private GUIContent _displayContent;
		private bool _shouldDisplay;

		private void InitDisplay ()
		{
			if (_displayContentInited) {
				return;
			}

			InitDisplayContent ();
			InitDisplayStyle ();
			InitDisplayAnchor ();

			_displayContentInited = true;
		}

		private void InitDisplayContent ()
		{
			string loadTimeString = string.Empty;
			try {
				DateTime loadTime = DateTime.ParseExact (_vcsInfo.LoadTime, INFO_TIME_FORMAT, CultureInfo.InvariantCulture);
				loadTimeString = loadTime.ToLocalTime ().ToString (INFO_TIME_FORMAT);
			}
			catch (Exception e) {
				Debug.LogErrorFormat ("{0}->InitDisplayContent: load time parse failed [{1}]", GetType ().Name, e.Message);
			}

			string content = string.Format ("{0} [{1}]", loadTimeString, _vcsInfo.CommitId);
			_displayContent = new GUIContent (content);
		}

		private void DisplayVCSInfoOnGUI ()
		{
			if (!_shouldDisplay || _loadStatus != LoadStatus.Loaded) {
				return;
			}

			InitDisplay ();

			GUI.Box (_displayRect, _displayContent, _displayStyle);
			//GUI.Label (_displayRect, _displayContent, _displayStyle);
		}

		private void DisplayVCSInfo (bool display)
		{
			if (_shouldDisplay == display) {
				return;
			}

			_shouldDisplay = display;

			if (_shouldDisplay) {
				if (_loadStatus == LoadStatus.None) {
					StartCoroutine (LoadVCSInfoAsync ());
				}
			}
		}

		#endregion


		#region Display Style

		[Header ("Display")]
		[SerializeField]
		private Font _displayFont;

		[SerializeField]
		private Color _displayColor = Color.green;

		private GUIStyle _displayStyle;

		private void InitDisplayStyle ()
		{
			_displayStyle = new GUIStyle (GUI.skin.box);

			if (_displayFont != null) {
				_displayStyle.font = _displayFont;
			}
			_displayStyle.fontSize = Math.Max (Mathf.CeilToInt (Screen.height * 0.02f), 16);
			_displayStyle.alignment = TextAnchor.MiddleCenter;

			_displayStyle.normal.textColor = _displayColor;
		}

		#endregion


		#region Display Anchor

		private enum CocoVCSAnchor
		{
			None = CocoVCSAlignY.None | CocoVCSAlignX.None,
			UpperLeft = CocoVCSAlignY.Upper | CocoVCSAlignX.Left,
			UpperCenter = CocoVCSAlignY.Upper | CocoVCSAlignX.Center,
			UpperRight = CocoVCSAlignY.Upper | CocoVCSAlignX.Right,
			MiddleLeft = CocoVCSAlignY.Middle | CocoVCSAlignX.Left,
			MiddleCenter = CocoVCSAlignY.Middle | CocoVCSAlignX.Center,
			MiddleRight = CocoVCSAlignY.Middle | CocoVCSAlignX.Right,
			LowerLeft = CocoVCSAlignY.Lower | CocoVCSAlignX.Left,
			LowerCenter = CocoVCSAlignY.Lower | CocoVCSAlignX.Center,
			LowerRight = CocoVCSAlignY.Lower | CocoVCSAlignX.Right
		}

		private enum CocoVCSAlignX
		{
			None = 0,
			Left = 0x01,
			Center = 0x02,
			Right = 0x04
		}

		private enum CocoVCSAlignY
		{
			None = 0,
			Upper = 0x10,
			Middle = 0x20,
			Lower = 0x40
		}

		[SerializeField]
		private CocoVCSAnchor _displayAnchor = CocoVCSAnchor.LowerRight;

		private Rect _displayRect = new Rect (0, 0, 160, 20);

		private CocoVCSAlignX GetAlignX (CocoVCSAnchor anchor)
		{
			return (CocoVCSAlignX)((int)anchor & 0x0F);
		}

		private CocoVCSAlignY GetAlignY (CocoVCSAnchor anchor)
		{
			return (CocoVCSAlignY)((int)anchor & 0xF0);
		}

		private CocoVCSAnchor GetAnchor (CocoVCSAlignX alignX, CocoVCSAlignY alignY)
		{
			return (CocoVCSAnchor)((int)alignX | (int)alignY);
		}

		private void InitDisplayAnchor ()
		{
			_displayRect.size = _displayStyle.CalcSize (_displayContent) + new Vector2 (4, 0);

			CocoVCSAlignX alignX = GetAlignX (_displayAnchor);
			switch (alignX) {
			case CocoVCSAlignX.Center:
				_displayRect.x = (Screen.width - _displayRect.width) / 2;
				break;
			case CocoVCSAlignX.Right:
				_displayRect.x = Screen.width - _displayRect.width;
				break;
			default:
				_displayRect.x = 0;
				break;
			}

			CocoVCSAlignY alignY = GetAlignY (_displayAnchor);
			switch (alignY) {
			case CocoVCSAlignY.Middle:
				_displayRect.y = (Screen.height - _displayRect.height) / 2;
				break;
			case CocoVCSAlignY.Lower:
				_displayRect.y = Screen.height - _displayRect.height;
				break;
			default:
				_displayRect.y = 0;
				break;
			}
		}

		#endregion


		#region Detect Touch

		private void InitDetect ()
		{
			InitDetectPoint ();
			InitDetectMatch ();
		}

		private void DetectTouch ()
		{
			if (Application.isMobilePlatform) {
				if (Input.touchCount > 0) {
					Touch touch = Input.GetTouch (0);
					switch (touch.phase) {
					case TouchPhase.Began:
						TouchPosition (touch.position);
						break;
					}
				}
				return;
			}

			if (Input.GetMouseButtonDown (0)) {
				TouchPosition (Input.mousePosition);
			}
		}

		private void TouchPosition (Vector2 pos)
		{
			// timeout checking
			if (_matchedCount > 0) {
				if (Time.time - _lastMatchTime > _detectTimeout) {
					ResetMatchQueue ();
				}
			}

			CocoVCSAnchor point = GetTouchPoint (pos);
			if (point == CocoVCSAnchor.None) {
				ResetMatchQueue ();
			}

			if (AddPointInMatchQueue (point)) {
				//Debug.LogWarning (_matchQueueStartIndex + " - " + _matchedCount);
				// match finish
				if (_matchedCount >= _detectTotalCount) {
					ResetMatchQueue ();
					DisplayVCSInfo (!_shouldDisplay);
				}
				return;
			}

			// refresh queue from next start index
			int startIndex = (_matchQueueStartIndex + 1) % _detectTotalCount;
			int endIndex = (startIndex + _matchedCount) % _detectTotalCount;
			RefreshMatchQueue (startIndex, endIndex);
		}

		private void RefreshMatchQueue (int startIndex, int endIndex)
		{
			// queue is empty
			if (startIndex == endIndex) {
				ResetMatchQueue ();
				return;
			}

			// check match
			_matchQueueStartIndex = startIndex;
			_matchedCount = 0;
			for (int matchIndex = startIndex, detectIndex = 0;
				matchIndex != endIndex;
				matchIndex = (matchIndex + 1) % _detectTotalCount, detectIndex++) {
				if (_matchQueue [matchIndex] != _detectSequence [detectIndex]) {
					_matchedCount = 0;
					break;
				}

				_matchedCount++;
			}

			// have matched point
			if (_matchedCount > 0) {
				return;
			}

			// refresh queue from next start index
			startIndex = (startIndex + 1) % _detectTotalCount;
			RefreshMatchQueue (startIndex, endIndex);
		}

		#endregion


		#region Detect Point

		[Header ("Detect")]
		[SerializeField]
		private CocoVCSAnchor[] _detectSequence = { CocoVCSAnchor.UpperCenter, CocoVCSAnchor.LowerCenter, CocoVCSAnchor.MiddleLeft, CocoVCSAnchor.MiddleRight };

		[SerializeField]
		private Vector2 _detectBounds = new Vector2 (0.15f, 0.15f);

		[SerializeField]
		private float _detectTimeout = 2;

		private int _detectTotalCount;

		private void InitDetectPoint ()
		{
			_detectTotalCount = _detectSequence.Length;
		}

		private CocoVCSAnchor GetTouchPoint (Vector2 pos)
		{
			// x
			CocoVCSAlignX alignX = CocoVCSAlignX.None;

			pos.x /= Screen.width;
			if (pos.x <= _detectBounds.x) {
				alignX = CocoVCSAlignX.Left;
			} else if (pos.x >= 1 - _detectBounds.x) {
				alignX = CocoVCSAlignX.Right;
			} else if (Mathf.Abs (pos.x - 0.5f) <= _detectBounds.x / 2) {
				alignX = CocoVCSAlignX.Center;
			}

			// y
			CocoVCSAlignY alignY = CocoVCSAlignY.None;
			pos.y /= Screen.height;
			if (pos.y >= 1 - _detectBounds.y) {
				alignY = CocoVCSAlignY.Upper;
			} else if (pos.y <= _detectBounds.y) {
				alignY = CocoVCSAlignY.Lower;
			} else if (Mathf.Abs (pos.y - 0.5f) < _detectBounds.y / 2) {
				alignY = CocoVCSAlignY.Middle;
			}

			CocoVCSAnchor anchor = GetAnchor (alignX, alignY);
			//Debug.Log ("GetTouchPoint: " + pos + " -> " + anchor);
			return anchor;
		}

		#endregion


		#region Detect Match

		private CocoVCSAnchor[] _matchQueue;
		private int _matchQueueStartIndex;
		private int _matchedCount;
		private float _lastMatchTime;

		private void InitDetectMatch ()
		{
			_matchQueue = new CocoVCSAnchor[_detectTotalCount];
			ResetMatchQueue ();
		}

		private void ResetMatchQueue ()
		{
			_matchQueueStartIndex = 0;
			_matchedCount = 0;
		}

		private bool AddPointInMatchQueue (CocoVCSAnchor point)
		{
			int index = (_matchQueueStartIndex + _matchedCount) % _detectTotalCount;
			_matchQueue [index] = point;

			if (point != _detectSequence [_matchedCount]) {
				return false;
			}

			_matchedCount++;
			_lastMatchTime = Time.time;
			return true;
		}

		#endregion


		#region Detect Sequence

		#endregion
	}

	[Serializable]
	public class CocoVCSInfo
	{
		public string CommitId = string.Empty;
		public string LoadTime = string.Empty;
	}
}