using UnityEngine;
using System.Collections.Generic;
using System;

namespace TabTale.Plugins.PSDK {

	public enum InterstitialLocation
	{
		SESSION_START,
		SCENE_TRANSITIONS,
		ACHIEVEMENT_WON,
		BACK_TO_MAIN_MENU,
		FAIL,
		GENERIC_LOCATION,
		IN_SCENE,
		POST_LEVEL,
		PRE_LEVEL,
		REPLAY_LEVEL,
		RETRY
	}

	public interface IPsdk {

		/** \brief Fired when all sounds need to be muted */
		event Action OnMuteSound;

		/** \brief Fired when sounds are OK to be unmuted
	 *
	 * The application needs to enable sounds again, unless the user has disabled sound in the application's settings.
	 */
		event Action OnUnmuteSound;

		/** \brief Fired when a session-start (Chartboost) is about to be shown. */
		event Action OnSessionStartShown;

		/** \brief Fired after a session-start (Chartboost) has been closed. */
		event Action OnSessionStartHidden;

		/** \brief Fired when the availability of a rewarded-video has changed. 
	 * 
	 * The callback is passed true if rewarded-video is available, false otherwise.
	 */
		event Action<bool /*isAvailable*/> OnRewardedVideoAvailabilityChanged;

		/** \brief Fired when a purchase or restoration of an item is successful
     * 
     * @param TabTale.InAppPurchasableItem Info about the item that was purchased/restored
     * @param bool true if item was restored, false if purchased
     */
		event Action<TabTale.InAppPurchasableItem /*item*/, bool /*isRestore*/> OnPurchaseOrRestoreSucceeded;

		/** \brief Fired when a purchase has failed
     * 
     * @param TabTale.InAppPurchasableItem Info about the item whose purchase failed
     */
		event Action<TabTale.InAppPurchasableItem /*item*/> OnPurchaseFailed;

		/** \brief Fired when user has successfuly logged in to game services.
     */
		event Action OnGameServicesLoginSucceeded;

		/** \brief Fired when the attempt to log in to game services has failed. (e.g. the user cancelled the request)
     */
		event Action OnGameServicesLoginFailed;

		/** \brief Fired when user has logged out of game services.
     */
		event Action OnGameServicesLogout;

		/** \brief Fired when a new session has started. Usually after the app has spent 5 minutes in background.
     */
		event Action OnNewSession;

		/** \brief Fired when the game needs to restart. Usually after the app has spent 1 hour in background.
     */
		event Action OnRestartGame;

		/** \brief Queries the availability of a rewarded-video. */
		bool IsRewardedVideoAvailable();

		/** \brief Show a rewarded-video.
	 *
	 * @param resultDelegate Callback to be invoked after the video has stopped/ended. The application should only apply the reward if the callback is passed `true`.
	 */
		void ShowRewardedVideo(Action<bool> resultDelegate);

		/** \brief Show a scene-transition interstitial
	 * 
     * @param interstitialLocation The game "location" at which the interstitial is shown. 
	 * @param completionDelegate Optional callback to be invoked after the interstitial has been closed.
	 */
		bool ShowInterstitial(InterstitialLocation interstitialLocation, Action completionDelegate);

		/** \brief Show a app-shelf (more apps) */
		bool ShowAppShelf();

		/** \brief Report a successfully completed purchase. No need to call this if you handle purchases through PSDK.
	 * 
	 * @param price Price of the item. Should not contain currency symbols.
	 * @param currencyCode E.g. "USD", "GBP", etc
	 * @param productId IAP item ID
	 */
		void ReportPurchase(string price, string currencyCode, string productId);

		/** \brief Log a Flurry event
	 * 
	 * @param eventName Name of the event
	 * @param eventParameters Optional event parameters
	 */
		void LogEvent(string eventName, IDictionary<string,object> eventParameters = null, long analyticsAgents = AnalyticsTargets.ANALYTICS_TARGET_FLURRY | AnalyticsTargets.ANALYTICS_TARGET_TT_ANALYTICS);

		/** \brief Log a timed Flurry event.
	 * 
	 * Termination of the event needs to be marked by calling `EndTimedEvent`. Note that pending events will terminated automatically if the application is pushed to the background.
	 * 
	 * @param eventName Name of the event
	 * @param eventParameters Optional event parameters
	 */
		void LogTimedEvent(string eventName, IDictionary<string,object> eventParameters = null);

		/** \brief End an active timed Flurry event.
	 * 
	 * @param eventName Name of the event. Must match the name of an active timed Flurry event.
	 * @param eventParameters Optional event parameters. If a parameter with the same name already appeared at `LogTimedEvent`, its value will be overriden.
	 */ 
		void EndTimedEvent(string eventName, IDictionary<string,object> eventParameters = null);

		/** \brief Initiate IAP for a specific item.
     * 
     * @param itemId Store-independent ID of the item. PSDK takes care of mapping store-independent to store-specific IDs and vice versa.
     * 
     */
		void PurchaseItem(string itemId);

		/** \brief Initiate IAP for a specific item.
     * 
     * @param itemId Store-independent ID of the item. PSDK takes care of mapping store-independent to store-specific IDs and vice versa.
     * 
     */
		void RestorePurchases(string itemId);

		/** \brief Disables ads permanently. No need to call this if you handle purchases through PSDK.
     */
		void DisableAdsPermanently();

		/** \brief Show the Rate-Us page
     */
		void ShowRateUs();

		/** \brief Returns whether user is logged in to game services.
     */
		bool IsLoggedInGameServices();

		/** \brief Request login to game services.
     */
		void LoginGameServices();

		/** \brief Request logout of game services.
     */
		void LogoutGameServices();

		/** \brief Show Leaderboard
     */
		void ShowLeaderboard();

		/** \brief Report score for leaderboard
     * 
     * @param leaderboardId Store-specific ID of the Leaderboard
     * @param score Score to report
     */
		void ReportScore(string leadboardId, long score);


		void ShareScreenshot();
		void ShareImage(string pathToImage);
		void ShareVideo(string pathToVideo);
		void ShareAppLink();

		void ShowBanners();
		void HideBanners();
	}
}
