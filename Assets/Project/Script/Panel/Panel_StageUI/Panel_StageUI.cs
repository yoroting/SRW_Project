using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _SRW;
using MYGRIDS;
using MyClassLibrary;			// for parser string



//public class Panel_StageUI : MonoBehaviour 
public class Panel_StageUI : Singleton<Panel_StageUI>
{
	public const string Name = "Panel_StageUI";

	public GameObject BackGroundObj; // back ground
	public GameObject TilePlaneObj; // plane of all tiles sprite
	public GameObject MaskPanelObj; // plane mask

	public cMyGrids	Grids;				// main grids . only one

	STAGE_DATA	StageData;

	// drag canvas limit								
	float fMinOffX ;
	float fMaxOffX ;
	float fMinOffY ;
	float fMaxOffY ;

	// 



	// widget
	Dictionary< int , STAGE_EVENT > EvtPool;			// add event id 
	List< STAGE_EVENT >				WaitPool;			// wait to exec pool
	Dictionary< int , int > EvtCompletePool;			// record event complete round 

	STAGE_EVENT						NextEvent;
	private cTextArray 				m_cScript;			// 劇本 腳本集合
	private int						m_nFlowIdx;			// 腳本演到哪段
	bool							IsEventEnd;			// 
	bool							CheckEventPause;	// 	event check pause

	// Select effect
	Dictionary< string , GameObject >  OverCellPool;			// Over tile effect pool ( in = cell key )
	Dictionary< string , GameObject >  OverCellAtkPool;			// Over tile effect pool( attack ) ( in = cell key )
	//List< GameObject >				OverCellPool;			 

	//Dictionary< int , GameObject > EnemyPool;			// EnemyPool this should be group pool


	Dictionary< int , Panel_unit > IdentToUnit;			// allyPool
	Dictionary< int , UNIT_DATA > UnitDataPool;			// ConstData pool

	// ScreenRatio
	// float fUIRatio;


	void Awake( ){	

		//UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);	
		//fUIRatio = (float)mRoot.activeHeight / Screen.height;

		//float ratio = (float)mRoot.activeHeight / Screen.height;

		// UI Event
		// UIEventListener.Get(BackGroundObj).onClick += OnBackGroundClick;
	
		// grid
		//Grids = new cMyGrids ();
		GameScene.Instance.Initial ();

		Grids = GameScene.Instance.Grids;						// smart pointer reference

		StageData = new STAGE_DATA();
		//event
		EvtPool = new Dictionary< int , STAGE_EVENT >();			// add event id 
		WaitPool = new List< STAGE_EVENT >();					// check ok. waitinf to execute event
		EvtCompletePool = new Dictionary< int , int >();

		// unit
		IdentToUnit = new Dictionary< int , Panel_unit >();		//
		UnitDataPool = new Dictionary< int , UNIT_DATA >();

		OverCellPool 	= new Dictionary< string , GameObject >();			// Over tile effect pool ( in = cell key )
		OverCellAtkPool = new Dictionary< string , GameObject >();			

		// Debug Code jump to stage
		GameDataManager.Instance.nStageID = 1;

		// Stage Event
		GameEventManager.AddEventListener(  StageBGMEvent.Name , OnStageBGMEvent );

		GameEventManager.AddEventListener(  StagePopCharEvent.Name , OnStagePopCharEvent );
		GameEventManager.AddEventListener(  StagePopMobEvent.Name , OnStagePopMobEvent );
		GameEventManager.AddEventListener(  StagePopMobGroupEvent.Name , OnStagePopMobGroupEvent );

		GameEventManager.AddEventListener(  StageDelCharEvent.Name , OnStageDelCharEvent );
		GameEventManager.AddEventListener(  StageDelMobEvent.Name , OnStageDelMobEvent );

		// char event 
		GameEventManager.AddEventListener(  StageCharMoveEvent.Name , OnStageCharMoveEvent );

		// cmd event
		GameEventManager.AddEventListener(  StageShowMoveRangeEvent.Name , OnStageShowMoveRangeEvent );
		GameEventManager.AddEventListener(  StageShowAttackRangeEvent.Name , OnStageShowAttackRangeEvent );
		GameEventManager.AddEventListener(  StageRestorePosEvent.Name , OnStageRestorePosEvent );

		// create singloten

		// can't open panel in awake
	}

