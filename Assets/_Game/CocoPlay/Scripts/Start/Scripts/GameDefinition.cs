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
		Doll = 3,
		RecordVolue = 4,
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
		Head = 0x01,
		Body = 0x02,
		Arm = 0x04,
		Ear = 0x08,
		Eye = 0x10,
		Leg = 0x20,
		Nose = 0x40,
		Tail = 0x80,
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


