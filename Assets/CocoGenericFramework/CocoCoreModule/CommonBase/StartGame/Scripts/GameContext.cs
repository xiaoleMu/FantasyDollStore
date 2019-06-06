using UnityEngine;
using System.Collections;
using TabTale.Analytics;
using CocoPlay;

namespace TabTale
{
	public class GameContext : BaseContext
	{
		public GameContext (MonoBehaviour view) : base (view)
		{            
			StartStrange ();
		}

		public void StartStrange ()
		{
			BindLifecycle ();
			BindServices ();
		}

		void BindLifecycle ()
		{
			injectionBinder.Bind<StartSignal> ().ToSingleton ();
			injectionBinder.Bind<OnDestroySignal> ().ToSingleton ();
		}

		void BindServices ()
		{	
			#if !AMAZON
			injectionBinder.ReBind<ISocialService>().To<SocialService>().ToSingleton().CrossContext();
			#endif
			injectionBinder.ReBind<IAnalyticsService> ().To<AnalyticsService> ().ToSingleton ().CrossContext ();
			injectionBinder.Bind<IRateUsService> ().To<RateUsService> ().ToSingleton ().CrossContext ();

			injectionBinder.ReBind<FirstSceneLoadedSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<FirstSceneLoadedSignal> ().To<FirstSceneLoadedCommand> ().To<CocoSessionStartCommand>().Once();
			injectionBinder.ReBind<PsdkSessionStartSignal> ().ToSingleton ().CrossContext ();
			commandBinder.Bind<PsdkSessionStartSignal> ().To<CocoSessionStartCommand> ();

			injectionBinder.Bind<UpdateRvStatusSignal>().ToSingleton ();

			//gesturer 手势操作时发送
			injectionBinder.Bind<TapGestureSignal>().ToSingleton ();
			injectionBinder.Bind<DragGestureSignal>().ToSingleton ();
			injectionBinder.Bind<PinchGestureSignal>().ToSingleton ();
			injectionBinder.Bind<SwipeGestureSignal>().ToSingleton ();
			injectionBinder.Bind<TwoFingerDragGestureSignal>().ToSingleton ();
			injectionBinder.Bind<TwistGestureSignal>().ToSingleton ();
			injectionBinder.Bind<EnableFingerGestureSignal>().ToSingleton ();
			injectionBinder.Bind<DownEventSignal>().ToSingleton ();
			injectionBinder.Bind<UpEventSignal>().ToSingleton ();

			injectionBinder.Bind<RequestGeneralLocalNotificationSignal> ().ToSingleton ();

			injectionBinder.Bind<CocoGlobalRecordModel> ().ToSingleton ();
		}

	}
}
