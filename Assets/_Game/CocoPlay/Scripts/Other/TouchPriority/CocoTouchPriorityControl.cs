using UnityEngine;
using System.Collections.Generic;
using TabTale;
using UnityEngine.EventSystems;

namespace CocoPlay
{
	public class CocoTouchPriorityControl : GameView
	{
		#region Init/Clean

		protected override void RemoveListeners ()
		{
			_gestureConfigDataDic.Keys.ForEach (UnListenGesture);
			base.RemoveListeners ();
		}

		#endregion


		#region Gesture Data

		private CocoTouchPriorityGestureConfigData<T> GetOrAddGestureConfigData<T> (CocoGestureType gestureType) where T : Gesture, new ()
		{
			CocoTouchPriorityGestureConfigData<T> configData;
			if (_gestureConfigDataDic.ContainsKey (gestureType)) {
				configData = _gestureConfigDataDic [gestureType] as CocoTouchPriorityGestureConfigData<T>;
				if (configData == null) {
					Debug.LogErrorFormat ("{0}->GetOrAddGestureConfigData: can NOT convert gesture [{1}] to type [{2}].", GetType ().Name, gestureType,
						typeof(CocoTouchPriorityGestureConfigData<T>).Name);
				}
			} else {
				ListenGesture (gestureType);
				configData = new CocoTouchPriorityGestureConfigData<T> (gestureType);
				_gestureConfigDataDic.Add (gestureType, configData);
			}

			return configData;
		}

		private CocoTouchPriorityFingerEventConfigData<T> GetOrAddFingerEventConfigData<T> (CocoGestureType gestureType) where T : FingerEvent, new ()
		{
			CocoTouchPriorityFingerEventConfigData<T> configData;
			if (_gestureConfigDataDic.ContainsKey (gestureType)) {
				configData = _gestureConfigDataDic [gestureType] as CocoTouchPriorityFingerEventConfigData<T>;
				if (configData == null) {
					Debug.LogErrorFormat ("{0}->GetOrAddGestureConfigData: can NOT convert gesture [{1}] to type [{2}].", GetType ().Name, gestureType,
						typeof(CocoTouchPriorityFingerEventConfigData<T>).Name);
				}
			} else {
				ListenGesture (gestureType);
				configData = new CocoTouchPriorityFingerEventConfigData<T> (gestureType);
				_gestureConfigDataDic.Add (gestureType, configData);
			}

			return configData;
		}

		private bool ProcessGestureData (CocoGestureType gestureType, object gesture)
		{
			if (!IsGestureProcessing (gestureType)) {
				return false;
			}

			CocoTouchPriorityConfigData data = _gestureConfigDataDic.GetValue (gestureType);
			if (data == null) {
				return false;
			}

			data.ProcessGesture (gesture);
			return true;
		}

		private void CancelGestureDataProcess (CocoGestureType gestureType)
		{
			if (IsGestureProcessing (gestureType)) {
				//Debug.LogErrorFormat ("{0}: {1}->CancelGestureDataProcess: {2} cancelled", Time.time, GetType ().Name, gestureType);
				EndGestureProcess (gestureType);
				CocoTouchPriorityConfigData data = _gestureConfigDataDic.GetValue (gestureType);
				if (data != null) {
					data.CancelGestureProcess ();
				}
			}

			CancelLowPriorityGestureProcess (gestureType);
		}

//		public void PrintData ()
//		{
//			_gestureConfigDataDic.ForEach (data => {
//				string strLog = string.Format ("{0}: high [{1}]", data.GestureType, data.HighPriorityGestureTypes);
//				if (data.LowPriorityGestureTypes != null) {
//					strLog = string.Format ("{0}, low [{1}]: ", strLog, data.LowPriorityGestureTypes.Length);
//					data.LowPriorityGestureTypes.ForEach (gestureType => strLog += gestureType.ToString () + ", ");
//				}
//				Debug.LogError (strLog);
//			});
//		}

		#endregion


		#region Gesture Enable/Disable

		private readonly Dictionary<CocoGestureType, CocoTouchPriorityConfigData> _gestureConfigDataDic =
			new Dictionary<CocoGestureType, CocoTouchPriorityConfigData> ();

