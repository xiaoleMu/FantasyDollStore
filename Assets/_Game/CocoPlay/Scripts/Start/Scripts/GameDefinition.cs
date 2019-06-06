using UnityEngine;
using System.Collections;
using TabTale;
using strange.extensions.signal.impl;

namespace Game
{
	public enum CocoSceneID
	{
		None = 0,
		CoverPage = 1,
		Map = 2,
		Makeup = 3,
		NailSalon = 4,
		Spa = 5,
		Hairsalon = 6,
		Contest = 7,
		Dressup = 8,
		MakeupFace = 11,
		MakeupEyes = 12,

		DressupCasual = 13,
		DressupDesigner = 14,
		DressupDesigner2 = 15,
		DressupParty = 16,
		DressupSummer = 17,
		DressupUrban = 18,

		PersonalStylist = 19,
		CoverShoot = 20,
		
		DIY = 30,
	    Intro = 31,
	    Video = 32
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
		Portuguese,
	}

	/// <summary>
	/// Coco user interface button I.
	/// </summary>
	public enum CocoUIButtonID
	{
		None = 0,
		Common_Back = 1,
		Common_Ok = 2,
		Common_Complete = 3,
		Common_Next = 4,

		Popup_Close = 100,
		Popup_Save = 101,
		
		
		// DIY --------------
		
		DIY_StepPrev = 150,
		DIY_StepNext = 151,
		DIY_StyleItem = 160,
		DIY_PatternItem = 161,
		DIY_PaintItem= 162,
		DIY_DecorationItem = 163,
		
		DIY_PatternNormal = 164,
		DIY_PatternCustom = 165,
		DIY_PatternDrawing = 166,
		DIY_PatternShoot = 167,
		DIY_PatternAlbum = 168,
		
		DIY_StepCleanContent = 170,
		
		DIY_VideoPlay = 180,
		DIY_VideoExit = 181,
		DIY_VideoPost = 182,
		DIY_VideoPopupClose = 183,
		
		DIY_Request_Avatar = 185,
		DIY_Request_Content_Go = 186,
		DIY_Request_Content_Close = 187,
		DIY_Request_Content_Step = 188,
		
		// Map -------------------------
		Map_Ranking = 302,
		Map_DailyBonus = 303,

		Map_Btn_MoreApps = 310,
		Map_Btn_Store = 311,
		Map_Btn_Contest = 312,
		Map_Btn_PersonalStylist = 313,
		Map_Btn_Hair = 314,
		Map_Btn_MakeupFace = 315,
		Map_Btn_MakeupEyes = 316,
		Map_Btn_DressupCasual = 317,
		Map_Btn_DressupDesigner = 318,
		Map_Btn_DressupDesigner2 = 319,
		Map_Btn_DressupParty = 320,
		Map_Btn_DressupSummer = 321,
		Map_Btn_DressupUrban = 322,
		Map_Btn_NailSalon = 323,
		
		Map_Btn_Video = 340,
		Map_Btn_Order = 341,
		

		Contest_Vote_A = 701,
		Contest_Vote_B = 702,
	}

	/// <summary>
	/// Coco audio I.
	/// </summary>
	public enum CocoProductID
	{
		FullVersion
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
		EyeLine
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


		/// <summary>
		/// 上身 - 背包
		/// </summary>
		Top_Backpack = 0x080000,

		/// <summary>
		/// 其他 - 宠物
		/// </summary>
		Other_Prop = 0x100000,


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
		/// <summary>
		/// 道具 - 眼睛
		/// </summary>
		Other_Prop_Glasses = Other_Prop | Head_Glasses,
	}


	public enum GameRoleID
	{
		Coco = 0,
		Ava = 1,
		Lily = 2,
		Rose = 3,
	}

	public enum GameLayerID
	{
		Default = 0,
		TransparentFX = 1,
		IgnoreRaycast = 2,
		Water = 4,
		UI = 5,
		Background = 8,
		Effect = 9,
		Light = 10,
		CutScene = 11,
		Paint = 12,
		MapModel = 13,
		Video = 14
	}
}


