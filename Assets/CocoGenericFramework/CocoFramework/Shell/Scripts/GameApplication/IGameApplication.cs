using UnityEngine;
using System.Collections;
using TabTale.AssetManagement;
using TabTale.SceneManager;
using TabTale.Publishing;
using TabTale.Analytics;
//using TabTale.GameServices;

namespace TabTale
{
	public interface IGameApplication : ISceneController, IComponentContainer
	{
		IAssetManager AssetManager { get; }

		ILocalNotificationServices NotificationServices { get; }		

		ISceneManager SceneManager { get; }	

		IGeneralDialogService GeneralDialog { get; }

		IBackButtonService BackButtonService{get;}

		ITaskFactory TaskFactory { get; }

		IModalityManager ModalityManager { get; }		

		IApplicationEvents ApplicationEvents { get; }

		IConfigurationService Configuration { get; }
		
		IDispatcher Dispatcher { get; }

		IServiceResolver ServiceResolver { get; }

		IModuleContainer ModuleContainer { get; }

		ICoroutineFactory CoroutineFactory { get; }

		bool StrangeServicesInitDone { get; set; }
	
		//FIXME: Should be part of the interface?
		PopupResultHandler PopupResultHandler { get; }


	}
}
