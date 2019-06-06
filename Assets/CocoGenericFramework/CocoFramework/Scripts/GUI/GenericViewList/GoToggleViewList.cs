using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TabTale
{
	public class GoToggleViewList<TItem,TData, TDataId> : GenericViewList<TItem,TData,TDataId> where TItem:class
	{
		public event Action<TItem> ToggleSelectedEvent = (i)=>{};
		//public event Action<TItem> ToggleUnSelectedEvent = (i)=>{};
		
		int _selectedIndex = -1;

		protected override void OnItemAdded (TItem itemView, GameObject itemGo)
		{
			base.OnItemAdded (itemView, itemGo);

			GoToggle goToggle = itemGo.GetComponent<GoToggle>();
			goToggle.ToggleEvent+=OnToggle;
		}

		protected override void OnItemRemoved (TItem itemView, GameObject itemGo)
		{
			base.OnItemRemoved (itemView, itemGo);

			GoToggle goToggle = itemGo.GetComponent<GoToggle>();
			goToggle.ToggleEvent-=OnToggle;
		}

		void OnToggle (GoToggle toggle)
		{
			SelectItemByIndex(gameObjectList.IndexOf(toggle.gameObject),true);
		}

		public void SelectItemByIndex(int value, bool allowEventDispatch = true){

			if(_selectedIndex!=-1)
				gameObjectList[_selectedIndex].GetComponent<GoToggle>().IsOn = false;

			_selectedIndex=value;
			gameObjectList[_selectedIndex].GetComponent<GoToggle>().IsOn=true;

			if(allowEventDispatch)
				ToggleSelectedEvent(_itemList[_selectedIndex]);
		}

		public TItem SelectedItem {
			get {
				if(_selectedIndex<0 || _selectedIndex>=_itemList.Count)
					return null;
				
				return _itemList [_selectedIndex];
			}
		}

		public void SelectItemById(TDataId id, bool allowEventDispatch = true){
			
			int foundItemIndex = GetItemIndexById(id);
			
			if (foundItemIndex != -1 ){
				SelectItemByIndex(foundItemIndex, allowEventDispatch);
			}
		}
		
		public void ResetSelected(){
			_selectedIndex=-1;
			gameObjectList.ForEach(t=>t.GetComponent<GoToggle>().IsOn = false);
		}

		public int SelectedIndex {
			get {
				return _selectedIndex;
			}
		}

	}

	public class GoToggleViewList : GoToggleViewList<Component,object,string>{

	}

}