	// Use this for initialization
	void Start () {

		// load const data
		StageData = ConstDataManager.Instance.GetRow<STAGE_DATA> ( GameDataManager.Instance.nStageID );
		if( StageData == null )
			return ;

		// load scene file
		if( LoadScene( StageData.n_SCENE_ID ) == false )
			return ;

		// EVENT 
		GameDataManager.Instance.nRound = 0;
		GameDataManager.Instance.nActiveCamp  = _CAMP._PLAYER;

		//Record All Event to execute
		EvtPool.Clear();
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


		IsEventEnd = false;
		//Dictionary< int , STAGE_EVENT > EvtPool;			// add event id 

	

	}

	void FixPlanePosition()
	{
		// ensure canvrs in screen
		if( TilePlaneObj != null )
		{
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

	// Update is called once per frame
	void Update () {

		FixPlanePosition ();



		// block other event
		if( IsAnyActionRunning() == true )
			return;

		//=== Unit Action ====
		// Atk / mov / other action need to preform first
		RunUnitAction();

		if (BattleManager.Instance.IsBattlePhase ()) {// this will throw many unit action
			BattleManager.Instance.Run();
			return;
		}

		//==================	
		RunEvent( );				// this will throw many unit action

		if( IsRunningEvent() == true  )
			return; // block other action  

		// check if need pop cmd UI auto
		if ( CheckPopNextCMD () == true)
			return;


		//===================		// this will throw unit action or trig Event
		if ( RunCampAI( GameDataManager.Instance.nActiveCamp ) == false )
		{
			// no ai to run. go to next faction
			GameDataManager.Instance.NextCamp();
		}

		//

	}

	void OnDestroy()
	{
	
	}

	Panel_CMDUnitUI OpenCMDUI( _CMD_TYPE type , Panel_unit cmder )
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

	void CloseCMDUI()
	{
		Panel_CMDUnitUI panel = MyTool.GetPanel<Panel_CMDUnitUI>( PanelManager.Instance.JustGetUI(Panel_CMDUnitUI.Name)) ;
		if( panel != null )
		{
			panel.CancelCmd();
		}
		ClearOverCellEffect();
		cCMD.Instance.eCMDSTATUS = _CMD_STATUS._NONE;
	}

	void RollBackCMDUIWaitTargetMode()
	{
		cCMD.Instance.eCMDSTATUS = _CMD_STATUS._WAIT_CMDID;
		cCMD.Instance.eCMDID 	   = _CMD_ID._NONE;
		cCMD.Instance.eCMDTARGET = _CMD_TARGET._ALL;   // only unit
		ClearOverCellEffect ();
		PanelManager.Instance.OpenUI( Panel_CMDUnitUI.Name );
	}

	void OnCellClick(GameObject go)
	{
		if( IsAnyActionRunning() == true )
			return;
		
		if( IsRunningEvent() == true  )
			return; // block other action  


		UnitCell unit = go.GetComponent<UnitCell>() ;
		if( unit != null ){
			string str = string.Format( "CellOnClick( {0},{1}) " , unit.X() , unit.Y() );
			string sKey =	unit.Loc.GetKey();
			Debug.Log(str);
			if( cCMD.Instance.eCMDSTATUS == _CMD_STATUS._WAIT_CMDID ) // is waiting cmd id. this is moving act 
			{
				if( cCMD.Instance.eCMDTYPE == _CMD_TYPE._ALLY )
				{
					if( OverCellPool.ContainsKey( sKey ) == true  )
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
						CloseCMDUI();
						// need cancel cmd
					
					}
					return ;
				}
				else 
				{
					// close 
					CloseCMDUI();
				}
			}
			else if( cCMD.Instance.eCMDSTATUS == _CMD_STATUS._WAIT_TARGET )
			{
				if( cCMD.Instance.eCMDTARGET == _CMD_TARGET._ALL || 
				   cCMD.Instance.eCMDTARGET == _CMD_TARGET._POS   ){

					// make one map skill cmd

				}
				else { 
					// this is impossible
					if( OverCellAtkPool.ContainsKey( sKey ) == false   )
					{
						RollBackCMDUIWaitTargetMode();
						//Panel_CMDUnitUI.BackWaitCmd();		// back if the cmd is not exists	
					}
				}
				return ;
			}


			//cCMD.Instance.eCMDTYPE = _CMD_TYPE._CELL;
			// clear over effect
		//	ClearOverCellEffect( );

			// check to open sys ui
			// avoid re open
//			if( PanelManager.Instance.CheckUIIsOpening( "Panel_CMDUI" ) == false  )
//			{
			OpenCMDUI( _CMD_TYPE._CELL , null );
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

	// if any char be click
	void OnCharClick(GameObject go)
	{
		if( IsAnyActionRunning() == true )
			return;
		
		if( IsRunningEvent() == true  )
			return; // block other action  

		// avoid any ui opening
		// clear over effect



		Panel_unit unit = go.GetComponent<Panel_unit>() ;
		if( unit == null )
			return ;

		if (cCMD.Instance.eCMDSTATUS == _CMD_STATUS._NONE) {
			//GameObject obj = PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name);
			Panel_CMDUnitUI panel = OpenCMDUI( _CMD_TYPE._ALLY , unit );
			CreateMoveOverEffect (unit);
			return;
		}
		else if (cCMD.Instance.eCMDSTATUS == _CMD_STATUS._WAIT_TARGET ){
//			Panel_CMDUnitUI panel = MyTool.GetPanel< Panel_CMDUnitUI >( PanelManager.Instance.JustGetUI( Panel_CMDUnitUI.Name ) ); 
//			if( panel )
//				panel.CancelCmd();
			//GameObject obj = PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name);
			string sKey = unit.Loc.GetKey ();
			Debug.Log( "OnCharClick" + sKey + ";Ident"+unit.Ident() ); 
			if ( OverCellAtkPool.ContainsKey (sKey) == true) {
				Panel_CMDUnitUI panel = MyTool.GetPanel<Panel_CMDUnitUI> ( PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name) );
				if (panel != null) {
					panel.SetTarget( unit );
			}
			}
			ClearOverCellEffect ();

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
		
		// avoid any ui opening
		// clear over effect
		Panel_unit unit = go.GetComponent<Panel_unit>() ;
		if( unit == null )
			return ;
		string sKey = unit.Loc.GetKey ();
		Debug.Log( "OnMobClick" + sKey + ";Ident"+unit.Ident() ); 

		// open new cmd ui when this idn't a new cmd
		if (cCMD.Instance.eCMDSTATUS == _CMD_STATUS._WAIT_TARGET ) {		
			if ( OverCellAtkPool.ContainsKey (sKey) == true) {
				// send atk cmd
				Panel_CMDUnitUI panel = MyTool.GetPanel< Panel_CMDUnitUI >( PanelManager.Instance.JustGetUI( Panel_CMDUnitUI.Name ) ); 
				panel.SetTarget( unit );
			}

			ClearOverCellEffect(  );
			return;
		}


		ClearOverCellEffect(  );

		OpenCMDUI ( _CMD_TYPE._ENEMY , unit );

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
		if (scn == null)
			return false;
		string filename = "Assets/StreamingAssets/scn/"+scn.s_MODLE_ID+".scn";		


		if( Grids.Load( filename )==true )
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
		GameSystem.PlayBGM ( scn.n_BGM );


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
			}


			return cell;
		}
			//_TILE._GREEN

		return null;
	}

	public void ClearOverCellEffect(  )
	{
		// clear eff
		//OverCellPool
		foreach( KeyValuePair< string , GameObject> pair in OverCellPool )
		{
			if( pair.Value != null )
			{
				NGUITools.Destroy( pair.Value );
				//pair.Value = null;
			}
		}
		OverCellPool.Clear ();
		foreach( KeyValuePair< string , GameObject> pair in OverCellAtkPool )
		{
			if( pair.Value != null )
			{
				NGUITools.Destroy( pair.Value );
				//pair.Value = null;
			}
		}
		OverCellAtkPool.Clear ();

	}

	public void CreateMoveOverEffect( Panel_unit unit )
	{
		foreach( KeyValuePair< string , GameObject> pair in OverCellPool )
		{
			if( pair.Value != null )
			{
				NGUITools.Destroy( pair.Value );
				//pair.Value = null;
			}
		}
		OverCellPool.Clear ();
		if (unit == null)
			return;

		List<iVec2> moveList =  Grids.GetRangePool (unit.Loc, 4);
		// start create over eff
		foreach( iVec2 v in moveList )
		{
			GameObject over = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/MoveOverEffect");
			if( over != null )
			{
				over.name = string.Format("Over({0},{1},{2})", v.X , v.Y , 0 );
				SynGridToLocalPos( over , v.X , v.Y) ;
				
				//UIEventListener.Get(over).onClick += OnOverClick;
				
				OverCellPool.Add( v.GetKey() , over );
			}
		}
	}

	public void CreateAttackOverEffect( Panel_unit unit )
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

		List<iVec2> AtkList =  Grids.GetRangePool ( unit.Loc, 1 );
		AtkList.RemoveAt (0); // remove self pos

		foreach( iVec2 v in AtkList )
		{
			GameObject over = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/AttackOverEffect");
			if( over != null )
			{
				over.name = string.Format("Over({0},{1},{2})", v.X , v.Y , 0 );
				SynGridToLocalPos( over , v.X , v.Y) ;
				OverCellAtkPool.Add( v.GetKey() , over );
			}
		}

	}
	// Check any action is running

