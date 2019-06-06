using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using TabTale;
using UnityEngine.UI;

#if COCO_FAKE
using CocoUIButtonID = CocoPlay.Fake.CocoUIButtonID;

#else
using CocoUIButtonID = Game.CocoUIButtonID;
#endif

namespace CocoPlay
{

	public class CocoUINormalButton : GameView, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		// id
		[SerializeField]
		CocoUIButtonIDProperty buttonId;

		public bool UseButtonID {
			get {
				return buttonId.FirstUsed;
			}
			set {
				if (buttonId == null) {
					buttonId = new CocoUIButtonIDProperty ();
				}
			}
		}

		public CocoUIButtonID ButtonID {
			get {
				return buttonId.First;
			}
			set {
				if (buttonId == null) {
					buttonId = new CocoUIButtonIDProperty (value);
				} else {
					buttonId.FirstUsed = true;
					buttonId.First = value;
				}
			}
		}

		public string ButtonKey {
			get {
				return buttonId.Second;
			}
			set {
				if (buttonId == null) {
					buttonId = new CocoUIButtonIDProperty (value);
				} else {
					buttonId.FirstUsed = false;
					buttonId.Second = value;
				}
			}
		}

		#region Init/clean

		protected override void OnRegister ()
		{
			base.OnRegister ();

			m_OriginalScale = transform.localScale;
			if (m_MainImage == null) {
				m_MainImage = GetComponentInChildren<Image> (true);
			}
			m_OriginalSprite = m_MainImage != null ? m_MainImage.sprite : null;
		}

		protected override void AddListeners ()
		{
			base.AddListeners ();
			uiButtonTriggerClickSignal.AddListener (OnTriggerClick);
		}

		protected override void RemoveListeners ()
		{
			uiButtonTriggerClickSignal.RemoveListener (OnTriggerClick);
			base.RemoveListeners ();
		}

		#endregion


		#region Touch Enable

		protected virtual bool IsTouchEnabled {
			get {
				return CocoMainController.Instance.TouchEnable;
			}
		}

		#endregion


		#region Press

		// press
		[Header ("Press")]
		public CocoOptionalFloatProperty pressScale = new CocoOptionalFloatProperty (1.1f);
		protected Vector3 m_OriginalScale;
		public CocoOptionalSpriteProperty pressSprite = new CocoOptionalSpriteProperty();
		[SerializeField]
		Image m_MainImage;
		Sprite m_OriginalSprite;


		protected Image MainImage {
			get {
				return m_MainImage;
			}
		}

		public Camera uiCamera {
			get {
				return m_MainImage.canvas.worldCamera;
			}
		}

		protected int m_UniqueId = int.MaxValue;
		protected virtual void OnButtonPress (bool press)
		{
			if (!IsTouchEnabled && press)
				return;

		    if (audioPlayType == AudioPlayType.PressDown && press)
		    {
		        PlayTapSound();
		    }
			

			if (pressScale.Used) {
				Vector3 targetScale = press ? m_OriginalScale * pressScale.Value : m_OriginalScale;
				if (m_UniqueId != int.MaxValue) LeanTween.cancel (gameObject, m_UniqueId);
				m_UniqueId = LeanTween.scale (gameObject, targetScale, 0.15f).setIgnoreTimeScale(true).uniqueId;
			}

			if (pressSprite.Used) {
				SwitchSpriteOnPress (press);
			}
		}

		protected virtual void SwitchSpriteOnPress (bool press)
		{
			if (m_MainImage == null) {
				return;
			}

			m_MainImage.sprite = press ? pressSprite.Value : m_OriginalSprite;
		}

		#endregion


		#region Click

	    protected enum AudioPlayType
	    {
	        PressDown,
	        Click
	    }
		// click
		[Header ("Click")]
		public bool handleClick = true;
		public CocoUIButtonAudioProperty clickAudio = new CocoUIButtonAudioProperty();
	    [SerializeField] protected AudioPlayType audioPlayType;
		[Inject]
		public CocoUIButtonClickSignal uiButtonClickSignal { get; set; }

		[Inject]
		public CocoUIButtonTriggerClickSignal uiButtonTriggerClickSignal { get; set; }

	    public event Action<CocoUINormalButton> OnClickEvent;

		protected void OnButtonClick ()
		{
			if (!IsTouchEnabled)
				return;

			if (!handleClick) {
				return;
			}

			OnClick ();
		}

		void OnTriggerClick (CocoUIButtonIDProperty id)
		{
			if (!handleClick) {
				return;
			}

			if (buttonId.ValueIsEquals (id)) {
				OnClick ();
			}
		}

		protected virtual void OnClick ()
		{
		    if (audioPlayType == AudioPlayType.Click)
		    {
		        PlayTapSound();
		    }
		    
			uiButtonClickSignal.Dispatch (this);
		    if (OnClickEvent != null)
		        OnClickEvent(this);
		}

	    public void PlayTapSound()
	    {
	        if (clickAudio.FirstUsed) {
	            CocoAudio.PlaySound (clickAudio.First);
	        } else {
	            CocoAudio.PlaySound (clickAudio.Second);
	        }
	    }

		#endregion


		#region IPointer Down/Up/Click Handler implementation

		void IPointerDownHandler.OnPointerDown (PointerEventData eventData)
		{
			OnButtonPress (true);
		}

		void IPointerUpHandler.OnPointerUp (PointerEventData eventData)
		{
			OnButtonPress (false);
		}

		void IPointerClickHandler.OnPointerClick (PointerEventData eventData)
		{
			OnButtonClick ();
		}

		#endregion
	}
}
