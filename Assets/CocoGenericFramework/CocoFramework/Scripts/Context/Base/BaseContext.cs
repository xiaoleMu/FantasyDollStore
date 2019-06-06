using UnityEngine;
using System.Collections;
using strange.extensions.context.impl;
using strange.extensions.context.api;
using strange.extensions.command.api;
using strange.extensions.command.impl;

namespace TabTale {

	public class BaseContext : MVCSContext {

		public BaseContext (MonoBehaviour view) : base(view)
		{
		}
	//	
	//	public BaseContext (MonoBehaviour view, ContextStartupFlags flags) : base(view, flags)
	//	{
	//	}
		
		// Unbind the default EventCommandBinder and rebind the SignalCommandBinder
		protected override void addCoreComponents()
		{
			base.addCoreComponents();
			injectionBinder.Unbind<ICommandBinder>();
			injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
		}
		
		// Override Start so that we can fire the StartSignal 
		override public IContext Start()
		{
			base.Start();

			return this;
		}
		
		protected override void mapBindings()
		{
			base.mapBindings();
		}

		public void SendStartSignal(){
			StartSignal startSignal= (StartSignal)injectionBinder.GetInstance<StartSignal>();
			startSignal.Dispatch();
		}

        public void SendOnDestroySignal()
        {
            OnDestroySignal destroySignal = (OnDestroySignal)injectionBinder.GetInstance<OnDestroySignal>();
            destroySignal.Dispatch();
        }

	}

}
