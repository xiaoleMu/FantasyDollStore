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

		#endregion


		//#region Stylist
		//
		//Stylist_Lever = 1050,
		//Photobooth_Photo = 1051,
		//
		//#endregion


		#region scene bg

		Bgm_Common = 3001,
		Bgm_Cover = 3002,
		Bgm_Map = 3003,
		Bgm_Doll = 3004,

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
			AddAudioData (CocoAudioID.Button_Click_01, "sound/1.2");
			AddAudioData (CocoAudioID.Button_Click_02, "sound/1.3");

			AddAudioData (CocoAudioID.Bgm_Common, "bgm/bg_01");
			AddAudioData (CocoAudioID.Bgm_Cover, "bgm/bg_02");
			AddAudioData (CocoAudioID.Bgm_Map, "bgm/bg_03");
			AddAudioData (CocoAudioID.Bgm_Doll, "bgm/bg_04");


			InitSceneBgMusics ();
		}

		private Dictionary<CocoSceneID, CocoAudioID[]> m_SceneBgMusics;

		private void InitSceneBgMusics ()
		{
			m_SceneBgMusics = new Dictionary<CocoSceneID, CocoAudioID[]> {
				{ CocoSceneID.None, new[] { CocoAudioID.Bgm_Common } },
				{ CocoSceneID.CoverPage, new[] { CocoAudioID.Bgm_Cover } },
				{ CocoSceneID.Map, new[] { CocoAudioID.Bgm_Map } },
				{ CocoSceneID.Doll, new[] { CocoAudioID.Bgm_Doll } },
			};
		}

		public CocoAudioID GetSceneBgMusicID (CocoSceneID sceneId)
		{
			CocoAudioID[] audioIds = m_SceneBgMusics.GetValue (sceneId) ?? m_SceneBgMusics.GetValue (CocoSceneID.None);

			return CocoData.GetRandomItem (audioIds);
		}
	}
}