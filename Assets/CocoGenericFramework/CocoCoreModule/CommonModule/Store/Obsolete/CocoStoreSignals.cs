using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;

public partial class CocoSignals {
	//商店
	public static Signal <bool> sceneLockMaskSignal = new Signal<bool> ();
	public static Signal refreshButtonRvSignal = new Signal ();
	public static Signal updatePriceSignal = new Signal ();
	public static Signal StorePopupCloseSignal = new Signal ();
	public static Signal <int> sceneLockAutoToMiniStore = new Signal<int> ();
}
