using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using _SRW;
using MYGRIDS;
using MyClassLibrary;			// for parser string



public class Panel_StageUI : MonoBehaviour 
//public class Panel_StageUI : Singleton<Panel_StageUI>
{
	public const string Name = "Panel_StageUI";

	const int st_CellObjPoolSize  = 100;		//


	static Panel_StageUI instance;
	public static Panel_StageUI Instance
	{
		get
		{
//			#if UNITY_EDITOR
//			if (isApplicationQuit)
//				return null;
//			#endif

			if(instance == null) // special get 
			{
				GameObject go = PanelManager.Instance.JustGetUI( Name );
				if( go ){
					instance = go.GetComponent<Panel_StageUI>(); 
					//return go.GetComponent<Panel_StageUI>(); 
				}
			}			
			return instance;
		}
	}


	public GameObject BackGroundObj; // back ground
	public GameObject TilePlaneObj; // plane of all tiles sprite
	public GameObject MaskPanelObj; // plane mask


	public GameObject UnitPanelObj; // unit plane

	//public GameObject TileObj; 	//  no need this
	public GameObject MoveEftObj; 	// 
	public GameObject AtkEftObj; 	// 
	public GameObject AoeEftObj; 	// 
	public GameObject ValueEftObj; 	// 

	Panel_unit TarceMoveingUnit; //  Trace the moving unit

	public cMyGrids	Grids;				// main grids . only one

	STAGE_DATA	StageData;

	// Four flaf
	bool	bIsLoading	;

	public bool	bIsStageEnd;								// this stage is end
	bool	bIsReady;

	// drag canvas limit								
	float fMinOffX ;
	float fMaxOffX ;
	float fMinOffY ;
	float fMaxOffY ;

	// widget
	Dictionary< int , STAGE_EVENT > EvtPool;			// add event id 
	List< STAGE_EVENT >				WaitPool;			// wait to exec pool
//	Dictionary< int , int > EvtCompletePool;			// record event complete round 

	STAGE_EVENT						NextEvent;
	private cTextArray 				m_cScript;			// 劇本 腳本集合
	private int						m_nFlowIdx;			// 腳本演到哪段
	bool							IsEventEnd;			// 
	bool							IsPreBattleEvent;	// is prebattle event
//	bool							CheckEventPause;	// 	event check pause

	// Select effect
	Dictionary< string , GameObject >  OverCellPool;			// Over tile effect pool ( in = cell key )
	Dictionary< string , GameObject >  OverCellAtkPool;			// Over tile effect pool( attack ) ( in = cell key )

	Dictionary< string , GameObject > OverCellPathPool;
	Dictionary< string , GameObject > OverCellAOEPool;
	//List< GameObject >				OverCellPool;			 

	//Dictionary< int , GameObject > EnemyPool;			// EnemyPool this should be group pool


	Dictionary< int , Panel_unit > IdentToUnit;			// allyPool
	//Dictionary< int , UNIT_DATA > UnitDataPool;			// ConstData pool


	// ScreenRatio
	// float fUIRatio;
	public void CreateAllDataPool()
	{

		StageData = new STAGE_DATA();
		//event

		EvtPool = new Dictionary< int , STAGE_EVENT >();			// add event id 

		WaitPool = new List< STAGE_EVENT >();					// check ok. waitinf to execute event
		//EvtCompletePool = new Dictionary< int , int >();
		
		// unit
		IdentToUnit = new Dictionary< int , Panel_unit >();		//
		
		
		OverCellPool 	= new Dictionary< string , GameObject >();			// Over tile effect pool ( in = cell key )
		OverCellAtkPool = new Dictionary< string , GameObject >();					
		OverCellPathPool= new Dictionary< string , GameObject >();

		OverCellAOEPool= new Dictionary< string , GameObject >();


		//List < iVec2 >
		UnitPanelObj.CreatePool( st_CellObjPoolSize / 2 );

		MoveEftObj.CreatePool( st_CellObjPoolSize );
		AtkEftObj.CreatePool( st_CellObjPoolSize );
		AoeEftObj.CreatePool( st_CellObjPoolSize /4 );

		ValueEftObj.CreatePool( st_CellObjPoolSize / 10  );
	}

	public void RegeditGameEvent( bool bTrue )
	{
		if( bTrue ){
		// Stage Event
		GameEventManager.AddEventListener(  StageBGMEvent.Name , OnStageBGMEvent );
		
		GameEventManager.AddEventListener(  StagePopUnitEvent.Name , OnStagePopUnitEvent );
		GameEventManager.AddEventListener(  StagePopGroupEvent.Name , OnStagePopGroupEvent );
		
		//	GameEventManager.AddEventListener(  StageDelCharEvent.Name , OnStageDelCharEvent ); // different func form some little different process
		//	GameEventManager.AddEventListener(  StageDelMobEvent.Name , OnStageDelMobEvent );
		GameEventManager.AddEventListener(  StageDelUnitEvent.Name , OnStageDelUnitEvent );
		
		GameEventManager.AddEventListener(  StageDelUnitByIdentEvent.Name , OnStageDelUnitByIdentEvent );
		
		GameEventManager.AddEventListener(  StageMoveToUnitEvent.Name , OnStageMoveToUnitEvent );

		// char event 
		GameEventManager.AddEventListener(  StageCharMoveEvent.Name , OnStageCharMoveEvent );
	//	GameEventManager.AddEventListener(  StageUnitActionFinishEvent.Name , OnStageUnitActionFinishEvent );
		GameEventManager.AddEventListener(  StageWeakUpCampEvent.Name , OnStageWeakUpCampEvent );
		
		
		// cmd event
		GameEventManager.AddEventListener(  StageShowMoveRangeEvent.Name , OnStageShowMoveRangeEvent );
		GameEventManager.AddEventListener(  StageShowAttackRangeEvent.Name , OnStageShowAttackRangeEvent );
		GameEventManager.AddEventListener(  StageRestorePosEvent.Name , OnStageRestorePosEvent );
		
		
		GameEventManager.AddEventListener(  StageBattleAttackEvent.Name , OnStageBattleAttackEvent );
		}
		else 
		{

			// need remove. or it will send to a destory obj
			GameEventManager.RemoveEventListener(  StageBGMEvent.Name , OnStageBGMEvent );
			
			GameEventManager.RemoveEventListener(  StagePopUnitEvent.Name , OnStagePopUnitEvent );
			GameEventManager.RemoveEventListener(  StagePopGroupEvent.Name , OnStagePopGroupEvent );
			
			//	GameEventManager.AddEventListener(  StageDelCharEvent.Name , OnStageDelCharEvent ); // different func form some little different process
			//	GameEventManager.AddEventListener(  StageDelMobEvent.Name , OnStageDelMobEvent );
			GameEventManager.RemoveEventListener(  StageDelUnitEvent.Name , OnStageDelUnitEvent );
			
			GameEventManager.RemoveEventListener(  StageDelUnitByIdentEvent.Name , OnStageDelUnitByIdentEvent );
			
			GameEventManager.RemoveEventListener(  StageMoveToUnitEvent.Name , OnStageMoveToUnitEvent );
			// char event 
			GameEventManager.RemoveEventListener(  StageCharMoveEvent.Name , OnStageCharMoveEvent );
			//		GameEventManager.RemoveEventListener(  StageUnitActionFinishEvent.Name , OnStageUnitActionFinishEvent );
			GameEventManager.RemoveEventListener(  StageWeakUpCampEvent.Name , OnStageWeakUpCampEvent );
			
			
			// cmd event
			GameEventManager.RemoveEventListener(  StageShowMoveRangeEvent.Name , OnStageShowMoveRangeEvent );
			GameEventManager.RemoveEventListener(  StageShowAttackRangeEvent.Name , OnStageShowAttackRangeEvent );
			GameEventManager.RemoveEventListener(  StageRestorePosEvent.Name , OnStageRestorePosEvent );
			
			
			GameEventManager.RemoveEventListener(  StageBattleAttackEvent.Name , OnStageBattleAttackEvent );
		}

	}

	void Awake( ){	
		instance = this; 		// special singleton
		bIsReady = false;
		bIsStageEnd = false;
		//UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);	
		//fUIRatio = (float)mRoot.activeHeight / Screen.height;

		//float ratio = (float)mRoot.activeHeight / Screen.height;

		// UI Event
		// UIEventListener.Get(BackGroundObj).onClick += OnBackGroundClick;
	
		// grid
		//Grids = new cMyGrids ();
		GameScene.Instance.Initial ();							// it will avoid double initial inside.

		Grids = GameScene.Instance.Grids;						// smart pointer reference


	//	ActionPool = List< uAction >();				// record all action to do 
		// Debug Code jump to stage
	//	GameDataManager.Instance.nStageID = 1;


		// create singloten

		// can't open panel in awake
		// Don't open create panel in stage awake. i have develop mode that place stage in scene initial. the panelmanager don't initial here
	

	}



	void OnEnable()
	{
		// start the loading panel

	}

	void Clear()
	{
		//
		ClearOverCellEffect ();

		// clear unit 
		foreach (KeyValuePair<int , Panel_unit  > pair in IdentToUnit) {
			NGUITools.Destroy( pair.Value.gameObject );
		}
		IdentToUnit.Clear ();

		// EVENT 
		GameDataManager.Instance.ResetStage ();

		//GameDataManager.Instance.nRound = 0;		// many mob pop in talk ui. we need a 0 round to avoid issue
		//GameDataManager.Instance.nActiveCamp  = _CAMP._PLAYER;
		
		//Record All Event to execute
		EvtPool.Clear();

		WaitPool.Clear ();
		//EvtCompletePool.Clear ();

		IsEventEnd = false;
		bIsStageEnd = false;
		bIsReady = false;

	}

//	IEnumerator StageLoading(  )
//	{
//		// Custom Update Routine which repeats forever
//		do
//		{
//			// wait one frame and continue
//			yield return 0;
//					
//			if ( bIsReady == true )
//			{
//				Debug.Log( "LoadingCoroutine End"  + Time.time );
//				// end
//				PanelManager.Instance.CloseUI( "Panel_Loading");
//				yield break;
//			}		
//		} while (true);
//
//	}


	// Use this for initialization
	void Start () {

		long tick = System.DateTime.Now.Ticks;

		System.GC.Collect();			// Free memory resource here

		// create pool
		CreateAllDataPool();

		Debug.Log( "stage srart loding"  );

		// loading panel
	//	PanelManager.Instance.OpenUI( "Panel_Loading");
		bIsLoading = true;
	//	StartCoroutine("StageLoading" );

		// clear data
		Clear ();
		
		Debug.Log( "stageloding:clearall"  );		
		// load const data
		StageData = ConstDataManager.Instance.GetRow<STAGE_DATA> ( GameDataManager.Instance.nStageID );
		if( StageData == null ){
			Debug.LogFormat( "stageloding:StageData fail with ID {0}  "  , GameDataManager.Instance.nStageID );
			return;
		}
		
		// load scene file
		if( LoadScene( StageData.n_SCENE_ID ) == false ){
			Debug.LogFormat( "stageloding:LoadScene fail with ID {0} "   , StageData.n_SCENE_ID );
			return;
		}
		
		// EVENT 
		//GameDataManager.Instance.nRound = 0;		// many mob pop in talk ui. we need a 0 round to avoid issue
		
		//Record All Event to execute
		//EvtPool.Clear();
		char [] split = { ';' };
		string [] strEvent = StageData.s_EVENT.Split( split );
		for( int i = 0 ; i< strEvent.Length ; i++ )
		{
			int nEventID = int.Parse( strEvent[i] );
			STAGE_EVENT evt = ConstDataManager.Instance.GetRow<STAGE_EVENT> ( nEventID );
			if( evt != null ){
				EvtPool.Add( nEventID , evt );
			}
		}
		
		Debug.Log( "stageloding:create event Pool complete"   );

//		// clear data
//		Clear ();
//
//		// load const data
//		StageData = ConstDataManager.Instance.GetRow<STAGE_DATA> ( GameDataManager.Instance.nStageID );
//		if( StageData == null )
//			return ;
//		
//		// load scene file
//		if( LoadScene( StageData.n_SCENE_ID ) == false )
//			return ;
//		
//		// EVENT 
//		//GameDataManager.Instance.nRound = 0;		// many mob pop in talk ui. we need a 0 round to avoid issue
//
//		//Record All Event to execute
//		//EvtPool.Clear();
//		char [] split = { ';' };
//		string [] strEvent = StageData.s_EVENT.Split( split );
//		for( int i = 0 ; i< strEvent.Length ; i++ )
//		{
//			int nEventID = int.Parse( strEvent[i] );
//			STAGE_EVENT evt = ConstDataManager.Instance.GetRow<STAGE_EVENT> ( nEventID );
//			if( evt != null ){
//				EvtPool.Add( nEventID , evt );
//			}
//		}
		

		// regedit game event
		RegeditGameEvent( true );	
		// create sub panel?

		Panel_CMDUnitUI.OpenCMDUI( _CMD_TYPE._SYS , 0 );
		PanelManager.Instance.OpenUI( Panel_UnitInfo.Name );
		PanelManager.Instance.OpenUI( Panel_MiniUnitInfo.Name );

		//Dictionary< int , STAGE_EVENT > EvtPool;			// add event id 
		bIsLoading = false;	

		long during = System.DateTime.Now.Ticks-tick;
		Debug.Log( "stage srart loding complete. total ticket:"  + during );


	}

	void FixPlanePosition()
	{
		// ensure canvrs in screen
		if( TilePlaneObj != null )
		{
			if (TarceMoveingUnit != null ) {
				if( TarceMoveingUnit.IsMoving()== false )
				{
					TarceMoveingUnit = null;
				}else{
					// force to unit
					Vector3 v = TarceMoveingUnit.transform.localPosition;
					v.x *= -1;
					v.y *= -1;

					//get time from speed 200pix /sec
					Vector3 d = TilePlaneObj.transform.localPosition - v;
					float time = d.magnitude  /1000 ;
					TweenPosition tw = TweenPosition.Begin<TweenPosition>(TilePlaneObj , time );
					if( tw ){
						tw.SetStartToCurrentValue();
						tw.to = v;
					}

					//TilePlaneObj.transform.localPosition  = v ;

				}
			}

			//float fMouseX = Input.mousePosition.x;
			//float fMouseY = Input.mousePosition.y;
			
			Vector3 vOffset = TilePlaneObj.transform.localPosition;
			// X 
			if( vOffset.x < fMinOffX ){
				vOffset.x = fMinOffX;
			}
			else if( vOffset.x > fMaxOffX ){
				vOffset.x = fMaxOffX;
			}
			// Y
			if( vOffset.y < fMinOffY ){
				vOffset.y = fMinOffY;
			}
			else if( vOffset.y > fMaxOffY ){
				vOffset.y = fMaxOffY;
			}
			TilePlaneObj.transform.localPosition = vOffset;
		}

	}

	void OnReady()
	{
		// close all other panel
		UnitPanelObj.SetActive( false ); // unit plane
		
		//public GameObject TileObj; 	//  no need this
		MoveEftObj.SetActive( false ); 	// 
		AtkEftObj.SetActive( false ); 	// 
		AoeEftObj.SetActive (false);

		ValueEftObj.SetActive (false);
//		if (BattleValue.nValueCount != 0) {
//		}


		Panel_CMDUnitUI.CloseCMDUI();
		PanelManager.Instance.CloseUI( Panel_UnitInfo.Name );
		PanelManager.Instance.CloseUI( Panel_MiniUnitInfo.Name );

		// close loadint UI
		PanelManager.Instance.CloseUI( "Panel_Loading");

		bIsReady = true;		// all ready .. close the loading ui
	}

	// Update is called once per frame
	void Update () {

		// check if first run
		if( bIsReady == false ){
			if( bIsLoading ) {
				return;
			}
			else {
				// close pre open panel
				OnReady();

			}
			// check ready complete
			return;
		}
		FixPlanePosition ();

		if (bIsStageEnd == true)
			return;

		// Real update
		GameDataManager.Instance.Update();

		// block other event
		if( IsAnyActionRunning() == true ) // wait all tween / fx / textbox / battle msg finish / unit move
			return;							// don't check event run finish here.

		//=== Unit Action ====
		// Atk / mov / other action need to preform first
		if (ActionManager.Instance.Run () == true)
			return;

		// avoid event close soon when call talkui already
		if( PanelManager.Instance.CheckUIIsOpening( Panel_Talk.Name) == true )
			return ;

		//==================	
		if( RunEvent( ) == true )// this will throw many unit action and interrupt battle.
			return ;

		// if one event need to run battle. it should pause ein event
		if (BattleManager.Instance.IsBattlePhase ()) {// this will throw many unit action
			BattleManager.Instance.Run();
			return;
		}

	//	if( GetEventToRun() == true )
	//		return;

	//	if( IsRunningEvent() == true  )
	//		return true; // block other action  


		// check if need pop cmd UI auto
		if ( CheckPopNextCMD () == true)
			return;

		if (CheckUnitDead () == true) // check here for event_manager  can detect hp ==0 first , some event may relive the deading unit
			return;

		// check drop event
		if (BattleManager.Instance.ProcessDrop () == true)
			return;

		//===================		// this will throw unit action or trig Event
		if ( RunCampAI( GameDataManager.Instance.nActiveCamp ) == false )
		{
			// no ai to run. go to next faction
			GameDataManager.Instance.NextCamp();
		}


		//===================onchar

		// startup panel when opening event done.
		if( GameDataManager.Instance.nRound == 0 )
		{
			GameDataManager.Instance.nRound  =1;			  // Go to round 1 	
			PanelManager.Instance.OpenUI( Panel_Round.Name ); 
			//StageWeakUpCampEvent cmd = new StageWeakUpCampEvent ();
			//cmd.nCamp = GameDataManager.Instance.nActiveCamp;
			//GameEventManager.DispatchEvent ( cmd );
		}

	}

	void OnDestroy()
	{
		// free singl
		instance = null;

		RegeditGameEvent( false );
		// create singloten


	}



	void OnCellClick(GameObject go)
	{
		if( IsAnyActionRunning() == true )
			return;
		
		if( IsRunningEvent() == true  )
			return; // block other action  

		if (GameDataManager.Instance.nActiveCamp != _CAMP._PLAYER)
			return;

	
		UnitCell unit = go.GetComponent<UnitCell>() ;
		if( unit != null ){
		
			string str = string.Format( "CellOnClick( {0},{1}) " , unit.X() , unit.Y() );
			string sKey =	unit.Loc.GetKey();
			Debug.Log(str);
			bool bIsAtkCell = OverCellAtkPool.ContainsKey( sKey );
			bool bIsOverCell = OverCellPool.ContainsKey( sKey );

		

			if( cCMD.Instance.eCMDSTATUS == _CMD_STATUS._WAIT_TARGET )
			{
				if( cCMD.Instance.eCMDTARGET == _CMD_TARGET._ALL || 
				   cCMD.Instance.eCMDTARGET == _CMD_TARGET._POS   ){

					if( bIsAtkCell == false )
					{
						Panel_CMDUnitUI.RollBackCMDUIWaitTargetMode();
					}
					else{
					// make one map skill cmd
						Panel_CMDUnitUI panel = Panel_CMDUnitUI.JustGetCMDUI();
						panel.SetPos(  unit.X() , unit.Y() );
					}
					
				}
				else { 
					// this is impossible
					if( bIsAtkCell == false )
					{
						Panel_CMDUnitUI.RollBackCMDUIWaitTargetMode();
						//Panel_CMDUnitUI.BackWaitCmd();		// back if the cmd is not exists	
					}
				}
				return ;
			}
			else if( cCMD.Instance.eCMDTYPE == _CMD_TYPE._WAITATK && cCMD.Instance.eCMDID == _CMD_ID._NONE ) // special case for fast attack
			{

				if( bIsAtkCell ){
					// pass
				}
				else// rollback
				{
					Panel_CMDUnitUI.CancelCmd();
				}

				return;
			}
			else if( cCMD.Instance.eCMDSTATUS == _CMD_STATUS._WAIT_CMDID ) // is waiting cmd id. this is moving act 
			{
				if( cCMD.Instance.eCMDTYPE == _CMD_TYPE._ALLY )
				{
					if( bIsOverCell == true || Config.GOD ) // god for cheat warp
					{	
						ClearOverCellEffect( );
						cCMD.Instance.eNEXTCMDTYPE = _CMD_TYPE._WAITATK;
						PanelManager.Instance.CloseUI( Panel_CMDUnitUI.Name ); //only colse ui to wait

						StageCharMoveEvent evt = new StageCharMoveEvent ();
						evt.nIdent =  cCMD.Instance.nCmderIdent; // current oper ident 
						evt.nX = unit.X ();
						evt.nY = unit.Y ();
						GameEventManager.DispatchEvent (evt);		
						return;
					}
					else if( cCMD.Instance.eCMDTARGET == _CMD_TARGET._UNIT )
					{
						Panel_CMDUnitUI.CloseCMDUI();
						// need cancel cmd
					
					}
					return ;
				}
				else 
				{
					// close 
					Panel_CMDUnitUI.CloseCMDUI();
				}
			}



			//cCMD.Instance.eCMDTYPE = _CMD_TYPE._CELL;
			// clear over effect
		//	ClearOverCellEffect( );

			// check to open sys ui
			// avoid re open
//			if( PanelManager.Instance.CheckUIIsOpening( "Panel_CMDUI" ) == false  )
//			{
			Panel_CMDUnitUI.OpenCMDUI( _CMD_TYPE._CELL , null );
//			cCMD.Instance.eCMDTYPE = _CMD_TYPE._CELL; 
//				GameObject obj = PanelManager.Instance.OpenUI( Panel_CmdSysUI.Name );
//				if (obj != null) {
				//	NGUITools.SetActive( obj, true );
				//	Vector3 vLoc = this.gameObject.transform.localPosition ;
					//UICamera.mainCamera.ScreenPointToRay
				//	UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);					
				//	float ratio = (float)mRoot.activeHeight / Screen.height;

				//	vLoc.x = MyTool.ScreenToLocX( Input.mousePosition.x );
				//	vLoc.y = MyTool.ScreenToLocY( Input.mousePosition.y );

				//	obj.transform.localPosition = vLoc;// MousePosition;
					//
//					Panel_CmdSysUI co = obj.GetComponent< Panel_CmdSysUI >();
//					if( co )
//					{					
//					}
//				}
//			}
		}
	}

	// give up this month now
	void OnCellPress(GameObject go ,bool pressed)
	{
		if (pressed == false) {
			Debug.Log( "cell gree press");
			// if cast skill
			if (cCMD.Instance.nSkillID > 0) {
				//do cast cmd
				Debug.LogFormat ("cast out skill {0}", cCMD.Instance.nSkillID);
			}
		} else {
			Debug.Log( "cell  press");
		}
	}

	// if any char be click
	void OnCharClick(GameObject go)
	{
		if( IsAnyActionRunning() == true )
			return;
		
		if( IsRunningEvent() == true  )
			return; // block other action  

		if (GameDataManager.Instance.nActiveCamp != _CAMP._PLAYER)
			return;
	
		// avoid any ui opening
		// clear over effect

		Panel_unit unit = go.GetComponent<Panel_unit>() ;
		if( unit == null )
			return ;

		if (cCMD.Instance.eCMDSTATUS == _CMD_STATUS._NONE  ) {
			//GameObject obj = PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name);
			//CloseCMDUI();

			Panel_CMDUnitUI panel = Panel_CMDUnitUI.OpenCMDUI( _CMD_TYPE._ALLY , unit );
			CreateMoveOverEffect (unit);
			return;
		}

		else if (cCMD.Instance.eCMDSTATUS == _CMD_STATUS._WAIT_CMDID ){
			if( cCMD.Instance.eCMDTYPE == _CMD_TYPE._WAITATK )
			{
				Panel_CMDUnitUI.CancelCmd (); // if have cms\d . cancel it
			}
			else{
				Panel_CMDUnitUI.CloseCMDUI();
				Panel_CMDUnitUI panel = Panel_CMDUnitUI.OpenCMDUI( _CMD_TYPE._ALLY , unit );			
				CreateMoveOverEffect (unit);
			}
			return ;
		}
		else if (cCMD.Instance.eCMDSTATUS == _CMD_STATUS._WAIT_TARGET ){
		//	Panel_CMDUnitUI panel = MyTool.GetPanel< Panel_CMDUnitUI >( PanelManager.Instance.JustGetUI( Panel_CMDUnitUI.Name ) ); 
		//	if( panel )
		//		panel.CancelCmd();
			//GameObject obj = PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name);
			string sKey = unit.Loc.GetKey ();
			Debug.Log( "OnCharClick" + sKey + ";Ident"+unit.Ident() ); 

			// check target is vaild
			bool bInAtkCell = OverCellAtkPool.ContainsKey (sKey);
			
			if( bInAtkCell && (_PK_MODE._ENEMY ==  MyTool.GetSkillPKmode (cCMD.Instance.nSkillID) ) ){
				return ;
			}

			if ( bInAtkCell == true) {
				Panel_CMDUnitUI panel = MyTool.GetPanel<Panel_CMDUnitUI> ( PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name) );
				if (panel != null) {
					panel.SetTarget( unit );
				}
				ClearOverCellEffect ();
			}
			return ;
		}





//		if( obj != null )
//		{
//			Panel_CMDUnitUI panel = obj.GetComponent<Panel_CMDUnitUI>();
//			if( panel != null )
//			{
//				panel.Setup( unit );
//			}

//		}
		//if( PanelManager.Instance.CheckUIIsOpening( Panel_CMDUnitUI.Name ) )
		//unit.OnClick( this );

		//unit.Identify;
	}


	void OnMobClick(GameObject go)
	{
		if( IsAnyActionRunning() == true )
			return;
		
		if( IsRunningEvent() == true  )
			return; // block other action  

		if (GameDataManager.Instance.nActiveCamp != _CAMP._PLAYER)
			return;

		// avoid any ui opening
		// clear over effect
		Panel_unit unit = go.GetComponent<Panel_unit>() ;
		if( unit == null )
			return ;
		string sKey = unit.Loc.GetKey ();
		Debug.Log( "OnMobClick" + sKey + ";Ident"+unit.Ident() ); 

		bool bInAtkCell = OverCellAtkPool.ContainsKey (sKey);

		if( bInAtkCell && (_PK_MODE._PLAYER ==  MyTool.GetSkillPKmode (cCMD.Instance.nSkillID) ) ){
			return ;
		}

		ClearOverCellEffect(  ); // all solution will clear

	

		// open new cmd ui when this idn't a new cmd
		if (bInAtkCell) {




			if (cCMD.Instance.eCMDSTATUS == _CMD_STATUS._WAIT_TARGET) {		

				// send atk cmd
				Panel_CMDUnitUI panel = Panel_CMDUnitUI.JustGetCMDUI ();//   MyTool.GetPanel< Panel_CMDUnitUI >( PanelManager.Instance.JustGetUI( Panel_CMDUnitUI.Name ) ); 
				if( cCMD.Instance.eCMDTARGET ==  _CMD_TARGET._UNIT || cCMD.Instance.eCMDTARGET ==  _CMD_TARGET._ALL )			{
					panel.SetTarget (unit); 
				}
				else if( cCMD.Instance.eCMDTARGET ==  _CMD_TARGET._POS){
					panel.SetPos( unit.Loc.X , unit.Loc.Y );
				}
				return;
			} else if (cCMD.Instance.eCMDTYPE == _CMD_TYPE._WAITATK && cCMD.Instance.eCMDID == _CMD_ID._NONE) { 

				// change cmd target to unit 
				//cCMD.Instance.eCMDTARGET =  _CMD_TARGET._UNIT;
				cCMD.Instance.eCMDID = _CMD_ID._ATK;			// smart set a atk cmd

				// send atk cmd
				Panel_CMDUnitUI panel = Panel_CMDUnitUI.JustGetCMDUI ();//   MyTool.GetPanel< Panel_CMDUnitUI >( PanelManager.Instance.JustGetUI( Panel_CMDUnitUI.Name ) ); 
				panel.SetTarget (unit);

				return; // fast normal attack
			}
		}



		// can't attack . open a normal enemy cmd
		if( cCMD.Instance.eCMDTYPE != _CMD_TYPE._SYS  )
			Panel_CMDUnitUI.CancelCmd (); // if have cms\d . cancel it
		//Panel_CMDUnitUI.CloseCMDUI ();

		// open new enemy cmd ui
		Panel_CMDUnitUI.OpenCMDUI ( _CMD_TYPE._ENEMY , unit );

		CreateMoveOverEffect ( unit );

		// close sel 
		//cCMD.Instance.eCMDTYPE = _CMD_TYPE._ALLY;



//		if (PanelManager.Instance.CheckUIIsOpening ( Panel_CMDUnitUI.Name )) {
//			Panel_CMDUnitUI panel = MyTool.GetPanel< Panel_CMDUnitUI >( PanelManager.Instance.JustGetUI( Panel_CMDUnitUI.Name ) ); 

//			if ( cCMD.Instance.eCMDTARGET == _CMD_TARGET._UNIT || cCMD.Instance.eCMDTARGET == _CMD_TARGET._ALL ) {
//				panel.SetTarget (unit);
//			}
//		}

	}
	bool LoadScene( int nScnid )
	{

		SCENE_NAME scn = ConstDataManager.Instance.GetRow<SCENE_NAME> ( nScnid );
		if (scn == null){
			Debug.LogFormat( "LoadScene fail with ID {0}" , nScnid );
			return false;
		}

		// string filename = "Assets/StreamingAssets/scn/"+scn.s_MODLE_ID+".scn"; // old moethod

		string dataPathRelativeAssets = "scn/";
		string rootPath = null;

		#if UNITY_EDITOR                        
		rootPath = "file://" +Application.dataPath + "/StreamingAssets/" + dataPathRelativeAssets + scn.s_MODLE_ID + ".scn";
		#elif UNITY_IPHONE
		rootPath =  "file://" +Application.dataPath + "/Raw/" + scn.s_MODLE_ID + ".scn";
		#elif UNITY_ANDROID
		rootPath = "jar:file://"+Application.dataPath + "!/assets/" + dataPathRelativeAssets + scn.s_MODLE_ID + ".scn";
		#else
		rootPath = "file://" +Application.dataPath + "/StreamingAssets/" + dataPathRelativeAssets + scn.s_MODLE_ID + ".scn";
		#endif

		// real to binary
		WWW www = new WWW(rootPath);
//		if(endFunc != null){
//			yield return www;
//		}else{
			while(!www.isDone){
				
			}
//		}
//		string txt = Application.dataPath;
//		string txt2 = Application.persistentDataPath;
//		string utext = txt + "/" +txt2;
//		Debug.Log( "unity data path :"+utext );

		Debug.Log( "load scn file on:"+rootPath );

		if( Grids.Load( www.bytes )==true )
		//if( Grids.Load( rootPath )==true )
		//if( Grids.Load( filename )==true )
		{
			// start to create sprite
			for( int i = -Grids.hW ; i <= Grids.hW ; i++ ){
				for( int j = -Grids.hH ; j <= Grids.hH ; j++ )
				{			
					_TILE t = Grids.GetValue( i , j  );
				
					GameObject cell = GetTileCellPrefab( i , j , t ); 
					if( cell == null )
					{
						// debug message
						string err = string.Format( "Error: Create Tile Failed in Scene({0}),X({1},Y({2},T({3} )" , scn.s_MODLE_ID , i , j ,t ); 
						Debug.Log(err);

					}
				}
			}
			// reget the drag limit 
			Resize();
		}

		// change bgm '
		// all stage have start event for speicial bgm
		//GameSystem.PlayBGM ( scn.n_BGM );


		return true;
	}
	public void Resize( )
	{	
		Grids.SetPixelWH (Config.TileW, Config.TileH);  // re size

		fMaxOffX  =  (Grids.TotalW - Screen.width )/2; 
		fMinOffX  =  -1*fMaxOffX;		
		
		fMaxOffY  =  (Grids.TotalH - Screen.height )/2; 
		fMinOffY  =  -1*fMaxOffY;		
		
	}

	GameObject GetTileCellPrefab( int x , int y , _TILE t )
	{
		SCENE_TILE tile = ConstDataManager.Instance.GetRow<SCENE_TILE> ((int)t);
		if (tile != null) {
			//tile.s_FILE_NAME;
			GameObject cell = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/TileCell");
			UISprite sprite = cell.GetComponent<UISprite>(); 
			if( sprite != null )
			{
				sprite.spriteName = tile.s_FILE_NAME;

			}
			UIDragObject drag = cell.GetComponent<UIDragObject>(); 
			if( drag != null )
			{
				drag.target = TilePlaneObj.transform ;

			}

			// tranform
//			float locx =0, locy =0;
//			Grids.GetRealXY(ref locx , ref locy , new iVec2( x , y ) );			
//			Vector3 pos = new Vector3( locx , locy , 0 );
			if( cell != null ){
				SynGridToLocalPos( cell , x, y ) ;
				//cell.transform.localPosition = pos; 

				cell.name = string.Format("Cell({0},{1},{2})", x , y , 0 );

				UnitCell unit = cell.GetComponent<UnitCell>() ;
				if( unit != null ){
					unit.X( x );
					unit.Y( y );

				}
				//==========================================================
				UIEventListener.Get(cell).onClick += OnCellClick;
			//	UIEventListener.Get(cell).onPress += OnCellPress;

			}


			return cell;
		}
			//_TILE._GREEN

		return null;
	}

	public GameObject SpwanBattleValueObj( )
	{
		GameObject go = ValueEftObj.Spawn( MaskPanelObj.transform );
		go.SetActive (true);
		return go;
	}



	public void ClearOverCellEffect(  )
	{
		// clear eff
		//OverCellPool

		MoveEftObj.RecycleAll();

		if( OverCellPool != null ){
			OverCellPool.Clear ();
		}
		//
		AtkEftObj.RecycleAll();
		if( OverCellAtkPool != null ){
			OverCellAtkPool.Clear ();
		}


		AoeEftObj.RecycleAll ();
		if (OverCellAOEPool != null) {
			OverCellAOEPool.Clear();
		}

	}

	public void CreateMoveOverEffect( Panel_unit unit )
	{
		MoveEftObj.RecycleAll();

		if( OverCellPool != null ){
			OverCellPool.Clear ();
		}
		if (unit == null)
			return;

		//return; // 
		// find move
		cUnitData pdata = GameDataManager.Instance.GetUnitDateByIdent ( unit.Ident() ); 
		// don't zoc 
//		List<iVec2> posList = GetUnitPosList ( );

	//	List<iVec2> moveList =  Grids.GetRangePool (unit.Loc, pdata.GetMov()  , 1);
		Grids.ClearIgnorePool();
		Grids.AddIgnorePool( GetUnitPKPosPool(unit , true  ) );

		List<iVec2> moveList =  Grids.MoveAbleCell (unit.Loc, pdata.GetMov() );

		// try ZOC!!!
	//	List<iVec2> final = Panel_StageUI.Instance.Grids.FilterZocPool (unit.Loc, ref moveList, ref posList);
	//	moveList = final;

		long  tick =  System.DateTime.Now.Ticks; 

		// start create over eff
		foreach( iVec2 v in moveList )
		{
			if ( cMyGrids.IsWalkAbleTile( Grids.GetValue( v ) ) == false  )
			{
				continue;
			}
			// check if this vec can reach


//			List<iVec2> path = PathFinding( unit , unit.Loc , v , pdata.GetMov() );
//			if( path.Count <= 0 )
//				continue;

			// create move over cell
			//GameObject over = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/MoveOverEffect");
			GameObject over = MoveEftObj.Spawn(  TilePlaneObj.transform );
		
			if( over != null )
			{
				over.name = string.Format("Move Over({0},{1},{2})", v.X , v.Y , 0 );
				SynGridToLocalPos( over , v.X , v.Y) ;
				//UIEventListener.Get(over).onClick += OnOverClick;
				
				OverCellPool.Add( v.GetKey() , over );
				over.SetActive( true );
			//	over.transform.SetParent ( TilePlaneObj.transform );
			}
		}

		long  during =  System.DateTime.Now.Ticks - tick ; 

		Debug.Log( "create moveeffect cell with ticket:" + during );
	}

	public void CreateAttackOverEffect( Panel_unit unit , int nRange=1)
	{
		foreach( KeyValuePair< string , GameObject> pair in OverCellAtkPool )
		{
			if( pair.Value != null )
			{
				NGUITools.Destroy( pair.Value );
				//pair.Value = null;
			}
		}
		OverCellAtkPool.Clear ();
		if (unit == null)
			return;

		if (nRange < 1)
			nRange = 1;

		List<iVec2> AtkList =  Grids.GetRangePool ( unit.Loc, nRange );
		AtkList.RemoveAt (0); // remove self pos

		foreach( iVec2 v in AtkList )
		{
			//GameObject over = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/AttackOverEffect");
			GameObject over = AtkEftObj.Spawn( TilePlaneObj.transform );
			if( over != null )
			{
				over.name = string.Format("ATK Over({0},{1},{2})", v.X , v.Y , 0 );
				SynGridToLocalPos( over , v.X , v.Y) ;
				OverCellAtkPool.Add( v.GetKey() , over );
				over.SetActive( true );
			}
		}

	}

	// unity func
	public void CreatePathOverEffect( List<iVec2> path )
	{
		foreach( KeyValuePair< string , GameObject> pair in OverCellPathPool )
		{
			if( pair.Value != null )
			{
				NGUITools.Destroy( pair.Value );
				//pair.Value = null;
			}
		}
		OverCellPathPool.Clear ();
		if (path == null)
			return;

		foreach( iVec2 v in path )
		{
			GameObject over = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/PathOverEffect");
			if( over != null )
			{
				over.name = string.Format("PATHOVER({0},{1},{2})", v.X , v.Y , 0 );
				SynGridToLocalPos( over , v.X , v.Y) ;
				
				//UIEventListener.Get(over).onClick += OnOverClick;
				
				OverCellPathPool.Add( v.GetKey() , over );
				over.SetActive( true );
			}
		}

	}

	public void CreateAOEOverEffect( int nX , int nY , int nAOE )
	{
		AoeEftObj.RecycleAll();

		if( OverCellAOEPool != null ){
			OverCellAOEPool.Clear ();
		}

		Panel_unit unit = MyTool.CMDUI().pCmder;
		int nOrgX = unit.X ();
		int nOrgY = unit.Y ();

		List<iVec2> aoeList = MyTool.GetAOEPool (nX, nY, nAOE ,nOrgX, nOrgY );
		// 將來要處理 旋轉!
	
		// get rotation





		// start create over eff
		foreach( iVec2 v in aoeList )
		{	
			// create move over cell
			//GameObject over = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/MoveOverEffect");
			GameObject over = AoeEftObj.Spawn(  TilePlaneObj.transform );
			
			if( over != null )
			{
				over.name = string.Format("AOE OVER({0},{1},{2})", v.X , v.Y , 0 );
				SynGridToLocalPos( over , v.X , v.Y) ;
				//UIEventListener.Get(over).onClick += OnOverClick;
				
				OverCellAOEPool.Add( v.GetKey() , over );

				over.SetActive( true );
				//	over.transform.SetParent ( TilePlaneObj.transform );
			}
		}
		//Debug.Log( "create Aoeeffect cell with ticket:" + during );
		Panel_CheckBox panel = MyTool.GetPanel<Panel_CheckBox>( PanelManager.Instance.OpenUI ( Panel_CheckBox.Name ) );
		if (panel) {
			panel.SetAoeCheck();
		}
	}
	// Check any action is running

	public bool IsAnyActionRunning()
	{
		if(BattleMsg.nMsgCount > 0)
			return true;

		// this is very slow for play
//		if(ValueEftObj.CountSpawned () > 0)
//			return true; 

		//IsAnyActionRunning
	//	if( BattleValue.nValueCount > 0  ) // bug here!!
	//		return true;

//		if( PanelManager.Instance.CheckUIIsOpening( Panel_Talk.Name) == true )
//			return true;

		if( PanelManager.Instance.CheckUIIsOpening( Panel_Round.Name ) == true )
			return true;	

		if( PanelManager.Instance.CheckUIIsOpening( Panel_Skill.Name ) == true )
			return true;	

		foreach( KeyValuePair< int , Panel_unit > pair in IdentToUnit )
		{
			if( pair.Value == null )
				continue;
			if( pair.Value.IsIdle() == false )
				return true;
		}



		return false;
	}

	// UnitAction
	public Panel_unit GetUnitByIdent( int Ident ) 
	{
		if( IdentToUnit.ContainsKey( Ident ) == true ) 
		{
			return IdentToUnit[ Ident ];
		}
		return null;
	}

	// find unit by charid
	public Panel_unit GetUnitByCharID( int nCharID )
	{
		foreach (KeyValuePair< int ,Panel_unit > pair in IdentToUnit) {
			if( pair.Value!= null )
			{
				if( pair.Value.CharID == nCharID )
				{
					return pair.Value;
				}
			}
		}
		return null;
	}


	// Faction AI
	public List<Panel_unit> GetUnitListByCamp( _CAMP nCamp )
	{
		List<Panel_unit> lst = new List<Panel_unit> ();
		cCamp camp = GameDataManager.Instance.GetCamp( nCamp );
		if (camp != null) {
			foreach( int ident in camp.memLst )
			{
				Panel_unit unit = GetUnitByIdent( ident ); 
				if( unit != null )
				{
					lst.Add( unit );
				}
			}
		}

		return lst;
	}

	// get nearest pk unit
	public Dictionary< Panel_unit , int > GetUnitDistPool( Panel_unit unit , bool bCanPK )
	{
		Dictionary< Panel_unit , int > pool = new Dictionary< Panel_unit , int > (); // unit , dist
		foreach( KeyValuePair<int ,Panel_unit > pair in IdentToUnit )
		{
			if( unit.CanPK( pair.Value ) == bCanPK )
			{
				int nDist = pair.Value.Loc.Dist( unit.Loc );
				pool.Add( pair.Value , nDist );
			}
		}
		return pool;
	}


	bool RunCampAI( _CAMP nCamp )
	{
		// our faction don't need AI process
		if( nCamp == _CAMP._PLAYER )
			return true; // player is playing 

		// change faction if all unit moved or dead.
		List<Panel_unit> lst = GetUnitListByCamp (nCamp);
		foreach (Panel_unit unit in lst ) {
			if( unit.CanDoCmd() )
			{
				unit.RunAI();
				return true;
			}
		}
		return false;
	}



	// Check event
	bool CheckEventCanRun( STAGE_EVENT evt )
	{
		cTextArray sCond = new cTextArray( );
		sCond.SetText( evt.s_CONDITION );
		// check all line . if one line success . this event check return true

		int nCol = sCond.GetMaxCol();
		for( int i= 0 ; i <nCol ; i++ )
		{
			//if( CheckEventCondition( sCond.GetTextLine( i ) ) )
			if( MyScript.Instance.CheckEventCondition( sCond.GetTextLine( i ) ) )
			{
				return true;
			}
		}
		return false;
	}


	//==========Execute Event =================
	// true : is running
	bool RunEvent(  )
	{
		// always check to wait list
		//GetEventToRun();

		// get next event
		if( NextEvent == null )
		{
			GetEventToRun();
			// get next event to run

		}

		// if event is running

		  //run event
		if( NextEvent != null )
		{
			NextLine();					// execute one line

			//NextLine();					// parser event to run
			if( IsNextEventCompleted() )
			{
				// record event comp
				//EvtCompletePool.Add( NextEvent.n_ID , GameDataManager.Instance.nRound );

				if( GameDataManager.Instance.EvtDonePool.ContainsKey( NextEvent.n_ID )  )
				{
					GameDataManager.Instance.EvtDonePool[ NextEvent.n_ID ] = GameDataManager.Instance.nRound; // update newest complete round
				}
				else{
					GameDataManager.Instance.EvtDonePool.Add( NextEvent.n_ID , GameDataManager.Instance.nRound );
				}

				// clear event for next
				NextEvent = null;
				IsEventEnd = true;

				// all end . check again  for new condition status
			//	if( WaitPool.Count <= 0 ){
				if( IsRunningEvent() == false )
				{
					GetEventToRun();
				}
			//	}

			}
		}

//		 switch status
//		GameDataManager.Instance.nRound = 0;
//		GameDataManager.Instance.nActiveFaction  = 0;
		return ( IsRunningEvent() );
	}

	public bool IsLoopEvent( STAGE_EVENT evt )
	{
		if (evt != null) {
			if ( (evt.n_TYPE & 1) > 0  )  // 1 is loop event
			{
				return true;
			}
		}
		return false;
	}
	void GetEventToRun()
	{
		//if( IsRunningEvent() )
		if( NextEvent != null )		// avoid double run
			return ;
		if( (EvtPool==null) )
			return  ;
		if( EvtPool.Count<=0)
			return ;
		
		List< int > removeLst = new List< int >();
		// get next event to run
		foreach( KeyValuePair< int ,STAGE_EVENT > pair in EvtPool ) 
		{
			if( CheckEventCanRun( pair.Value ) == true ){		// check if this event need run
				//NextEvent = pair.Value ; 		// run in next loop
				WaitPool.Add( pair.Value );
				// check is loop event?
				if( IsLoopEvent( pair.Value )== false  )
				{
					removeLst.Add( pair.Key );
				}
			}
		}
		// remove key , never check it again
		foreach( int key in removeLst )
		{
			if( EvtPool.ContainsKey( key ) )
			{
				EvtPool.Remove( key );
			}
		}
		// prepare event to run
		if( WaitPool.Count > 0 )
		{
			NextEvent =  WaitPool[0];
			WaitPool.RemoveAt( 0 );
			
			PreEcecuteEvent();					// parser next event to run
		}

		// return (WaitPool.Count > 0 || NextEvent !=null);
	}


	// check any event is runing or wait running
	bool IsRunningEvent ()
	{
		if( WaitPool.Count > 0 )
			return true;
		if( NextEvent != null )
			return true;

		// running talk ui is running event too
		if( IsAnyActionRunning() ) // many trig action. all event need wait them complete
			return true;

		//avoid event close soon when call talkui already
		if( PanelManager.Instance.CheckUIIsOpening( Panel_Talk.Name) == true )
			return true;

		return false;
	}

	void PreEcecuteEvent(  )
	{
		if( NextEvent == null ){
			return ;
		}
		//==========
		IsEventEnd = false;
		// record event script 
		m_cScript = new cTextArray( );
		m_cScript.SetText( NextEvent.s_BEHAVIOR );	

		m_nFlowIdx = 0;
		//NextLine();			// script to run
	}

	void NextLine()// script to run
	{
		if( IsEventEnd == true )
			return;
		if( m_nFlowIdx >= m_cScript.GetMaxCol() )
			return;
		//if( m_nFlowIdx  )
		CTextLine line = m_cScript.GetTextLine( m_nFlowIdx++ );
		if( line != null )
		{

			MyScript.Instance.ParserScript( line );
		}
	}

	// check Next Event is completed
	bool IsNextEventCompleted()
	{
		if( IsEventEnd == true )
			return true;

		if( m_nFlowIdx < m_cScript.GetMaxCol() )
			return false;
		// is all tween complete

		return true ;
	}




	// Widget func	 

	GameObject AddUnit( _CAMP nCampID , int nCharID , int x , int y  , int nLeaderIdent = 0 )
	{
//		CHARS charData = GameDataManager.Instance.GetConstCharData (nCharID); //ConstDataManager.Instance.GetRow<CHARS>( nCharID );
//		if( charData == null)
//			return null;
		// get data from Const data
		if (TilePlaneObj == null) {
			Debug.Log( "Stage Addunit to null TilePlane" );
			return null;
		}
		//GameObject obj = ResourcesManager.CreatePrefabGameObj( TilePlaneObj , "Prefab/Panel_Unit" );

		GameObject obj = UnitPanelObj.Spawn( TilePlaneObj.transform );
		if( obj == null )return null;
		obj.name = string.Format ("unit-{0}",nCharID );	
		
		
		// charge face text				
//		UITexture tex = obj.GetComponentInChildren<UITexture>();
//		
//		if( tex )
//		{
//			if(tex != null){
//				//	DynamicAssetBundleLoader.LoadTexture(tex,DynamicAssetBundleLoader.SSAssetType.Card, "CARD_" + card.PicName);
//				
//				string url = "Art/char/" + charData.s_FILENAME +"_S";
//				Texture t= Resources.Load <Texture>( url  ) ;
//				tex.mainTexture = t;
//				// tex.MakePixelPerfect(); don't make pixel it
//			}
//		}

		// regedit to gamedata manager
		Panel_unit unit = obj.GetComponent<Panel_unit>();
		//UNIT_DATA unit = GameDataManager.Instance.CreateChar( nCharID );
		if( unit != null )
		{
			// fix to a valid pos
			iVec2 pos = FindEmptyPos( new iVec2(x , y));
			int posx = x;
			int posy = y;
			if( pos !=null ){
				posx = pos.X;
				posy = pos.Y;
			}

			// setup param
			unit.CreateChar( nCharID , posx , posy );
			unit.SetCamp( nCampID );	

			unit.SetLevel( StageData.n_MOB_LV );


			IdentToUnit.Add( unit.Ident() , unit  ) ;// stage gameobj

			// set game data
			unit.pUnitData.n_LeaderIdent = nLeaderIdent;
		//	unit.pUnitData.n_X			 = x;
		//	unit.pUnitData.n_Y			 = y;
		}
		
		// position // set in create
		//obj.transform.localPosition =  MyTool.SnyGridtoLocalPos( x , y , ref Grids ) ; 
		
		if (nCampID == _CAMP._PLAYER) {		
			UIEventListener.Get (obj).onClick += OnCharClick;
		} else if (nCampID == _CAMP._ENEMY) {
			UIEventListener.Get (obj).onClick += OnMobClick;
		}
		
		// if obj out of screen. move to it auto
		MoveToGameObj ( obj , false );



		// all ready
		NGUITools.SetActive( obj , true );
		return obj;
	}

	void DelUnitbyIdent(  int nIdent )
	{
		Panel_unit unit = IdentToUnit[ nIdent ];
		if( unit != null )
		{
			cCamp camp =  GameDataManager.Instance.GetCamp( unit.eCampID  );
			if( camp != null )
			{
				camp.memLst.Remove ( nIdent );
			}
			//unit.pUnitData; 
			//NGUITools.Destroy( unit.gameObject );
			unit.Recycle( );
			int nsize = UnitPanelObj.GetPooled().Count ;

			//UnitPanelObj.Recycle( .Spawn( TilePlaneObj.transform );
		}
		IdentToUnit.Remove( nIdent );
	}

	// Take care use ident to delete
	void DelUnit(  int nCharid )
	{
		//Dictionary< _CAMP , cCamp > CampPool = GameDataManager.Instance.GetCamp;			// add Camp
		foreach (KeyValuePair<_CAMP , cCamp > pair in GameDataManager.Instance.CampPool) {


			foreach( int id in pair.Value.memLst )
			{
				// id = ident
				Panel_unit unit = null;
				if( IdentToUnit.TryGetValue(id , out unit ) )
				{
					if( nCharid == unit.CharID )
					{
						unit.Recycle();
						//NGUITools.Destroy( unit.gameObject );

						IdentToUnit.Remove( id );
						pair.Value.memLst.Remove( id );
						return;
					}
				}

			}
		}
	}


	void DelChar( _CAMP nCampID , int nChar )
	{
		cCamp camp =  GameDataManager.Instance.GetCamp( nCampID );
		if( camp == null )
			return ;
		List< int > remove = new List< int >();
		foreach( int id in camp.memLst )
		{
			if( IdentToUnit.ContainsKey( id ) == true )
			{
				Panel_unit unit = IdentToUnit[ id ];
				if( unit != null )
				{
					if( nChar != unit.CharID )
					{
						continue;
					}
					//unit.pUnitData; 
					//NGUITools.Destroy( unit.gameObject );
					unit.Recycle();
				}
				IdentToUnit.Remove( id );
				remove.Add( id );
			}
			else{
				// fail obj??
				remove.Add( id );
			}
		}

		foreach( int id in remove )
		{
			camp.memLst.Remove( id );
		}
	}

	public Vector3 GetGridSynLocalPos( GameObject obj , int nx , int ny )
	{
		Vector3 v = obj.transform.localPosition;
		v.x = Grids.GetRealX ( nx );
		v.y = Grids.GetRealY ( ny );
		return v;
	}
	public void SynGridToLocalPos( GameObject obj , int nx , int ny )
	{
		Vector3 v = obj.transform.localPosition;
		v.x = Grids.GetRealX ( nx );
		v.y = Grids.GetRealY ( ny );
		obj.transform.localPosition = v ;
	}

	public bool CheckPopNextCMD()
	{
		if (cCMD.Instance.eNEXTCMDTYPE == _CMD_TYPE._WAITATK || 
		    cCMD.Instance.eNEXTCMDTYPE == _CMD_TYPE._WAITMOVE )
		{

			cCMD.Instance.eCMDTYPE = cCMD.Instance.eNEXTCMDTYPE;
			cCMD.Instance.eNEXTCMDTYPE = _CMD_TYPE._SYS;

			Panel_CMDUnitUI.OpenCMDUI( cCMD.Instance.eCMDTYPE , null );
			//GameObject go = PanelManager.Instance.OpenUI( Panel_CMDUnitUI.Name ); // only open UI. don't change other param

			return  true;
		}	
		return false;
	}

	public bool CheckUnitDead()
	{
		foreach ( KeyValuePair<int , cUnitData > pair in GameDataManager.Instance.UnitPool) {
			if( pair.Value != null )
			{
				// it should die
				if( pair.Value.n_HP <= 0  )
				{
					// checked if DictionaryEntry already
					Panel_unit punit = Panel_StageUI.Instance.GetUnitByIdent( pair.Key );
					if( punit!= null ){
						if( punit.bIsDead == false )
						{
							punit.SetDead();
							return true;
						}

					}

				}
			}

		}
		



		return false;
	}

	public void TraceUnit( Panel_unit unit )
	{
		if (unit == null)
			return;

		TarceMoveingUnit = unit;
	}


	public void MoveToGameObj( GameObject obj , bool force = false , float time = 1.0f)
	{
		if (obj == null)
			return;
		Vector3 v = obj.transform.localPosition;
		Vector3 canv = TilePlaneObj.transform.localPosition; // shift

		if (force == false)
		{
			Vector3 realpos = v + canv;
			int hW = (Config.WIDTH )/2 - Config.TileW;
			int hH = (Config.HEIGHT)/2 - Config.TileH;
			if( (realpos.x < hW  && realpos.x > -hW ) && (realpos.y < hH && realpos.y > -hH ) )
				return; // pass
		}

		//TilePlaneObj.transform.localPosition = -v;
		float during = time;

		TweenPosition tw = TweenPosition.Begin<TweenPosition> (TilePlaneObj, during);
		if (tw != null) {
			tw.SetStartToCurrentValue();
			tw.to = -v;
		}

		Panel_unit unit = obj.GetComponent< Panel_unit > ();
		if (unit != null) {
			TraceUnit( unit );
		}

	}

	// Game event func
	public void OnStageBGMEvent(GameEvent evt)
	{
		//Debug.Log ("OnStagePopCharEvent");
		StageBGMEvent Evt = evt as StageBGMEvent;
		if (Evt == null)
			return;
		if( StageData != null )
		{
			SCENE_NAME scn = ConstDataManager.Instance.GetRow<SCENE_NAME> ( StageData.n_SCENE_ID );
			if (scn == null)
				return ;

			GameSystem.PlayBGM( scn.n_BGM );
		}
	}

	public void OnStagePopUnitEvent(GameEvent evt)
	{
		//Debug.Log ("OnStagePopMobEvent");
		StagePopUnitEvent Evt = evt as StagePopUnitEvent;
		if (Evt == null)
			return;
		int nPopNum = 1;
		if (Evt.nValue1 > 1)
			nPopNum = Evt.nValue1;

		for (int i=0; i < nPopNum; i++) {
			GameObject obj = AddUnit (Evt.eCamp, Evt.nCharID, Evt.nX, Evt.nY);
			if (obj != null) {		
			} else {
				Debug.Log (string.Format ("OnStagePopUnitEvent Fail with charid({0}) num({1})", Evt.nCharID  , nPopNum )  );			
			}
		}
	}


	public void OnStagePopGroupEvent(GameEvent evt)
	{
		Debug.Log ("OnStagePopGroupEvent");
		StagePopGroupEvent Evt = evt as StagePopGroupEvent;
		if (Evt == null)
			return;
		// get leader ident
		int nLeaderCharid = Evt.nLeaderCharID;
		cUnitData pLeader = GameDataManager.Instance.GetUnitDateByCharID( nLeaderCharid );
		if( pLeader == null ){
			Debug.Log (string.Format ("OnStagePopGroupEvent Fail with no leader id:{0}) ", nLeaderCharid   )  );			
			return;
		}

		int nLeaderIdent = GameDataManager.Instance.CreateGroupWithLeaderChar ( nLeaderCharid ); // create group . maybe 0 if leader don't exists
		//int nLeaderIdent =pLeader.n_Ident;

		//int nGroupID = 
		if( Evt.nPopType == 0 ){
			int sx = Evt.stX < Evt.edX ? Evt.stX :Evt.edX ;
			int sy = Evt.stY < Evt.edY ? Evt.stY :Evt.edY ;
			int ex = Evt.stX > Evt.edX ? Evt.stX :Evt.edX ; 
			int ey = Evt.stY > Evt.edY ? Evt.stY :Evt.edY ;

			for( int i = sx ; i <= ex ; i++  ){
				for( int j = sy ; j <= ey ; j++  ){
					GameObject obj = AddUnit ( pLeader.eCampID, Evt.nCharID , i , j , nLeaderIdent  );
					if (obj != null) {	

					} else {
						Debug.Log (string.Format ("OnStagePopGroupEvent Fail with charid({0}) index({1},{2})", Evt.nCharID  , i , j )  );			
					}

				}
			}
		}
	}

	public void OnStageDelUnitByIdentEvent(GameEvent evt)
	{
		StageDelUnitByIdentEvent Evt = evt as StageDelUnitByIdentEvent;
		if (Evt == null)
			return;
		DelUnitbyIdent ( Evt.nIdent );
	}
	


	public void OnStageDelUnitEvent(GameEvent evt)
	{
		//Debug.Log ("OnStagePopCharEvent");
		StageDelUnitEvent Evt = evt as StageDelUnitEvent;
		if (Evt == null)
			return;
		int nCharid = Evt.nCharID;
		DelUnit( nCharid );

	}

	public void OnStageCharMoveEvent(GameEvent evt)
	{
		//Debug.Log ("OnStagePopCharEvent");
		StageCharMoveEvent Evt = evt as StageCharMoveEvent;
		if (Evt == null)
			return;
		int nIdent = Evt.nIdent;
		int nX =  Evt.nX;
		int nY =  Evt.nY;
//		if( IdentToUnit.ContainsKey(nIdent) == false )  {
//			Debug.Log( "ERR: can't find unit to move" );
//		}

		Panel_unit unit = GetUnitByIdent ( nIdent); // IdentToUnit[ nIdent ];
		if( unit != null )
		{
			unit.MoveTo( nX , nY ); 

//			// check if need trace unit 
//			Vector3 v = unit.transform.localPosition;
//			Vector3 canv = TilePlaneObj.transform.localPosition; // shift
//
//			Vector3 realpos = v + canv;
//			int hW = (Config.WIDTH )/2 - Config.TileW;
//			int hH = (Config.HEIGHT)/2 - Config.TileH;
//			if( (realpos.x < hW  && realpos.x > -hW ) && (realpos.y < hH && realpos.y > -hH ) )
//			{
//				// trace it
//				TraceUnit( unit );
//			}
		}
		//DelChar( _CAMP._ENEMY , nCharid );

		// throw move event to cmd ui
		// Close UI
		//PanelManager.Instance.CloseUI( Panel_CMDUnitUI.Name );
		CmdCharMoveEvent cmd = new CmdCharMoveEvent ();
		cmd.nIdent = nIdent;
		cmd.nX = nX;
		cmd.nY = nY;

		GameEventManager.DispatchEvent ( cmd );
	}

