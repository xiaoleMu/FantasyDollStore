using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine.UI;

namespace CocoPlay
{
	public class CocoScoundButton : CocoUINormalButton
	{
		[SerializeField]
		Image m_IconImage;
		[SerializeField]
		Sprite OnSprite;
		[SerializeField]
		Sprite OffSprite;

		protected override void Start ()
		{
			base.Start ();

			if(m_IconImage == null)
				m_IconImage = GetComponent <Image> ();

			//CocoAudio.IsOn = true;
			UpdateSprite ();
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

