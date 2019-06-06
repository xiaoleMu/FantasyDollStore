using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace CocoPlay
{
	/// <summary>
	/// Selectable Button
	/// 可选择的按钮，有选中和没有选中两种状态
	/// </summary>
	public class CocoUISelectableButton : CocoUINormalButton
	{

		#region Init/Clean

		protected override void OnRegister ()
		{
			base.OnRegister ();

			if (m_SelectGo != null && m_SelectImage == null) {
				m_SelectImage = GetComponentInChildren<Image> (true);
			}
			if (m_SelectImage != null) {
				m_OriginSelectSprite = m_SelectImage.sprite;
			}
			
			IsSelected = m_IsSelected;
		}

		#endregion


		#region Select

		[Header ("Select")]
		[SerializeField]
		bool m_IsSelected = false;
		[SerializeField]
		bool m_HideMainOnSelected = true;

		[SerializeField]
		GameObject m_SelectGo = null;
		public CocoOptionalSpriteProperty selectPressSprite;
		[SerializeField]
		Image m_SelectImage;
		Sprite m_OriginSelectSprite;


		public bool IsSelected {
			get {
				return m_IsSelected;
			}
			set {
				m_IsSelected = value;
				if (m_SelectGo != null) {
					m_SelectGo.SetActive (m_IsSelected);
				}
				if (m_HideMainOnSelected) {
					if (MainImage.transform == transform) {
						MainImage.enabled = !m_IsSelected;
					} else {
						MainImage.gameObject.SetActive (!m_IsSelected);
					}
				}
			}
		}

		protected override void SwitchSpriteOnPress (bool press)
		{
			if (IsSelected) {
				if (selectPressSprite.Used) {
					m_SelectImage.sprite = press ? selectPressSprite.Value : m_OriginSelectSprite;
				}
				return;
			}

			base.SwitchSpriteOnPress (press);
		}

		#endregion

	}
}