		public void EnableGesture<T> (CocoGestureType gestureType, System.Action<T> onGestureAction) where T : Gesture, new ()
		{
			if (onGestureAction == null) {
				return;
			}

			CocoTouchPriorityGestureConfigData<T> configData = GetOrAddGestureConfigData<T> (gestureType);
			if (configData != null) {
				configData.OnGesture += onGestureAction;
			}
		}

		public void EnabledFingerEvent<T> (CocoGestureType gestureType, System.Action<T> onGestureAction)
			where T : FingerEvent, new ()
		{
			if (onGestureAction == null) {
				return;
			}

			CocoTouchPriorityFingerEventConfigData<T> configData = GetOrAddFingerEventConfigData<T> (gestureType);
			if (configData != null) {
				configData.OnGesture += onGestureAction;
			}
		}

		public void DisableGesture<T> (CocoGestureType gestureType, System.Action<T> onGestureAction) where T : Gesture, new ()
		{
			if (onGestureAction == null) {
				return;
			}

			if (!_gestureConfigDataDic.ContainsKey (gestureType)) {
				return;
			}

			CocoTouchPriorityGestureConfigData<T> configData =
				_gestureConfigDataDic [gestureType] as CocoTouchPriorityGestureConfigData<T>;
			if (configData == null) {
				return;
			}

			configData.OnGesture -= onGestureAction;
			if (configData.OnGesture == null) {
				UnListenGesture (gestureType);
				_gestureConfigDataDic.Remove (gestureType);
			}
		}

		public void DisableFingerEvent<T> (CocoGestureType gestureType, System.Action<T> onGestureAction)
			where T : FingerEvent, new ()
		{
			if (onGestureAction == null) {
				return;
			}

			if (!_gestureConfigDataDic.ContainsKey (gestureType)) {
				return;
			}

			CocoTouchPriorityFingerEventConfigData<T> configData =
				_gestureConfigDataDic [gestureType] as CocoTouchPriorityFingerEventConfigData<T>;
			if (configData == null) {
				return;
			}

			configData.OnGesture -= onGestureAction;
			if (configData.OnGesture == null) {
				UnListenGesture (gestureType);
				_gestureConfigDataDic.Remove (gestureType);
			}
		}

		#endregion


		#region Gesture Priority

		public void SetGesturePriorities (CocoGestureType gestureType, CocoGestureType[] lowPriorityGestureTypes)
		{
			CocoTouchPriorityConfigData data = _gestureConfigDataDic.GetValue (gestureType);
			if (data == null) {
				return;
			}

			data.LowPriorityGestureTypes = lowPriorityGestureTypes;

			lowPriorityGestureTypes.ForEach (lowPriorityGestureType => {
				CocoTouchPriorityConfigData lowPriorityPriorityData = _gestureConfigDataDic.GetValue (lowPriorityGestureType);
				if (lowPriorityPriorityData == null) {
					return;
				}

				lowPriorityPriorityData.HighPriorityGestureTypes |= gestureType;
			});
		}

		private bool IsHighPriorityGestureActiving (CocoGestureType gestureType)
		{
			CocoTouchPriorityConfigData data = _gestureConfigDataDic.GetValue (gestureType);
			if (data == null) {
				return false;
			}

			return (data.HighPriorityGestureTypes & _activeGestureTypes) != 0;
		}

		private void CancelLowPriorityGestureProcess (CocoGestureType gestureType)
		{
			CocoTouchPriorityConfigData data = _gestureConfigDataDic.GetValue (gestureType);
			if (data == null) {
				return;
			}

			if (data.LowPriorityGestureTypes != null) {
				data.LowPriorityGestureTypes.ForEach (CancelGestureDataProcess);
			}
		}

		#endregion


		#region Gesture Process

		private CocoGestureType _processingGestureTypes = CocoGestureType.None;
		private CocoGestureType _activeGestureTypes = CocoGestureType.None;

		private bool IsGestureProcessing (CocoGestureType gestureType)
		{
			return (_processingGestureTypes & gestureType) != 0;
		}

