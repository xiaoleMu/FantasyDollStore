using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TabTale;
using CocoPlay;

namespace Game {
	public class GameABTestChangeStoreIcon : GameView {
		[SerializeField]
		Sprite m_StoreIcon;
		[SerializeField]
		Sprite m_ADFreeIcon;

		[Inject]
		public CocoGlobalRecordModel globalRecordModel {get; set;}

		// Use this for initialization
		void Start () {
			#if ABTEST

			switch (globalRecordModel.CurGPType){
			case GPType.Test_A:
				GetComponent<Image>().sprite = m_StoreIcon;
				break;

			case GPType.Test_B:
				GetComponent<Image>().sprite = m_ADFreeIcon;
				break;

				default:
				GetComponent<Image>().sprite = m_StoreIcon;
				break;
			}

			#else

			GetComponent<Image>().sprite = m_StoreIcon;

			#endif
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}
