using UnityEngine;
using System.Collections;

public class Panel_CMDUnitUI : MonoBehaviour {

	public GameObject InfoButton;
	public GameObject MoveButton;
	public GameObject AttackButton;
	public GameObject SkillButton;
	public GameObject SchoolButton;
	public GameObject CancelButton;


	// Use this for initialization
	void Start () {
		UIEventListener.Get(InfoButton).onClick += OnInfoButtonClick;
		UIEventListener.Get(MoveButton).onClick += OnMoveButtonClick;;
		UIEventListener.Get(AttackButton).onClick += OnAttackButtonClick;;
		UIEventListener.Get(SkillButton).onClick += OnSkillButtonClick;;
		UIEventListener.Get(SchoolButton).onClick += OnSchoolButtonClick;
		UIEventListener.Get(CancelButton).onClick += OnCancelButtonClick;;



	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//click
	void OnInfoButtonClick(GameObject go)
	{
		// 查情報
	}
	void OnMoveButtonClick(GameObject go)
	{
	}
	void OnAttackButtonClick(GameObject go)
	{
	}
	void OnSkillButtonClick(GameObject go)
	{
	}
	void OnSchoolButtonClick(GameObject go)
	{
		// 結束遊戲
	}
	void OnCancelButtonClick(GameObject go)
	{
		// 取消
		PanelManager.Instance.CloseUI( "Panel_CMDSYSUI" );
	}

}
