using UnityEngine;
using System.Collections;
using LitJson;

namespace TabTale
{
	public class GameConnector : Module
	{
		IGeneralDialogService _dialogService;
		protected override IEnumerator ModuleInitializer ()
		{
#if UNITY_EDITOR
			if(!activeInEditor)
				yield break;
#endif
			Debug.Log ("Handshake Start ------------");
			_dialogService = _serviceResolver.Get<IGeneralDialogService>();

			_handshakeState = HandshakeLoopState.TryHandshake;
			
			_connectionHandler = new ConnectionHandler();
			_connectionHandler.Init(null,this);

			
			while (_handshakeState != HandshakeLoopState.CanContinue) 
			{
				if (_handshakeState == HandshakeLoopState.TryHandshake)
					yield return StartCoroutine (_connectionHandler.SendRequest (ConnectionHandler.RequestType.GetVersion, HandleHandshakeResponse));
				else
					yield return new WaitForSeconds(1.0f);
			}

			Debug.Log ("Handshake end ------------");
			yield break;
		}
		
		private void HandleHandshakeResponse (ConnectionHandler.RequestResult result, string response)
		{
			Debug.Log ("HandleHandshakeResponse (result=" + result + ")\nresponse=" + response);
			if (result != ConnectionHandler.RequestResult.Ok) {
				HandleErrorResponse (result, response);
				return;
			}
			_handshakeState = HandshakeLoopState.CanContinue;
			Debug.Log ("HandleGetVersionResponse - no update required");
		}
		
		public void HandleErrorResponse (ConnectionHandler.RequestResult result, string response)
		{
			Debug.Log ("GameConnector.HandleErrorResponse (result=" + result + ")\nresponse: " + response);
			
			string title = "";
			string message = "";
			
			switch (result) {
			case (ConnectionHandler.RequestResult.NoInternet):
				title = "Connection Error";
				message = "Unable to connect with server. Check you internet connection and try again";
				break;
			case (ConnectionHandler.RequestResult.CantReachServer):
				title = "Server under maintenance";
				message = "Server is under maintenance. Please try again in 1 hour.";
				break;
			case (ConnectionHandler.RequestResult.InternalServerError):
				break;
			case (ConnectionHandler.RequestResult.OkWithErrorInside): 
				JsonData responseErrorJsonOnject = JsonMapper.ToObject (response);
				JsonData errorObject = (JsonData)responseErrorJsonOnject [keyError];
				ServerErrorData error = JsonMapper.ToObject<ServerErrorData> (errorObject.ToJson ());
				Debug.Log ("Handling server error message: " + error.ToString ());
				if (error.code == (int)ConnectionHandler.RequestResult.ClientUpdateRequired) {
					title = error.title;
					message = "You must update your game - update initiated (url=" + error.message + ").";
					break;
				}
				
				title = "Server under maintenance";
				message = "Server is under maintenance. Please try again in 1 hour.";
				break;
			}
			ShowDialog (title, message);
		}

		public void ShowDialog(string title, string message)
		{
			_handshakeState = HandshakeLoopState.DisconnectedOnHold;

			GeneralDialogData dialogData = new GeneralDialogData();
			dialogData.title = title;
			dialogData.message = message;
			dialogData.hasCloseButton = false;
			dialogData.buttons.Add(new DialogButtonData() {
				caption = "Retry", 
				dispatchAction = () => {
					_handshakeState = HandshakeLoopState.TryHandshake;
				}
			});
			dialogData.buttons.Add(new DialogButtonData() {
				caption = "Continue without handshake", 
				dispatchAction = () => {
					_handshakeState = HandshakeLoopState.CanContinue;
				}
			});

			_dialogService.Show(dialogData);

		}
		
		public enum HandshakeLoopState
		{
			TryHandshake,
			DisconnectedOnHold,
			CanContinue
		}
		public HandshakeLoopState _handshakeState;
		private ConnectionHandler _connectionHandler;
		private string keyError = "error";

		public bool activeInEditor = true;
	}
}