		private bool StartGestureProcess (CocoGestureType gestureType)
		{
			if (IsHighPriorityGestureActiving (gestureType)) {
				return false;
			}

			//Debug.LogErrorFormat ("{0}: {1}->StartGestureProcess: {2} started", Time.time, GetType ().Name, gestureType);
			_processingGestureTypes |= gestureType;
			CancelLowPriorityGestureProcess (gestureType);
			return true;
		}

		private void EndGestureProcess (CocoGestureType gestureType)
		{
			//Debug.LogErrorFormat ("{0}: {1}->EndGestureProcess: {2} ended", Time.time, GetType ().Name, gestureType);
			_processingGestureTypes &= ~gestureType;
		}

		private void StartGestureActiving (CocoGestureType gestureType)
		{
			//Debug.LogErrorFormat ("{0}: {1}->StartGestureActiving: {2} actived", Time.time, GetType ().Name, gestureType);
			_activeGestureTypes |= gestureType;
		}

		private void EndGestureActiving (CocoGestureType gestureType)
		{
			//Debug.LogErrorFormat ("{0}: {1}->StartGestureActiving: {2} end actived", Time.time, GetType ().Name, gestureType);
			_activeGestureTypes &= ~gestureType;
		}

		#endregion


		#region Gesture Listen

		[Inject]
		public TapGestureSignal TapGestureSignal { get; set; }

		[Inject]
		public DragGestureSignal DragGestureSignal { get; set; }

		[Inject]
		public SwipeGestureSignal SwipeGestureSignal { get; set; }

		[Inject]
		public PinchGestureSignal PinchGestureSignal { get; set; }

		[Inject]
		public TwoFingerDragGestureSignal TwoFingerDragGestureSignal { get; set; }

		[Inject]
		public TwistGestureSignal TwistGestureSignal { get; set; }

		[Inject]
		public DownEventSignal DownEventSignal { get; set; }

		[Inject]
		public UpEventSignal UpEventSignal { get; set; }

		private void ListenGesture (CocoGestureType gestureType)
		{
			switch (gestureType) {
			case CocoGestureType.Tap:
				TapGestureSignal.AddListener (OnSceneTap);
				break;
			case CocoGestureType.Drag:
				DragGestureSignal.AddListener (OnSceneDrag);
				break;
			case CocoGestureType.Swipe:
				SwipeGestureSignal.AddListener (OnSceneSwipe);
				break;

			case CocoGestureType.Pinch:
				PinchGestureSignal.AddListener (OnScenePinch);
				break;
			case CocoGestureType.TwoFingerDrag:
				TwoFingerDragGestureSignal.AddListener (OnSceneTwoFingerDrag);
				break;
			case CocoGestureType.Twist:
				TwistGestureSignal.AddListener (OnSceneTwist);
				break;

			case CocoGestureType.FingerDown:
				DownEventSignal.AddListener (OnSceneFingerDown);
				break;
			case CocoGestureType.FingerUp:
				UpEventSignal.AddListener (OnSceneFingerUp);
				break;

			default:
				Debug.LogErrorFormat ("{0}->ListenGesture: gesture [{1}] NOT be supported now.", GetType ().Name, gestureType);
				break;
			}

			RegisterMultiTouchGesture (gestureType);
		}

		private void UnListenGesture (CocoGestureType gestureType)
		{
			switch (gestureType) {
			case CocoGestureType.Tap:
				TapGestureSignal.RemoveListener (OnSceneTap);
				break;
			case CocoGestureType.Drag:
				DragGestureSignal.RemoveListener (OnSceneDrag);
				break;
			case CocoGestureType.Swipe:
				SwipeGestureSignal.RemoveListener (OnSceneSwipe);
				break;

			case CocoGestureType.Pinch:
				PinchGestureSignal.RemoveListener (OnScenePinch);
				break;
			case CocoGestureType.TwoFingerDrag:
				TwoFingerDragGestureSignal.RemoveListener (OnSceneTwoFingerDrag);
				break;
			case CocoGestureType.Twist:
				TwistGestureSignal.RemoveListener (OnSceneTwist);
				break;

			case CocoGestureType.FingerDown:
				DownEventSignal.RemoveListener (OnSceneFingerDown);
				break;
			case CocoGestureType.FingerUp:
				UpEventSignal.RemoveListener (OnSceneFingerUp);
				break;

			default:
				Debug.LogErrorFormat ("{0}->UnListenGesture: gesture [{1}] NOT be supported now.", GetType ().Name, gestureType);
				break;
			}

			UnRegisterMultiTouchGesture (gestureType);
		}

