using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CocoPlay;
using TabTale;

namespace Game{
	public class RoleClickButton : CocoUINormalButton {
		[SerializeField]
		int m_Index;
		[SerializeField]
		Image m_IconImg;

		public int Index {
			get {
				return m_Index;
			}
		}

		public void ChangeStatus (bool _select){
			if (_select) {
				m_IconImg.SetSprite ("doll_bg", true);
			}
			else {
				m_IconImg.SetSprite ("doll_bg01", true);
			}
		}
	}
}
