using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;
using Game;


public class GameDollCategoryBtnClickSignal : Signal<GameDollCategoryButton>{}
public class GameDollItemBtnClickSignal : Signal<GameDollItemButton>{}
public class GameDollDetailItemBtnClickSignal : Signal<int>{}

public enum SceneStep {
	None,
	Step_Common,
	Step_Detail,
	Step_Finish
}