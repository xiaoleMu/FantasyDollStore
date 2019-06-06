using UnityEngine;
using UnityEngine.UI;

namespace CocoPlay
{
	public class GameSoundButton : CocoUINormalButton
	{
		[Inject]
		public CocoSceneLoadingFinishSignal loadFinishedSignal { get; set; }
		
		[SerializeField]
		Image m_IconImage;
		[SerializeField]
		Sprite OnSprite;
		[SerializeField]
		Sprite OffSprite;

		protected override void AddListeners()
		{
			base.AddListeners();
			loadFinishedSignal.AddListener(OnFinished);
		}

		protected override void RemoveListeners()
		{
			base.RemoveListeners();
			loadFinishedSignal.RemoveListener(OnFinished);
		}



		protected override void Start ()
		{
			base.Start ();

			Init();
		}

		void Init()
		{
			if(m_IconImage == null)
				m_IconImage = GetComponent <Image> ();

			//CocoAudio.IsOn = true;
			UpdateSprite ();
		}

		private void OnFinished()
		{
			Init();
		}

		protected override void OnClick ()
		{
			CocoAudio.IsOn = !CocoAudio.IsOn;
			UpdateSprite ();
		}

		private void UpdateSprite (){
			m_IconImage.sprite = CocoAudio.IsOn ? OnSprite : OffSprite;
			m_IconImage.SetNativeSize ();
		}
	}
}

