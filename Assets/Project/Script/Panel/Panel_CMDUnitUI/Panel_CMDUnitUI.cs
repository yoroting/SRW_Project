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

	public int 	nAtkerId; 					// counter attacker

	public GameObject NGuiGrids;

	public GameObject CmdButton;

//	public GameObject InfoButton;
//	public GameObject MoveButton;
//	public GameObject AttackButton;
//	public GameObject SkillButton;
//	public GameObject SchoolButton;
//	public GameObject CancelButton;


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

		CmdButton.CreatePool ( 10 );
		CmdButton.SetActive (false);

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
		if (CMD == null)
			return; // this is error

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
	//	CreateCMDList ( cCMD.Instance.eCMDTYPE );

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
		Panel_MiniUnitInfo.CloseUI ();
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

			UIEventListener.Get(t.gameObject).onClick -= OnCMDButtonClick;;  // need for objpool 
		//	NGUITools.Destroy( t.gameObject );
		}
		CmdButton.RecycleAll ();

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
		CMD.eLASTCMDTYPE = CMD.eCMDTYPE;
		CMD.eCMDTYPE = eType;

		foreach( _CMD_ID id in cmdList )
		{	
			//GameObject obj = ResourcesManager.CreatePrefabGameObj( this.NGuiGrids , "Prefab/CMD_BTN" ); // create cmd and add to grid
			GameObject obj = CmdButton.Spawn( NGuiGrids.transform );
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

	
		if( _CMD_TYPE._WAITATK == CMD.eCMDTYPE )
		{
			// auto show atk cell
			Panel_StageUI.Instance.ClearOverCellEffect ();
			Panel_StageUI.Instance.CreateAttackOverEffect (pCmder);

		}

		// show sample info
		if (pCmder != null) {
			Panel_MiniUnitInfo.OpenUI (pCmder.pUnitData );
		}
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

	static public void CancelCmd( )
	{
		Panel_CMDUnitUI plane = JustGetCMDUI ();
		// if it is wait mode. restore it
		if( cCMD.Instance.eCMDTYPE == _CMD_TYPE._WAITATK ||
		   cCMD.Instance.eCMDTYPE == _CMD_TYPE._WAITMOVE //
		   )
		{
			// restore cmd 
			plane.RestoreCMD( );
			return;
		}
	
		plane.EndCMDUI (); // really close

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
		//GameDataManager.Instance.nInfoIdent = pCmder.Ident ();
		//PanelManager.Instance.OpenUI ( Panel_UnitInfo.Name );
		Panel_UnitInfo.OpenUI(  pCmder.Ident () );

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
			Panel_Skill.OpenUI (pCmder.Ident () , _SKILL_TYPE._ABILITY , nAtkerId , cCMD.Instance.eCMDTYPE);
		}
	}

	public void SkillCmd( )
	{
		if (pCmder != null) {
			Panel_Skill.OpenUI (pCmder.Ident () , _SKILL_TYPE._SKILL ,nAtkerId , cCMD.Instance.eCMDTYPE );
		}
	}
	

	public void CounterCmd( ) // counter atk
	{

		BattleManager.Instance.nDeferSkillID = 0; // counter normaly
		BattleManager.Instance.eDefCmdID	= _CMD_ID._COUNTER;	
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

	public void SaveCmd( ) // only in count mode
	{
		// save to 1
		//cSaveData.Save (1 , _SAVE_PHASE._STAGE );
		Panel_SaveLoad.OpenSaveMode ( _SAVE_PHASE._STAGE );

		EndCMDUI (); // really close
	}

	public void LoadCmd( ) // only in count mode
	{
		// always load from stage
		// cSaveData.Load (1 , _SAVE_PHASE._STAGE , Panel_StageUI.Instance.gameObject  );
		// cSaveData.Load (1, _SAVE_PHASE._STAGE);
	//	Panel_SaveLoad.  .OpenLoadMode ( );
		Panel_SaveLoad.OpenLoadMode ( _SAVE_PHASE._STAGE );

		EndCMDUI (); // really close
	}

	public void GameEndCmd ()
	{
		EndCMDUI (); // really close

		// close stage ui
		if (PanelManager.Instance.CheckUIIsOpening (Panel_StageUI.Name)){
			// entry endstage
			Panel_StageUI.Instance.EndStage ();

			Panel_StageUI.Instance.ShowStage (false);
			// free here waill cause some  StartCoroutine of stageUI break 
			PanelManager.Instance.DestoryUI( Panel_StageUI.Name ); 
		}

		// reopen main UI
		PanelManager.Instance.OpenUI( MainUIPanel.Name );
	}

	public void  RunSuicide(  )
	{
		PanelManager.Instance.CloseUI( Name );
		if (pCmder != null) {
			pCmder.OnSelected ( false );
			cUnitData pCmdData  = GameDataManager.Instance.GetUnitDateByIdent( pCmder.Ident() );
			if( pCmdData != null ){
				pCmdData.AddHp( -2099999999 );
			}
			//pCmder.SetDead ();
		}
		pCmder = null;
		CMD.Clear ();
		// send clear over
		Panel_StageUI.Instance.ClearOverCellEffect ();
	}

	public void  StageCheatCmd(  )
	{
	//	GameDataManager.Instance.nMoney += 99999999;
	//	PanelManager.Instance.OpenUI (Panel_SysCheat.Name);
	}

	public void  StageSysCheatCmd(  )
	{
		//	GameDataManager.Instance.nMoney += 99999999;
		PanelManager.Instance.OpenUI (Panel_SysCheat.Name);
	}

	public void KillAllEnemyCmd ()
	{
		foreach (KeyValuePair< int , cUnitData> pair in GameDataManager.Instance.UnitPool) {
			if( pair.Value != null ){
				if( pair.Value.eCampID == _CAMP._ENEMY ){
					pair.Value.AddHp( -2099999999 );
				}
			}
		}
		Panel_StageUI.Instance.CheckUnitDead (true);

		EndCMDUI();
	}

	//==================================
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
		OpenCMDUI ( CMD.eCMDTYPE , null );

		//PanelManager.Instance.CloseUI( Name );
		//PanelManager.Instance.OpenUI( Name );
		//CreateCMDList ( cCMD.Instance.eCMDTYPE );
		Panel_StageUI.Instance.MoveToGameObj (pCmder.gameObject ,false , 0.2f );
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
	// 攻擊來源
	public void SetAttacker( int  atkerid )
	{
		nAtkerId = atkerid;
	}

	//post
	public void SetPos( int nGridX , int nGridY )
	{
		CMD.nTarGridX = nGridX;
		CMD.nTarGridY = nGridY;

		// trig attack event
		if( CMD.nAOEID > 0 ){
			CMD.eCMDAOETARGET =  _CMD_TARGET._POS;
			//int x = nGridX;
			//int y = nGridY;
			
			Panel_StageUI.Instance.ClearOverCellEffect ();
			Panel_StageUI.Instance.CreateAOEOverEffect( pCmder  , nGridX , nGridY , CMD.nAOEID);

			Panel_CheckBox panel = GameSystem.OpenCheckBox();
			if (panel) {
				panel.SetAoeCheck();
			}
			return;	// block cmd
		}

		cCMD.Instance.eCMDTARGET =  _CMD_TARGET._POS;

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

		if( CMD.nAOEID > 0 ){
			CMD.eCMDAOETARGET =  _CMD_TARGET._UNIT;

			int x = pCmder.X();
			int y = pCmder.Y();
			if( unit != null ){
				x = unit.X();
				y = unit.Y();
			}
			Panel_StageUI.Instance.ClearOverCellEffect ();
			Panel_StageUI.Instance.CreateAOEOverEffect( pCmder , x , y , CMD.nAOEID );
			Panel_CheckBox panel = GameSystem.OpenCheckBox();
			if (panel) {
				panel.SetAoeCheck();
			}
			return;
			// block cmd
		}

		cCMD.Instance.eCMDTARGET =  _CMD_TARGET._UNIT;

		// trig attack event
		// check Need Make Cmd
		MakeCmd ();
		// close cmd ui
		//Clear ();
		PanelManager.Instance.CloseUI( Name );
	}

	public void AOE_OK(){

		cCMD.Instance.eCMDTARGET =  CMD.eCMDAOETARGET;
		MakeCmd ();
		// close cmd ui
		//Clear ();
		PanelManager.Instance.CloseUI( Name );
	}

	public void AOE_Cancel(){

		// clear AOE
		Panel_StageUI.Instance.ClearOverCellEffect ();
		if (CMD.eCMDTARGET == _CMD_TARGET._SELF) {
			Panel_Skill.OpenUI (pCmder.Ident (), _SKILL_TYPE._LAST ,0 , cCMD.Instance.eCMDTYPE );
		} else {
			int nRange = 1;
			if (CMD.nSkillID > 0) {
				SKILL skl = ConstDataManager.Instance.GetRow<SKILL> (CMD.nSkillID);
				nRange = skl.n_RANGE;
			}
			Panel_StageUI.Instance.CreateAttackOverEffect (pCmder, nRange);

		}
		if (CMD.eCMDTYPE == _CMD_TYPE._WAITATK) {
//			int nRange = 1;
//			if (CMD.nSkillID > 0) {
//				SKILL skl = ConstDataManager.Instance.GetRow<SKILL> (CMD.nSkillID);
//				nRange = skl.n_RANGE;
//			}
//			Panel_StageUI.Instance.CreateAttackOverEffect (pCmder, nRange);
		} else {
			//CancelCmd();
			// open skill ui

			//PanelManager.Instance.OpenUI( Panel_Skill.Name );

		}
	}

	public void SetSkillID( int nSkillID )
	{
		if (0 == nSkillID) {
			// cancel skill ui
			CMD.nSkillID = 0;
			CMD.nAOEID = 0;
		}
		else {
			// set a skill
			SKILL skl = ConstDataManager.Instance.GetRow<SKILL>( nSkillID ); 
			CMD.nSkillID = nSkillID;
			CMD.nAOEID = skl.n_AREA;

			// need take care counter
			if( CMD.eCMDTYPE == _CMD_TYPE._COUNTER )
			{
				// counter cast
				if( skl.n_FINISH > 0 ){
					 // set def skill to battle manager
					BattleManager.Instance.nDeferSkillID = nSkillID;// let battle manager to cast it
					BattleManager.Instance.eDefCmdID	= _CMD_ID._COUNTER; // start to counter
					EndCMDUI();
					return ; // this is finish task
				}
				else{

					BattleManager.Instance.PlayCounterCast( pCmder.Ident() , nAtkerId , nSkillID );
					//cast directly don't make a battle cmd to destory original enemy atk cmd

					Panel_CMDUnitUI.CloseCMDUI(); // normal close

				}
			}
			else{
				// normal cast
				switch( skl.n_TARGET )
				{
				case 0:		// self cast
					CMD.eCMDID 	   = _CMD_ID._ATK;			// enter atk mode
					CMD.eCMDTARGET = _CMD_TARGET._SELF;
					CMD.nTarGridX = pCmder.X() ;
					CMD.nTarGridY = pCmder.Y();

					MakeCmd();
					Panel_StageUI.Instance.ClearOverCellEffect ();
					CMD.Clear ();				// clear cmd status
					break;
				case 1:		// select a target enemy
				case 2:		// select a target ally
				{	// if skill need a target. go to target mode
					CMD.eCMDSTATUS = _CMD_STATUS._WAIT_TARGET;
					CMD.eCMDID 	   = _CMD_ID._ATK;			// enter atk mode
					
					Panel_StageUI.Instance.ClearOverCellEffect ();
					Panel_StageUI.Instance.CreateAttackOverEffect (pCmder , skl.n_RANGE );
					
					CMD.eCMDTARGET = _CMD_TARGET._UNIT;

					// show AOE
					//if( CMD.nAOEID > 0 ){
					//	Panel_StageUI.Instance.CreateAOEOverEffect( pCmder.X() , pCmder.Y() , CMD.nAOEID );
					//}


				}break;
				case 3:
				case 4:
				{  // Map AOE
					CMD.eCMDSTATUS = _CMD_STATUS._WAIT_TARGET;
					CMD.eCMDID 	   = _CMD_ID._ATK;			// enter atk mode
					
					Panel_StageUI.Instance.ClearOverCellEffect ();
					Panel_StageUI.Instance.CreateAttackOverEffect (pCmder , skl.n_RANGE );
					
					CMD.eCMDTARGET = _CMD_TARGET._POS;

				}
				break;

				// always Self AOE 
				case 6:
				case 7:
				{
					// show AOE direct
					CMD.eCMDSTATUS = _CMD_STATUS._WAIT_TARGET;
					CMD.eCMDID 	   = _CMD_ID._ATK;			// enter atk mode
					//CMD.eCMDAOETARGET = _CMD_TARGET._AOE;
					CMD.eCMDTARGET = _CMD_TARGET._SELF;		// this is a mark  flag
					CMD.nTarGridX = pCmder.X() ;
					CMD.nTarGridY = pCmder.Y();
					if(CMD.nAOEID==0)
						CMD.nAOEID = 1;  // avoid error aoe block cmd
					//if( CMD.nAOEID > 0 )
					//{
					CMD.eCMDAOETARGET = _CMD_TARGET._POS;
					Panel_StageUI.Instance.CreateAOEOverEffect( pCmder ,  pCmder.X() , pCmder.Y() , CMD.nAOEID );
					//}
					Panel_CheckBox panel = GameSystem.OpenCheckBox();
					if (panel) {
						panel.SetAoeCheck();
					}
					//MakeCmd();
					//Panel_StageUI.Instance.ClearOverCellEffect ();
					//CMD.Clear ();				// clear cmd status
					break;

				}
				break;
				}
			}//else{
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
						BattleManager.Instance.PlayCast( CMD.nCmderIdent , 0 ,  CMD.nTarGridX , CMD.nTarGridY ,CMD.nSkillID );
				
					}break;
				}
			}break;

			case _CMD_TARGET._UNIT:
			{
				switch( CMD.eCMDID )
				{
					case _CMD_ID._ATK:{ // attack cmd
						if( MyTool.IsDamageSkill( CMD.nSkillID )== true  ){
							BattleManager.Instance.PlayAttack( CMD.nCmderIdent , CMD.nTarIdent ,CMD.nSkillID );
						}
						else {
					BattleManager.Instance.PlayCast( CMD.nCmderIdent , CMD.nTarIdent , CMD.nTarGridX , CMD.nTarGridY ,CMD.nSkillID );	

						}
					}break;
				}
			}break;

		case _CMD_TARGET._SELF:
		{
			switch( CMD.eCMDID )
			{
			case _CMD_ID._ATK:{ // attack cmd
				BattleManager.Instance.PlayCast( CMD.nCmderIdent , CMD.nCmderIdent , CMD.nTarGridX , CMD.nTarGridY ,CMD.nSkillID );
				
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
			AbilityCmd ();
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
		} else if (name == _CMD_ID._SAVE.ToString ()) {
			SaveCmd ();
		} else if (name == _CMD_ID._LOAD.ToString ()) {
			LoadCmd ();
		}	else if (name == _CMD_ID._GAMEEND.ToString ()) {
			GameEndCmd ();
		}

// cheat code
		else if (name == _CMD_ID._SUICIDE.ToString ()) {
			RunSuicide ();
		}
		else if (name == _CMD_ID._CHEAT.ToString ()) {
			//StageSysCheatCmd ();
		}
		else if (name == _CMD_ID._SYSCHEAT.ToString ()) {
			StageSysCheatCmd ();
		}
		else if (name == _CMD_ID._WIN.ToString ()) {
			PanelManager.Instance.OpenUI (Panel_Win.Name);
			//RunSuicide(  );
		} else if (name == _CMD_ID._LOST.ToString ()) {
			//RunSuicide(  );
			PanelManager.Instance.OpenUI (Panel_Lost.Name);
		} else if (name == _CMD_ID._SYSCHEAT.ToString ()) {
			StageCheatCmd ();
		}
		else if (name == _CMD_ID._KILLALLE.ToString ()) {
			KillAllEnemyCmd ();
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
	public static Panel_CMDUnitUI OpenCMDUI( _CMD_TYPE type , int nIdent,int TarIdent=0  )
	{	
		return OpenCMDUI (type, Panel_StageUI.Instance.GetUnitByIdent (nIdent) , TarIdent );

	}
	public static Panel_CMDUnitUI OpenCMDUI( _CMD_TYPE type , Panel_unit cmder ,int TarIdent=0  )
	{
		cCMD.Instance.eCMDTYPE = type; 
		Panel_CMDUnitUI panel = MyTool.GetPanel<Panel_CMDUnitUI> ( PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name) );
		if( panel == null )
		{
			return panel;
		}

		// if cmder is change
		if (cmder != null) {
			panel.SetCmder (cmder);

			Panel_StageUI.Instance.MoveToGameObj( cmder.gameObject );

			// show sample info
			if (cmder != null) {
				Panel_MiniUnitInfo.OpenUI ( cmder.pUnitData );
			}
			//Panel_StageUI.Instance.TraceUnit( cmder );
		}
		panel.SetAttacker (TarIdent);

		cCMD.Instance.eCMDSTATUS = _CMD_STATUS._WAIT_CMDID;

		panel.CreateCMDList (type);
		//if (PanelManager.Instance.CheckUIIsOpening (Panel_CMDUnitUI.Name)) {
		//}


		return panel;
	}

	public static Panel_CMDUnitUI JustGetCMDUI(  )
	{
		return MyTool.GetPanel< Panel_CMDUnitUI >( PanelManager.Instance.JustGetUI( Panel_CMDUnitUI.Name ) ); 
	}

	// not reall close cmd ui . SOME param keep for ui re open or restore
	public static void CloseCMDUI()
	{
		Panel_CMDUnitUI panel = MyTool.GetPanel<Panel_CMDUnitUI>( PanelManager.Instance.JustGetUI(Panel_CMDUnitUI.Name)) ;
		if( panel != null )
		{
			//panel.CancelCmd(); // cancel will restore some time. can't call it here
		}
		if (panel.pCmder != null) {
			panel.pCmder.OnSelected ( false );
		}

		Panel_StageUI.Instance.ClearOverCellEffect();
		cCMD.Instance.eCMDSTATUS = _CMD_STATUS._NONE;
	}
	
	public static void RollBackCMDUIWaitTargetMode()
	{
		// always clear all effect
		Panel_StageUI.Instance.ClearOverCellEffect (); // for atk cell

		if (cCMD.Instance.nSkillID > 0) {
			Panel_Skill.OpenUI( Panel_CMDUnitUI.JustGetCMDUI().pCmder.Ident() , _SKILL_TYPE._LAST , 0 , cCMD.Instance.eCMDTYPE);
			cCMD.Instance.nSkillID =0;
			return ;
		}


		cCMD.Instance.eCMDSTATUS = _CMD_STATUS._WAIT_CMDID;
		cCMD.Instance.eCMDID 	   = _CMD_ID._NONE;
		cCMD.Instance.eCMDTARGET = _CMD_TARGET._ALL;   // only unit
		//cCMD.Instance.nSkillID = 0;
	

		//Panel_CMDUnitUI panel = JustGetCMDUI ();
		//panel.gameObject.SetActive (true);
		//Panel_CMDUnitUI panel =	MyTool.CMDUI ();
		// need reopen ui only . don't change any param
		GameObject obj = PanelManager.Instance.OpenUI ( Name );    // need areopen here 
		Panel_CMDUnitUI panel = obj.GetComponent<Panel_CMDUnitUI>();


		 //= PanelManager.Instance.OpenUI( Panel_CMDUnitUI.Name );
		if (cCMD.Instance.eCMDTYPE == _CMD_TYPE._WAITATK) {
			Panel_StageUI.Instance.CreateAttackOverEffect( panel.pCmder );
		} else {
			Panel_StageUI.Instance.CreateMoveOverEffect ( panel.pCmder ); // recreate
		}

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