		private void OnSceneTap (TapGesture tapGesture)
		{
			OnGesture (CocoGestureType.Tap, tapGesture);
		}

		private void OnSceneDrag (DragGesture dragGesture)
		{
			OnGesture (CocoGestureType.Drag, dragGesture);
		}

		private void OnSceneSwipe (SwipeGesture swipeGesture)
		{
			OnGesture (CocoGestureType.Swipe, swipeGesture);
		}

		private void OnScenePinch (PinchGesture pinchGesture)
		{
			OnGesture (CocoGestureType.Pinch, pinchGesture);
		}

		private void OnSceneTwoFingerDrag (DragGesture dragGesture)
		{
			OnGesture (CocoGestureType.TwoFingerDrag, dragGesture);
		}

		private void OnSceneTwist (TwistGesture twistGesture)
		{
			OnGesture (CocoGestureType.Twist, twistGesture);
		}

		private void OnSceneFingerDown (FingerDownEvent fingerDownEvent)
		{
			OnFingerEvent (CocoGestureType.FingerDown, fingerDownEvent);
		}

		private void OnSceneFingerUp (FingerUpEvent fingerUpEvent)
		{
			OnFingerEvent (CocoGestureType.FingerUp, fingerUpEvent);
		}

		private void OnGesture (CocoGestureType gestureType, ContinuousGesture gesture)
		{
			switch (gesture.Phase) {
			case ContinuousGesturePhase.Started:
				StartGestureActiving (gestureType);
				if (ShouldIgnoreGesture (gestureType, gesture.StartPosition)) {
					break;
				}
				if (!StartGestureProcess (gestureType)) {
					break;
				}
				ProcessGestureData (gestureType, gesture);
				break;
			case ContinuousGesturePhase.Updated:
				ProcessGestureData (gestureType, gesture);
				break;
			case ContinuousGesturePhase.Ended:
				if (ProcessGestureData (gestureType, gesture)) {
					EndGestureProcess (gestureType);
				}
				EndGestureActiving (gestureType);
				break;
			}
		}

		private void OnGesture (CocoGestureType gestureType, DiscreteGesture gesture)
		{
			if (ShouldIgnoreGesture (gestureType, gesture.Position)) {
				return;
			}
			if (!StartGestureProcess (gestureType)) {
				return;
			}

			ProcessGestureData (gestureType, gesture);
			EndGestureProcess (gestureType);
		}

		private void OnFingerEvent (CocoGestureType gestureType, FingerEvent fingerEvent)
		{
			if (ShouldIgnoreGesture (gestureType, fingerEvent.Position)) {
				return;
			}
			if (!StartGestureProcess (gestureType)) {
				return;
			}

			ProcessGestureData (gestureType, fingerEvent);
			EndGestureProcess (gestureType);
		}

		#endregion


		#region Multi Touch

		[SerializeField]
		private bool _isOriginMultiTouchRecorded = true;

		public bool IsOriginMultiTouchRecorded {
			get { return _isOriginMultiTouchRecorded; }
			set { _isOriginMultiTouchRecorded = value; }
		}

		private bool _originMultiTouchEnabled;
		private CocoGestureType _registeredMultiTouchGestureTypeFlags = CocoGestureType.None;

		private void RegisterMultiTouchGesture (CocoGestureType gestureType)
		{
			// already registered
			if ((gestureType & _registeredMultiTouchGestureTypeFlags) != 0) {
				return;
			}

			// not muti-touch gesture
			if ((gestureType & CocoGestureType.All_MultiTouch) == 0) {
				return;
			}

			if (_isOriginMultiTouchRecorded) {
				// first registered, record original setting
				if (_registeredMultiTouchGestureTypeFlags == CocoGestureType.None) {
					_originMultiTouchEnabled = Input.multiTouchEnabled;
					Input.multiTouchEnabled = true;
				}
			}

			_registeredMultiTouchGestureTypeFlags |= gestureType;
		}

