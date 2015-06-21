using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using _SRW;



public class Panel_CMDUnitUI : MonoBehaviour 
//public class Panel_CMDUnitUI : Singleton<Panel_CMDUnitUI>
{

	public const string Name = "Panel_CMDUnitUI";



	//static public  cCommand CMD;							// for global operate
	public  cCMD CMD;							// for global operate

	public Panel_unit pCmder; 						// setup it

	public GameObject NGuiGrids;

	public GameObject InfoButton;
	public GameObject MoveButton;
	public GameObject AttackButton;
	public GameObject SkillButton;
	public GameObject SchoolButton;
	public GameObject CancelButton;


//	bool bWaitMoveFinish ; 
	// widget Data

//	void Clear()
//	{
//		pCmder = null;
//		CMD.Clear ();

		//bWaitMoveFinish = false;
//	}
	// Use this for initialization
	void Awake()
	{
		CMD = cCMD.Instance;

	

//		UIEventListener.Get(InfoButton).onClick += OnInfoButtonClick;
//		UIEventListener.Get(MoveButton).onClick += OnMoveButtonClick;;
//		UIEventListener.Get(AttackButton).onClick += OnAttackButtonClick;;
//		UIEventListener.Get(SkillButton).onClick += OnSkillButtonClick;;
//		UIEventListener.Get(SchoolButton).onClick += OnSchoolButtonClick;
//		UIEventListener.Get(CancelButton).onClick += OnCancelButtonClick;;


		//
		//GameEventManager.AddEventListener(  CmdCharMoveEvent.Name , OnCmdCharMoveEvent );

		//==============================		
		ClearCmdPool ();
	}

	void Start () {
		//Clear ();  This line will cause first open Ui clear data with setuped. don't clear here
	}
	
	// Update is called once per frame, but it don't called when disable. can't become a trigger
	void Update () {
		if( pCmder == null )
		{
			//Debug.LogError( "ERR: UnitCMDUI with NULL Unit" );
		//	Clear();
			//nCmderIdent 	= 0;
		//	PanelManager.Instance.CloseUI( Name );
			return ;
		}

		if ( CMD.eCMDTARGET == _CMD_TARGET._UNIT ) {  // sel target only
			// check if move end.
	//		if( pCmder.IsMoving() == false )
			{
				// show atk range
//				if( bWaitMoveFinish == true )
				{
//					bWaitMoveFinish = false;
//					StageShowAttackRangeEvent evt = new StageShowAttackRangeEvent();
//					evt.nIdent = CMD.nCmderIdent;
//					GameEventManager.DispatchEvent ( evt );
				}

			}
		}
		else if (CMD.eCMDTARGET == _CMD_TARGET._POS) {  // move only
			
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

	void OnEnable() // before start
	{
		// create cmd by type
		CreateCMDList ( cCMD.Instance.eCMDTYPE );

	//	if (pUnit != null) {
	//		pUnit.OnSelected( true );
	//	}
	}

	void OnDisable()
	{
		// don't cleat in disable. this bug in close api
	//	ClearCmdPool ();
	//	if (pUnit != null) {
	//		pUnit.OnSelected( false );
	//	}
	}

	void ClearCmdPool()
	{
		if (NGuiGrids == null) {
			return ;
		}
		UIGrid grid = NGuiGrids.GetComponent<UIGrid>(); 
		if (grid == null) {
			return ;
		}
		List< Transform > lst = grid.GetChildList ();
		//List< GameObject > CmdBtnList = MyTool.GetChildPool( NGuiGrids );

		if (lst == null)
			return;

		foreach ( Transform t in lst) {

			///UIEventListener.Get(obj).onClick -= OnCMDButtonClick;;  // no need.. destory soon
			NGUITools.Destroy( t.gameObject );
		}

	}

	void CreateCMDList( _CMD_TYPE eType )
	{
		ClearCmdPool (); // always clear first
		List< _CMD_ID >  cmdList =  cCMD.Instance.GetCmdList ( eType );
		if (cmdList == null)
			return;
	
		if (NGuiGrids == null)
			return;
		// record cmd type
		CMD.eCMDTYPE = eType;

		foreach( _CMD_ID id in cmdList )
		{
			GameObject obj = ResourcesManager.CreatePrefabGameObj( this.NGuiGrids , "Prefab/CMD_BTN" ); // create cmd and add to grid
			if( obj != null )
			{
				obj.name = MyTool.GetCMDNameByID( id );
				UILabel lbl = obj.GetComponentInChildren<UILabel> ();
				if( lbl != null )
				{
					lbl.text = obj.name ;
					// Load Label by const data
				}
				UIEventListener.Get(obj).onClick += OnCMDButtonClick;
			}
		}
		// update
		UIGrid grid = NGuiGrids.GetComponent<UIGrid>(); 
		grid.repositionNow = true;		// need this for second pop to re pos

		//NGuiGrids.SetActive (true);

	}

//	void NormalCloseCmdUI()
//	{
//		PanelManager.Instance.CloseUI( Name );
//		if (pCmder != null) {
//			pCmder.OnSelected (false);
//		}
//		pCmder = null;
//		CMD.Clear ();
//		// send clear over
//		Panel_StageUI.Instance.ClearOverCellEffect ();
//	}

	public void CancelCmd( )
	{
		// if it is wait mode. restore it
		if( CMD.eCMDTYPE == _CMD_TYPE._WAITATK ||
		    CMD.eCMDTYPE == _CMD_TYPE._WAITMOVE //
		   )
		{
			// restore cmd 
			RestoreCMD( );
			return;
		}
	
		EndCMDUI (); // really close

	}


	public void WaitCmd( )
	{
		// this is one kind of cmd that reduce cmd times
		//if (pCmder) {
		//	pCmder.ActionFinished ();
			// cmd finish
			//StageUnitActionFinishEvent cmd = new StageUnitActionFinishEvent ();
			//cmd.nIdent = pCmder.Ident();
			//GameEventManager.DispatchEvent ( cmd );
		//}
		ActionManager.Instance.CreateWaitingAction ( pCmder.Ident() );

	
		EndCMDUI (); // really close

	}

	public void CharInfoCmd( )
	{
		GameDataManager.Instance.nInfoIdent = pCmder.Ident ();

		PanelManager.Instance.OpenUI ( Panel_UnitInfo.Name );

		//Clear ();
		//NormalCloseCmdUI ();.
		EndCMDUI (); // really close
	}

	public void AttackCmd( )
	{

		// normal atk

		CMD.eCMDSTATUS = _CMD_STATUS._WAIT_TARGET;
		CMD.eCMDID 	   = _CMD_ID._ATK;
		CMD.eCMDTARGET = _CMD_TARGET._UNIT;   // only unit


		Panel_StageUI.Instance.ClearOverCellEffect ();
		Panel_StageUI.Instance.CreateAttackOverEffect (pCmder);
		PanelManager.Instance.CloseUI( Name );
	}

	public void AbilityCmd( )
	{
		if (pCmder != null) {
			Panel_Skill.OpenUI (pCmder.Ident () , _SKILL_TYPE._ABILITY );
		}
	}

	public void SkillCmd( )
	{
		if (pCmder != null) {
			Panel_Skill.OpenUI (pCmder.Ident () , _SKILL_TYPE._SKILL );
		}
	}
	

	public void CounterCmd( ) // counter atk
	{

		BattleManager.Instance.nDeferSkillID = 0; // counter normaly
			
		EndCMDUI ();					
	}
	public void  RoundEndCmd(  )
	{
		GameDataManager.Instance.NextCamp();

		// restore all allay cmd times;
		EndCMDUI (); // really close
	}

	public void DefCmd( ) // only in count mode
	{
		BattleManager.Instance.eDefCmdID = _CMD_ID._DEF;

		EndCMDUI (); // really close
	}

	public void  RunSuicide(  )
	{
		PanelManager.Instance.CloseUI( Name );
		if (pCmder != null) {
			pCmder.OnSelected ( false );
			cUnitData pCmdData  = GameDataManager.Instance.GetUnitDateByIdent( pCmder.Ident() );
			if( pCmdData != null ){
				pCmdData.AddHp( -999999999 );
			}
			//pCmder.SetDead ();
		}
		pCmder = null;
		CMD.Clear ();
		// send clear over
		Panel_StageUI.Instance.ClearOverCellEffect ();
	}
	// untility func
	public void RestoreCMD( )
	{
		Panel_StageUI.Instance.ClearOverCellEffect ();
		if( pCmder )
		{
			//Panel_StageUI.Instance.SynGridToLocalPos( pCmder.gameObject , CMD.nOrgGridX , CMD.nOrgGridY );
			pCmder.SetXY(  CMD.nOrgGridX , CMD.nOrgGridY  );
			Panel_StageUI.Instance.CreateMoveOverEffect ( pCmder );
		}
		CMD.eCMDSTATUS  = _CMD_STATUS._WAIT_CMDID;
		CMD.eCMDTYPE 	= _CMD_TYPE._ALLY; // only ally can restore
		CMD.eCMDID = _CMD_ID._NONE;
		CMD.eCMDTARGET = _CMD_TARGET._ALL;

		// reopen for build cmd list
		PanelManager.Instance.CloseUI( Name );
		PanelManager.Instance.OpenUI( Name );
		//CreateCMDList ( cCMD.Instance.eCMDTYPE );
	}
	// pre
	public void SetCmder( Panel_unit unit )
	{
		//cancel old
		if (pCmder != null) {
			pCmder.OnSelected( false );
		}
		// clear	

		// setup origin param
		pCmder = unit;
		if( pCmder == null ){			
			return ;
		}


		// who will disable
		pCmder.OnSelected (true);

		// CMD param
		CMD.nCmderIdent = pCmder.Ident();
		CMD.nOrgGridX = pCmder.X();
		CMD.nOrgGridY = pCmder.Y();

		// keep cmd type
		CMD.eCMDSTATUS = _CMD_STATUS._WAIT_CMDID;
		CMD.eCMDTARGET = _CMD_TARGET._ALL;
		CMD.eCMDID 	   = _CMD_ID._NONE;	

	}
	//post
	public void SetPos( int nGridX , int nGridY )
	{
		// trig attack event
		
		// close cmd ui
		//Clear ();
		MakeCmd ();
		// check Need Make Cmd

		PanelManager.Instance.CloseUI( Name );
	}


	public void SetTarget( Panel_unit unit )
	{
		CMD.nTarIdent = 0;
		if( unit != null ){
			CMD.nTarIdent = unit.Ident();
		}
		// trig attack event
		// check Need Make Cmd
		MakeCmd ();
		// close cmd ui
		//Clear ();
		PanelManager.Instance.CloseUI( Name );
	}

	public void SetSkillID( int nSkillID )
	{
		if (0 == nSkillID) {
			// cancel skill ui
			CMD.nSkillID = 0;
		}
		else {
			// set a skill
			CMD.nSkillID = nSkillID;

			SKILL skl = ConstDataManager.Instance.GetRow<SKILL>( nSkillID ); 

			switch( skl.n_TARGET )
			{
			case 0:		// self cast
				CMD.eCMDID 	   = _CMD_ID._ATK;			// enter atk mode
				CMD.eCMDTARGET = _CMD_TARGET._POS;
				CMD.nTarGridX = -1;
				CMD.nTarGridY = -1;

				MakeCmd();

				Panel_StageUI.Instance.ClearOverCellEffect ();
				CMD.Clear ();				// clear cmd status
				break;
			case 1:		// select a target enemy
				// if skill need a target. go to target mode
				CMD.eCMDSTATUS = _CMD_STATUS._WAIT_TARGET;
				CMD.eCMDID 	   = _CMD_ID._ATK;			// enter atk mode
				
				Panel_StageUI.Instance.ClearOverCellEffect ();
				Panel_StageUI.Instance.CreateAttackOverEffect (pCmder , skl.n_RANGE );
				
				CMD.eCMDTARGET = _CMD_TARGET._UNIT;

				break;
			}


		}
		//


		PanelManager.Instance.CloseUI( Name );
	}

	public void MakeCmd( )
	{	
		if (pCmder == null) {
			return ;
		}
		// make cmd and send to all plane
		pCmder.OnSelected ( false );
		switch (CMD.eCMDTARGET)
		{
			case _CMD_TARGET._POS:
			{
				switch( CMD.eCMDID )
				{
					case _CMD_ID._ATK:{ // attack cmd
						BattleManager.Instance.PlayCast( CMD.nCmderIdent , CMD.nTarGridX , CMD.nTarGridY ,CMD.nSkillID );
				
					}break;
				}
			}break;

			case _CMD_TARGET._UNIT:
			{
				switch( CMD.eCMDID )
				{
					case _CMD_ID._ATK:{ // attack cmd
						BattleManager.Instance.PlayAttack( CMD.nCmderIdent , CMD.nTarIdent ,CMD.nSkillID );
					
					}break;
				}
			}break;
		}
		// Set pos

		// send clear over
		Panel_StageUI.Instance.ClearOverCellEffect ();
		CMD.Clear ();				// clear cmd status
		PanelManager.Instance.CloseUI (Name);
		// start battle
	}

	//click
	void OnCMDButtonClick(GameObject go)
	{
		string name = go.name;

		//_CMD_ID id = MyTool.GetCMDIDByName ( name );
		if (name == _CMD_ID._INFO.ToString ()) {
			CharInfoCmd ();
		} else if (name == _CMD_ID._MOVE.ToString ()) {
			// no need 
		} else if (name == _CMD_ID._ATK.ToString ()) {
			AttackCmd ();
		} else if (name == _CMD_ID._ABILITY.ToString ()) {
			AbilityCmd();
		} else if (name == _CMD_ID._SKILL.ToString ()) {
			SkillCmd ();
		} else if (name == _CMD_ID._WAIT.ToString ()) {
			WaitCmd ();
		} else if (name == _CMD_ID._CANCEL.ToString ()) {
			CancelCmd ();
		} else if (name == _CMD_ID._ROUNDEND.ToString ()) {
			RoundEndCmd ();
		} else if (name == _CMD_ID._DEF.ToString ()) {
			DefCmd ();
		} else if (name == _CMD_ID._COUNTER.ToString ()) {
			CounterCmd ();
		}

// cheat code
		else if (name == _CMD_ID._SUICIDE.ToString ()) {
			RunSuicide(  );
		}
		else if (name == _CMD_ID._WIN.ToString ()) {
			PanelManager.Instance.OpenUI( Panel_Win.Name );
			//RunSuicide(  );
		}
		else if (name == _CMD_ID._LOST.ToString ()) {
			//RunSuicide(  );
			PanelManager.Instance.OpenUI( Panel_Lost.Name );
		}
//


	}

//	void OnInfoButtonClick(GameObject go)
//	{
//		// 查情報
//
//	}
//	void OnMoveButtonClick(GameObject go)
//	{
//	}
//	void OnAttackButtonClick(GameObject go)
//	{
//
//
//	}
//	void OnSkillButtonClick(GameObject go)
//	{
//	}
//	void OnSchoolButtonClick(GameObject go)
//	{
//		// 結束遊戲
//	}
//	void OnCancelButtonClick(GameObject go)	{
//		CancelCmd ();
//	}

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
	//	CMD.eCMDTYPE = _CMD_TYPE._WAITATK;  // change cmd type
		//CMD.eCMDSTAT  = _CMD_TARGET._TARGET; // sel target only

	//	bWaitMoveFinish = true;


	}

	// cmd STATIC FUNC 
	public static Panel_CMDUnitUI OpenCMDUI( _CMD_TYPE type , int nIdent )
	{	
		return OpenCMDUI (type, Panel_StageUI.Instance.GetUnitByIdent (nIdent));

	}
	public static Panel_CMDUnitUI OpenCMDUI( _CMD_TYPE type , Panel_unit cmder )
	{
		cCMD.Instance.eCMDTYPE = type; 
		Panel_CMDUnitUI panel = MyTool.GetPanel<Panel_CMDUnitUI> ( PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name) );
		if( panel != null )
		{
			panel.SetCmder( cmder );
		}
		cCMD.Instance.eCMDSTATUS = _CMD_STATUS._WAIT_CMDID;
		
		return panel;
	}

	// not reall close cmd ui . SOME param keep for ui re open or restore
	public static void CloseCMDUI()
	{
		Panel_CMDUnitUI panel = MyTool.GetPanel<Panel_CMDUnitUI>( PanelManager.Instance.JustGetUI(Panel_CMDUnitUI.Name)) ;
		if( panel != null )
		{
			panel.CancelCmd();
		}
		if (panel.pCmder != null) {
			panel.pCmder.OnSelected ( false );
		}

		Panel_StageUI.Instance.ClearOverCellEffect();
		cCMD.Instance.eCMDSTATUS = _CMD_STATUS._NONE;
	}
	
	public static void RollBackCMDUIWaitTargetMode()
	{
		cCMD.Instance.eCMDSTATUS = _CMD_STATUS._WAIT_CMDID;
		cCMD.Instance.eCMDID 	   = _CMD_ID._NONE;
		cCMD.Instance.eCMDTARGET = _CMD_TARGET._ALL;   // only unit
		Panel_StageUI.Instance.ClearOverCellEffect ();
		PanelManager.Instance.OpenUI( Panel_CMDUnitUI.Name );
	}

	// really close Cmd UI . all param be clear
	void EndCMDUI(  )
	{
		Panel_StageUI.Instance.ClearOverCellEffect();
//		cCMD.Instance.eCMDSTATUS = _CMD_STATUS._NONE;
		PanelManager.Instance.CloseUI( Name );

		if (pCmder != null) {
			pCmder.OnSelected ( false );
			//pCmder.SetDead ();
			pCmder = null;
		}
		CMD.Clear ();
	}
}
