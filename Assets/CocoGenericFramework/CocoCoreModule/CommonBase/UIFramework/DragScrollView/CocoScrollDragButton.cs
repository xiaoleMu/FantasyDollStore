using UnityEngine;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CocoPlay{
	public enum DragMoveType
	{
		Both = 0,
		Horizontal = 1,
		Vertical = 2,
		None = 3,
	}

	public class CocoScrollDragButton : CocoUIDragButton
	{
		[SerializeField]
		protected bool m_EnableMove = true;
		[SerializeField]
		protected DragMoveType m_DefaultDragType = DragMoveType.Both;

		protected DragMoveType CurMoveType;
		protected Vector2 DragStartPos;
		protected CocoScrollControl m_ScrollView;

		protected bool m_EnableSetPos = true;

		public bool AutoBack = false;

		protected Transform m_StartParent;

		protected override void Start ()
		{
			base.Start ();
			if (m_ScrollView == null)
			{
				m_ScrollView = GetComponentInParent <CocoScrollControl> ();
			}
			m_StartParent = m_DragTrans.parent;
		}
			
		protected int startSiblingIndex;
		protected bool Drag_Start_Outside = false;
		
		protected bool m_EnableDrag = true;

		public void SetDragEnable(bool enable)
		{
			m_EnableDrag = enable;
		}

		protected override void OnClick ()
		{
			base.OnClick ();
			m_ScrollView.StopMove();
		}

		protected override void OnCCDragStart (PointerEventData eventData)
		{
			if (!m_EnableDrag)
				return;

			if (m_InDrag)
				return;

			base.OnCCDragStart (eventData);
			m_ScrollView.InDrag = true;
			m_ScrollView.SetGridEnable (false);
			CurMoveType = DragMoveType.None;
			DragStartPos = eventData.position;

			startSiblingIndex = m_DragTrans.GetComponent<RectTransform> ().GetSiblingIndex ();

			m_ScrollView.StopMove ();
			if (!m_EnableMove && m_ScrollView != null)
			{
				m_ScrollView.OnDragStart (eventData);
			}
		}

		protected override void OnCCDragMove (PointerEventData eventData)
		{
			if (!m_InDrag)
				return;

			if (!m_EnableMove && m_ScrollView != null)
			{
				m_ScrollView.OnDrag (eventData);
				return;
			}

			if (CurMoveType == DragMoveType.None)
			{
				CurMoveType = GetMoveType (DragStartPos, eventData.position);
				if (CurMoveType == m_ScrollView.DragType)
				{
					base.OnButtonPress (false);
					if (m_ScrollView != null)
					{
						m_ScrollView.OnDragStart (eventData);
					}
				}
				else if (CurMoveType == m_DefaultDragType)
				{
					m_DragTrans.SetSiblingIndex (1000);
					m_DragTrans.SetParent (m_ScrollView.DragRootObj.transform);
					m_DragTrans.position = CCTool.GetScreenToUIWorldPos (eventData.position);
					OnStartDarg ();
				}
			}
			else
			{
				if (CurMoveType == m_DefaultDragType)
				{
					if (m_EnableSetPos)
					{
						m_DragTrans.position = CCTool.GetScreenToUIWorldPos (eventData.position);
					}
				}
				else
				{
					m_DragTrans.SetSiblingIndex (startSiblingIndex);
					if (m_ScrollView != null)
						m_ScrollView.OnDrag (eventData);
				}
			}
		}

		protected virtual void OnStartDarg()
		{
			
		}

		protected override void OnCCDragEnd (PointerEventData eventData)
		{
			if (!m_InDrag) return;
			m_DragTrans.SetSiblingIndex (startSiblingIndex);
			if (!m_EnableMove && m_ScrollView != null)
			{
				m_ScrollView.OnDragEnd (eventData);
			}
			else if (CurMoveType == m_DefaultDragType || CurMoveType == DragMoveType.Both)
			{
				if (AutoBack || EnableBack () || m_EnableSetPos == false)
				{
					if (!m_ScrollView.Enable_Insert_Action)
					{
						StartCoroutine (ScrollMoveBack (eventData));
					}
					else
					{
						m_ScrollView.Insert (this.gameObject, startSiblingIndex);
						m_ScrollView.UpdateGrid ();
					}
				}
				else
				{
					m_ScrollView.UpdateGrid ();
				}
			}
			else if (m_ScrollView != null)
			{
				m_ScrollView.OnDragEnd (eventData);

			}
			InitStatus ();
			m_ScrollView.InDrag = false;
		}

		protected bool EnableBack ()
		{
			if (!m_ScrollView.Enable_Stay_Outside)
				return true;
			return IsOutFrame ();
		}

		protected bool IsOutFrame ()
		{
			RaycastHit hit = CCCollider.GetColliderHit (CocoMainController.UICamera, m_ScrollView.Collider, Input.mousePosition);
			return hit.collider != null;
		}

		static float MinOffset = 2.0f;

	    [SerializeField]
		float MaxAngel = 45.0f;

		protected DragMoveType GetMoveType (Vector2 start, Vector2 end)
		{
			if (m_DefaultDragType == DragMoveType.Both)
				return DragMoveType.Both;

			Vector2 dis = end - start;
			if (Vector2.SqrMagnitude (dis) > MinOffset)
			{
				float pAngel = Mathf.Atan2 (Mathf.Abs (dis.y), Mathf.Abs (dis.x)) * Mathf.Rad2Deg;
				DragMoveType dragType = (pAngel > MaxAngel) ? DragMoveType.Vertical : DragMoveType.Horizontal;
				return dragType;
			}
			else
			{
				return DragMoveType.None;
			}
		}

		public void InitStatus ()
		{
			m_InDrag = false;
			CurMoveType = DragMoveType.Both;
		}

		protected IEnumerator ScrollMoveBack (PointerEventData eventData)
		{
			m_DragTrans.SetParent (m_StartParent);
			m_DragTrans.SetSiblingIndex (startSiblingIndex);
			yield return StartCoroutine (IMoveBack ());
		}

		protected override void OnMoveBackFinish()
		{
			base.OnMoveBackFinish();
			m_ScrollView.SetGridEnable(true);
		}
	}
}
