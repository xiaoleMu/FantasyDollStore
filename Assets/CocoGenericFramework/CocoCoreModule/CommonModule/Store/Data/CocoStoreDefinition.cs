using strange.extensions.signal.impl;

namespace CocoPlay
{
	//信号
	public class CocoStoreUpdateStateSignal : Signal { }//购买成功后更新商店界面

	public enum CocoMiniStoreOpenType{
		AutoOpenMiniStore,
		ItemClickMiniStore,
	}
}
