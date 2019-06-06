using System.Collections.Generic;
using CocoPlay;

namespace Game
{
	public enum CocoAudioID
	{
		None = 0,


		#region soundeffect

		Button_Click_01 = 1001,
		Button_Click_02 = 1002,
		Button_Click_03 = 1003,
		Button_Click_04 = 1004,
		Button_Click_05 = 1005,
		Button_Click_06 = 1006,
		Button_Click_07 = 1007,

		//Box_Click = 1008,
		Box_Open = 1009,
		Coins_Drop = 1010,
		//Contest_Applause = 1011,
		Contest_Cards_In = 1012,
		Contest_Cheering = 1013,
		Contest_Counting_Down = 1014,
		Contest_Lighting_Up_Stars = 1015,
		Contest_Miss = 1016,
		Contest_Not_Winning = 1017,
		//Contest_Picturing = 1018,
		Contest_Plosive = 1019,
		Contest_Score_Raising = 1020,
		Contest_VS_In = 1021,
		Fireworks = 1022,

		Hair_Salon_Blower = 1023,
		Hair_Salon_Shampoo = 1024,
		Hair_Salon_Shower_Head = 1025,

		Open_Coins = 1026,
		//Open_Fans = 1027,
		Open_Items = 1028,
		//Rank_Up = 1029,
		//Skill_Bar = 1030,
		//Casher = 1031,

		#endregion


		//#region Stylist
		//
		//Stylist_Lever = 1050,
		//Photobooth_Photo = 1051,
		//
		//#endregion


		#region scene bg

		Bgm_Common = 3001,
		Bgm_DIY = 3002,
		Bgm_Map = 3003,
		Bgm_Contest = 3004,
		Bgm_Clip_01 = 3021,
		Bgm_Clip_02 = 3022,
		Bgm_Clip_03 = 3023,

		#endregion


		agegate_scroll = 4001,


		#region Contest

		//Contest_Tip_Appear = 715,
		Contest_Negative_Sound = 716,

		#endregion
	}


	public class GameAudioData : CocoAudioBaseData
	{
		//public static string player_pose_01 = "voice/player01@photo_01";
		//public static string player_pose_02 = "voice/player01@photo_02";
		//public static string player_pose_03 = "voice/player01@photo_03";

		//dress
		//public static string player_photo_01 = "voice/player01@photo_01";
		//public static string player_photo_02 = "voice/player01@photo_02";
		//public static string player_photo_03 = "voice/player01@photo_03";
		public static string player_dress_clothes = "voice/player@dress_clothes";
		public static string player_dress_nolike = "voice/player@dress_nolike";
		public static string player_dress_shoes = "voice/player@dress_shoes";


		//public static string player02_pose = "voice/player02@pose";

		//public static string player_lose01 = "voice/player@lose01";
		//public static string player_lose02 = "voice/player@lose02";
		//public static string player_congratulate01 = "voice/player@congratulate01";
		//public static string player_congratulate02 = "voice/player@congratulate02";

		//public static string player_negate = "voice/player@negate";
		//public static string player_personal_prop_01 = "voice/player@personal_prop_01";
		//public static string player_personal_prop_02 = "voice/player@personal_prop_02";
		//public static string player_personal_prop_03 = "voice/player@personal_prop_03";
		//public static string player_personal_prop_04 = "voice/player@personal_prop_04";
		//public static string player_personal_prop_05 = "voice/player@personal_prop_05";

		public static string player_salon_zhangwang = "voice/player@salon_zhangwang";
		public static string player_salon_zhaojingzi = "voice/player@salon_zhaojingzi";
		public static string player_salon_smile = "voice/player@salon_smile";
		//public static string player_salon_win = "voice/player@salon_win";

		//public static string player_trying01 = "voice/player@trying01";
		//public static string player_trying02 = "voice/player@trying02";
		//public static string player_vote_select = ""; //voice/player@vote_select
		//public static string player_vote_win = "voice/player@vote_win";

		//public static string contest_resultpop_zero = "sound_effect/negative_sound";


		#region new SFX

		public static string DIY_Decoration_Placing = "sfx/Placing_3D_decorations_sound";
		public static string Request_Follow_Unhappy = "sfx/avatar_change_to_unhappy_emoji_sound_effect";
		public static string DIY_Paint_Brush = "sfx/brush_paintting_sound";
		public static string DIY_Cut_MoveingOut = "sfx/clothes_cut_away_and_moving_out_of_screen";
		//public static string Video_Comment_Appear = "sfx/comment_appera_sound";
		public static string Request_New_Content = "sfx/computer_icon_call_to_action_sound";
		public static string DIY_EditBox_Delete = "sfx/delete_pattern_painting_decoration_sound";
		public static string DIY_Pattern_Drag = "sfx/pattern_texture_draging_sound";
		//public static string Request_Result_V = "sfx/request_result_V";
		//public static string Request_Result_X = "sfx/request_result_X";
		public static string DIY_Cut_Scissors = "sfx/scissors";
		//public static string DIY_Sticker_Pasting = "sfx/sticker_pasting_sound";

		#endregion


