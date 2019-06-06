using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TabTale;

namespace CocoPlay {
	public class CocoScrollControl : MonoBehaviour
	{
		ScrollRect m_ScrollRect;
		GridLayoutGroup m_GridGroup;

		[SerializeField]
		List<GameObject> m_MoveObj;

		[SerializeField]
		GameObject m_DragRootObj;

		public bool Enable_Stay_Outside = true;
		public bool Enable_Insert_Action = true;

		public GameObject DragRootObj
		{
			get{
				return m_DragRootObj;
			}
		}
		public GridLayoutGroup GridGroup{ get{return m_GridGroup;} }
		public ScrollRect ScrollRect{ get{return m_ScrollRect;} }

		[HideInInspector]
		public bool InDrag = false;

		Collider m_collider;
		public Collider Collider{get{return m_collider;}}

		public DragMoveType DragType
		{
			get
			{
				if(m_ScrollRect.vertical && m_ScrollRect.horizontal)
					return DragMoveType.Both;
				else if(m_ScrollRect.vertical)
					return DragMoveType.Vertical;
				else
					return DragMoveType.Horizontal;
			}
		}

		void Awake()
		{
			m_ScrollRect = GetComponent<ScrollRect> ();
			m_GridGroup = GetComponentInChildren<GridLayoutGroup>();
			if(m_DragRootObj == null)
			{
				m_DragRootObj = new GameObject("Dragroot");
				m_DragRootObj.transform.SetParent(this.transform);
				m_DragRootObj.transform.localScale = Vector3.one;
			}

			RectTransform rectTrans = GetComponent<RectTransform>();
			BoxCollider collider = GetComponent<BoxCollider>();
			if(collider == null)
			{
				collider = gameObject.AddComponent<BoxCollider>();
				collider.size = new Vector3(rectTrans.GetWidth(), rectTrans.GetHeight(), 1);
				collider.center = new Vector3(rectTrans.GetWidth()/2f, -rectTrans.GetHeight()/2f, 1);
			}
			m_collider = collider;
		}

		public void StopMove()
		{
			m_ScrollRect.StopMovement();
		}

		public void OnDragStart (PointerEventData eventData)
		{
			m_ScrollRect.OnBeginDrag(eventData);
		}

		public void OnDrag (PointerEventData eventData)
		{
			m_ScrollRect.OnDrag(eventData);
		}

		public void OnDragEnd (PointerEventData eventData)
		{
			m_ScrollRect.OnEndDrag(eventData);
		}

		public void SetGridEnable(bool enable)
		{
			m_GridGroup.enabled = enable;
		}

		public void Insert(GameObject obj, int index)
		{
			obj.transform.SetParent(m_GridGroup.transform);
			obj.transform.SetSiblingIndex(index);
			MoveItem(obj);
		}
		
		Coroutine moveCoroutine;
		public void UpdateGrid(bool AniEnable = true)
		{
			if(moveCoroutine != null)
				StopCoroutine(moveCoroutine);
			moveCoroutine = StartCoroutine(IUpdateGrid(AniEnable));
		}

		IEnumerator IUpdateGrid(bool AniEnable)
		{
			SetGridEnable(false);
			InitSize();

			CocoScrollDragButton[] buttons = m_GridGroup.GetComponentsInChildren<CocoScrollDragButton>();
			foreach(var button in buttons)
			{
				MoveItem(button.gameObject, AniEnable);
			}
			yield return new WaitForSeconds(0.3f);

			if(false == InDrag)
				SetGridEnable(true);
		}

		void MoveItem(GameObject obj, bool AniEnable = true)
		{
			float time = 0.2f;
			int index = obj.transform.GetSiblingIndex();

			if(m_ScrollRect.horizontal)
			{
				float x = m_GridGroup.cellSize.x *(index + 0.5f) + index * m_GridGroup.spacing.x;
				float y = m_GridGroup.cellSize.y * (-0.5f) + m_GridGroup.GetComponent<RectTransform>().GetHeight();
				Vector3 pos = new Vector3(x, y, 0);
				if(AniEnable)
					LeanTween.moveLocal(obj, pos, time);
				else
					obj.transform.localPosition = pos;
			}
			else if(m_ScrollRect.vertical)
			{
				float x = m_GridGroup.cellSize.x * (0.5f);
				float y = -1 * (m_GridGroup.cellSize.y *(index + 0.5f) + index * m_GridGroup.spacing.y);
				Vector3 pos = new Vector3(x, y, 0);
				if(AniEnable)
					LeanTween.moveLocal(obj, pos, time);
				else
					obj.transform.localPosition = pos;
			}
		}

