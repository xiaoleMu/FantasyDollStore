using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TabTale
{
	public class BackButtonService : MainView, IBackButtonService
	{
		[Inject]
		public IPsdkCoreServices psdkCoreServices { get; set; }

		List<IBackButtonListener> _listeners = new List<IBackButtonListener>();

		public void AddListener(IBackButtonListener listener)
		{
			_listeners.Add(listener);
		}

		public void RemoveListener(IBackButtonListener listener)
		{
			_listeners.Remove(listener);
		}
				
		void Update ()
		{
#if UNITY_ANDROID || UNITY_EDITOR
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				// Verify we're not currently seeing a chartboost interstitial (then we don't want to quit):
				if (psdkCoreServices.OnBackPressed())
					return;

				NotifyListeners();
			}  
#endif
		}

		public void DefaultBackButtonAction()
		{
			// Default back button behaviour on android - quit the application	
			Application.Quit();
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#endif
		}

		void NotifyListeners(){
			for(int i = _listeners.Count - 1; i >= 0; i--){
				if (_listeners [i] == null) {
					_listeners.RemoveAt (i);
					continue;
				}
				if (_listeners[i].HandleBackButtonPress()){
					return;
				}
			}
		}

		#region IService Implementation
		
		public ITask GetInitializer(IServiceResolver resolver)
		{
			return resolver.TaskFactory.FromEnumerableAction(Init);
		}
		
		IEnumerator Init()
		{
			yield break;
		}
		
		#endregion
	}

	public interface IBackButtonListener
	{
		void SubscribeToBackButtonEvent();
		void UnSubscribeFromBackButtonEvent();
		bool HandleBackButtonPress();
	}

	public interface IBackButtonService : IService{
		void AddListener(IBackButtonListener listener);
		void RemoveListener(IBackButtonListener listener);
		void DefaultBackButtonAction();
	}

	public class NullBackButtonService : NullService, IBackButtonService
	{
		#region IService Implementation
		
		public ITask GetInitializer(IServiceResolver resolver)
		{
			return resolver.TaskFactory.FromEnumerableAction(Init);
		}
		
		IEnumerator Init()
		{
			yield break;
		}
		
		#endregion

		#region IBackButtonService implementation

		public void AddListener (IBackButtonListener listener)
		{

		}

		public void RemoveListener (IBackButtonListener listener)
		{

		}

		public void DefaultBackButtonAction()
		{

		}
		#endregion
		
		
	}
}
