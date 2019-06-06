using UnityEngine;
using System.Collections;

namespace TabTale {

	public abstract class Module : TaskFactory, IModule
	{
		#region IModule implementation

		protected IModuleContainer _moduleContainer;
		protected IServiceResolver _serviceResolver;
		public ITask GetInitializer (IModuleContainer moduleContainer)
		{
			_moduleContainer = moduleContainer;
			_serviceResolver = moduleContainer.ServiceResolver;

			return FromEnumerableAction(ModuleInitializer);
		}

		protected abstract IEnumerator ModuleInitializer();

		public virtual void StartModule ()
		{
		}

		public virtual void StopModule ()
		{
		}

		public virtual void Terminate ()
		{
		}

		public virtual void UpdateModule (float timeDelta)
		{
		}

		public virtual int Stage
		{
			get { return loadingStage; }
		}

		public int loadingStage = 0;

		#endregion


	}
}
