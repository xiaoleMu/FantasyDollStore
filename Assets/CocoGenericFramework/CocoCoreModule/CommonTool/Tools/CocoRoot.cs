using System;
using TabTale;
using strange.extensions.injector.api;
using strange.extensions.command.api;


namespace CocoPlay
{
	public class CocoRoot
	{
		#region Instance

		private static CocoRoot m_Instance;

		private static CocoRoot Instance {
			get {
				if (m_Instance == null) {
					m_Instance = new CocoRoot ();
				}
				return m_Instance;
			}
		}

		private CocoRoot ()
		{
		}

		#endregion


		#region Context

		private GameContext m_Context;

		public static GameContext Context {
			get {
				if (Instance.m_Context == null) {
					if (GameApplication.Instance != null) {
						StrangeRoot strangeRoot = GameApplication.Instance.ModuleContainer.Get<StrangeRoot> ();
						if (strangeRoot != null) {
							GameRoot gameRoot = strangeRoot.GameRoot;
							if (gameRoot != null) {
								Instance.m_Context = (GameContext)gameRoot.context;
							}
						}
					}
				}
				return Instance.m_Context;
			}
		}

		#endregion


		#region Instance

		public static T GetInstance<T> (object name = null)
		{
			return Context.injectionBinder.GetInstance<T> (name);
		}

		#endregion


		#region Bind/Unbind

		public static IInjectionBinding Bind<T> (object name = null)
		{
			IInjectionBinding binding = Context.injectionBinder.Bind<T> ().ToSingleton ();
			if (name != null) {
				binding.ToName (name);
			}
			return binding;
		}

		public static IInjectionBinding BindType<T, TType> (object name = null)
		{
			IInjectionBinding binding = Context.injectionBinder.Bind<T> ().To<TType> ().ToSingleton ();
			if (name != null) {
				binding.ToName (name);
			}
			return binding;
		}

		public static IInjectionBinding BindType<T> (Type type, object name = null)
		{
			IInjectionBinding binding = Context.injectionBinder.Bind<T> ().To (type).ToSingleton ();
			if (name != null) {
				binding.ToName (name);
			}
			return binding;
		}

		public static IInjectionBinding BindValue<T> (T value, object name = null)
		{
			IInjectionBinding binding = Context.injectionBinder.Bind<T> ().ToValue (value);
			if (name != null) {
				binding.ToName (name);
			}
			return binding;
		}

		public static void Unbind<T> (object name = null)
		{
			Context.injectionBinder.Unbind<T> (name);
		}

		#endregion


		#region Command Bind

		public static ICommandBinding BindCommand<T, TCommand> (object name = null)
		{
			ICommandBinding binding = Context.commandBinder.Bind<T> ().To<TCommand> ();
			if (name != null) {
				binding.ToName (name);
			}
			return binding;
		}

		#endregion


		#region Stuff Bind

		public static IInjectionBinding StuffBind<T> (object name = null)
		{
			return Context.injectionBinder.GetBinding<T> (name) ?? Bind<T> (name);
		}

		public static IInjectionBinding StuffBindType<T, TType> (object name = null)
		{
			return Context.injectionBinder.GetBinding<T> (name) ?? BindType<T, TType> (name);
		}

		public static IInjectionBinding StuffBindType<T> (Type type, object name = null)
		{
			return Context.injectionBinder.GetBinding<T> (name) ?? BindType<T> (type, name);
		}

		public static IInjectionBinding StuffBindValue<T> (T value, object name = null)
		{
			return Context.injectionBinder.GetBinding<T> (name) ?? BindValue (value, name);
		}

		#endregion
	}
}