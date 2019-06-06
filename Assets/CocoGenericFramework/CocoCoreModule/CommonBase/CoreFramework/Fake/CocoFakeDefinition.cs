using UnityEngine;
using System.Collections;
using TabTale;
using strange.extensions.signal.impl;

namespace CocoPlay.Fake{
	public enum CocoSceneID
	{
		None = 0,
		CoverPage = 1,
		Map = 2,
	}


	public enum CocoLanguage
	{
		English,
		ChineseSimplified,
		Japanese,
		Korean,
		ChineseTraditional,
		Russia,

		French,
		Italien,
		German,
		Spanish,
		Portuguese
	}

	/// <summary>
	/// Coco user interface button I.
	/// </summary>
	public enum CocoUIButtonID
	{
		None,
		Fake_Back,
		Fake_Ok,
		Common_Back,

		Popup_Close,
	}

	/// <summary>
	/// Coco audio I.
	/// </summary>
	public enum CocoAudioID
	{
		None = 0,
		Button_Click_01 = 1,
	}

	public enum CocoProductID
	{
		
	}

	public enum CocoRoleBoneID
	{
		None,
		Root,
		Head,
		Body,
	}

	public enum CocoRoleRendererID
	{
		None,
		Head,
		Body,
		Eyelash,
		EyeBall,
		EyeBrow,
	}

	[System.Flags]
	public enum CocoDressCoverLayer
	{
		None = 0x00,

		// Head (头部) ----------------
		/// <summary>
		/// 头部 - 皮肤
		/// </summary>
		Head_Skin = 0x01,

		/// <summary>
		/// 头部 - 眼球
		/// </summary>
		Head_Eyeball = 0x02,
		/// <summary>
		/// 头部 - 眉毛
		/// </summary>
		Head_Eyebrow = 0x04,
		/// <summary>
		/// 头部 - 睫毛
		/// </summary>
		Head_Eyelash = 0x08,

		/// <summary>
		/// 头部 - 头发
		/// </summary>
		Head_Hair = 0x10,
		/// <summary>
		/// 头部 - 帽子
		/// </summary>
		Head_Hat = 0x20,

		/// <summary>
		/// 头部 - 眼镜
		/// </summary>
		Head_Glasses = 0x40,
		/// <summary>
		/// 头部 - 耳环
		/// </summary>
		Head_Earring = 0x80,

		// Top (上身) ----------------
		/// <summary>
		/// 上身 - 皮肤
		/// </summary>
		Top_Skin = 0x0100,

		/// <summary>
		/// 上身 - 内衣
		/// </summary>
		Top_Underwear = 0x0200,
		/// <summary>
		/// 上身 - 外衣
		/// </summary>
		Top_Coat = 0x0400,

		/// <summary>
		/// 上身 - 项链
		/// </summary>
		Top_Necklace = 0x0800,
		/// <summary>
		/// 上身 - 手镯
		/// </summary>
		Top_Bracelet = 0x1000,
		/// <summary>
		/// 上身 - 手套
		/// </summary>
		Top_Gloves = 0x2000,

		// Bottom (下身) ----------------
		/// <summary>
		/// 下身 - 皮肤
		/// </summary>
		Bottom_Skin = 0x4000,

		/// <summary>
		/// 下身 - 内裤
		/// </summary>
		Bottom_Underwear = 0x8000,
		/// <summary>
		/// 下身 - 丝袜
		/// </summary>
		Bottom_Stocking = 0x010000,
		/// <summary>
		/// 下身 - 外裤
		/// </summary>
		Bottom_Pants = 0x020000,

		/// <summary>
		/// 下身 - 鞋子
		/// </summary>
		Bottom_Shoes = 0x040000,


		// Special Groups (特殊组合) ----------------
		/// <summary>
		/// 身体
		/// </summary>
		Body = Top_Skin | Bottom_Skin,
		/// <summary>
		/// 连衣裙
		/// </summary>
		Dress = Top_Coat | Bottom_Pants,
		/// <summary>
		/// Spa 浴帽
		/// </summary>
		Spa_Hat = Head_Hair | Head_Hat,
	}
}