		public override void InitAudioDatas ()
		{
			AddAudioData (CocoAudioID.Button_Click_01, "sound_effect/button_click_01");
			AddAudioData (CocoAudioID.Button_Click_02, "sound_effect/button_click_02");
			AddAudioData (CocoAudioID.Button_Click_03, "sound_effect/button_click_03");
			AddAudioData (CocoAudioID.Button_Click_04, "sound_effect/button_click_04");
			AddAudioData (CocoAudioID.Button_Click_05, "sound_effect/button_click_05");
			AddAudioData (CocoAudioID.Button_Click_06, "sound_effect/button_click_06");
			AddAudioData (CocoAudioID.Button_Click_07, "sound_effect/button_click_07");
			//AddAudioData (CocoAudioID.Box_Click, "sound_effect/box_click");
			AddAudioData (CocoAudioID.Box_Open, "sound_effect/box_open");
			AddAudioData (CocoAudioID.Coins_Drop, "sound_effect/coin_drop");
			//AddAudioData (CocoAudioID.Contest_Applause, "sound_effect/contest_applause");
			AddAudioData (CocoAudioID.Contest_Cards_In, "sound_effect/contest_cards_in");
			AddAudioData (CocoAudioID.Contest_Cheering, "sound_effect/contest_cheering");
			AddAudioData (CocoAudioID.Contest_Counting_Down, "sound_effect/contest_counting_down");
			AddAudioData (CocoAudioID.Fireworks, "sound_effect/contest_fireworks");
			AddAudioData (CocoAudioID.Contest_Lighting_Up_Stars, "sound_effect/contest_lighting_up_stars");
			AddAudioData (CocoAudioID.Contest_Miss, "sound_effect/contest_miss");
			AddAudioData (CocoAudioID.Contest_Not_Winning, "sound_effect/contest_not_winning");
			//AddAudioData (CocoAudioID.Contest_Picturing, "sound_effect/contest_picturing");
			AddAudioData (CocoAudioID.Contest_Plosive, "sound_effect/contest_plosive");
			AddAudioData (CocoAudioID.Contest_Score_Raising, "sound_effect/contest_score_raising");
			AddAudioData (CocoAudioID.Contest_VS_In, "sound_effect/contest_vs_in");
			AddAudioData (CocoAudioID.Hair_Salon_Blower, "sound_effect/hair_salon_blower");
			AddAudioData (CocoAudioID.Hair_Salon_Shampoo, "sound_effect/hair_salon_shampoo");
			AddAudioData (CocoAudioID.Hair_Salon_Shower_Head, "sound_effect/hair_salon_shower_head");
			AddAudioData (CocoAudioID.Open_Coins, "sound_effect/open_coins");
			//AddAudioData (CocoAudioID.Open_Fans, "sound_effect/open_fans");
			AddAudioData (CocoAudioID.Open_Items, "sound_effect/open_items");
			//AddAudioData (CocoAudioID.Rank_Up, "sound_effect/rank_up");
			//AddAudioData (CocoAudioID.Skill_Bar, "sound_effect/skill_bar");
			//AddAudioData (CocoAudioID.Casher, "sound_effect/casher");
			AddAudioData (CocoAudioID.Contest_Negative_Sound, "sound_effect/negative_sound");

			//AddAudioData (CocoAudioID.Stylist_Lever, "sound_effect/stylist_lever");
			//AddAudioData (CocoAudioID.Photobooth_Photo, "sound_effect/photobooth_photo");

			AddAudioData (CocoAudioID.Bgm_Common, "bgm/coco_diy_activities_music");
			AddAudioData (CocoAudioID.Bgm_DIY, "bgm/coco_diy_diy_music");
			AddAudioData (CocoAudioID.Bgm_Map, "bgm/coco_diy_map_music");
			AddAudioData (CocoAudioID.Bgm_Contest, "bgm/coco_diy_selfie_music");
			AddAudioData (CocoAudioID.Bgm_Clip_01, "bgm/coco_diy_youtube_clip_music_1");
			AddAudioData (CocoAudioID.Bgm_Clip_02, "bgm/coco_diy_youtube_clip_music_2");
			AddAudioData (CocoAudioID.Bgm_Clip_03, "bgm/coco_diy_youtube_clip_music_3");

			AddAudioData (CocoAudioID.agegate_scroll, "sound_effect/machinery");

			InitSceneBgMusics ();
		}

		private Dictionary<CocoSceneID, CocoAudioID[]> m_SceneBgMusics;

		private void InitSceneBgMusics ()
		{
			m_SceneBgMusics = new Dictionary<CocoSceneID, CocoAudioID[]> {
				{ CocoSceneID.None, new[] { CocoAudioID.Bgm_Common } },
				{ CocoSceneID.Map, new[] { CocoAudioID.Bgm_Map } },
				{ CocoSceneID.Contest, new[] { CocoAudioID.Bgm_Contest } },
				{ CocoSceneID.DIY, new[] { CocoAudioID.Bgm_DIY } }
			};
		}

		public CocoAudioID GetSceneBgMusicID (CocoSceneID sceneId)
		{
			CocoAudioID[] audioIds = m_SceneBgMusics.GetValue (sceneId) ?? m_SceneBgMusics.GetValue (CocoSceneID.None);

			return CocoData.GetRandomItem (audioIds);
		}
	}
}