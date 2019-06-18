using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TabTale;
using CocoPlay;
using System;

#if COCO_FAKE
using CocoStoreID = CocoPlay.Fake.CocoStoreID;
using CocoSceneID = CocoPlay.Fake.CocoSceneID;
#else
using CocoStoreID = Game.CocoStoreID;
using CocoSceneID = Game.CocoSceneID;
#endif

public class SceneLockManager : CocoGenericPopupBase
{
	#if COCO_FAKE
	[Inject]
	public CocoGlobalData GlobalData {get; set;}
	#else
	[Inject]
	public Game.GameGlobalData GlobalData {get; set;}
	#endif

	[Inject]
	public ICocoStoreConfigData m_ConfigData {get; set;}

	[SerializeField]
	Image backButton;
	[SerializeField]
	Image homeButton;

    [SerializeField]
    CocoSceneID needCountDownScene;

	[SerializeField] private float _autoOpenDelay = 2f;

	protected override void Start ()
	{
		base.Start ();
		if (GlobalData.BackToSceneID != CocoSceneID.None)
		{
			homeButton.sprite = backButton.sprite;
		}
	}

	protected override void OnButtonClickWithButtonName (string buttonName)
	{
		base.OnButtonClickWithButtonName (buttonName);

		if (buttonName == "scenelock"){
			OpenMiniStore (CocoMiniStoreOpenType.ItemClickMiniStore);
		}
		else if(buttonName == "BackScene"){
			CloseBtnClick ();
		}
	}

	protected override void CloseBtnClick ()
	{
		ClosePopup ();

		if (GlobalData.BackToSceneID != CocoSceneID.None)
		{
			CocoMainController.EnterScene (GlobalData.BackToSceneID);
		}
		else{
			CocoMainController.EnterScene (CocoSceneID.Map);
		}
	}

	protected override void ShowPopup ()
	{
		base.ShowPopup ();

		StartCoroutine (ShowMiniStore ());
	}
		
	IEnumerator ShowMiniStore()
	{
		if (AutoOpenMiniStore){
			AutoOpenMiniStore = false;
			yield return new WaitForSeconds(_autoOpenDelay);
		
			OpenMiniStore (CocoMiniStoreOpenType.AutoOpenMiniStore);
		}
	}

	private void OpenMiniStore (CocoMiniStoreOpenType storeOpenType){
		if (m_OpeningMiniStore) return;

		m_OpeningMiniStore = true;
//		CocoMainController.Instance.ShowMiniStorePopup(OpenMiniStoreID, storeOpenType);
	}

	[Inject]
	public CocoPopupClosedSignal PopupCloseSignal {get; set;}
	[Inject]
	public CocoStoreControl StoreControl {get; set;} 

	bool AutoOpenMiniStore = true;
	bool m_OpeningMiniStore = false;
	public static CocoStoreID OpenMiniStoreID;
	public static DateTime UnlockTime = DateTime.MinValue;

	protected override void AddListeners ()
	{
		base.AddListeners ();

		PopupCloseSignal.AddListener (OnPopupClose);
	}

	protected override void RemoveListeners ()
	{
		PopupCloseSignal.RemoveListener (OnPopupClose);

		base.RemoveListeners ();
	}

	protected void OnPopupClose (string key){
		if (key == "MiniStore" || key == "MainStore"){
			m_OpeningMiniStore = false;
			if (StoreControl.IsPurchased (OpenMiniStoreID)){
				ClosePopup ();
			}
		}
	}

    [SerializeField]
    Text m_CountTimeText;
    [SerializeField]
    GameObject countDownPanel;


    IEnumerator TimeCount ()
    {
        if (m_CountTimeText == null)
            yield break;

        TimeSpan remainingTime;
        while (true)
        {
			remainingTime = UnlockTime - DateTime.Now;
            //Debug.LogError(remainingTime.TotalSeconds);
            if (remainingTime.TotalSeconds <= 0)
            {
                OnFinishTimeCountDown ();
                yield break;
            }

            string timeText = string.Format ("{0:D2}:{1:D2}:{2:D2}", remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds);
            m_CountTimeText.text = timeText;

            yield return new WaitForSeconds (1);
        }
    }

    Coroutine countDownCoroutine;
    void OnFinishTimeCountDown(){
        if (countDownCoroutine != null)
        {
            StopCoroutine(countDownCoroutine);
        }
        ClosePopup ();
    }

    protected override void Awake()
    {
        base.Awake();
        InitCountPanel();

    }

    void InitCountPanel(){

		if (UnlockTime > DateTime.Now)
        {
            countDownPanel.SetActive(true);
        } else
        {
            countDownPanel.SetActive(false);
        }
    }

   
    void OnEnable(){

        if (countDownPanel.activeSelf)
        {

            if (countDownCoroutine != null)
            {
                StopCoroutine(countDownCoroutine);
            }
            countDownCoroutine = StartCoroutine(TimeCount());
        }
    }

}
