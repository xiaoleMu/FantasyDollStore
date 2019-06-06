using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;

namespace TabTale
{
	public class ToggleViewList<TItem,TData, TDataId> : GenericViewList<TItem,TData,TDataId> where TItem:class
	{
		public event Action<TItem> ToggleSelectedEvent = (i)=>{};
		public event Action<TItem> ToggleUnSelectedEvent = (i)=>{};

		int _selectedIndex = -1;

		ToggleGroup _toggleGroup;

		protected override void Awake ()
		{
			base.Awake ();

			_toggleGroup = GetComponent<ToggleGroup>();

//			for (int i=0; i<gameObjectList.Count; i++){
//				ImplementToggle(gameObjectList[i]);
//			}
		}

		protected override void OnItemAdded (TItem itemView, GameObject itemGo)
		{
			base.OnItemAdded (itemView, itemGo);

			ImplementToggle(itemGo);

		}

		void ImplementToggle(GameObject itemGo){
			Toggle toggle = itemGo.GetComponent<Toggle>();
			if(toggle==null){
				CoreLogger.LogError("ToggleViewList","ImplementToggle missing Toggle component");
				return;
			}
			
			toggle.group = _toggleGroup;
			toggle.isOn = false;
			toggle.onValueChanged.AddListener(OnToggleValueChanged);
		}

		protected override void OnItemRemoved (TItem itemView, GameObject itemGo)
		{
			base.OnItemRemoved (itemView, itemGo);

			Toggle toggle = itemGo.GetComponent<Toggle>();
			toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
		}

		void OnToggleValueChanged (bool isSelected)
		{
			if(!isSelected){
				if(_toggleGroup.allowSwitchOff && _selectedIndex!=-1){
					ToggleUnSelectedEvent(_itemList[_selectedIndex]);
					_selectedIndex=-1;
				}
				return;
			}

			Toggle selectedToggle = null;

			foreach(Toggle t in _toggleGroup.ActiveToggles()){
				if (t.isOn){
					selectedToggle = t;
					break;
				}
			}

			if (selectedToggle != null){
				for (int i = 0; i < gameObjectList.Count; i++) {
					GameObject g = gameObjectList[i];

					if (g == selectedToggle.gameObject){
						_selectedIndex = i;
						ToggleSelectedEvent(_itemList[i]);
						break;
					}
				}
			}
		}

		public int SelectedIndex {
			get {
				return _selectedIndex;
			}
		}

		public void SelectItemByIndex(int value, bool allowEventDispatch = true){
			_selectedIndex = value;
			
			Toggle toggle = gameObjectList[_selectedIndex].GetComponent<Toggle>();

			if(!toggle.isOn){
				if (allowEventDispatch){
					toggle.isOn = true;
				}else{
					toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
					toggle.isOn = true;
					toggle.onValueChanged.AddListener(OnToggleValueChanged);
				}
			}
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
			_toggleGroup.SetAllTogglesOff();
		}
	}


	public class ToggleViewList :ToggleViewList<Component,object,string>{

	}

}