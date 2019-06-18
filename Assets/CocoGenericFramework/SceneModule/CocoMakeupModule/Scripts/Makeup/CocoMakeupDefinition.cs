using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;

namespace CocoPlay
{
	public class CocoMakeupPaintSignal : Signal<Vector2, CocoMakeupCategory>{}
	public class CocoMakeupPaintEndSignal : Signal<CocoMakeupCategory>{}
	public class MakeupClearSignal : Signal<CocoMakeupCategory>{}

	//枚举值
	public enum CocoMakeupCategory
	{
		None,
		Eyebrow,
		Eyecolor,
		Eyelash,
		Eyeline,
		Eyeshadow,
		Lipgloss,
		Lipstick,
		Cheek,
		Painting,
		ForeheadPainting,
		Grease,
		Beards,
		Accessories,
		Hair,
		Eyecolor_Boy,
		Cheek_Boy,
		Painting_Boy,
		Hair_Boy,
		Accessories_Boy,
		Moustache,
		girl_bracelet,
		girl_headdress,
		boy_earring,
		boy_necklace,
		boy_eyebrow,
		GirlHairColor,
		BoyHairColor,
		GirlHat,
		BoyHat,
		BoySkinColor,
		GirlSkinColor,
		BoyBeardColor,
		BoyMoustacheColor,
		GirlNecklace,
		GirlEarring
	}

	public enum CocoMakeupPaintType
	{
		ChangeTexture,
		PaintTexture
	}

	public enum CocoMakeupPaintLayer
	{
		Head,
		Head_Layer1,
		Head_Layer2,
		Head_Layer3,
		EyeLash,
		EyeBrow,
		Eye,
		EyeLine
	}
}
