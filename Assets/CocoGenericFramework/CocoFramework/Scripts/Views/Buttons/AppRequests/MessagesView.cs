using UnityEngine;
using System.Collections;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine.UI;
using System.Collections.Generic;

namespace TabTale {

	public class MessagesView : MainView 
	{
		public enum MessagesViewCounterType
		{
			Events,
			Requests,
			All
		}

		[Inject]
		public RequestStateModel requestsStateModel { get; set; }

		[Inject]
		public RequestReceivedSignal requestReceivedSignal { get; set; }

		[Inject]
		public EventSystemService eventSystemService { get; set; }

		[Inject]
		public EventEndedSignal eventEndedSignal { get; set; }

		public Text newCounter;

		public MessagesViewCounterType messagesType;

		public GameObject[] newMessagesArrivedUI;
		public GameObject[] noNewMessagesUI;

		protected override void OnRegister ()
		{
			base.OnRegister ();
			UpdateCounterUI();
		}

		protected override void AddListeners()
		{
			requestReceivedSignal.AddListener (OnRequestsReceived);
			eventEndedSignal.AddListener(OnEventEnded);
		}

		protected override void RemoveListeners()
		{
			requestReceivedSignal.RemoveListener (OnRequestsReceived);
			eventEndedSignal.RemoveListener(OnEventEnded);
		}

		private void OnEventEnded(string eventId)
		{
			UpdateCounterUI();
		}

		private void OnRequestsReceived(List<RequestStateData> newRequests)
		{
			UpdateCounterUI();
		}
			
		protected void UpdateCounterUI()
		{
			int messagesNum = CalcMessagesCount();

			if(messagesNum > 0)
			{
				ShowCircleAndCounter(true);

				newCounter.text = messagesNum.ToString();
			}
			else 
			{
				ShowCircleAndCounter (false);
			}
		}

		protected int CalcMessagesCount()
		{
			int messagesNum = 0;
			switch(messagesType)
			{
			case MessagesViewCounterType.Events:
				messagesNum = eventSystemService.GetEndedEvents().Count;
				break;
			case MessagesViewCounterType.Requests:
				messagesNum = requestsStateModel.NewRequestsCount;
				break;
			case MessagesViewCounterType.All:
				messagesNum = eventSystemService.GetEndedEvents().Count + requestsStateModel.NewRequestsCount;
				break;
			}

			return messagesNum;
		}
		private void ShowCircleAndCounter(bool enabled) {

			for (int i = 0; i < newMessagesArrivedUI.Length; i++) {
				newMessagesArrivedUI [i].SetActive (enabled);
			}
			for (int i = 0; i < noNewMessagesUI.Length; i++) {
				noNewMessagesUI [i].SetActive (!enabled);
			}
		}
	}
}