using System;
using TabTale;
using strange.extensions.injector.api;
using strange.extensions.command.api;

namespace CocoPlay
{
	public abstract class CocoModuleBase : GameView
	{
		#region Property

		private object m_ID;

		public object ID {
			get { return m_ID; }
			private set {
				m_ID = value;
				name = string.Format ("Module_{0}", GetType ().Name);
				if (m_ID != null) {
					name += "_" + m_ID;
				}
			}
		}

		#endregion


		#region Init/Clean

		public void Init (object moduleId)
		{
			ID = moduleId;

			InitSignals ();
			StuffSignals ();
			InitDatas ();
			StuffDatas ();
			InitObjects ();
			StuffObjects ();
			InitSubModules ();
			StuffSubModules ();
		}

		public void Clean ()
		{
			CleanSubModules ();
			CleanObjects ();
			CleanDatas ();
			CleanSignals ();
		}

		#endregion


		#region Module

		public virtual void StartModule ()
		{
		}

		#endregion


		#region Signal

		protected virtual void InitSignals ()
		{
		}

		protected virtual void CleanSignals ()
		{
		}

		protected virtual void StuffSignals ()
		{
		}

		#endregion


		#region Data

		protected virtual void InitDatas ()
		{
		}

		protected virtual void CleanDatas ()
		{
		}

		protected virtual void StuffDatas ()
		{
		}

		#endregion


		#region Object

		protected virtual void InitObjects ()
		{
		}

		protected virtual void CleanObjects ()
		{
		}

		protected virtual void StuffObjects ()
		{
		}

		#endregion


		#region Sub Module

		protected virtual void InitSubModules ()
		{
		}

		protected virtual void CleanSubModules ()
		{
		}

		protected virtual void StuffSubModules ()
		{
		}

		#endregion


		#region Helper

		protected IInjectionBinding Bind<T> ()
		{
			return CocoRoot.Bind<T> (ID);
		}

		protected IInjectionBinding BindType<T, TType> ()
		{
			return CocoRoot.BindType<T, TType> (ID);
		}

		protected IInjectionBinding BindType<T> (Type type)
		{
			return CocoRoot.BindType<T> (type, ID);
		}

		protected IInjectionBinding BindValue<T> (T value)
		{
			return CocoRoot.BindValue (value, ID);
		}

		protected void Unbind<T> ()
		{
			CocoRoot.Unbind<T> (ID);
		}

		protected ICommandBinding BindCommand<T, TCommand> ()
		{
			return CocoRoot.BindCommand<T, TCommand> ();
		}

		protected TModule BindSubModule<TModule> (string assetPath = null, object moduleId = null) where TModule : CocoModuleBase
		{
			return CocoMainController.Instance.AddModule<TModule> (assetPath, moduleId);
		}

		protected TModuleEntity BindSubModule<TModule, TModuleEntity> (string assetPath = null, object moduleId = null)
			where TModule : CocoModuleBase where TModuleEntity : CocoModuleBase
		{
			return CocoMainController.Instance.AddModule<TModule, TModuleEntity> (assetPath, moduleId);
		}

		protected TModule BindSubModule<TModule> (Type moduleType, string assetPath = null, object moduleId = null) where TModule : CocoModuleBase
		{
			return CocoMainController.Instance.AddModule<TModule> (moduleType, assetPath, moduleId);
		}

		protected TModule BindSubModule<TModule> (CocoModuleBase module, object moduleId = null) where TModule : CocoModuleBase
		{
			return CocoMainController.Instance.AddModule<TModule> (module, moduleId);
		}

		protected void UnbindSubModule<TModule> (object moduleId = null) where TModule : CocoModuleBase
		{
			CocoMainController.Instance.RemoveModule<TModule> (moduleId);
		}

		protected IInjectionBinding StuffBind<T> ()
		{
			return CocoRoot.StuffBind<T> (ID);
		}

		protected IInjectionBinding StuffBindType<T, TType> ()
		{
			return CocoRoot.StuffBindType<T, TType> (ID);
		}

		protected IInjectionBinding StuffBindType<T> (Type type)
		{
			return CocoRoot.StuffBindType<T> (type, ID);
		}

		protected IInjectionBinding StuffBindValue<T> (T value)
		{
			return CocoRoot.StuffBindValue (value, ID);
		}

		#endregion
	}
}