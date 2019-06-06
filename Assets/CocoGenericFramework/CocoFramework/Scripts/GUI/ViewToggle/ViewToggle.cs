using UnityEngine;
using System.Collections;

namespace TabTale
{
	public class ViewToggle : MonoBehaviour
	{
		public GameObject[] views; 

		protected GameObject _activeView;

		void Awake ()
		{
			for(int i=0;i<views.Length;i++)
				views[i].SetActive(false);
			
			_activeView = null;
		}

		public void OpenView(int viewIndex){
			OpenView(views[viewIndex]);
		}

		public void OpenView(GameObject view){
			
			ToggleView(view);
		}

		protected void ToggleView(GameObject screen){
			
			if(_activeView!=null){
				_activeView.SetActive(false);
			}
			
			_activeView = screen;
			
			_activeView.SetActive(true);
		}
	}

}
