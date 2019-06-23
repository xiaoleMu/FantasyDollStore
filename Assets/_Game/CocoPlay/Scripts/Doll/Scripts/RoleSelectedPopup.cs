using UnityEngine;
using System.Collections;
using CocoPlay;
using Game;
using System.Collections.Generic;
using UnityEngine.UI;

public class RoleSelectedPopup : CocoGenericPopupBase
{
    [Inject]
    public CocoRoleControl roleControl { get; set; }

    [Inject]
    public GameGlobalData gameGlobalData { get; set; }

    [Inject]
    public GameRecordStateModel recordStateModel { get; set; }

    [Inject]
    public GameRoleStateModel roleStateModel { get; set; }

    [SerializeField] private GameObject[] selectSprites;

    [SerializeField] private Image[] buttonSelectImage;

    [SerializeField] private Sprite unSelectSprite;

    [SerializeField] private Sprite SelectSprite;

    [SerializeField] private GameObject[] parObj;
    [SerializeField] private RawImage m_RawImage;

    private int m_SelectIndex = -1;


    private bool m_IsSelect = false;

    public GameObject closeBtn;

    protected override void OnButtonClickWithButtonName(string buttonName)
    {
        base.OnButtonClickWithButtonName(buttonName);
//        if (m_IsSelect)
//            return;
//        m_IsSelect = true;
//        if (m_SelectIndex != -1)
//        {
//            selectSprites[m_SelectIndex].SetActive(false);
//            buttonSelectImage[m_SelectIndex].sprite = unSelectSprite;
//        }
//
//        GameRoleID tempId = GameRoleID.Ava;
//
//        switch (buttonName)
//        {
//            case "character1":
//                m_SelectIndex = 0;
//                roleStateModel.curRoleId = GameRoleID.Ava.ToString();
//                tempId = GameRoleID.Ava;
//                break;
//            case "character2":
//                m_SelectIndex = 1;
//                roleStateModel.curRoleId = GameRoleID.Mary.ToString();
//                tempId = GameRoleID.Mary;
//                break;
//            case "character3":
//                m_SelectIndex = 2;
//                roleStateModel.curRoleId = GameRoleID.Judi.ToString();
//                tempId = GameRoleID.Judi;
//                break;
//            case "character4":
//                m_SelectIndex = 3;
//                roleStateModel.curRoleId = GameRoleID.Cate.ToString();
//                tempId = GameRoleID.Cate;
//                break;
//        }
//
//        roleStateModel.selectRoleDB.eyeball = gameGlobalData.roleBasicInfos(tempId).eyeball;
//        roleStateModel.selectRoleDB.eyebrow = gameGlobalData.roleBasicInfos(tempId).eyebrow;
//        roleStateModel.selectRoleDB.skincolor = gameGlobalData.roleBasicInfos(tempId).skincolor;
//        roleStateModel.selectRoleDB.facesize = gameGlobalData.roleBasicInfos(tempId).facesize;
//        roleStateModel.selectRoleDB.eyesize = gameGlobalData.roleBasicInfos(tempId).eyesize;
//        roleStateModel.selectRoleDB.nosesie = gameGlobalData.roleBasicInfos(tempId).nosesie;
//        roleStateModel.selectRoleDB.mouthsize = gameGlobalData.roleBasicInfos(tempId).mouthsize;
//		roleStateModel.selectRoleDB.hairColorIndex = gameGlobalData.roleBasicInfos(tempId).hairColorIndex;
//
//        parObj[m_SelectIndex].SetActive(true);
//        selectSprites[m_SelectIndex].SetActive(true);
//        buttonSelectImage[m_SelectIndex].sprite = SelectSprite;
//
//        if (gameGlobalData.AllSelectCharacters != null)
//        {
//            var caDressupPose3 = new CCAnimationData("du_pose03","voice/player@du_pose03");
//            gameGlobalData.AllSelectCharacters[m_SelectIndex].Animation.Play(caDressupPose3);
//        }
//
//        StartCoroutine(SelectRole());
//        if (!gameGlobalData.FirstTimeFlowFinished)
//            CocoFlurry.LogEvent("FirstTimeFlow_2_Character_Selected", "Character", tempId.ToString());
    }

    private IEnumerator SelectRole()
    {
        yield return new WaitForSeconds(3f);
        CloseBtnClick();
    }

    protected override void CloseBtnClick()
    {
        if (m_SelectIndex != -1)
            parObj[m_SelectIndex].SetActive(false);
        base.CloseBtnClick();
    }


    private Transform m_ParentTrans;
    private RenderTexture m_RenderTexture;
    private Camera m_RoleCamera;

    protected override void ShowPopup()
    {
        m_ScaleParent.transform.localScale = Vector3.zero;

        if (m_ParentTrans == null)
            m_ParentTrans = GameObject.Find("Character").transform;

        m_RoleCamera = m_ParentTrans.Find("SelectRoleCamera").GetComponent<Camera>();
        m_RoleCamera.enabled = true;

        m_RenderTexture = new RenderTexture(1024, 650, 24, RenderTextureFormat.ARGB32) {antiAliasing = 4};
        m_RenderTexture.Create();
        m_RoleCamera.targetTexture = m_RenderTexture;
        m_RawImage.texture = m_RenderTexture;
        m_RawImage.gameObject.SetActive(true);

        base.ShowPopup();
    }

    protected override void ClosePopup()
    {
        base.ClosePopup();

        m_RawImage.texture = null;
        if (m_RoleCamera != null)
        {
            m_RoleCamera.targetTexture = null;
            m_RoleCamera.enabled = false;
            m_RawImage.texture = null;
        }
        if (m_RenderTexture != null)
            Destroy(m_RenderTexture);


        Camera.main.targetTexture = null;
    }
		
}