//	public void OnStageUnitActionFinishEvent(GameEvent evt)
//	{
//		StageUnitActionFinishEvent Evt = evt as StageUnitActionFinishEvent;
//		if (Evt == null)
//			return;
//		int nIdent = Evt.nIdent;
//		Panel_unit unit = GetUnitByIdent ( nIdent); // IdentToUnit[ nIdent ];
//		if (unit != null) {
//			unit.ActionFinished();
//		}
//	}

	public void OnStageWeakUpCampEvent(GameEvent evt)
	{
		StageWeakUpCampEvent Evt = evt as StageWeakUpCampEvent;
		if (Evt == null)
			return;
		List< Panel_unit > lst = GetUnitListByCamp ( Evt.nCamp );
		foreach( Panel_unit unit in lst )
		{
			//unit.pUnitData.AddActionTime( 1 );
			unit.pUnitData.WeakUp();
			//unit.AddActionTime( 1 ); // al add 1 time to action
		}
	}

	public void OnStageShowMoveRangeEvent(GameEvent evt)
	{
		//Debug.Log ("OnStagePopCharEvent");
		StageShowMoveRangeEvent Evt = evt as StageShowMoveRangeEvent;
		if (Evt == null)
			return;
		Panel_unit pUnit = GetUnitByIdent (Evt.nIdent);
		if (pUnit == null)
			return;

		CreateMoveOverEffect (pUnit);

	}
	public void OnStageShowAttackRangeEvent(GameEvent evt)
	{
		//Debug.Log ("OnStagePopCharEvent");
		StageShowAttackRangeEvent Evt = evt as StageShowAttackRangeEvent;
		if (Evt == null)
			return;
		Panel_unit pUnit = GetUnitByIdent (Evt.nIdent);
		if (pUnit == null)
			return;
		CreateAttackOverEffect (pUnit);

	}
	public void OnStageRestorePosEvent(GameEvent evt)
	{
		//Debug.Log ("OnStagePopCharEvent");
		StageRestorePosEvent Evt = evt as StageRestorePosEvent;
		if (Evt == null)
			return;
		Panel_unit pUnit = GetUnitByIdent (Evt.nIdent);
		if (pUnit == null)
			return;

	}

	public void OnStageBattleAttackEvent(GameEvent evt)
	{
		//Debug.Log ("OnStagePopCharEvent");
		StageBattleAttackEvent Evt = evt as StageBattleAttackEvent;
		if (Evt == null)
			return;

		// auto close all say window
		TalkSayEndEvent sayevt = new TalkSayEndEvent();
		sayevt.nChar = 0;		
		GameEventManager.DispatchEvent ( sayevt  );
		// attack 

		Panel_unit pAtkUnit = GetUnitByCharID ( Evt.nAtkCharID );
		Panel_unit pDefUnit = GetUnitByCharID ( Evt.nDefCharID );
		if (pAtkUnit == null || pDefUnit == null)
			return;
		int nRange = 1;
		if (Evt.nAtkSkillID != 0) {

		}

		// check if need move 
		int nDist = pAtkUnit.Loc.Dist (pDefUnit.Loc);
		if (nDist > nRange) {

			List< iVec2> path = GameScene.Instance.Grids.PathFinding( pAtkUnit.Loc , pDefUnit.Loc , 0  ); // no any block
			//PathFinding
			
			if( path.Count > 2 )
			{
				iVec2 last = path[path.Count -2 ];
				ActionManager.Instance.CreateMoveAction( pAtkUnit.Ident() , last.X , last.Y );	
				MoveToGameObj( pAtkUnit.gameObject , false );
			}

			// send move act

			//iVec2 tarPos =  FindEmptyPos( pDefUnit.Loc );
			//ActionManager.Instance.CreateMoveAction( pAtkUnit.Ident() , tarPos.X , tarPos.Y );


		}

		// send attack
		//Panel_StageUI.Instance.MoveToGameObj(pDefUnit.gameObject , false );  // move to def 
		ActionManager.Instance.CreateAttackAction( pAtkUnit.Ident() , pDefUnit.Ident(), Evt.nAtkSkillID );
		
	}

	public void OnStageMoveToUnitEvent(GameEvent evt)
	{
		//Debug.Log ("OnStagePopCharEvent");
		StageMoveToUnitEvent Evt = evt as StageMoveToUnitEvent;
		if (Evt == null)
			return;


		// auto close all say window
		TalkSayEndEvent sayevt = new TalkSayEndEvent();
		sayevt.nChar = 0;		
		GameEventManager.DispatchEvent ( sayevt  );


		// force close 
		Panel_unit pAtkUnit = GetUnitByCharID (Evt.nAtkCharID);
		Panel_unit pDefUnit = GetUnitByCharID (Evt.nDefCharID);
		if (pAtkUnit == null || pDefUnit == null)
			return;
		int nDist = pAtkUnit.Loc.Dist (pDefUnit.Loc);
		if (nDist > 1) {
			
			List< iVec2> path = GameScene.Instance.Grids.PathFinding( pAtkUnit.Loc , pDefUnit.Loc , 0  ); // no any block
			//PathFinding
			
			if( path.Count > 2 )
			{
				iVec2 last = path[path.Count -2 ];
				ActionManager.Instance.CreateMoveAction( pAtkUnit.Ident() , last.X , last.Y );	
				MoveToGameObj( pAtkUnit.gameObject , false );
			}
			// move only
			
		}
	}


	public bool CheckIsEmptyPos( iVec2 pos )
	{
		if (Grids.Contain (pos) == false)
			return false;

		// check tile
		if( cMyGrids.IsWalkAbleTile( Grids.GetValue( pos ) ) == false )
			return false;

		// check thiing

		// check unit
		foreach( KeyValuePair< int , Panel_unit  > pair in IdentToUnit )
		{
			if( pair.Value.Loc.Collision( pos ) ){
				return false;
			}
		}

		return true;
	}

	public iVec2 FindEmptyPos( iVec2 st  , int len = 999 )
	{
		if (CheckIsEmptyPos (st)) {
			return st;
		}
		// get a empty pos that can pop 

		for (int i=1; i < len; i++) {
			List<iVec2> pool = Grids.GetRangePool( st , i , i-1 );
			if( pool == null )
					continue;
			foreach( iVec2 pos in pool )
			{
				if( CheckIsEmptyPos(pos) ){
					return pos;
				}
			}
		}


		Debug.Log ( " Error ! can't find a Empty Pos");
		return null;


	}
	// 
	public List< iVec2> GetAllUnitPosList( )
	{
		List< iVec2> lst = new List< iVec2> ();
		foreach (KeyValuePair < int , Panel_unit > p in IdentToUnit) {
			lst.Add( p.Value.Loc );
		}

		return lst;
	}

	public List< iVec2>  GetUnitPKPosPool( Panel_unit unit , bool bCanPK  )
	{

		List< iVec2> lst = new List< iVec2> ();
		foreach (KeyValuePair < int , Panel_unit > pair in IdentToUnit) {
			if( unit.CanPK( pair.Value ) == bCanPK )
			{
				lst.Add( pair.Value.Loc );
			}
		}		
		return lst;
	}

