using UnityEngine;
using System.Collections;
using _SRW;



public class Panel_CMDUnitUI : MonoBehaviour 
//public class Panel_CMDUnitUI : Singleton<Panel_CMDUnitUI>
{

	public const string Name = "Panel_CMDUnitUI";

	//static public  cCommand CMD;							// for global operate
	public  cCMD CMD;							// for global operate

	public Panel_unit pUnit; 						// setup it
	public GameObject InfoButton;
	public GameObject MoveButton;
	public GameObject AttackButton;
	public GameObject SkillButton;
	public GameObject SchoolButton;
	public GameObject CancelButton;


	bool bWaitMoveFinish ; 
	// widget Data

	void Clear()
	{
		pUnit = null;
		bWaitMoveFinish = false;
	}
	// Use this for initialization
	void Awake()
	{
		CMD = cCMD.Instance;

		UIEventListener.Get(InfoButton).onClick += OnInfoButtonClick;
		UIEventListener.Get(MoveButton).onClick += OnMoveButtonClick;;
		UIEventListener.Get(AttackButton).onClick += OnAttackButtonClick;;
		UIEventListener.Get(SkillButton).onClick += OnSkillButtonClick;;
		UIEventListener.Get(SchoolButton).onClick += OnSchoolButtonClick;
		UIEventListener.Get(CancelButton).onClick += OnCancelButtonClick;;

		//
		GameEventManager.AddEventListener(  CmdCharMoveEvent.Name , OnCmdCharMoveEvent );
	}

	void Start () {
		//Clear ();  This line will cause first open Ui clear data with setuped. don't clear here
	}
	
	// Update is called once per frame
	void Update () {
		if( pUnit == null )
		{
			//Debug.LogError( "ERR: UnitCMDUI with NULL Unit" );
		//	Clear();
			//nCmderIdent 	= 0;
		//	PanelManager.Instance.CloseUI( Name );
			return ;
		}

		if ( CMD.eCMDSTAT == _CMD_STATUS._TARGET) {  // sel target only
			// check if move end.
			if( pUnit.IsMoving() == false )
			{
				// show atk range
				if( bWaitMoveFinish == true )
				{
					bWaitMoveFinish = false;
					StageShowAttackRangeEvent evt = new StageShowAttackRangeEvent();
					evt.nIdent = CMD.nCmderIdent;
					GameEventManager.DispatchEvent ( evt );
				}

			}
		}
		else if (CMD.eCMDSTAT == _CMD_STATUS._MOVE) {  // move only
			
		}
		else // normal
		{
		}

	}

	void Destroy()
	{
	//	if (pUnit != null) {
	//		pUnit.OnSelected( false );
	//	}
	}

	void OnEnable()
	{

	//	if (pUnit != null) {
	//		pUnit.OnSelected( true );
	//	}
	}

	void OnDisable()
	{
	//	if (pUnit != null) {
	//		pUnit.OnSelected( false );
	//	}
	}

	public void SetCmder( Panel_unit unit )
	{
		//cancel old
		if (pUnit != null) {
			pUnit.OnSelected( false );
		}
		// clear
		Clear ();
		if( unit == null ){

			return ;
		}
		// setup origin param
		pUnit = unit;
		CMD.nCmderIdent = pUnit.Ident();
		// who will disable
		pUnit.OnSelected (true);

		CMD.nOrgGridX = pUnit.X();
		CMD.nOrgGridY = pUnit.Y();


	}
	public void SetTarget( Panel_unit unit )
	{
		CMD.nTarIdent = 0;
		if( unit != null ){
			CMD.nTarIdent = unit.Ident();
		}
		// trig attack event

		// close cmd ui
		Clear ();
		PanelManager.Instance.CloseUI( Name );
	}

	public void CancelCmd( )
	{
		Clear ();
		PanelManager.Instance.CloseUI( Name );
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
	void OnCancelButtonClick(GameObject go)	{
		if (pUnit != null) {
			pUnit.OnSelected (false);
		}

		// 取消
		Clear ();
//		nCharIdent 	= 0;
//		pUnit 		= null;
		PanelManager.Instance.CloseUI( Name );

	}

	// Game Event
	public void OnCmdCharMoveEvent(GameEvent evt)
	{
		CmdCharMoveEvent Evt = evt as CmdCharMoveEvent;
		if (Evt == null)
			return;
		int nIdent = Evt.nIdent;
		int nX =  Evt.nX;
		int nY =  Evt.nY;
		if (nIdent != CMD.nCmderIdent)
			return;
		// entry next phase
		CMD.eCMDSTAT  = _CMD_STATUS._TARGET; // sel target only
		bWaitMoveFinish = true;


	}
}
