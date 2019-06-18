using UnityEngine;
using System.Collections.Generic;

namespace CocoPlay
{
	public class CocoDressItemModelSet
	{
		public CocoDressItemModelSet (CocoDressItemHolder itemHolder)
		{
			ItemHolder = itemHolder;
		}

		public CocoDressItemHolder ItemHolder { get; private set; }

		public List<SkinnedMeshRenderer> ItemRenderers { get; set; }

		private bool m_IsActive = true;

		public bool IsActive {
			get {
				return m_IsActive;
			}
			set {
				if (m_IsActive == value) {
					return;
				}

				m_IsActive = value;

				if (ItemRenderers != null) {
					ItemRenderers.ForEach (smr => smr.gameObject.SetActive (m_IsActive));
				}
			}
		}
	}
}
