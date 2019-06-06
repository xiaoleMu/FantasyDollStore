using UnityEngine;
using System.Collections;

namespace TabTale.Publishing {
	
	/// <summary>
	/// Result of displaying an external popup during a game.
	/// </summary>
	public enum PopupEndResult
	{
		/// <summary>
		/// Result is unknown - whether the popup was not closed yet, or something bad happened.
		/// </summary>
		Unknown, 
		
		/// <summary>
		/// This usually means a technical failure to display the popup, regardless of content,
		/// or perhaps a failure to retrieve content.
		/// </summary>
		Fail, 
		
		/// <summary>
		/// The content was displayed and closed, and the user interaced with the popup.
		/// </summary>
		ContentConsumed, 
		
		/// <summary>
		/// The popup was displayed and closed, and the user did not interace with it.
		/// </summary>
		Close, 
		
		/// <summary>
		/// No content was available
		/// FIXME: was the popup displayed or not?
		/// </summary>
		NoContent
	};
	
	public enum ContentState
	{
		Ready, 
		NotReady, 
		Showing,
		Closed,
		Unknown,
	}

	/*
	/// <summary>
	/// This is a placeholder, for the "token" of the natively 
	/// created by implementors.
	/// </summary>
	public interface IPopupHandle : IModalHandle
	{
	}
	*/

	public struct PopupResult
	{
		public PopupEndResult Result;
		public object Payload;

		public static explicit operator PopupResult(PopupEndResult result)  // explicit byte to digit conversion operator
		{
			return new PopupResult(result);
        }
        
        public PopupResult(PopupEndResult result, object payload = null)
		{
			this.Result = result;
			this.Payload = payload;
		}
	}

	/// <summary>
	/// This is a callback which will receive the handle to a popup when it's available.
	/// </summary>
	public delegate void PopupHandleReceiver(IModalHandle handle);

	/// <summary>
	/// When a popup closes, for whatever reasons, it calls this handler and reports its
	/// result.
	/// </summary>
	public delegate void PopupResultHandler(IModalHandle handle, PopupResult result);

	/// <summary>
	/// Providers are publishers which allow subscribers to listen to changes in specific states.
	/// This is usually the state of a "location" in terms of whether or not it's "ready" - 
	/// whether ads and/or content for that location are ready to display.
	/// </summary>
	public delegate void ContentStateChangeHandler(ApplicationLocation location, ContentState oldState, ContentState newState);
	
	/// <summary>
	/// The Popups Provider, AKA Modals Manager, AKA Interstitials, 
	/// AKA Location Manager, is responsible for fetching, managing and displaying
	/// various modal (on top of everything) publishing materials, such as interstitial
	/// ads, app shelves, campaigns, etc. Its PSDK equivalent is the location manager
	/// </summary>
	public interface IPopupsProvider : IService, IMultiPublisher<ApplicationLocation, ContentStateChangeHandler>
	{
        bool IsReady { get; }

		IEnumerator GetShowRoutine(ApplicationLocation location, PopupHandleReceiver handleReceiver, PopupResultHandler resultHandler);
		IModalHandle Show(ApplicationLocation location, PopupResultHandler handler, bool isOverriding = true);
        IModalHandle Show(ApplicationLocation location, bool isOverriding = true);

		ContentState GetContentState(ApplicationLocation location);
	}
}