//	public List< iVec2>  GetTarPosPKPosPool( Panel_unit unit , bool bCanPK  , int nTarX , int nTarY , int nDist = 0 )
//	{
//		List< iVec2> lst = new List< iVec2> ();
//		foreach (KeyValuePair < int , Panel_unit > pair in IdentToUnit) {
//			if( unit.CanPK( pair.Value ) == bCanPK )
//			{
//				lst.Add( pair.Value.Loc );
//			}
//		}		
//		return lst;
//
//	}

	public List< iVec2 > PathFinding( Panel_unit unit , iVec2 st , iVec2 ed , int nStep = 999)
	{
		// nStep = 999; // debug

		List< iVec2 > path = null;
		//List< iVec2 > unitList =  GetUnitPosList();

		// try the short path
		//this.GetUnitPKPosPool (unit, true);

		Grids.ClearIgnorePool();
		Grids.AddIgnorePool (  GetUnitPKPosPool( unit , true  ) );  // all is block in first find

//		List< iVec2 > nearList = Grids.GetRangePool( ed , 1  ); // the 4 pos can't stand ally
//		foreach( iVec2 pos in nearList ){
//			if( CheckIsEmptyPos( pos ) == false ){
//				Grids.AddIgnorePos( pos );
//			}
//		}

		// avoid the end node have ally
		//List< iVec2 > nearList =  GetUnitPKPosPool( unit , );


		path = Grids.PathFinding( st , ed , nStep  );

//		if (path == null) {
//			Grids.ClearIgnorePool();
//			//Grids.AddIgnorePool ( GetUnitPosList() );  // only enemy is block in second find
//			Grids.AddIgnorePool(  GetUnitPKPosPool( unit , true  ) ); //
//			path = Grids.PathFinding( st , ed , nStep  );
//		}

		if (Config.GOD == true) {
			CreatePathOverEffect (path); // draw path
		}

		return path;
	}

	// tool func to get aoe affect pool
	public List < iVec2 > GetAOEPool( int nX , int nY , int nAoe )
	{
		iVec2 st = new iVec2 ( nX , nY );
		Dictionary< string , iVec2 > tmp = new Dictionary< string , iVec2 >();
		tmp.Add ( st.GetKey() , st );
		//			pool.Add ( st );
		
		AOE aoe = ConstDataManager.Instance.GetRow<AOE> ( nAoe ) ;
		if (aoe != null) {
			// add extra first
			cTextArray TA = new cTextArray();
			TA.SetText ( aoe.s_EXTRA );
			for( int i = 0 ; i < TA.GetMaxCol(); i++ )
			{
				CTextLine line  = TA.GetTextLine( i );
				for( int j = 0 ; j < line.GetRowNum() ; j++ )
				{
					string s = line.m_kTextPool[ j ];
					
					string [] arg = s.Split( ",".ToCharArray() );
					if( arg.Length < 2 )
						continue;
					if( arg[0] != null && arg[1] != null )
					{
						int x = int.Parse( arg[0] );
						int y = int.Parse( arg[1] );
						iVec2 v = st.MoveXY( x , y );

						if( Grids.Contain( v ) == false )
							continue;

						string key = v.GetKey();
						if( tmp.ContainsKey( key ) == false ){
							tmp.Add( key , v );
						}
					}
				}
			}
			// get range pool	
			List<iVec2> r = Grids.GetRangePool( st , aoe.n_MAX , aoe.n_MIN );
			
			
			
		}
		
		List < iVec2 > pool = new List < iVec2 > ();
		return pool;
	}


}