		public void Insert(GameObject obj)
		{
			float MinDis = -1;
			int NearIndex = 0;
			float x = obj.transform.position.x;
			float y = obj.transform.position.y;
			CocoScrollDragButton[] buttons = m_GridGroup.GetComponentsInChildren<CocoScrollDragButton>();

			if(m_ScrollRect.horizontal)
			{
				foreach(var button in buttons)
				{
					float dis =  Mathf.Abs(button.transform.position.x - x);
					if(MinDis < 0 || dis < MinDis)
					{
						MinDis = dis;
	//					int SiblingIndex = button.GetComponent<RectTransform>().GetSiblingIndex();
						if(x > button.transform.position.x)
							NearIndex += 1;
					}
				}
				Insert(obj, NearIndex);
			}
			else if(m_ScrollRect.vertical)
			{
				foreach(var button in buttons)
				{
					float dis =  Mathf.Abs(button.transform.position.y - y);
					if(MinDis < 0 || dis < MinDis)
					{
						MinDis = dis;
	//					NearIndex = button.GetComponent<RectTransform>().GetSiblingIndex();
						if(y < button.transform.position.y)
							NearIndex += 1;
					}
				}
				Insert(obj, NearIndex);
			}
		}

		public void Clear()
		{
			GridGroup.transform.RemoveAllChildren();
		}

		public void UpdateGridNoAnction()
		{
			SetGridEnable(false);
			InitSize();
			SetGridEnable(true);
		}

		public void InitSize()
		{
			CocoScrollDragButton[] buttons = m_GridGroup.GetComponentsInChildren<CocoScrollDragButton>();
			if(m_ScrollRect.horizontal)
			{
				int count = m_GridGroup.constraintCount;
				int lenght = buttons.Length/count;
				if(buttons.Length%count != 0)
				{
					lenght += 1;
				}

				float width = m_GridGroup.cellSize.x*lenght + m_GridGroup.spacing.x * (lenght-1);
				width += m_GridGroup.padding.left + m_GridGroup.padding.right;
				m_GridGroup.GetComponent<RectTransform>().SetWidth(width);
			}
			else if(m_ScrollRect.vertical)
			{
				int horizontal_count = 1;
				if(m_GridGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
					horizontal_count = m_GridGroup.constraintCount;
				else
					horizontal_count = (int)(m_GridGroup.GetComponent<RectTransform>().GetWidth()/m_GridGroup.cellSize.x);
				
				if (horizontal_count <= 1) horizontal_count = 1; 
				int lenght = buttons.Length/horizontal_count;
				if(buttons.Length%horizontal_count != 0)
				{
					lenght += 1;
				}

				float height = m_GridGroup.cellSize.y*lenght + m_GridGroup.spacing.y * (lenght-1);
				height += m_GridGroup.padding.top + m_GridGroup.padding.bottom;
				m_GridGroup.GetComponent<RectTransform>().SetHeight(height);
			}
		}

		public IEnumerator LoadItemList(System.Action LoadAction)
		{
			
			CocoMainController.Instance.TouchEnable = false;
			ScrollRect.enabled = false;
			RectTransform rectTrans = GridGroup.GetComponent<RectTransform>();

			CCAction.MoveLocalY(GridGroup.gameObject, -768, 0.3f);
			yield return new WaitForSeconds(0.3f);
			Clear();
			yield return new WaitForEndOfFrame();
			GridGroup.enabled = true;
			if(LoadAction != null)
				LoadAction();

			yield return new WaitForEndOfFrame();
			CCAction.MoveLocalY(GridGroup.gameObject, 0, 0.3f);
			yield return new WaitForSeconds(0.3f);
			ScrollRect.enabled = true;
			CocoMainController.Instance.TouchEnable = true;
		}
	}
}