	bool IsAnyActionRunning()
	{
		if (BattleMsg.nMsgCount > 0)
			return true;

		if( PanelManager.Instance.CheckUIIsOpening( "Panel_Talk" ) == true )
			return true;

		foreach( KeyValuePair< int , Panel_unit > pair in IdentToUnit )
		{
			if( pair.Value == null )
				continue;
			if( pair.Value.IsMoving() )
			{
				return true;
			}
			if( pair.Value.IsAnimate() )
			{
				return true;
			}
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

	void RunUnitAction()
	{


	}


	// Faction AI
	bool RunCampAI( _CAMP nCamp )
	{
		// our faction don't need AI process
		if( nCamp == _CAMP._PLAYER )
			return true; // player is playing 

		// change faction if all unit moved or dead.
		cCamp unit = GameDataManager.Instance.GetCamp( nCamp );
		if( unit != null )
		{
			if( unit.memLst.Count <= 0 ){
				return false;
			}
			// Run one unit AI

			return true;
		}
		return false;
	}


	void CheckEventToRun()
	{
		if( (EvtPool==null) )
			return ;
		if( EvtPool.Count<=0)
			return;

		List< int > removeLst = new List< int >();
		// get next event to run
		foreach( KeyValuePair< int ,STAGE_EVENT > pair in EvtPool ) 
		{
			if( CheckEvent( pair.Value ) == true ){		// check if this event need run
				//NextEvent = pair.Value ; 		// run in next loop
				WaitPool.Add( pair.Value );
				removeLst.Add( pair.Key );
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
	}

	// Check event
	bool CheckEvent( STAGE_EVENT evt )
	{
		cTextArray sCond = new cTextArray( );
		sCond.SetText( evt.s_CONDITION );
		// check all line . if one line success . this event check return true

		int nCol = sCond.GetMaxCol();
		for( int i= 0 ; i <nCol ; i++ )
		{
			if( CheckEventCondition( sCond.GetTextLine( i ) ) )
			{
				return true;
			}
		}
		return false;
	}

	bool CheckEventCondition( CTextLine line )
	{
		if( line == null )
			return false;

		List<cTextFunc> funcList =line.GetFuncList();
		foreach( cTextFunc func in funcList )
		{
			if( func.sFunc == "GO" )
			{
				if( ConditionGO( ) == false )
				{
					return false;
				}	
			}
			if( func.sFunc == "ALLDEAD" )
			{
				if( ConditionAllDead( func.At(0) ) == false )
				{
					return false;
				}				
			}
			else if( func.sFunc == "DEAD"  )
			{
				if( ConditionUnitDead( func.At(0), func.At(1) ) == false )
				{
					return false;
				}				
			}
			else if( func.sFunc == "ROUND"  )
			{
				if( ConditionRound( func.At(0) ) == false )
				{
					return false;
				}				
			}		
			else if( func.sFunc == "AFTER"  )
			{
				if( ConditionAfter( func.At(0),func.At(1)  ) == false )
				{
					return false;
				}				
			}
		}
		return true;
	}

	// condition check 
	bool ConditionGO(  ) // always active
	{
		return true;
	}

	bool ConditionAllDead( int nCampID )
	{
		// assign id
		cCamp unit = GameDataManager.Instance.GetCamp( (_CAMP)nCampID );
		if( unit != null )
		{
			return (unit.memLst.Count<=0) ;
		}
		return true;
	}

	bool ConditionUnitDead( int nCampID ,int nCharID )
	{
		// assign id
		cCamp camp = GameDataManager.Instance.GetCamp( (_CAMP)nCampID );
		if( camp != null )
		{
			foreach( int no in  camp.memLst )
			{
				Panel_unit unit = this.IdentToUnit[ no ];
				if( unit != null )
				{
					if( unit.CharID ==  nCharID )
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	bool ConditionRound( int nID )
	{
	//	if( GameDataManager.Instance.nRoundStatus != 0 )
	//		return false;

		return (GameDataManager.Instance.nRound >=nID ) ;
	}
	bool ConditionAfter( int nID , int nRound  )
	{
		if( EvtCompletePool.ContainsKey( nID )  ){
			int nCompleteRound = EvtCompletePool[ nID ];

			return ( GameDataManager.Instance.nRound >= ( nCompleteRound + nRound ) );
		}
		return false;
	}

	//==========Execute Event =================
	void RunEvent(  )
	{
		// always check to wait list
		CheckEventToRun();

		// get next event
		if( NextEvent == null )
		{
			if( this.WaitPool.Count > 0 )
			{
				NextEvent =  WaitPool[0];
				WaitPool.RemoveAt( 0 );

				PreEcecuteEvent();					// parser event to run
			}

			// get next event to run

		}

		// if event id running

		  //run event
		if( NextEvent != null )
		{
			NextLine();					// execute one line

			//NextLine();					// parser event to run
			if( IsNextEventCompleted() )
			{
				// clear event for next
				NextEvent = null;
				IsEventEnd = true;

				// all end . check again  for new condition status
				if( WaitPool.Count <= 0 ){
					CheckEventToRun();
				}
			}
		}

		// switch status
		//GameDataManager.Instance.nRound = 0;
		//GameDataManager.Instance.nActiveFaction  = 0;

	}

	// check any event is runing or wait running
	bool IsRunningEvent ()
	{
		if( WaitPool.Count > 0 )
			return true;
		if( NextEvent != null )
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
			ParserScript( line );
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


	void ParserScript( CTextLine line )
	{
		List<cTextFunc> funcList =line.GetFuncList();
		foreach( cTextFunc func in funcList )
		{
			if( func.sFunc == "POPCHAR" )
			{
				int charid = func.At( 0 );
				StagePopCharEvent evt = new StagePopCharEvent ();
				evt.nCharID = func.At( 0 );
				evt.nX		= func.At( 1 );
				evt.nY		= func.At( 2 );
				GameEventManager.DispatchEvent ( evt );
			}
			else if( func.sFunc == "POPMOB" )
			{
				int charid = func.At( 0 );
				StagePopMobEvent evt = new StagePopMobEvent ();
				evt.nCharID = func.At( 0 );
				evt.nX		= func.At( 1 );
				evt.nY		= func.At( 2 );
				GameEventManager.DispatchEvent ( evt );
			}
			else if( func.sFunc == "POPMOBGROUP" )
			{
				// create from cline table

			}
			else if( func.sFunc == "TALK"  )
			{
#if UNITY_EDITOR
			//	return ;
#endif
				int nID = func.At( 0 );
				GameSystem.TalkEvent( nID );
			}
			else if( func.sFunc == "BGM"  )
			{
				int nID = func.At( 0 );
				// change bgm 
				GameSystem.PlayBGM ( nID );


			}
		}
	}


	// Widget func	 
	GameObject AddChar( _CAMP nCampID , int nCharID , int x , int y )
	{
		CHARS charData = ConstDataManager.Instance.GetRow<CHARS>( nCharID );
		if( charData == null)
			return null;
		// get data from Const data
		GameObject obj = ResourcesManager.CreatePrefabGameObj( TilePlaneObj , "Prefab/Panel_Unit" );
		if( obj == null )return null;
			
		// charge face text				
		UITexture tex = obj.GetComponentInChildren<UITexture>();
			
		if( tex )
		{
			if(tex != null){
					//	DynamicAssetBundleLoader.LoadTexture(tex,DynamicAssetBundleLoader.SSAssetType.Card, "CARD_" + card.PicName);

				string url = "Assets/Art/char/" + charData.s_FILENAME +"_S.png";
				Texture t= Resources.LoadAssetAtPath( url , typeof(Texture) ) as Texture; ;
				tex.mainTexture = t;
					// tex.MakePixelPerfect(); don't make pixel it
			}
		}
		// regedit to gamedata manager
		Panel_unit unitobj = obj.GetComponent<Panel_unit>();
		//UNIT_DATA unit = GameDataManager.Instance.CreateChar( nCharID );
		if( unitobj != null )
		{
			// setup param
			unitobj.CreateChar( nCharID , x , y );
		}

		// position // set in create
		//obj.transform.localPosition =  MyTool.SnyGridtoLocalPos( x , y , ref Grids ) ; 

		if (nCampID == _CAMP._PLAYER) {		
			UIEventListener.Get (obj).onClick += OnCharClick;
		} else if (nCampID == _CAMP._ENEMY) {
			UIEventListener.Get (obj).onClick += OnMobClick;
		}

		// all ready
		NGUITools.SetActive( obj , true );
		return obj;
	}

	// Take care use ident to delete
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
					NGUITools.Destroy( unit.gameObject );
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

			PanelManager.Instance.OpenUI( Panel_CMDUnitUI.Name ); // only open UI. don't change other param
			return  true;
		}	
		return false;
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

	public void OnStagePopCharEvent(GameEvent evt)
	{
		//Debug.Log ("OnStagePopCharEvent");
		StagePopCharEvent Evt = evt as StagePopCharEvent;
		if (Evt == null)
			return;
		GameObject obj = AddChar( _CAMP._PLAYER , Evt.nCharID , Evt.nX , Evt.nY );
		if( obj != null )
		{
			Panel_unit unit = obj.GetComponent<Panel_unit>();
			//set as ally			
			GameDataManager.Instance.AddCampMember( _CAMP._PLAYER , unit.pUnitData.n_Ident ); // global game data
			
			IdentToUnit.Add( unit.pUnitData.n_Ident , unit  ) ;// stage gameobj
		}
		else{
			Debug.Log ( string.Format("OnStagePopCharEvent Fail with charid({0})" , Evt.nCharID ));

		}

	}

	public void OnStagePopMobEvent(GameEvent evt)
	{
		//Debug.Log ("OnStagePopMobEvent");
		StagePopMobEvent Evt = evt as StagePopMobEvent;
		if (Evt == null)
			return;
		GameObject obj = AddChar( _CAMP._ENEMY , Evt.nCharID , Evt.nX , Evt.nY );
		if( obj != null )
		{
			Panel_unit unitobj = obj.GetComponent<Panel_unit>();
			//set as ally						
			GameDataManager.Instance.AddCampMember( _CAMP._ENEMY , unitobj.pUnitData.n_Ident ); // global game data
			
			IdentToUnit.Add( unitobj.pUnitData.n_Ident , unitobj  ) ;// stage gameobj
		}
		else{
			Debug.Log ( string.Format("OnStagePopCharEvent Fail with charid({0})" , Evt.nCharID ));
			
		}
	}

	public void OnStagePopMobGroupEvent(GameEvent evt)
	{
		Debug.Log ("OnStagePopMobGroupEvent");
		StagePopMobGroupEvent Evt = evt as StagePopMobGroupEvent;
		if (Evt == null)
			return;
		
	}

	public void OnStageDelCharEvent(GameEvent evt)
	{
		//Debug.Log ("OnStagePopCharEvent");
		StageDelCharEvent Evt = evt as StageDelCharEvent;
		if (Evt == null)
			return;
		int nCharid = Evt.nCharID;
		DelChar( _CAMP._PLAYER , nCharid );

	}

	public void OnStageDelMobEvent(GameEvent evt)
	{
		//Debug.Log ("OnStagePopCharEvent");
		StageDelMobEvent Evt = evt as StageDelMobEvent;
		if (Evt == null)
			return;
		int nCharid = Evt.nCharID;
		DelChar( _CAMP._ENEMY , nCharid );
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


	// 
}
