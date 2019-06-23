using UnityEngine;
using System.Collections;
using CocoPlay;
using Game;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class RoleSelectedPopup : CocoGenericPopupBase
{
    [SerializeField] private RawImage m_RawImage;


    private Transform m_ParentTrans;
    private RenderTexture m_RenderTexture;
    private Camera m_RoleCamera;

    protected override void ShowPopup()
    {
		InitRoleBtns ();

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

		m_VBtn.OnClickEvent -= OnPopupCloseEvent;
		m_CloseBtn.OnClickEvent -= OnPopupCloseEvent;
		for (int i=0; i<m_RoleBtns.Length; i++){
			m_RoleBtns[i].OnClickEvent -= OnRoleClick;
		}

		if (OnCloseDollSelect != null)
			OnCloseDollSelect (m_ChangeDoll);
    }
		

	#region Btn

	public Action<bool> OnCloseDollSelect;

	[Inject]
	public GameDollData dollData {get; set;}

	[SerializeField]
	RoleClickButton[] m_RoleBtns;
	[SerializeField]
	CocoUINormalButton m_VBtn;
	[SerializeField]
	CocoUINormalButton m_CloseBtn;

	private RoleClickButton m_CurRoleBtn = null;

	private void InitRoleBtns (){
		m_VBtn.OnClickEvent += OnPopupCloseEvent;
		m_CloseBtn.OnClickEvent += OnPopupCloseEvent;
		for (int i=0; i<m_RoleBtns.Length; i++){
			if (dollData.curSelectRole == i){
				m_RoleBtns[i].ChangeStatus (true);
				m_CurRoleBtn = m_RoleBtns[i];
			}
			else {
				m_RoleBtns[i].ChangeStatus (false);
			}

			m_RoleBtns[i].OnClickEvent += OnRoleClick;
		}
	}

	private void OnRoleClick (CocoUINormalButton btn){
		RoleClickButton t_Btn = (RoleClickButton)btn;
		if (m_CurRoleBtn != null){
			m_CurRoleBtn.ChangeStatus (false);
		}

		dollData.curSelectRole = t_Btn.Index;
		t_Btn.ChangeStatus (true);
		m_CurRoleBtn = t_Btn;
	}

	bool m_ChangeDoll = false;
	private void OnPopupCloseEvent (CocoUINormalButton btn){
		if (btn == m_VBtn){
			m_ChangeDoll = true;
		}
		else {
			m_ChangeDoll = false;
		}
	}

	#endregion
}