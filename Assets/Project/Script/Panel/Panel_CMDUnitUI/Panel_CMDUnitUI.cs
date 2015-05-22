using UnityEngine;
using System.Collections;

public class Panel_CMDUnitUI : MonoBehaviour {

	public const string Name = "Panel_CMDUnitUI";

	static public int nCharIdent;			// Operatr char ident

	Panel_unit pUnit; 						// setup it

	public GameObject InfoButton;
	public GameObject MoveButton;
	public GameObject AttackButton;
	public GameObject SkillButton;
	public GameObject SchoolButton;
	public GameObject CancelButton;


	// widget Data

	// Use this for initialization
	void Awake()
	{
		UIEventListener.Get(InfoButton).onClick += OnInfoButtonClick;
		UIEventListener.Get(MoveButton).onClick += OnMoveButtonClick;;
		UIEventListener.Get(AttackButton).onClick += OnAttackButtonClick;;
		UIEventListener.Get(SkillButton).onClick += OnSkillButtonClick;;
		UIEventListener.Get(SchoolButton).onClick += OnSchoolButtonClick;
		UIEventListener.Get(CancelButton).onClick += OnCancelButtonClick;;

	}

	void Start () {
		if( pUnit == null )
			return ;



	}
	
	// Update is called once per frame
	void Update () {
		if( pUnit == null )
		{
			Debug.Log( "ERR: SysCMDUI with NULL Unit" );
			nCharIdent 	= 0;
			PanelManager.Instance.CloseUI( Name );
		}

	}


	public void Setup( Panel_unit unit )
	{
		pUnit = unit;
		if( pUnit == null ){
			nCharIdent = 0;
			return ;
		}
		nCharIdent = pUnit.ID();
		// who will disable

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
		nCharIdent 	= 0;
		pUnit 		= null;
		PanelManager.Instance.CloseUI( Name );
	}

}