		private void UnRegisterMultiTouchGesture (CocoGestureType gestureType)
		{
			// not registered
			if ((gestureType & _registeredMultiTouchGestureTypeFlags) == 0) {
				return;
			}

			_registeredMultiTouchGestureTypeFlags &= ~gestureType;

			if (_isOriginMultiTouchRecorded) {
				// all unregistered, resume original setting
				if (_registeredMultiTouchGestureTypeFlags == CocoGestureType.None) {
					Input.multiTouchEnabled = _originMultiTouchEnabled;
				}
			}
		}

		#endregion


		#region Ignore If Over UI

		private readonly Dictionary<CocoGestureType, LayerMask> _ignoreTouchUILayerMasks = new Dictionary<CocoGestureType, LayerMask> ();

		public void SetIgnoreTouchUILayers (CocoGestureType gestureType, LayerMask uiLayerMask)
		{
			if (_ignoreTouchUILayerMasks.ContainsKey (gestureType)) {
				_ignoreTouchUILayerMasks [gestureType] = uiLayerMask;
				return;
			}

			_ignoreTouchUILayerMasks.Add (gestureType, uiLayerMask);
		}

		private bool ShouldIgnoreGesture (CocoGestureType gestureType, Vector2 screenPosition)
		{
			LayerMask layerMask = _ignoreTouchUILayerMasks.ContainsKey (gestureType) ? _ignoreTouchUILayerMasks [gestureType] : (LayerMask) Physics.DefaultRaycastLayers;
			return PointOverGui (screenPosition, layerMask);
		}

		#endregion


		#region Over GUI By Lean Touch

		private static List<RaycastResult> _tempRaycastResults = new List<RaycastResult> (10);

		private static PointerEventData _tempPointerEventData;

		private static EventSystem _tempEventSystem;

		public static bool PointOverGui (Vector2 screenPosition)
		{
			LayerMask layerMask = Physics.DefaultRaycastLayers;
			return PointOverGui (screenPosition, layerMask);
		}

		public static bool PointOverGui (Vector2 screenPosition, LayerMask layerMask)
		{
			return RaycastGui (screenPosition, layerMask).Count > 0;
		}

		public static List<RaycastResult> RaycastGui (Vector2 screenPosition)
		{
			LayerMask layerMask = Physics.DefaultRaycastLayers;
			return RaycastGui (screenPosition, layerMask);
		}

		// This will return all the RaycastResults under the 'screenPosition' using the specified layerMask
		// The first result (0) should be the top most UI element
		public static List<RaycastResult> RaycastGui (Vector2 screenPosition, LayerMask layerMask)
		{
			_tempRaycastResults.Clear ();

			var currentEventSystem = EventSystem.current;

			if (currentEventSystem != null) {
				// Create point event data for this event system?
				if (currentEventSystem != _tempEventSystem) {
					_tempEventSystem = currentEventSystem;

					if (_tempPointerEventData == null) {
						_tempPointerEventData = new PointerEventData (_tempEventSystem);
					} else {
						_tempPointerEventData.Reset ();
					}
				}

				// Raycast event system at the specified point
				_tempPointerEventData.position = screenPosition;

				currentEventSystem.RaycastAll (_tempPointerEventData, _tempRaycastResults);

				// Loop through all results and remove any that don't match the layer mask
				if (_tempRaycastResults.Count > 0) {
					for (var i = _tempRaycastResults.Count - 1; i >= 0; i--) {
						var raycastResult = _tempRaycastResults [i];
						var raycastLayer = 1 << raycastResult.gameObject.layer;

						if ((raycastLayer & layerMask) == 0) {
							_tempRaycastResults.RemoveAt (i);
						}
					}
				}
			}

			return _tempRaycastResults;
		}

		#endregion
	}
}