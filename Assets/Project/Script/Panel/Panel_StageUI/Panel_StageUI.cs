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
                  //  go.layer = 0;
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


    public GameObject UnitSpiteObj; // unit sprite obj
    public GameObject MobActEffObj; // Mob action mask
  
    GameObject ActEffFolObj;


    //public GameObject TileObj; 	//  no need this
    public GameObject MoveEftObj; 	// 
	public GameObject AtkEftObj; 	// 
	public GameObject AoeEftObj; 	// 
	public GameObject ValueEftObj; 	// 
	public GameObject AvgObj;		//


	Panel_unit TarceMoveingUnit; //  Trace the moving unit

	public MyGrids	Grids;				// main grids . only one

	STAGE_DATA	StageData;

	// For flag
	bool	bIsLoading	;
	bool	bIsReady;

	public bool	bIsStageEnd;								// this stage is end
	bool 							bIsMoveToObj;		// is move camera to obj
//	public bool	bIsRestoreData;								// this stage is need restore data
	public bool m_bIsSkipMode;							// skip for script perform

	// drag canvas limit								
	float fMinOffX ;
	float fMaxOffX ;
	float fMinOffY ;
	float fMaxOffY ;

    float fFxPlayTime;                      //特效播放時間強制解鎖用

	// widget
	Dictionary< int , STAGE_EVENT > EvtPool;			// add event id 
	//List< STAGE_EVENT > EvtPool;			// add event id 
	// change to evt list for custom order by list 
	//List< STAGE_EVENT > EvtPool;			// add event id 

	List< STAGE_EVENT >				WaitPool;			// wait to exec pool
//	Dictionary< int , int > EvtCompletePool;			// record event complete round 

	STAGE_EVENT						NextEvent;
	private cTextArray 				m_cScript;			// 劇本 腳本集合
	private int						m_nFlowIdx;			// 腳本演到哪段
	bool							IsEventEnd;			// 
//	bool							IsPreBattleEvent;	// is prebattle event



//	bool							CheckEventPause;	// 	event check pause

	// Select effect
	Dictionary< string , GameObject >  OverCellPool;			// Over tile effect pool ( str = cell key )
	Dictionary< string , GameObject >  OverCellAtkPool;			// Over tile effect pool( attack ) ( str = cell key )

	Dictionary< string , GameObject > OverCellPathPool;
	Dictionary< string , GameObject > OverCellAOEPool;

	Dictionary< string , GameObject >	OverCellZocPool;
	//List< GameObject >				OverCellPool;
	Dictionary< string , GameObject > OverCellMarkPool;		// mark cell

	//Dictionary< int , GameObject > EnemyPool;			// EnemyPool this should be group pool


	Dictionary< int , Panel_unit > IdentToUnit;			// allyPool

	List< iVec2 >	tmpScriptMoveEnd;					// script 用的單位移動 mrak pool 防止 script 讓不同單位移動到 同樣座標

	//Dictionary< int , UNIT_DATA > UnitDataPool;			// ConstData pool

    void Awake()
    {
        instance = this; 		// special singleton
        bIsReady = false;

        bIsStageEnd = false;
		m_bIsSkipMode = false;
        //UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);	
        //fUIRatio = (float)mRoot.activeHeight / Screen.height;

        //float ratio = (float)mRoot.activeHeight / Screen.height;

        // UI Event
        // UIEventListener.Get(BackGroundObj).onClick += OnBackGroundClick;

        // grid
        //Grids = new cMyGrids ();
        GameScene.Instance.Initial();							// it will avoid double initial inside.

        Grids = GameScene.Instance.Grids;						// smart pointer reference

        //	ActionPool = List< uAction >();				// record all action to do 
        // Debug Code jump to stage
        //	GameDataManager.Instance.nStageID = 1;


        // create singloten

        // can't open panel in awake
        // Don't open create panel in stage awake. i have develop mode that place stage in scene initial. the panelmanager don't initial here


    }

    // Use this for initialization
    void Start()
    {

  //      long tick = System.DateTime.Now.Ticks;

        System.GC.Collect();			// Free memory resource here

        // create pool
        CreateAllDataPool();

   //     Debug.Log("stage srart loding");

        // loading panel
        //	PanelManager.Instance.OpenUI( "Panel_Loading");
        bIsLoading = true;
        //	StartCoroutine("StageLoading" );

        // clear data
        Clear();

 //       Debug.Log("stageloding:clearall");
        // load const data
        StageData = ConstDataManager.Instance.GetRow<STAGE_DATA>(GameDataManager.Instance.nStageID);
        if (StageData == null)
        {
            Debug.LogFormat("stageloding:StageData fail with ID {0}  ", GameDataManager.Instance.nStageID);
            return;
        }

        // load scene file
        if (LoadScene(StageData.n_SCENE_ID) == false)
        {
            Debug.LogFormat("stageloding:LoadScene fail with ID {0} ", StageData.n_SCENE_ID);
            return;
        }

        // EVENT 
        //GameDataManager.Instance.nRound = 0;		// many mob pop in talk ui. we need a 0 round to avoid issue

        //Record All Event to execute
        EvtPool.Clear();
		char[] split = { ';',' ',',' };

		string[] strMission = StageData.s_MISSION.Split(split);
		for (int i = 0; i < strMission.Length; i++)
		{
			int nMissionID = 0;
			if( int.TryParse( strMission[i],  out nMissionID ) ){
				if( 0 == nMissionID )
					continue;

				STAGE_EVENT evt = ConstDataManager.Instance.GetRow<STAGE_EVENT>(nMissionID);
				if (evt != null)
				{
					if( EvtPool.ContainsKey( nMissionID ) == false )
					{
						EvtPool.Add(nMissionID, evt);
					}
				}
			}
		}

		// mission
		string[] strEvent = StageData.s_EVENT.Split(split);
		for (int i = 0; i < strEvent.Length; i++)
		{
			int nEventID;
			if( int.TryParse( strEvent[i] , out nEventID ))
			{
				if( 0 == nEventID )
					continue;
				STAGE_EVENT evt = ConstDataManager.Instance.GetRow<STAGE_EVENT>(nEventID);
				if (evt != null)
				{
					if( EvtPool.ContainsKey( nEventID ) == false )
					{
						EvtPool.Add(nEventID, evt);
					}
				}
			}
		}

//		Debug.Log("stageloding:create event Pool complete");
		
		GameDataManager.Instance.SetBGMPhase( 0 );
		//GameDataManager.Instance.nPlayerBGM = StageData.n_PLAYER_BGM ;   //我方
		//GameDataManager.Instance.nEnemyBGM  = StageData.n_ENEMY_BGM;	 // 敵方
		//GameDataManager.Instance.nFriendBGM = StageData.n_FRIEND_BGM;	// 友方

        // regedit game event
        RegeditGameEvent(true);
        // create sub panel?

//        Panel_CMDUnitUI.OpenCMDUI(_CMD_TYPE._SYS, 0);

		Panel_UnitInfo.OpenUI (0);
        
        //PanelManager.Instance.OpenUI(Panel_UnitInfo.Name);
		Panel_MiniUnitInfo.OpenUI (null);
        //PanelManager.Instance.OpenUI(Panel_MiniUnitInfo.Name);

        //Dictionary< int , STAGE_EVENT > EvtPool;			// add event id 

//		if (bIsRestoreData == true) {
//			RestoreBySaveData();
//		}



        bIsLoading = false;		// debug mode no coror to close loading

  //      long during = System.DateTime.Now.Ticks - tick;
  //      Debug.Log("stage srart loding complete. total ticket:" + during);
    }


    void OnEnable()
    {
        // start the loading panel
    }


    // Update is called once per frame
    void Update()
    {

        // check if first run
        if (bIsReady == false)
        {
            if (bIsLoading)
            {
                return;
            }
            else
            {
                // close pre open panel
                OnReady();

            }
            // check ready complete
            return;
        }
        FixPlanePosition();

        if (bIsStageEnd == true)
            return;

        // Real update .. update each frae is not a good idea
       // GameDataManager.Instance.Update();

        // block other event
        if (IsAnyActionRunning() == true) // wait all tween / fx / textbox / battle msg finish / unit move
            return;							// don't check event run finish here.

        //=== Unit Action ====
        // Atk / mov / other action need to preform first
        if (ActionManager.Instance.Run() == true)
            return;

        // avoid event close soon when call talkui already
        if (PanelManager.Instance.CheckUIIsOpening(Panel_Talk.Name) == true)
            return;

        //==================	
        if (RunEvent() == true)// this will throw many unit action and interrupt battle.
            return;

        // if one event need to run battle. it should pause ein event
        if (BattleManager.Instance.IsBattlePhase())
        {// this will throw many unit action
            BattleManager.Instance.Run();
            return;
        }

        //	if( GetEventToRun() == true )
        //		return;

        //	if( IsRunningEvent() == true  )
        //		return true; // block other action  


        // check if need pop cmd UI auto
        if (CheckPopNextCMD() == true)
            return;

        if (CheckUnitDead() == true) // check here for event_manager  can detect hp ==0 first , some event may relive the deading unit
            return;

        // check drop event
        if (BattleManager.Instance.ProcessDrop() == true)
            return;

		if( bIsStageEnd == true ){  // 每個敵方單位行動後都可能觸發，關卡結束 要避免AI 繼續運算
			return ; // block all ai here
		}

        //===================		// this will throw unit action or trig Event
        if (RunCampAI(GameDataManager.Instance.nActiveCamp) == false)
        {
            // no ai to run. go to next faction
            GameDataManager.Instance.NextCamp();
        }


        //===================onchar

        // startup panel when opening event done.
        if (GameDataManager.Instance.nRound == 0)
        {
            GameDataManager.Instance.nRound = 1;			  // Go to round 1 	
            PanelManager.Instance.OpenUI(Panel_Round.Name);
            //StageWeakUpCampEvent cmd = new StageWeakUpCampEvent ();
            //cmd.nCamp = GameDataManager.Instance.nActiveCamp;
            //GameEventManager.DispatchEvent ( cmd );
        }

    }

    void OnDestroy()
    {
        // free singl
        instance = null;

        RegeditGameEvent(false);
        // create singloten


    }
    
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

		OverCellZocPool = new Dictionary< string , GameObject >(); // ZOC

		OverCellAOEPool= new Dictionary< string , GameObject >();

		OverCellMarkPool = new Dictionary< string , GameObject >();		// mark cell

		tmpScriptMoveEnd = new List< iVec2 >();                 // script 用的單位移動 mrak pool 防止 script 讓不同單位移動到 同樣座標

        //List < iVec2 >
        UnitSpiteObj.CreatePool( 1);//st_CellObjPoolSize / 2 

		MoveEftObj.CreatePool( st_CellObjPoolSize );
		AtkEftObj.CreatePool( st_CellObjPoolSize );
		AoeEftObj.CreatePool( st_CellObjPoolSize /4 );

        ValueEftObj.transform.localPosition = Vector3.zero; 
        ValueEftObj.CreatePool( st_CellObjPoolSize / 10  );

		AvgObj.CreatePool ( st_CellObjPoolSize / 10 );

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

	void Clear()  // public for gamedatamanage to load 
	{
        //
        MobActEffObj.SetActive(false);

        //
        ClearOverCellEffect ();

		// clear unit 
		foreach (KeyValuePair<int , Panel_unit  > pair in IdentToUnit) {
			//NGUITools.Destroy( pair.Value.gameObject );
			//pair.Value.FreeUnitData();
			pair.Value.Recycle();
		}
		IdentToUnit.Clear ();

        fFxPlayTime = 0.0f;

        // EVENT 
        GameDataManager.Instance.ResetStage ();

		//GameDataManager.Instance.nRound = 0;		// many mob pop in talk ui. we need a 0 round to avoid issue
		//GameDataManager.Instance.nActiveCamp  = _CAMP._PLAYER;

		//CFX_AutoDestructShuriken.nFXCount = 0;  // clear all fx count

		NextEvent = null;

		//Record All Event to execute
		EvtPool.Clear();

		WaitPool.Clear ();

		tmpScriptMoveEnd.Clear ();
		//EvtCompletePool.Clear ();

		IsEventEnd = false;
		bIsStageEnd = false;
		bIsReady = false;

		bIsMoveToObj = false;
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

	void FixPlanePosition()
	{
		// ensure canvrs in screen
	//	if( TilePlaneObj != null )
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
					Vector3 d = this.transform.localPosition - v;
					float time = d.magnitude  /1000 ;
					TweenPosition tw = TweenPosition.Begin<TweenPosition>(this.gameObject , time );
					if( tw ){
						tw.SetStartToCurrentValue();
						tw.to = v;
					}

					//TilePlaneObj.transform.localPosition  = v ;

				}
			}

			//float fMouseX = Input.mousePosition.x;
			//float fMouseY = Input.mousePosition.y;
			
			Vector3 vOffset = gameObject.transform.localPosition;
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
            gameObject.transform.localPosition = vOffset;

//            if (MaskPanelObj != null ) {
//                MaskPanelObj.transform.position = TilePlaneObj.transform.position;
//            }
		}

        if (ActEffFolObj != null) {
            MobActEffObj.transform.position = ActEffFolObj.transform.position;
        }

	}

	void OnReady()
	{
        // close all other panel
        UnitSpiteObj.SetActive( false ); // unit plane
		
		//public GameObject TileObj; 	//  no need this
		MoveEftObj.SetActive( false ); 	// 
		AtkEftObj.SetActive( false ); 	// 
		AoeEftObj.SetActive (false);

		ValueEftObj.SetActive (false);
		AvgObj.SetActive (false);

//		if (BattleValue.nValueCount != 0) {
//		}


	//	Panel_CMDUnitUI.CloseCMDUI();

		PanelManager.Instance.CloseUI( Panel_UnitInfo.Name );
		PanelManager.Instance.CloseUI( Panel_MiniUnitInfo.Name );

		// close loadint UI
		PanelManager.Instance.CloseUI( "Panel_Loading");

		GameDataManager.Instance.ePhase = _SAVE_PHASE._STAGE;		// save to stage phase

		GameSystem.bFXPlayMode = true;								// start play fx

		bIsReady = true;		// all ready .. close the loading ui
	}

	void OnCellClick(GameObject go)
	{
		if( IsAnyActionRunning() == true )
			return;
		
		if( IsRunningEvent() == true  )
			return; // block other action  

        if (GameDataManager.Instance.nActiveCamp != _CAMP._PLAYER)
        {          
            return;
        }

		if (this.bIsStageEnd == true)
			return;

		if (BattleManager.Instance.IsBattlePhase ())
			return;

	
		UnitCell unit = go.GetComponent<UnitCell>() ;
		if( unit != null ){
		
	//		string str = string.Format( "CellOnClick( {0},{1}) " , unit.X() , unit.Y() );
	//		Debug.Log(str);
			string sKey =	unit.Loc.GetKey();
			bool bIsAtkCell = OverCellAtkPool.ContainsKey( sKey );
			bool bIsOverCell = OverCellPool.ContainsKey( sKey );

//			if( Config.GOD ){
//				bIsAtkCell = true; // god can atk all place
//			}
		

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
                if (cCMD.Instance.eCMDTYPE == _CMD_TYPE._ALLY) // 我方
                {
                    if (bIsOverCell == true || Config.GOD) // god for cheat warp
                    {
                        ClearOverCellEffect();
                        cCMD.Instance.eNEXTCMDTYPE = _CMD_TYPE._WAITATK;
                        PanelManager.Instance.CloseUI(Panel_CMDUnitUI.Name); //only colse ui to wait

                        StageCharMoveEvent evt = new StageCharMoveEvent();
                        evt.nIdent = cCMD.Instance.nCmderIdent; // current oper ident 
                        evt.nX = unit.X();
                        evt.nY = unit.Y();
                        GameEventManager.DispatchEvent(evt);
                        return;
                    }
                    else if (cCMD.Instance.eCMDTARGET == _CMD_TARGET._UNIT)
                    {
                        Panel_CMDUnitUI.CloseCMDUI();
                        // need cancel cmd

                    }
                    return;
                }
                else if (cCMD.Instance.eCMDTYPE == _CMD_TYPE._ENEMY )
                {
                    Panel_CMDUnitUI.CloseCMDUI();
                    if (Config.GOD) // for cheat move mob
                    {
                        ClearOverCellEffect();
                        cCMD.Instance.eNEXTCMDTYPE = _CMD_TYPE._WAITATK;
                        PanelManager.Instance.CloseUI(Panel_CMDUnitUI.Name); //only colse ui to wait

                        StageCharMoveEvent evt = new StageCharMoveEvent();
                        evt.nIdent = cCMD.Instance.nCmderIdent; // current oper ident 
                        evt.nX = unit.X();
                        evt.nY = unit.Y();
                        GameEventManager.DispatchEvent(evt);
                        return;
                    }
                  
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

			cCMD.Instance.nCMDGridX = unit.X();
			cCMD.Instance.nCMDGridY = unit.Y();

		}
	}



	void OnUnitClick(GameObject go)
	{
		if( IsAnyActionRunning() == true )
			return;
		
		if( IsRunningEvent() == true  )
			return; // block other action  

		if (this.bIsStageEnd == true)
			return;

		if (BattleManager.Instance.IsBattlePhase ())
			return;

		Panel_unit unit = go.GetComponent<Panel_unit>() ;
		if( unit == null )
			return ;
		if (unit.eCampID == _CAMP._PLAYER) {
			OnCharClick( unit );
		}
		else if(unit.eCampID == _CAMP._ENEMY) {
			OnMobClick( unit );
		}
		else if(unit.eCampID == _CAMP._FRIEND) {
			OnMobClick( unit );
		}
	}

	// if any char be click
	void OnCharClick(Panel_unit unit)
	{
//		if( IsAnyActionRunning() == true )
//			return;
//		
//		if( IsRunningEvent() == true  )
//			return; // block other action  

		if (GameDataManager.Instance.nActiveCamp != _CAMP._PLAYER)
			return;
	
		// avoid any ui opening
		// clear over effect

		//Panel_unit unit = go.GetComponent<Panel_unit>() ;
		if( unit == null )
			return ;

		if (cCMD.Instance.eCMDSTATUS == _CMD_STATUS._NONE  ) {
			//GameObject obj = PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name);
			//CloseCMDUI();
			if( unit.CanDoCmd()  ){
				Panel_CMDUnitUI.OpenCMDUI( _CMD_TYPE._ALLY , unit ); // player
			}
			else{
				Panel_CMDUnitUI.OpenCMDUI( _CMD_TYPE._ENEMY , unit );
			}
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
				if( unit.CanDoCmd()  ){
					Panel_CMDUnitUI.OpenCMDUI( _CMD_TYPE._ALLY , unit ); // player
				}
				else{
					Panel_CMDUnitUI.OpenCMDUI( _CMD_TYPE._ENEMY , unit );
				}		
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
	//		Debug.Log( "OnCharClick" + sKey + ";Ident"+unit.Ident() ); 

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
	}

	void OnMobClick(Panel_unit unit)
	{
//		if( IsAnyActionRunning() == true )
//			return;
//		
//		if( IsRunningEvent() == true  )
//			return; // block other action  

		if (GameDataManager.Instance.nActiveCamp != _CAMP._PLAYER)
			return;

		// avoid any ui opening
		// clear over effect
		//Panel_unit unit = go.GetComponent<Panel_unit>() ;
		if( unit == null )
			return ;
		string sKey = unit.Loc.GetKey ();
//		Debug.Log( "OnMobClick" + sKey + ";Ident"+unit.Ident() ); 

		bool bInAtkCell = OverCellAtkPool.ContainsKey (sKey);

		if( bInAtkCell  ){
			if( _PK_MODE._PLAYER ==  MyTool.GetSkillPKmode (cCMD.Instance.nSkillID) ){
				if( unit.eCampID == _CAMP._ENEMY ){
					return ;
				}

			}
			else{
				if( unit.eCampID == _CAMP._FRIEND ){
					return ;
				}
			}
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
		{
			//clear all exists tile obj
			List< GameObject> goList = MyTool.GetChildPool( TilePlaneObj );
			foreach( GameObject go in goList )
			{
				NGUITools.Destroy( go );
			}

            //CreateBackgroundTexture(TilePlaneObj, Grids.TotalW, Grids.TotalH, "Art/MAP/20738-1");
			if( string.IsNullOrEmpty( Grids.sBackGround ) == false ){
				CreateBackgroundTexture(TilePlaneObj, Grids.TotalW, Grids.TotalH, Grids.sBackGround );
			}
            //GameObject background =  GetBackGroundPrefab( Grids );
            //if( background != null ){
          //  GameObject tiles = NGUITools.AddChild(TilePlaneObj);
           // tiles.name = "Tiles";
            //}
            // start to create sprite
            for ( int i = -Grids.hW ; i <= Grids.hW ; i++ ){
				for( int j = -Grids.hH ; j <= Grids.hH ; j++ )
				{			
					_TILE t = Grids.GetValue( i , j  );
					if( t== _TILE._NULL )	
						continue;

					GameObject cell = GetTileCellPrefab( i , j , t , TilePlaneObj); 
					if( cell == null )
					{
						// debug message
						string err = string.Format( "Error: Create Tile Failed in Scene({0}),X({1},Y({2},T({3} )" , scn.s_MODLE_ID , i , j ,t ); 
						Debug.Log(err);
                        continue;
					}

                    UnitCell unityCell = cell.GetComponent<UnitCell>();
                    if (unityCell == null)
                        continue;

                    List<MyThing> thingList = null;
                    if (Grids.ThingPool.TryGetValue(unityCell.Loc.GetKey(), out thingList))
                    {
                        foreach(MyThing myThing in thingList)
                        {
                            if (myThing.Cell == null)
                                continue;

                            GameObject thing = ResourcesManager.CreatePrefabGameObj(cell, "Prefab/Thing");
                            if (thing == null)
                            {
                                Debug.LogFormat("Create thing fail.(Key={0})", unityCell.Loc.GetKey());
                                continue;
                            }

                            UISprite thingSprite = thing.GetComponent<UISprite>();
                            if (thingSprite == null)
                            {
                                Debug.LogFormat("Thing Sprite is null.(Key={0})", unityCell.Loc.GetKey());
                                continue;
                            }

                            thing.name = myThing.Layer.ToString();

                            thingSprite.spriteName = MyGrids.GetThingSpriteName(myThing.Cell.Value);
                            thingSprite.depth = myThing.Layer;

                            NGUITools.SetDirty(thingSprite.gameObject);
                        }
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
		if (fMaxOffX < 0)
			//fMaxOffX = 0;
			fMaxOffX = Screen.width/2;

		fMinOffX  =  -1*fMaxOffX;		

		//===============
		fMaxOffY  =  (Grids.TotalH - Screen.height )/2; 
		if (fMaxOffY < 0)
			//fMaxOffY = 0;
			fMaxOffY = Screen.height/2;

		fMinOffY  =  -1*fMaxOffY;		
		
	}

	GameObject GetTileCellPrefab( int x , int y , _TILE t , GameObject tiles )
	{
		SCENE_TILE tile = ConstDataManager.Instance.GetRow<SCENE_TILE> ((int)t);
		if (tile != null) {
			//tile.s_FILE_NAME;
			GameObject cell = ResourcesManager.CreatePrefabGameObj(tiles, "Prefab/TileCell");
			UISprite sprite = cell.GetComponent<UISprite>(); 
			if( sprite != null )
			{
				sprite.spriteName = tile.s_FILE_NAME;

			}
			UIDragObject drag = cell.GetComponent<UIDragObject>(); 
			if( drag != null )
			{
                drag.target = this.transform; //tiles.transform.parent;// TilePlaneObj.transform ;

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

    /// <summary>
    /// 產生背景圖片
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="name"></param>
    public static GameObject CreateBackgroundTexture(GameObject parent, int width, int height, string name)
    {
        GameObject obj = ResourcesManager.CreatePrefabGameObj(parent, "Prefab/BGTexture");
        ChangeBackgroundTexture(obj, width, height, name);

        return obj;
    }

    public static void ChangeBackgroundTexture(GameObject gameObject, int width, int height, string name)
    {
        UITexture uitex = gameObject.GetComponent<UITexture>();
        if (uitex != null)
        {
            //string url = "Art/map/" + grid.sBackGround;
            //string url = "Art/MAP/" + "20738-1";
            //Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
            Texture t = Resources.Load(name, typeof(Texture)) as Texture; ;
            uitex.mainTexture = t;

            uitex.width = width;
            uitex.height = height;
            uitex.depth = -1; // < 0 

        }
        UIDragObject drag = gameObject.GetComponent<UIDragObject>();
        if (drag != null)
        {
            drag.target =  Panel_StageUI.instance.gameObject.transform ;// gameObject.transform.parent.transform;
        }
    }

    //GameObject GetBackGroundPrefab(MyGrids grid)
    //{
    //    GameObject obj = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/BGTexture");
    //    UITexture uitex = obj.GetComponent<UITexture>();
    //    if (uitex != null)
    //    {
    //        //string url = "Art/map/" + grid.sBackGround;
    //        string url = "Art/MAP/" + "20738-1";
    //        //Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
    //        Texture t = Resources.Load(url, typeof(Texture)) as Texture; ;
    //        uitex.mainTexture = t;

    //        uitex.width = grid.TotalW;
    //        uitex.height = grid.TotalH;
    //        uitex.depth = -1; // < 0 

    //    }
    //    UIDragObject drag = obj.GetComponent<UIDragObject>();
    //    if (drag != null)
    //    {
    //        drag.target = TilePlaneObj.transform;

    //    }
    //    return obj;
    //}

    public GameObject SpwanBattleValueObj(GameObject Obj ,  Vector3 vPos  )
	{
		GameObject go = ValueEftObj.Spawn( Obj.transform  , vPos );
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


		ClearAOECellEffect();

	}

	public void ClearAOECellEffect(  )
	{
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
		List< iVec2 > pkPosPool = GetUnitPKPosPool(unit , true  );

		if( !pdata.IsTag( _UNITTAG._CHARGE ) ) {
			Grids.AddIgnorePool( pkPosPool );
			Grids.AddIgnorePool( Grids.GetZocPool( unit.Loc ,ref  pkPosPool ) ); // APPLY ZOC	
		}

		List<iVec2> moveList =  Grids.MoveAbleCell (unit.Loc, pdata.GetMov() );

		// try ZOC!!!
	//	List<iVec2> final = Panel_StageUI.Instance.Grids.FilterZocPool (unit.Loc, ref moveList, ref posList);
	//	moveList = final;

//		long  tick =  System.DateTime.Now.Ticks; 

		// start create over eff
		foreach( iVec2 v in moveList )
		{
			if ( MyGrids.IsWalkAbleTile( Grids.GetValue( v ) ) == false  )
			{
				continue;
			}
			// check if this vec can reach


//			List<iVec2> path = PathFinding( unit , unit.Loc , v , pdata.GetMov() );
//			if( path.Count <= 0 )
//				continue;

			// create move over cell
			//GameObject over = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/MoveOverEffect");
			GameObject over = MoveEftObj.Spawn(  MaskPanelObj.transform );
		
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

//		long  during =  System.DateTime.Now.Ticks - tick ; 

//		Debug.Log( "create moveeffect cell with ticket:" + during );
	}

	public void CreateAttackOverEffect( Panel_unit unit , int nSkillID=0 )
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

        int nRange = 1,  nMinRange = 0;
        MyTool.GetSkillRange(nSkillID, out nRange, out nMinRange);


        List<iVec2> AtkList = null;

        if (nRange == -1 )  { // infinte

            AtkList = GetUnitPKPosPool(unit ,  MyTool.GetSkillCanPKmode(nSkillID) );

        }
        else    { // 
            if(nRange < 0) {
                nRange = 1;
            }

            AtkList = Grids.GetRangePool(unit.Loc, nRange, nMinRange);
        }


		
		//AtkList.RemoveAt (0); // remove self pos

		foreach( iVec2 v in AtkList )
		{
			//GameObject over = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/AttackOverEffect");
			GameObject over = AtkEftObj.Spawn( MaskPanelObj.transform );
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

	public void CreateZocOverEffect( List<iVec2> path )
	{
		foreach( KeyValuePair< string , GameObject> pair in OverCellZocPool )
		{
			if( pair.Value != null )
			{
				NGUITools.Destroy( pair.Value );
				//pair.Value = null;
			}
		}
		OverCellZocPool.Clear ();
		if (path == null)
			return;
		
		foreach( iVec2 v in path )
		{
			if( OverCellZocPool.ContainsKey( v.GetKey() ) )
				continue;

			GameObject over = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/ZocOverEffect");
			if( over != null )
			{
				over.name = string.Format("ZocOVER({0},{1},{2})", v.X , v.Y , 0 );
				SynGridToLocalPos( over , v.X , v.Y) ;
				
				//UIEventListener.Get(over).onClick += OnOverClick;
				
				OverCellZocPool.Add( v.GetKey() , over );
				over.SetActive( true );
			}
		}
		
	}

	public void CreateAOEOverEffect( Panel_unit CastUnit  ,int nX , int nY , int nAOE , bool bCanPK = true)
	{
		AoeEftObj.RecycleAll();

		if( OverCellAOEPool != null ){
			OverCellAOEPool.Clear ();
		}
		if( CastUnit == null )
			return ;

		Panel_unit unit = CastUnit ; // MyTool.CMDUI().pCmder;
		int nOrgX = unit.X ();
		int nOrgY = unit.Y ();

        List<iVec2> aoeList = null;  
        // 將來要處理 旋轉!
        

        if (nAOE == -1)
        { // infinte

            aoeList = GetUnitPKPosPool(unit, bCanPK );

        }
        else
        { //
            if (nAOE == 0)
            {
                nAOE = 1;  // avoid error aoe block cmd
            }
            aoeList =  MyTool.GetAOEPool(nX, nY, nAOE, nOrgX, nOrgY);            
        }



        // start create over eff
        foreach ( iVec2 v in aoeList )
		{	
			// create move over cell
			//GameObject over = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/MoveOverEffect");
			GameObject over = AoeEftObj.Spawn(  MaskPanelObj.transform );
			
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
//		Panel_CheckBox panel = MyTool.GetPanel<Panel_CheckBox>( PanelManager.Instance.OpenUI ( Panel_CheckBox.Name ) );
//		if (panel) {
//			panel.SetAoeCheck();
//		}
	}
	// Check any action is running

	public void PlayFX( int nFxID , int nX , int nY , bool bOnMask= true )
	{
        GameObject go;
        if(bOnMask) {
            go = GameSystem.PlayFX(MaskPanelObj, nFxID);
        }
        else     {
            go = GameSystem.PlayFX(TilePlaneObj, nFxID);
        }
        // show effect fx        
		if (go != null) {
			go.transform.localPosition = MyTool.SnyGridtoLocalPos( nX , nY , ref Grids );
		}
	}

	public void PlayAOEFX( Panel_unit CastUnit  , int nFxID , int nX , int nY , int nAOE )
	{
		if (nAOE == 0) {
			Debug.LogError( "stage play AOE fx fail with 0 ");
			return ;
		}

		Panel_unit unit = CastUnit ; // MyTool.CMDUI().pCmder;
		int nOrgX = unit.X ();
		int nOrgY = unit.Y ();
		
		List<iVec2> aoeList = MyTool.GetAOEPool (nX, nY, nAOE ,nOrgX, nOrgY );
		foreach (iVec2 v in aoeList) {
			PlayFX( nFxID , v.X , v.Y , true );
		}
	}

    public bool IsAnyActionRunning()
    {
        // for detect win event
        if ((TilePlaneObj != null) && !TilePlaneObj.activeSelf) {
            return false;
        }

        if (BattleMsg.nMsgCount > 0)
            return true;

        if (bIsMoveToObj)
            return true;

       // ParticleSystem[] cacheParticleList = this.gameObject.GetComponentsInChildren<ParticleSystem>();
        //foreach (ParticleSystem ps in cacheParticleList)
        //{
        //    if (ps.isStopped == false )
        //    {
        //        ParticleSystem t = ps;
        //        return true;
        //    }
        //}

        if (CFX_AutoDestructShuriken.nFXCount > 0)
        {
            fFxPlayTime += Time.deltaTime;
            if (fFxPlayTime >= 30.0f)
            {
                Debug.LogError("FX dead locked over 30sec . release it");
                CFX_AutoDestructShuriken.nFXCount = 0;

                // log for debug
                ParticleSystem[] cacheParticleList = this.gameObject.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem ps in cacheParticleList)
                {
                    Debug.LogErrorFormat( "dead lock particle with[ {0} ] " , ps.name );
                }
            }
            // safe release
            
            //            if (cacheParticleList == null || cacheParticleList.Length <=0 )
            //            {
            //              CFX_AutoDestructShuriken.nFXCount = 0;
            //           }

            return true;
        }


        fFxPlayTime = 0.0f;
        // this is very slow for play
        //		if(ValueEftObj.CountSpawned () > 0)
        //			return true; 

        //IsAnyActionRunning
        //	if( BattleValue.nValueCount > 0  ) // bug here!!
        //		return true;

        //		if( PanelManager.Instance.CheckUIIsOpening( Panel_Talk.Name) == true )
        //			return true;

        if ( PanelManager.Instance.CheckUIIsOpening( Panel_Round.Name ) == true )
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
		foreach (KeyValuePair< int ,Panel_unit > pair in IdentToUnit) {
			if( pair.Value!= null )
			{
				if( pair.Value.eCampID == nCamp )
				{
					lst.Add( pair.Value );
					//return pair.Value;
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
	// get nearest pk unit
	public Dictionary< Panel_unit , int > GetUnitHpPool( Panel_unit unit , bool bCanPK , int nLimit=999)
	{
		Dictionary< Panel_unit , int > pool = new Dictionary< Panel_unit , int > (); // unit , dist
		foreach( KeyValuePair<int ,Panel_unit > pair in IdentToUnit )
		{
			if( unit.CanPK( pair.Value ) == bCanPK )
			{
				int nDist = unit.Loc.Dist( pair.Value.Loc );
				if( nDist > nLimit )
					continue;

				//  int nDist = pair.Value.Loc.Dist( unit.Loc );
				int nHP = pair.Value.pUnitData.n_HP;
				pool.Add( pair.Value , nHP );
			}
		}
		return pool;
	}

	bool RunCampAI( _CAMP nCamp )
	{
		// our faction don't need AI process
		if (nCamp == _CAMP._PLAYER) {

			return true; // player is playing 
		}

		// change faction if all unit moved or dead.
		List<Panel_unit> lst = GetUnitListByCamp (nCamp);
		foreach (Panel_unit unit in lst ) {
			if( bIsStageEnd == true ){  // 每個敵方單位行動後都可能觸發，關卡結束 要避免AI 繼續運算
				return true; // block all ai here
			}

			if( unit.CanDoCmd() )
			{
				unit.RunAI();
				return true;
			}
		}
		return false;
	}

	// return true
	public bool CheckPlayerRoundEnd()
	{
		//==================
		if( GameDataManager.Instance.nActiveCamp != _CAMP._PLAYER ){
			return false;  // don't check in enemy round

		}

		if (BattleManager.Instance.HaveDrop ()) {
			return false;
		}


		// change faction if all unit moved or dead.
		List<Panel_unit> lst = GetUnitListByCamp (_CAMP._PLAYER);

		foreach (Panel_unit unit in lst ) {
			if( bIsStageEnd == true ){  // 每個敵方單位行動後都可能觸發，關卡結束 要避免AI 繼續運算
				return true; // block all ai here
			}
			
			if( unit.CanDoCmd() )
			{
				return false;
			}
		}

		// pop cmd end ui
		Panel_CheckBox panel = GameSystem.OpenCheckBox();
		if (panel) {
			panel.SetRoundEndCheck();
		}

		return true;
	}


	// Check event
	bool CheckEventCanRun( STAGE_EVENT evt )
	{	
		return MyScript.Instance.CheckEventCanRun (evt);

//		cTextArray sCond = new cTextArray( );
//		sCond.SetText( evt.s_CONDITION );
//		// check all line . if one line success . this event check return true
//
//		int nCol = sCond.GetMaxCol();
//		for( int i= 0 ; i <nCol ; i++ )
//		{
//			//if( CheckEventCondition( sCond.GetTextLine( i ) ) )
//			if( MyScript.Instance.CheckEventCondition( sCond.GetTextLine( i ) ) )
//			{
//				return true;
//			}
//		}
//		return false;
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
			if( NextEvent!= null ){
				Debug.LogFormat( "Get Event{0} to run" ,NextEvent.n_ID  );
			}

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
			if ( (evt.n_TYPE & 1) == 1  )  // 1 is loop event
			{
				return true;
			}
		}
		return false;
	}
	// this is debug tool func
	public void TrigEventToRun( int nEventID )
	{
		if( NextEvent != null )		// avoid double run
		{
			Debug.LogError( " TrigEventToRun Err!! nextevent exist");
			return ;
		}
		if( (EvtPool==null) || (EvtPool.Count<=0)){
			Debug.LogError( " TrigEventToRun Err!! EvtPool is null");
			return  ;
		}
		
		if (bIsStageEnd) {
			Debug.LogError( " TrigEventToRun Err!! stage is end");
			return;
		}
		if (bIsLoading) {
			Debug.LogError( " TrigEventToRun Err!! stage is loading");
			return;
		}
		//======================================
		STAGE_EVENT evt;
		if( EvtPool.TryGetValue( nEventID , out evt ) )
		{
			//WaitPool.Add( evt );
			// check is loop event?
			if( IsLoopEvent( evt )== false  )
			{
				if( EvtPool.ContainsKey( nEventID ) )
				{
					EvtPool.Remove( nEventID );
				}

				//removeLst.Add( pair.Key );
			}
			// run event directly
			NextEvent = evt;
			PreEcecuteEvent();					// parser next event to run
			Debug.LogFormat( " TrigEventToRun ok!! event {0}  " , nEventID );
		}
		else{
			Debug.LogErrorFormat( " TrigEventToRun Err!! event{0} not exist " , nEventID );
		}

	}
	//
	void GetEventToRun()
	{
		//if( IsRunningEvent() )
		if( NextEvent != null )		// avoid double run
			return ;
		if( (EvtPool==null) )
			return  ;
		if( EvtPool.Count<=0)
			return ;

		if (bIsStageEnd) {
			Debug.LogError( " check event when stage end");
		}
		if (bIsLoading) {
			Debug.LogError( " check event when stage loading");
		}

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
			tmpScriptMoveEnd.Clear();					// script 用的單位移動 mrak pool 防止 script 讓不同單位移動到 同樣座標
		
			MyScript.Instance.ParserScript( line );
			tmpScriptMoveEnd.Clear();					// script 用的單位移動 mrak pool 防止 script 讓不同單位移動到 同樣座標
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

//	// Widget func	 
	public void CheckMissionComplete()
	{
		return;

//		foreach (KeyValuePair< int ,STAGE_EVENT > pair in EvtPool) {
//			if( (pair.Value.n_TYPE & 2) != 2  ){
//				continue;
//			}
//			//=====================================================
//			if(	CheckEventCanRun( pair.Value ) )
//			{
//				// skip run event
//				cTextArray Ta = new cTextArray( );
//				Ta.SetText( pair.Value.s_BEHAVIOR );
//
//				// 
//				int nMax = Ta.GetMaxCol();
//				for( int i = 0 ; i <nMax ; i++ )
//				{
//					CTextLine line = m_cScript.GetTextLine( i );
//					if( line != null )
//					{						
//						MyScript.Instance.ParserScript( line );
//					}
//				}
//
//			}
//		}
		//====================================


	}

    // talk ui will run script line by line , need a func to be called to clear cache
    public void ClearSciptLineCacheData()
    {
        tmpScriptMoveEnd.Clear();
    }

    public void SetScriptSkipMode( bool bEnable )
    {
        m_bIsSkipMode = bEnable;
        ClearSciptLineCacheData();
    }

	public GameObject CreateUnitByUnitData( cUnitData data )
	{
		if(  data == null )
			return null;
//		int nCharID = data.n_CharID;
//		int x = data.n_BornX;
//		int y = data.n_BornY;
//		GameObject obj = UnitPanelObj.Spawn( TilePlaneObj.transform );
//		if( obj == null )return null;
//		obj.name = string.Format ("unit-{0}",nCharID );	
//		Panel_unit unit = obj.GetComponent<Panel_unit>();
//		if (unit != null) {
//			iVec2 pos = FindEmptyPos( new iVec2(x , y));
//			int posx = x;
//			int posy = y;
//			if( pos !=null ){
//				posx = pos.X;
//				posy = pos.Y;
//			}
//			unit.CreateChar( nCharID , posx , posy , data );
//
//		}
		return AddUnit( data ,data.eCampID , data.n_CharID , data.n_X , data.n_Y , data.n_LeaderIdent );
	}


	GameObject AddUnit( cUnitData data , _CAMP nCampID , int nCharID , int x , int y  , int nLeaderIdent = 0  )
	{
//		CHARS charData = GameDataManager.Instance.GetConstCharData (nCharID); //ConstDataManager.Instance.GetRow<CHARS>( nCharID );
//		if( charData == null)
//			return null;
		// get data from Const data
		if (UnitPanelObj == null) {
			Debug.Log("Stage Addunit to null UnitPanel");
			return null;
		}
		if (data == null) {
			Debug.LogError( "Stage Addunit with null unitdata");
			return null;
		}


		//GameObject obj = ResourcesManager.CreatePrefabGameObj( TilePlaneObj , "Prefab/Panel_Unit" );

		GameObject obj = UnitSpiteObj.Spawn( UnitPanelObj.transform );
		if( obj == null )return null;
		obj.name = string.Format ("unit-{0}",nCharID );	

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
			data.n_X = posx;
			data.n_Y = posy;
			// load data . if char exist in storage pool
			// no more operate game data pool
//			if( (data==null) && (nCampID==_CAMP._PLAYER) ){
//				data = GameDataManager.Instance.GetStorageUnit( nCharID );
//				if( data != null ){
//					data.n_Ident = GameDataManager.Instance.GenerSerialNO();
//				}
//			}
		


			// setup param
			unit.SetUnitData( data );
			//unit.CreateChar( nCharID , posx , posy , data );

			// set up unit default value
			// setup lv before
			if( data == null ){
				//campid = data.eCampID;
				//lv = data.n_Lv;
//				unit.SetCamp( nCampID );	
//				unit.SetLevel( StageData.n_MOB_LV );
//			unit.SetLeader( nLeaderIdent );

			}
			else {
				// setup in create char
				//unit.SetCamp( campid );	
				//unit.SetLevel( lv );
				//unit.SetLeader( nLeaderIdent );
			}

			//add to stage obj
			if( IdentToUnit.ContainsKey( unit.Ident() )  ){
				Debug.LogErrorFormat( " Err Add doublie ident{0}, charid{1} of panel_unit to stage " , unit.Ident(), unit.CharID );

			}
			else {
				IdentToUnit.Add( unit.Ident() , unit  ) ;// stage gameobj
			}

			//ensure data in storage is remove
//			GameDataManager.Instance.RemoveStorageUnit( nCharID );
			unit.SetBorn (); // start born animate
		
		}
		
		// position // set in create
		//obj.transform.localPosition =  MyTool.SnyGridtoLocalPos( x , y , ref Grids ) ; 

		UIEventListener.Get (obj).onClick = null;
		UIEventListener.Get (obj).onClick += OnUnitClick;

		// if obj out of screen. move to it auto
		MoveToGameObj ( obj , false );



		// all ready
		NGUITools.SetActive( obj , true );
		return obj;
	}

	void DelUnitbyIdent(  int nIdent )
	{
		if (IdentToUnit.ContainsKey (nIdent) == false) {
			Debug.LogErrorFormat( "DelUnitbyIdent with no key {0}" ,nIdent  );
			return ;
		}

		Panel_unit unit = IdentToUnit[ nIdent ];
		if( unit != null )
		{
//			cCamp camp =  GameDataManager.Instance.GetCamp( unit.eCampID  );
//			if( camp != null )
//			{
//				camp.memLst.Remove ( nIdent );
//			}		
			unit.FreeUnitData();
			unit.Recycle( );
		}
		IdentToUnit.Remove( nIdent );
	}

	// Take care use ident to delete
	void LeaveUnit(  int nCharid )
	{
		//Dictionary< _CAMP , cCamp > CampPool = GameDataManager.Instance.GetCamp;			// add Camp
		foreach (KeyValuePair< int , Panel_unit > pair in IdentToUnit) {
			if( pair.Value == null ){
				Debug.LogErrorFormat( "DelUnit with null unit at charid {0} ",nCharid );
				continue;
			}
			if( pair.Value.CharID != nCharid )
				continue;
			
			pair.Value.SetLeave();
		}
	}


	void DelUnit(  int nCharid , int nNum=0 )
	{
        //change to del all unit with char

        int nCount = 0;
        List<int> mlist = new List<int>();
		//Dictionary< _CAMP , cCamp > CampPool = GameDataManager.Instance.GetCamp;			// add Camp
		foreach (KeyValuePair< int , Panel_unit > pair in IdentToUnit) {
			if( pair.Value == null ){
				Debug.LogErrorFormat( "DelUnit with null unit at charid {0} ",nCharid );
				continue;
			}
			if( pair.Value.CharID != nCharid )
				continue;

			pair.Value.FreeUnitData();
			pair.Value.Recycle();
			mlist.Add( pair.Key );

            // num
            nCount ++;
            if (nNum > 0 && nCount >= nNum) {
                break;
            }

        }


		///
		foreach( int id in mlist )
		{
			IdentToUnit.Remove( id );
		}
	}


//	void DelChar( _CAMP nCampID , int nCharID )
//	{
////		cCamp camp =  GameDataManager.Instance.GetCamp( nCampID );
////		if( camp == null )
////			return ;
//		List< int > remove = new List< int >(); // ident pool
//		//foreach( int id in camp.memLst )
//		foreach( KeyValuePair<int , Panel_unit> pair in IdentToUnit )
//		{
//			if( pair.Value == null )
//				continue;
//			if( (pair.Value.CharID) == nCharID && (pair.Value.eCampID ==nCampID) ){
//				//NGUITools.Destroy( unit.gameObject );
//				pair.Value.FreeUnitData(); // dis connect with gamedata manager
//				pair.Value.Recycle();
//
//				remove.Add( pair.Key );
//			}
//
//		}
//
//		foreach( int id in remove )
//		{
//			IdentToUnit.Remove( id );
//		//	camp.memLst.Remove( id );
//		}
//	}

	public void EndStage()
	{
		bIsStageEnd = true;
		
		GameDataManager.Instance.EndStage ();   // 處理戰場結束的資料回存
	}

	public void ShowStage( bool bShow )
	{
		// release action to avoid dead lock
		// check keep action // 處理禪留事件
		if( ActionManager.Instance.HaveAction() ){
			Debug.LogError( "some action keep in stage clear . it need release");
		}

		ActionManager.Instance.ReleaseAction ();

		if (TilePlaneObj != null) {
//			TweenAlpha tw = TweenAlpha.Begin<TweenAlpha>( TilePlaneObj , 1.0f );
//			if( tw != null )
//			{
//				if( bShow  )
//				{
//					MyTool.SetAlpha( TilePlaneObj , 0.0f );
//					tw.from = 0.0f;
//					tw.to   = 1.0f;
//
//				}
//				else 
//				{
//					MyTool.SetAlpha( TilePlaneObj , 1.0f );
//					tw.from = 1.0f;
//					tw.to   = 0.0f;
//
//				}
//			}
			TilePlaneObj.SetActive (bShow);
		}
        if (UnitPanelObj != null) {
            UnitPanelObj.SetActive(bShow);
        }


	}

	public GameObject AddAVGObj( int nIdent , bool bAssist= false , bool batk = true )
	{

		// no use avg now
//		int nIdx = 0;
//		if (bAssist) {
//			nIdx ++;
//		}
//		cUnitData pData = GameDataManager.Instance.GetUnitDateByIdent ( nIdent );
//		if (pData!= null) {
//			GameObject obj = AvgObj.Spawn( MaskPanelObj.transform , new Vector3( 0.0f , -640.0f , 0.0f )  );
//			if( obj != null ){
//				SRW_AVGObj avg = obj.GetComponent< SRW_AVGObj > ();
//				if( avg != null ){
//					avg.ChangeFace( pData.n_CharID );
//					Vector3 tarPos = new Vector3();
//					if( pData.eCampID == _CAMP._ENEMY ){ tarPos.x = -(240.0f+ nIdx*64);  }
//					else{ tarPos.x = (240.0f+ nIdx*64 ); }
//					//============================================
//					TweenPosition tw = TweenPosition.Begin<TweenPosition>( obj , 0.3f );
//					if( tw != null ){
//						tw.from =  new Vector3( 0.0f , -640.0f , 0.0f );
//						tw.to = tarPos;
//					}
//					// index
//					if(nIdx==0 ){
//						avg._FaceTexObj.depth = 2;
//					}
//					else{
//						avg._FaceTexObj.depth = 1;
//					}
//				}
//			}
//			return obj;
//		}
		return null;
	}

	public void FadeOutAVGObj(  )
	{
		foreach (GameObject obj in AvgObj.GetSpawned()) {
			SRW_AVGObj avg = obj.GetComponent< SRW_AVGObj > ();
			if( avg != null ){
				avg.FadeOut();
				avg.SetScale( 2.0f ); 
			}
		}
	}


	public void ClearAVGObj(  )
	{
		AvgObj.RecycleAll ();
	}

	public GameObject AddMark( int x , int y  )
	{
		GameObject mark = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/PathOverEffect");
		if( mark != null )
		{
			iVec2 v = new iVec2( x , y );
			mark.name = string.Format("PATHOVER({0},{1},{2})", v.X , v.Y , 0 );
			SynGridToLocalPos( mark , v.X , v.Y) ;
				
				//UIEventListener.Get(over).onClick += OnOverClick;
				
			OverCellMarkPool.Add( v.GetKey() , mark );
			mark.SetActive( true );
			return mark;
		}
		return null;
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
			Panel_CMDUnitUI.NextCMDUI(  );// only open UI. don't change other param
			//GameObject go = PanelManager.Instance.OpenUI( Panel_CMDUnitUI.Name ); 

			return  true;
		}	
		return false;
	}

	public bool CheckUnitDead( bool bAll = false )
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
							if( !bAll ){
								return true;
							}
						}

					}

				}
			}
		}

		return false;
	}

	public void TraceUnit( Panel_unit unit )
	{
		if (unit == null || bIsStageEnd )
			return;
		if (TarceMoveingUnit != null) // 避免富庶單位移動時會震動
			return;


		TarceMoveingUnit = unit;
	}


	public void MoveToGameObj( GameObject obj , bool force = false , float time = 1.0f)
	{
		if (obj == null || bIsStageEnd)
			return;
		Vector3 v = obj.transform.localPosition;
		Vector3 canv = gameObject.transform.localPosition; // shift
		Vector3 realpos = v + canv;
		if (force == false)
		{

			int hW = (Config.WIDTH )/2 - Config.TileW;
			int hH = (Config.HEIGHT)/2 - Config.TileH;
			if( (realpos.x < hW  && realpos.x > -hW ) && (realpos.y < hH && realpos.y > -hH ) )
				return; // pass
		}

		//TilePlaneObj.transform.localPosition = -v;
		float dist = Vector3.Magnitude( realpos );

		float during = dist/500.0f; // 這是最小值
							//距離過大要算最大值
	//	if (force == true) {
			if (during > time) 
				during = time;		// 不管多遠都不要超過1 秒
	//	}

		TweenPosition tw = TweenPosition.Begin<TweenPosition> (gameObject, during);
		if (tw != null) {
			tw.SetStartToCurrentValue();
			tw.to = -v;
			bIsMoveToObj = true;
			MyTool.TweenSetOneShotOnFinish( tw , MoveToGameObjEnd ); 

		}

		Panel_unit unit = obj.GetComponent< Panel_unit > ();
		if (unit != null) {
			TraceUnit( unit );
		}

	}

	public void MoveToGameObjEnd()
	{
		bIsMoveToObj = false;
	}

	public void PlayStageBGM()
	{
		if( StageData != null )
		{
			GameSystem.PlayBGM( 0 ); // stop bgm for replay

			//SCENE_NAME scn = ConstDataManager.Instance.GetRow<SCENE_NAME> ( StageData.n_SCENE_ID );
			//if (scn == null)
			//	return ;
			if( GameDataManager.Instance.nActiveCamp == _CAMP._ENEMY )
			{
				GameSystem.PlayBGM( GameDataManager.Instance.nEnemyBGM  );
			}
			else if( GameDataManager.Instance.nActiveCamp == _CAMP._FRIEND )
			{
				GameSystem.PlayBGM( GameDataManager.Instance.nFriendBGM );
			}
			else if( GameDataManager.Instance.nActiveCamp == _CAMP._PLAYER )
			{
				GameSystem.PlayBGM( GameDataManager.Instance.nPlayerBGM  );
			}

		}
	}

	public iVec2 SkillHitBackbyCharID(  int AtkChar , int DefChar , int nDist  )
	{
		cUnitData catker = GameDataManager.Instance.GetUnitDateByCharID ( AtkChar );
		cUnitData cdefer = GameDataManager.Instance.GetUnitDateByCharID ( DefChar );
		if (catker == null || cdefer == null)
			return null;

		return SkillHitBack ( catker.n_Ident , cdefer.n_Ident , nDist );
	}

	// SKill effect 
	public iVec2 SkillHitBack(   int AtkID , int DefID , int nDist  )
	{
		Panel_unit Atker = GetUnitByIdent( AtkID ); 
		Panel_unit Defer = GetUnitByIdent( DefID ); 
		return SkillHitBack (Atker , Defer , nDist );
	}

	public iVec2 SkillHitBack(  Panel_unit Atker , Panel_unit Defer , int nDist  )
	{

		if (Atker == null || Defer == null || nDist == 0)
			return null;
		iVec2 vDir = new iVec2( Defer.Loc -  Atker.Loc );
		//正規
		if( vDir.X != 0 )
			vDir.X /=    Mathf.Abs( vDir.X);
		if( vDir.Y != 0 )
			vDir.Y /=    Mathf.Abs( vDir.Y);
		if( nDist < 0 ){
			// pull		
			vDir *= -1;
		}
		//nDist += 3; // for debug
		int d = Mathf.Abs( nDist  ) ;
		iVec2 vFinal = new iVec2( Defer.Loc );
		for( int i= 0 ; i < d ; i++ )
		{
			iVec2 tmp = vFinal.MoveXY( vDir.X , vDir.Y );
			if( CheckIsEmptyPos( tmp ) == false )
			{
				break;
			}
			vFinal = tmp;
		}

		// check if real hit back
		if( vFinal.Collision( Defer.Loc ) ){
			return null;

		}

		return vFinal;
	}

	// IO
	public bool CheckIsEmptyPos( iVec2 pos )
	{
		if (Grids.Contain (pos) == false)
			return false;
		
		// check tile
		if( MyGrids.IsWalkAbleTile( Grids.GetValue( pos ) ) == false )
			return false;
		
		// check thiing
		
		// check unit
		foreach( KeyValuePair< int , Panel_unit  > pair in IdentToUnit )
		{
			if( pair.Value.Loc.Collision( pos ) ){
				return false;
			}
		}

		// script 用的單位移動 mrak pool 防止 script 讓不同單位移動到 同樣座標
		foreach (iVec2 v in tmpScriptMoveEnd) {
			if( v.Collision( pos ) ){					
				return false;
			}
		}

		return true;
	}
	
	public iVec2 FindEmptyPos( iVec2 st  , int len = 999 )
	{
		// len == 0
		if (CheckIsEmptyPos (st)) {
			return st;
		}
		// len == 1
		List<iVec2> pool = st.AdjacentList (1);
		foreach( iVec2 pos in pool )
		{
			if( CheckIsEmptyPos(pos) ){
				return pos;
			}
		}
		// len == 2  ,, special rule
		pool.Add (st.MoveXY (1, 1));		pool.Add (st.MoveXY (1, -1));		pool.Add (st.MoveXY (-1, 1));		pool.Add (st.MoveXY (-1, -1));
		pool.Add (st.MoveXY (2, 0));		pool.Add (st.MoveXY (0, 2));		pool.Add (st.MoveXY (-2, 0));		pool.Add (st.MoveXY (0, -2));
		foreach( iVec2 pos in pool )
		{
			if( CheckIsEmptyPos(pos) ){
				return pos;
			}
		}

		//len > 3
		// get a empty pos that can pop 		
		for (int i=2; i < len; i++) {
			pool.Clear ();
			pool = Grids.GetRangePool( st , i , i-1 );
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
	public iVec2 FindEmptyPosToAttack( iVec2 to  , iVec2 from,int len = 999 )
	{
		List<iVec2> pool = new List<iVec2> ();
		//len > 3
		// get a empty pos that can pop 		
		for (int i=1; i < len; i++) {
			pool.Clear ();
			if( i == 1 ){
				pool = to.AdjacentList (1);
			}
			else if( i == 2 )
			{
                pool.Add(to.MoveXY(1, 1)); pool.Add(to.MoveXY(1, -1)); pool.Add(to.MoveXY(-1, 1)); pool.Add(to.MoveXY(-1, -1));
                pool.Add(to.MoveXY(2, 0)); pool.Add(to.MoveXY(0, 2)); pool.Add(to.MoveXY(-2, 0)); pool.Add(to.MoveXY(0, -2));
                
			}
			else {
				pool = Grids.GetRangePool( to , i , i-1 );
			}
			// start sort 
			if( pool == null || pool.Count == 0  )
				continue;

			Dictionary< iVec2 , int > distpool = new Dictionary< iVec2 , int > ();
			foreach (iVec2 v in pool) {
				if (CheckIsEmptyPos (v) == true) {// 目標 pos 不可以站人
					int d =v.Dist (from);
					distpool.Add (v, d );
				}
			}

			var itemsdist = from pair2 in distpool orderby pair2.Value ascending select pair2;
			foreach (KeyValuePair<iVec2 , int> pair2 in itemsdist) {
				// sort already
				if( pair2.Key != null  ){
					return pair2.Key;
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
		List< iVec2 > zocPool = null;
		Grids.ClearIgnorePool();

		cUnitData pData = unit.pUnitData;

		if( !pData.IsTag( _UNITTAG._CHARGE ) ) { 
			List< iVec2 > pkPosPool = GetUnitPKPosPool( unit , true  );
			Grids.AddIgnorePool (  pkPosPool );  // all is block in first find

			if( false== MyScript.bParsing  ){// no zoc during script parse
				zocPool = Grids.GetZocPool(unit.Loc , ref pkPosPool );
#if UNITY_EDITOR
                //if( Config.GOD ) // always draw in edit
                {
                    CreateZocOverEffect( zocPool );
				}
#endif 
                Grids.AddIgnorePool( zocPool ); // APPLY ZOC	
			}
		}
		
		// avoid the end node have ally
		//List< iVec2 > nearList =  GetUnitPKPosPool( unit , );
		
		
		path = Grids.PathFinding( st , ed , nStep  );
		

		if (Config.GOD == true) {
			CreatePathOverEffect (path); // draw path
		}

		return path;
	}
	
	// tool func to get aoe affect pool
//	public List < iVec2 > GetAOEPool( int nX , int nY , int nAoe )
//	{
//		iVec2 st = new iVec2 ( nX , nY );
//		Dictionary< string , iVec2 > tmp = new Dictionary< string , iVec2 >();
//		tmp.Add ( st.GetKey() , st );
//		//			pool.Add ( st );
//		
//		AOE aoe = ConstDataManager.Instance.GetRow<AOE> ( nAoe ) ;
//		if (aoe != null) {
//			// add extra first
//			cTextArray TA = new cTextArray();
//			TA.SetText ( aoe.s_EXTRA );
//			for( int i = 0 ; i < TA.GetMaxCol(); i++ )
//			{
//				CTextLine line  = TA.GetTextLine( i );
//				for( int j = 0 ; j < line.GetRowNum() ; j++ )
//				{
//					string s = line.m_kTextPool[ j ];
//					
//					string [] arg = s.Split( ",".ToCharArray() );
//					if( arg.Length < 2 )
//						continue;
//					if( arg[0] != null && arg[1] != null )
//					{
//						int x = int.Parse( arg[0] );
//						int y = int.Parse( arg[1] );
//						iVec2 v = st.MoveXY( x , y );
//						
//						if( Grids.Contain( v ) == false )
//							continue;
//						
//						string key = v.GetKey();
//						if( tmp.ContainsKey( key ) == false ){
//							tmp.Add( key , v );
//						}
//					}
//				}
//			}
//			// get range pool	
//			List<iVec2> r = Grids.GetRangePool( st , aoe.n_MAX , aoe.n_MIN );
//			
//			
//			
//		}
//		
//		List < iVec2 > pool = new List < iVec2 > ();
//		return pool;
//	}
	
	
	
	// restore from save data
	public IEnumerator SaveLoading( cSaveData save )
	{
		//GameDataManager.Instance.nStoryID = nStoryID;
		//GameDataManager.Instance.nStageID = save.n_StageID;


		PanelManager.Instance.OpenUI ("Panel_Loading");
		
		yield return  new WaitForEndOfFrame ();
		
		if (save.ePhase == _SAVE_PHASE._MAINTEN) {
			
			Panel_Mainten panel  = MyTool.GetPanel<Panel_Mainten>( PanelManager.Instance.OpenUI ( Panel_Mainten.Name ) );
			yield return  new WaitForEndOfFrame();
			
			panel.RestoreBySaveData( save );
			
			
		} else if (save.ePhase == _SAVE_PHASE._STAGE) {
			
			//PanelManager.Instance.OpenUI( Panel_StageUI.Name );  // don't run start() during open
			
			//check need clear stage
			//		Panel_StageUI.Instance.bIsRestoreData = true;
			Panel_StageUI.Instance.RestoreBySaveData ( save) ;
			
		}		
		yield return  new WaitForEndOfFrame();
		
		cSaveData.SetLoading (false);
		
		if (save.ePhase != _SAVE_PHASE._STAGE) {
			PanelManager.Instance.DestoryUI( Name );
			//PanelManager.Instance.CloseUI (Name);  			// close stage when mainten
		}
	
		PanelManager.Instance.CloseUI ("Panel_Loading");
		yield break;
		
	}
	
	public void LoadSaveGame( cSaveData save )
	{
		if (save  == null)
			return;
		
		// set stage is end & load to avoud update check()
		this.bIsLoading = true;
		this.bIsStageEnd = true;


		StartCoroutine (  SaveLoading( save  ) );
		
	}
	
	public bool RestoreBySaveData( cSaveData save )
	{
		//	cSaveData save = GameDataManager.Instance.SaveData;
		if (save == null)
			return false;
		
		
		System.GC.Collect();			// Free memory resource here

		Panel_Loading ploading = MyTool.GetPanel<Panel_Loading>( PanelManager.Instance.JustGetUI ( Panel_Loading.Name ) );
		if( ploading != null )	{
			ploading.ShowStoryName();
		}

		// clear data
		Clear();						// 
//		Debug.Log("stagerestore :clearall");
		
		if (save.ePhase == _SAVE_PHASE._MAINTEN) {
			// restore to mainten ui
			return true;
		}


		// stage data  set in stage load
		GameDataManager.Instance.nRound   = save.n_Round;
		GameDataManager.Instance.nActiveCamp = save.e_Camp ;
		GameDataManager.Instance.nMoney = save.n_Money ;

		// re build done event 
		GameDataManager.Instance.EvtDonePool = MyTool.ConvetToIntInt( save.EvtDonePool );
		
		
		// check if in the same stage
		if (StageData != null && StageData.n_ID != save.n_StageID ) {
			// re load scene
			GameDataManager.Instance.nStageID = save.n_StageID ;
			// load const data
			StageData = ConstDataManager.Instance.GetRow<STAGE_DATA>(GameDataManager.Instance.nStageID);
			if (StageData == null)
			{
				Debug.LogFormat("stagerestore:StageData fail with ID {0}  ", GameDataManager.Instance.nStageID);
				return false;
			}
			
			// load scene file
			if (LoadScene(StageData.n_SCENE_ID) == false)
			{
				Debug.LogFormat("stagerestore:LoadScene fail with ID {0} ", StageData.n_SCENE_ID);
				return false;
			}
		}
		
		// re build evt pool
		EvtPool.Clear ();
		char[] split = { ';',' ' , ',' };

		// mission
		string[] strMission = StageData.s_MISSION.Split(split);
		for (int i = 0; i < strMission.Length; i++)
		{
			int nMissionID = 0;
			if( int.TryParse( strMission[i],  out nMissionID ) ){
				if( 0 == nMissionID )
					continue;
				STAGE_EVENT evt = ConstDataManager.Instance.GetRow<STAGE_EVENT>(nMissionID);
				if (evt != null)
				{
					if( EvtPool.ContainsKey( nMissionID ) == false )
					{
						if( GameDataManager.Instance.EvtDonePool.ContainsKey( nMissionID ) == true ){
							continue;
						}
						
						EvtPool.Add(nMissionID, evt);
					}
				}
			}
		}

		//event
		string[] strEvent = StageData.s_EVENT.Split(split);
		for (int i = 0; i < strEvent.Length; i++)
		{
			int nEventID ;
			if( int.TryParse( strEvent[i],  out nEventID ) ){
				if( 0 == nEventID )
					continue;

				STAGE_EVENT evt = ConstDataManager.Instance.GetRow<STAGE_EVENT>(nEventID);
				if (evt != null)
				{
					// check if not loop event
					if( (evt.n_TYPE&1) != 1 ){
						if( GameDataManager.Instance.EvtDonePool.ContainsKey( nEventID ) == true )
						{
							continue;
						}
					}
					
					EvtPool.Add(nEventID, evt);
				}
			}
		}


//		Debug.Log("stageloding:create event Pool complete");


		GameDataManager.Instance.SetBGMPhase( 0 );
//		GameDataManager.Instance.nPlayerBGM = StageData.n_PLAYER_BGM;
//		GameDataManager.Instance.nEnemyBGM = StageData.n_ENEMY_BGM;
//		GameDataManager.Instance.nFriendBGM = StageData.n_FRIEND_BGM;
		
		
		
		// group pool
		//	GameDataManager.Instance.GroupPool = GroupPool ;
		GameDataManager.Instance.GroupPool   = MyTool.ConvetToIntInt( save.GroupPool );

		// unit pool
		GameDataManager.Instance.ImportSavePool (save.CharPool);

		// recreate panel unit
		foreach (KeyValuePair<int , cUnitData > pair  in   GameDataManager.Instance.UnitPool) {
			if(	 pair.Value.n_HP <= 0 )
				continue; // don't create for dead unit

			GameObject obj = CreateUnitByUnitData( pair.Value ) ;
			if (obj != null) {		
				// normal create
			} else {
				Debug.LogErrorFormat ("RestoreBySaveData PopUnit Fail with char({0}) )",pair.Value.n_CharID  )  ;			
			}
		}
//		foreach( cUnitSaveData s in save.CharPool )
//		{
//			cUnitData data = GameDataManager.Instance.CreateCharbySaveData( s  , true );
//			
//			GameObject obj = AddUnit( data ) ;
//			if (obj != null) {		
//			} else {
//				Debug.Log (string.Format ("RestoreBySaveData PopUnit Fail with charid({0}) )", s.n_CharID  )  );			
//			}
//		}
		
		// reset bgm
		
		if( save.nPlayerBGM > 0 )
			GameDataManager.Instance.nPlayerBGM = save.nPlayerBGM ;   //我方
		if( save.nEnemyBGM > 0 )
			GameDataManager.Instance.nEnemyBGM  = save.nEnemyBGM;	 // 敵方
		if( save.nFriendBGM > 0 )
			GameDataManager.Instance.nFriendBGM = save.nFriendBGM;	// 友方
		
		GameDataManager.Instance.ePhase = _SAVE_PHASE._STAGE;		// save to stage phase
		
		// stage bgm
		PlayStageBGM ();
		//GameDataManager.Instance.ImportSavePool( save.CharPool );
		//		bIsRestoreData = false;

		bIsLoading = false;
		bIsStageEnd = false;


		return true;
	}

    public void OnStageUnitActMask( GameObject opObj , bool bEnable )
    {
        MobActEffObj.SetActive(bEnable);
        if (opObj != null) {
            ActEffFolObj = opObj;
            MobActEffObj.transform.position = opObj.transform.position;
        }
    }
    


	// Game event func
	public void OnStageBGMEvent(GameEvent evt)
	{
		//Debug.Log ("OnStagePopCharEvent");
		StageBGMEvent Evt = evt as StageBGMEvent;
		if (Evt == null)
			return;
		PlayStageBGM ();

	}

	public void OnStagePopUnitEvent(GameEvent evt)
	{
        // auto close all say window
        //		TalkSayEndEvent sayevt = new TalkSayEndEvent();
        //		sayevt.nChar = 0;		
        //		GameEventManager.DispatchEvent ( sayevt  );
        Panel_Talk.Show(false);

        //Debug.Log ("OnStagePopMobEvent");
        StagePopUnitEvent Evt = evt as StagePopUnitEvent;
		if (Evt == null)
			return;
		int nPopNum = 1;
		if (Evt.nValue1 > 1)
			nPopNum = Evt.nValue1;

        for (int i=0; i < nPopNum; i++) {
            int nX = Evt.nX;
            int nY = Evt.nY;

            if (Evt.nRadius > 0)
            {
                nX = Random.Range(nX - Evt.nRadius, nX + Evt.nRadius);
                nY = Random.Range(nY - Evt.nRadius, nY + Evt.nRadius);
            }


            cUnitData cData = GameDataManager.Instance.StagePopUnit( Evt.nCharID,Evt.eCamp, nX, nY , StageData.n_MOB_LV ); 

			GameObject obj = CreateUnitByUnitData (  cData );
			if (obj != null) {		
			} else {
				Debug.Log (string.Format ("OnStagePopUnitEvent Fail with charid({0}) num({1})", Evt.nCharID  , nPopNum )  );			
			}
		}
	}


	public void OnStagePopGroupEvent(GameEvent evt)
	{
        // auto close all say window
        //		TalkSayEndEvent sayevt = new TalkSayEndEvent();
        //		sayevt.nChar = 0;		
        //		GameEventManager.DispatchEvent ( sayevt  );
        Panel_Talk.Show(false);

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
					//cUnitData cData =  GameDataManager.Instance.StagePopUnit( Evt.nCharID,Evt.eCamp, i , j ,nLeaderIdent  ); 
					cUnitData cData = GameDataManager.Instance.StagePopUnit( Evt.nCharID  , pLeader.eCampID , i , j  , StageData.n_MOB_LV , nLeaderIdent );
					if( cData == null ){
						continue ;
					}


					GameObject obj = CreateUnitByUnitData ( cData  );
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
        // auto close all say window
        //		TalkSayEndEvent sayevt = new TalkSayEndEvent();
        //		sayevt.nChar = 0;		
        //		GameEventManager.DispatchEvent ( sayevt  );
        Panel_Talk.Show(false);

        //Debug.Log ("OnStagePopCharEvent");
        StageDelUnitEvent Evt = evt as StageDelUnitEvent;
		if (Evt == null)
			return;
		int nCharid = Evt.nCharID;

		if (m_bIsSkipMode) {
//			DelUnit (nCharid);
//			return;
		}
 		// normal mode to play
		LeaveUnit (nCharid);

	}

	public void OnStageUnitCampEvent( int nCharid , _CAMP nCampid )
	{
		// auto close all say window
		TalkSayEndEvent sayevt = new TalkSayEndEvent();
		sayevt.nChar = 0;		
		GameEventManager.DispatchEvent ( sayevt  );

		foreach (KeyValuePair< int , Panel_unit> pair in this.IdentToUnit) {
			if( pair.Value.CharID == nCharid )
			{
				if( pair.Value.eCampID !=  nCampid ){
					pair.Value.SetCamp( nCampid );
				}
			}
		}
	}


	public void OnStageCharMoveEvent(GameEvent evt)
	{
        // say end
        //		TalkSayEndEvent sayevt = new TalkSayEndEvent();
        //		sayevt.nChar = 0;		
        //		GameEventManager.DispatchEvent ( sayevt  );
        Panel_Talk.Show(false);


        //Debug.Log ("OnStagePopCharEvent");
        StageCharMoveEvent Evt = evt as StageCharMoveEvent;
		if (Evt == null)
			return;
		int nIdent = Evt.nIdent;
		int nX =  Evt.nX;
		int nY =  Evt.nY;


		if( nIdent == 0 && Evt.nCharID > 0 )
		{
			cUnitData data =GameDataManager.Instance.GetUnitDateByCharID( Evt.nCharID ) ;
			if( data != null )
			{
				nIdent = data.n_Ident;
			}
		}
//		if( IdentToUnit.ContainsKey(nIdent) == false )  {
//			Debug.Log( "ERR: can't find unit to move" );
//		}
		// find vaild pos



		Panel_unit unit = GetUnitByIdent ( nIdent); // IdentToUnit[ nIdent ];
		if( unit != null )
		{
			iVec2 pos = new iVec2(nX ,nY );
			if( CheckIsEmptyPos(pos)== false ){
				pos = FindEmptyPosToAttack  ( pos , unit.Loc );
			}
			//
			if (m_bIsSkipMode) { 
				unit.SetXY( pos.X , pos.Y );
			}
			else {
				unit.MoveTo( pos.X , pos.Y ); 
			}

			if( MyScript.bParsing ){
                if (!m_bIsSkipMode)
                {
                    tmpScriptMoveEnd.Add(pos);              // script 用的單位移動 mrak pool 防止 script 讓不同單位移動到 同樣座標
                }
			}
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
		else{
			Debug.LogError( " Err! OnStageCharMoveEvent with null unit");
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

		// change bgm
		PlayStageBGM ();

		// relive undead
		GameDataManager.Instance.ReLiveUndeadUnit (Evt.nCamp);


		// weakup  
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
        // auto close all say window
        //		TalkSayEndEvent sayevt = new TalkSayEndEvent();
        //		sayevt.nChar = 0;		
        //		GameEventManager.DispatchEvent ( sayevt  );
        Panel_Talk.Show(false);


        //Debug.Log ("OnStagePopCharEvent");
        StageBattleAttackEvent Evt = evt as StageBattleAttackEvent;
		if (Evt == null)
			return;

        int nNum = Evt.nNum;
        if (nNum == 0)
        {
         //   nNum = 1;
        }
        int nResult = Evt.nResult;

        // attack 

      //  Panel_unit pAtkUnit = GetUnitByCharID ( Evt.nAtkCharID );
		Panel_unit pDefUnit = GetUnitByCharID ( Evt.nDefCharID );
		if (pDefUnit == null)
			return;

	//	cUnitData pAtker = pAtkUnit.pUnitData;
		cUnitData pDefer = pDefUnit.pUnitData;

//		int nAtkId = pAtkUnit.Ident ();
		int nDefId = pDefUnit.Ident ();
		int nSkillID = Evt.nAtkSkillID;
		int nRange = 1;

		if (Evt.nAtkSkillID != 0) {
			SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(Evt.nAtkSkillID); 
			if( skl != null ){
				nRange = skl.n_RANGE;
			}
		}

        int count = 0;
        // show skill name
        foreach (KeyValuePair<int, Panel_unit> pair in IdentToUnit)
        {
            if (pair.Value == null || pair.Value.CharID != Evt.nAtkCharID )
                continue;
            cUnitData pAtker = pair.Value.pUnitData;
            int nAtkId = pair.Value.Ident();
            // check if need move 
            int nDist = pair.Value.Loc.Dist(pDefUnit.Loc);
            if (nDist > nRange)
            {

                iVec2 last = FindEmptyPosToAttack(pDefUnit.Loc, pair.Value.Loc);

                if (m_bIsSkipMode)
                {
                    // change pos directly 
                    pair.Value.SetXY(last.X, last.Y);
                }
                else
                {
                    ActionManager.Instance.CreateMoveAction(pair.Value.Ident(), last.X, last.Y);
                    MoveToGameObj(pair.Value.gameObject, false);
                    TraceUnit(pair.Value);

                    tmpScriptMoveEnd.Add(last ); // avoid double pos
                    // add to tmp 
                }
            }
            // skill hit effect
            List<cUnitData> pool = new List<cUnitData>();
            BattleManager.GetAffectPool(pAtker, pDefer, nSkillID, 0, 0, ref pool); // will convert inside

            if ((pDefer != null) && pool.Contains(pDefer) == false)
            {
                pool.Add(pDefer);
            }
            if (m_bIsSkipMode)
            {
                // perform pos directly
                // perform pos directly		
                foreach (cUnitData d in pool)
                {
                    List<cHitResult> HitResult = BattleManager.CalSkillHitResult(pAtker, d, nSkillID);
                    ActionManager.Instance.ExecActionHitResult(HitResult, m_bIsSkipMode);  // play directly without action to avoid 1 frame error
                    ActionManager.Instance.ExecActionEndResult(HitResult, m_bIsSkipMode);
                }

            }
            else
            {
                ActionManager.Instance.CreateCastAction(nAtkId, Evt.nAtkSkillID, nDefId);

                // send attack
                //Panel_StageUI.Instance.MoveToGameObj(pDefUnit.gameObject , false );  // move to def 
                uAction act = ActionManager.Instance.CreateAttackAction(nAtkId, nDefId, Evt.nAtkSkillID);
                if (act != null)
                {
                    //act.AddHitResult (new cHitResult (cHitResult._TYPE._HIT, nAtkId, Evt.nAtkSkillID));

                    foreach (cUnitData d in pool)
                    {
                        if ( 1 == nResult ) {
                            act.AddHitResult(new cHitResult(cHitResult._TYPE._DODGE, d.n_Ident, 0));
                        }

                        act.AddHitResult(BattleManager.CalSkillHitResult(pAtker, d, nSkillID));
                    }

                }
            }
             // check break;
            if (count++ >= nNum && nNum > 0) // nNum = -1 is all
            {
                break;
            }
        }



		// check if need move 
		//int nDist = pAtkUnit.Loc.Dist (pDefUnit.Loc);
		//if (nDist > nRange) {

		//	iVec2 last = FindEmptyPosToAttack ( pDefUnit.Loc , pAtkUnit.Loc);

		//	if (m_bIsSkipMode)
		//	{
		//		// change pos directly 
		//		pAtkUnit.SetXY( last.X , last.Y );
		//	}
		//	else{    
		//		ActionManager.Instance.CreateMoveAction( pAtkUnit.Ident() , last.X , last.Y );	
		//		MoveToGameObj( pAtkUnit.gameObject , false );
		//		TraceUnit( pAtkUnit );
		//	}
		//}

		//List< cUnitData> pool = new List< cUnitData> ();
		
		
		//BattleManager.GetAffectPool (pAtker ,pDefer ,nSkillID, 0 ,0, ref pool ); // will convert inside
		
		//if(  (pDefer!=null) && pool.Contains (pDefer) == false) {
		//	pool.Add( pDefer );
		//}


		// attak perform 
		//if (m_bIsSkipMode) {
		//	 // perform pos directly
		//	// perform pos directly		
		//	foreach( cUnitData d in pool )
		//	{
		//		List<cHitResult> HitResult = BattleManager.CalSkillHitResult(pAtker , d , nSkillID  );
		//		ActionManager.Instance.ExecActionHitResult(HitResult ,m_bIsSkipMode );  // play directly without action to avoid 1 frame error
		//		ActionManager.Instance.ExecActionEndResult(HitResult ,m_bIsSkipMode );
		//	}

		//} else {
		//	ActionManager.Instance.CreateCastAction (nAtkId, Evt.nAtkSkillID, nDefId );

		//	// send attack
		//	//Panel_StageUI.Instance.MoveToGameObj(pDefUnit.gameObject , false );  // move to def 
		//	uAction act = ActionManager.Instance.CreateAttackAction (nAtkId, nDefId, Evt.nAtkSkillID);
		//	if (act != null) {
		//		//act.AddHitResult (new cHitResult (cHitResult._TYPE._HIT, nAtkId, Evt.nAtkSkillID));

		//		foreach( cUnitData d in pool )
		//		{	
		//			act.AddHitResult( BattleManager.CalSkillHitResult(pAtker , d , nSkillID  ) );				
		//		}

		//	}
		//}
	}



	public void OnStageBattleCastEvent(GameEvent evt)
	{
        // auto close all say window
        //		TalkSayEndEvent sayevt = new TalkSayEndEvent();
        //		sayevt.nChar = 0;		
        //		GameEventManager.DispatchEvent ( sayevt  );
        Panel_Talk.Show(false);

        //Debug.Log ("OnStagePopCharEvent");
        StageBattleCastEvent Evt = evt as StageBattleCastEvent;
		if (Evt == null)
			return;
		
		
		// attack 
		
		Panel_unit pAtkUnit = GetUnitByCharID ( Evt.nAtkCharID );
		Panel_unit pDefUnit = GetUnitByCharID ( Evt.nDefCharID );
		if (pAtkUnit == null )
			return;
		int nAtkId = pAtkUnit.Ident ();
		int nDefId = 0;
		cUnitData pAtker = pAtkUnit.pUnitData;
		cUnitData pDefer = null;

		if (pDefUnit != null) {
			nDefId = pDefUnit.Ident ();
			pDefer = pDefUnit.pUnitData;
		}
		// skill param
		int nSkillID = Evt.nAtkSkillID;
//		int nRange = 1;
//		int nHitBack = 0;
//		if (Evt.nAtkSkillID != 0) {
//			SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(Evt.nAtkSkillID); 
//			if( skl != null ){
//				nRange = skl.n_RANGE;
//				nHitBack = skl.n_HITBACK;
//			}
//		}

		// cast directly
		int nX = Evt.nTargetX ;
		int nY = Evt.nTargetY ;


		List< cUnitData> pool = new List< cUnitData> ();

		BattleManager.ConvertSkillTargetXY( pAtker , nSkillID , nDefId , ref nX , ref nY );
		BattleManager.GetAffectPool (pAtker ,pDefer ,nSkillID,nX ,nY, ref pool ); // will convert

		if(  (pDefer!=null) && pool.Contains (pDefer) == false) {
			pool.Add( pDefer );
		}

		// attak perform 
		if (m_bIsSkipMode) {
			// perform pos directly		
			foreach( cUnitData d in pool )
			{
				List<cHitResult> HitResult = BattleManager.CalSkillHitResult(pAtker , d , nSkillID  );

				ActionManager.Instance.ExecActionHitResult(HitResult ,m_bIsSkipMode );  // play directly without action to avoid 1 frame error
				ActionManager.Instance.ExecActionEndResult(HitResult ,m_bIsSkipMode );

//					Panel_unit pUnit = GetUnitByIdent( d.n_Ident );
//					if( pUnit != null ){
//						iVec2 vFinal = SkillHitBack (pAtkUnit, pUnit, nHitBack);
//						if (vFinal != null) {
//							pDefUnit.SetXY( vFinal.X , vFinal.Y );
//						}
//					}
			}
			
		} else {
			ActionManager.Instance.CreateCastAction (nAtkId, Evt.nAtkSkillID, nDefId , nX , nY );

			// send attack
			//Panel_StageUI.Instance.MoveToGameObj(pDefUnit.gameObject , false );  // move to def 
			uAction act = ActionManager.Instance.CreateHitAction (nAtkId, nX , nY ,nSkillID );
			if (act != null) {
				//act.AddHitResult (new cHitResult (cHitResult._TYPE. , nAtkId, Evt.nAtkSkillID));
				//act.AddHitResult (new cHitResult (cHitResult._TYPE._BEHIT, nDefId, Evt.nAtkSkillID));
				
				//add skill perform

				foreach( cUnitData d in pool )
				{

					//act.AddHitResult (new cHitResult (cHitResult._TYPE._BEHIT, d.n_Ident , nSkillID )); // for hit fx


					act.AddHitResult( BattleManager.CalSkillHitResult(pAtker , d , nSkillID  ) );
//					if (nHitBack != 0) {
//						Panel_unit pUnit = GetUnitByIdent( d.n_Ident );
//						if( pUnit != null ){
//							iVec2 vFinal = SkillHitBack (pAtkUnit, pUnit, nHitBack);
//							if (vFinal != null) {
//								//pDefUnit.SetXY( vFinal.X , vFinal.Y );
//								act.AddHitResult (new cHitResult (cHitResult._TYPE._HITBACK, nDefId, vFinal.X, vFinal.Y));
//							}
//						}						
//					}
				}

//					iVec2 vFinal = SkillHitBack (pAtkUnit, pDefUnit, nHitBack);
//					if (vFinal != null) {
//						act.AddHitResult (new cHitResult (cHitResult._TYPE._HITBACK, nDefId, vFinal.X, vFinal.Y));
//					}

			}
		}
	}



	public void OnStageMoveToUnitEvent(GameEvent evt)
	{
        // auto close all say window
        //		TalkSayEndEvent sayevt = new TalkSayEndEvent();
        //		sayevt.nChar = 0;		
        //		GameEventManager.DispatchEvent ( sayevt  );
        Panel_Talk.Show(false);

        //Debug.Log ("OnStagePopCharEvent");
        StageMoveToUnitEvent Evt = evt as StageMoveToUnitEvent;
		if (Evt == null)
			return;


		// force close 
		Panel_unit pAtkUnit = GetUnitByCharID (Evt.nAtkCharID);
		Panel_unit pDefUnit = GetUnitByCharID (Evt.nDefCharID);
		if (pAtkUnit == null || pDefUnit == null)
			return;
		int nDist = pAtkUnit.Loc.Dist (pDefUnit.Loc);
		if (nDist > 1) {
			iVec2 last = FindEmptyPosToAttack ( pDefUnit.Loc , pAtkUnit.Loc );
			if (m_bIsSkipMode)
			{
				// change pos directly 
				pAtkUnit.SetXY( last.X , last.Y );

			}
			else{    
				ActionManager.Instance.CreateMoveAction( pAtkUnit.Ident() , last.X , last.Y );	
				MoveToGameObj( pAtkUnit.gameObject , false );
				TraceUnit( pAtkUnit );
			}

			tmpScriptMoveEnd.Add( last );				// script 用的單位移動 mrak pool 防止 script 讓不同單位移動到 同樣座標

//			List< iVec2> path = MobAI.FindPathToTarget( pAtkUnit , pDefUnit , 999 );
//			//List< iVec2> path = PathFinding( pAtkUpDefUnitnit , pAtkUnit.Loc ,  pDefUnit.Loc , 0  ); // no any block
//			//PathFinding
//			
//			if( path != null && path.Count > 1 )
//			{
//				iVec2 last = path[path.Count -1 ];
//
//				if (m_bIsSkipMode)
//				{
//					// change pos directly 
//					pAtkUnit.SetXY( last.X , last.Y );
//				}
//				else{    
//
//					ActionManager.Instance.CreateMoveAction( pAtkUnit.Ident() , last.X , last.Y );	
//					MoveToGameObj( pAtkUnit.gameObject , false );
//					TraceUnit( pAtkUnit );
//				}
//			}
			// move only
			
		}
	}
	// 本作法 會造成存讀黨的問題
//	public void OnStageDelEventEvent( int nEvtID )
//	{
//		if (nEvtID == 0)
//			return;
//		if (EvtPool.ContainsKey (nEvtID)) {
//			EvtPool.Remove( nEvtID );
//		}
//		foreach (STAGE_EVENT evt in WaitPool) {
//			if( evt.n_ID == nEvtID )
//			{
//				WaitPool.Remove( evt );
//				break;
//			}
//		}
//		return;
//
//	}

	public void OnStagePlaySound( string  SoundFile  )
	{
		GameSystem.PlaySound( SoundFile );
	}
	

	public void OnStagePlayFX(int nCharID , int nFXID  )
	{
		if (nFXID == 0) {
			return ;
		}
        //		TalkSayEndEvent sayevt = new TalkSayEndEvent();
        //		sayevt.nChar = 0;		
        //		GameEventManager.DispatchEvent ( sayevt  );
        Panel_Talk.Show(false);

        int nTot = 0;
		foreach (KeyValuePair< int ,Panel_unit> pair in IdentToUnit) {
			Panel_unit unit = pair.Value;
			if (unit.CharID != nCharID)
				continue;

			nTot++;
            unit.PlayFX(nFXID);
            //GameSystem.PlayFX(unit.gameObject , nFXID );

            // move camera
            MoveToGameObj(unit.gameObject , true , 0.5f );		
		}
		if (nTot == 0) {
			Debug.LogErrorFormat ("OnStagePlayFX {0} on null unit with char {1} ", nFXID, nCharID);
		}
	}

    public void OnStagePosPlayFX(int nX, int nY,  int nFXID , int nDown = 0 )
    {
        if (nFXID == 0)
        {
            return;
        }
        //		TalkSayEndEvent sayevt = new TalkSayEndEvent();
        //		sayevt.nChar = 0;		
        //		GameEventManager.DispatchEvent ( sayevt  );
        Panel_Talk.Show(false);

        CameraMoveTo(nX, nY);

        PlayFX( nFXID, nX, nY , (0== nDown) );
    }



    public void OnStageAddBuff(int nCharID , int nBuffID , int nCastCharID , int nDel = 0 )
	{
		if (nBuffID == 0) {
			return ;
		}
        //		TalkSayEndEvent sayevt = new TalkSayEndEvent();
        //		sayevt.nChar = 0;		
        //		GameEventManager.DispatchEvent ( sayevt  );
        Panel_Talk.Show(false);  // some buff is invisible

        int nCastIdent = 0;
		if (nCastCharID != 0) {
			nCastIdent = GameDataManager.Instance.GetIdentByCharID( nCastCharID );
		}


		int nTot = 0;
		foreach (KeyValuePair< int ,Panel_unit> pair in IdentToUnit) {
			Panel_unit unit = pair.Value;
			if (unit.CharID != nCharID)
				continue;
			//
			if (unit.pUnitData == null)
				continue;
			nTot++;
			if( nDel == 0 ){
				unit.pUnitData.Buffs.AddBuff (nBuffID, nCastIdent, 0, 0);
			}
			else{
				unit.pUnitData.Buffs.DelBuff( nBuffID, true );
				
			}
		}
		if (nTot == 0) {
			Debug.LogErrorFormat ("OnStageAddBuff {0} on null unit with char {1} , type{2}", nBuffID, nCharID,nDel);
		}
	}

	public void OnStageCampAddBuff( int nCampID , int nBuffID , int nCastCharID , int nDel = 0 )
	{
		if (nBuffID == 0) {
			return ;
		}
        // close say ui
        TalkSayEndEvent sayevt = new TalkSayEndEvent();
        sayevt.nChar = 0;
        GameEventManager.DispatchEvent(sayevt);



        int nCastIdent = 0;
		if (nCastCharID != 0) {
			nCastIdent = GameDataManager.Instance.GetIdentByCharID( nCastCharID );
		}

		// add , del buff
		int nTot = 0;
		_CAMP camp = (_CAMP)nCampID;
		foreach (KeyValuePair< int ,Panel_unit> pair in IdentToUnit) {
			Panel_unit unit = pair.Value;
			if (unit.eCampID !=  camp)
				continue;
			//
			if (unit.pUnitData == null)
				continue;
			nTot++;
			if( nDel == 0 ){
				unit.pUnitData.Buffs.AddBuff (nBuffID, nCastIdent , 0, 0);
			}
			else{
				unit.pUnitData.Buffs.DelBuff( nBuffID, true );
				
			}
		}
		if (nTot == 0) {
			Debug.LogErrorFormat ("OnStageCampAddBuff {0} on null unit with camp {1} , type{2}", nBuffID, nCampID,nDel);
		}
	}

	//
	public void OnStageAddUnitValue(int nCharID , int nType , float f )
	{
		if (f == 0)
			return;		
		int nTot = 0;
		foreach (KeyValuePair< int ,Panel_unit> pair in IdentToUnit) {
			Panel_unit unit = pair.Value;
			if( unit == null )
				continue;
			if( unit.CharID != nCharID )
				continue;
			//
			if( unit.pUnitData == null )
				continue;
			nTot++;
			int nValue = 0;
			switch( nType ){
				case 0:{
					nValue = (int)(unit.pUnitData.GetMaxHP()*f);
					unit.pUnitData.AddHp( nValue );
				}	break; // HP
				case 1:{
					nValue = (int)(unit.pUnitData.GetMaxDef()*f);
					unit.pUnitData.AddDef( nValue );
				}	break; // DEF
				case 2:{
					nValue = (int)(unit.pUnitData.GetMaxMP()*f);
					unit.pUnitData.AddMp( nValue );
				}	break; // MP
			}
			//====================================
			if( nValue != 0 ){	
				unit.ShowValueEffect( nValue , nType ); // MP
			}
		}

		if (nTot == 0) {
			Debug.LogErrorFormat("OnStageAddUnitValue on null unit with char {0} at type{1}= {2}% " , nCharID,nType, f );
		}
		//else {
		//	Debug.LogErrorFormat("OnStageAddUnitValue on null unit with char {0} at type{1}= {2}% " , nCharID,nType, f );
		//} 
	}
    public void OnStageSetUnitValue(int nCharID, int nType, float f , int i)
    {      
        int nTot = 0;
        foreach (KeyValuePair<int, Panel_unit> pair in IdentToUnit)
        {
            Panel_unit unit = pair.Value;
            if (unit == null)
                continue;
            if (unit.CharID != nCharID)
                continue;
            //
            if (unit.pUnitData == null)
                continue;
            nTot++;
            int nValue = 0;
            switch (nType)
            {
                case 0:
                    {
                        nValue = (int)(unit.pUnitData.GetMaxHP() * f) + i;
                        unit.pUnitData.n_HP = nValue;
                    }
                    break; // HP
                case 1:
                    {
                        nValue = (int)(unit.pUnitData.GetMaxDef() * f) +i ;
                        unit.pUnitData.n_DEF = nValue;
                    }
                    break; // DEF
                case 2:
                    {
                        nValue = (int)(unit.pUnitData.GetMaxMP() * f)+i;
                        unit.pUnitData.n_MP = nValue;
                    }
                    break; // MP
            }
            //====================================
            if (nValue != 0)
            {
                unit.ShowValueEffect(nValue, nType); // MP
            }
        }

        if (nTot == 0)
        {
            Debug.LogErrorFormat("OnStageAddUnitValue on null unit with char {0} at type{1}= {2}% ", nCharID, nType, f);
        }
        //else {
        //	Debug.LogErrorFormat("OnStageAddUnitValue on null unit with char {0} at type{1}= {2}% " , nCharID,nType, f );
        //} 
    }
    public void OnStageAddSchool(int nCharID , int nSchool , int nLv  )
	{
		foreach (KeyValuePair< int ,Panel_unit> pair in IdentToUnit) {
			Panel_unit unit = pair.Value;
			if( unit == null )
				continue;
			if( unit.CharID != nCharID )
				continue;
			//
			if( unit.pUnitData == null )
				continue;

			unit.pUnitData.LearnSchool( nSchool ,  nLv );
			unit.pUnitData.ActiveSchool( nSchool );
			unit.pUnitData.UpdateAllAttr();
		}

	}


	public void OnStageRelive(int nCharID  )
	{
		foreach (KeyValuePair< int ,Panel_unit> pair in IdentToUnit) {
			Panel_unit unit = pair.Value;
			if( unit == null )
				continue;
			if( unit.CharID != nCharID )
				continue;
			//
			if( unit.pUnitData == null )
				continue;
			unit.pUnitData.Relive();
		}
	}


	public void OnStagePopMarkEvent( int x1 ,int y1 , int x2  , int y2  )
	{
        // auto close all say window
        //		TalkSayEndEvent sayevt = new TalkSayEndEvent();
        //		sayevt.nChar = 0;		
        //		GameEventManager.DispatchEvent ( sayevt  );
        Panel_Talk.Show(false);

        int sx = x1 < x2 ? x1 : x2;
		int sy = y1 < y2 ? y1 : y2;
		int ex = x1 > x2 ? x1 : x2; 
		int ey = y1 > y2 ? y1 : y2 ;
		
		for( int i = sx ; i <= ex ; i++  ){
			for( int j = sy ; j <= ey ; j++  ){
				GameObject obj = AddMark ( i , j  );
				if (obj != null) {	
					
				} else {
					Debug.Log (string.Format ("OnStagePopMarkEvent Fail with ({0},{1}) ",  i , j )  );			
				}
			}
		}

		// camera to center
		Vector3 v1 = MyTool.SnyGridtoLocalPos (x1, y1, ref Grids);
		Vector3 v2 = MyTool.SnyGridtoLocalPos (x2, y2, ref Grids);
		CameraMoveTo ((v1.x + v2.x) / 2, (v1.y + v2.y) / 2);
	}

	public void OnStageCameraCenterEvent( int x1 , int y1 )
	{
		if (m_bIsSkipMode)
			return;

		Vector3 v1 = MyTool.SnyGridtoLocalPos (x1, y1, ref Grids);
		CameraMoveTo ( v1.x , v1.y );
	}

	public void CameraMoveTo( float fx , float fy )
	{
		if (m_bIsSkipMode)
			return;

		Vector3 v = new Vector3 (fx, fy);
		Vector3 canv = gameObject.transform.localPosition; // shift
		Vector3 realpos = v + canv;
		
		
		//TilePlaneObj.transform.localPosition = -v;
		float dist = Vector3.Magnitude( realpos );
		
		float during = dist/500.0f; // 這是最小值
		//距離過大要算最大值
		//	if (force == true) {
		if (during > 1.0f) 
			during = 1.0f;		// 不管多遠都不要超過1 秒
		//	}
		
		TweenPosition tw = TweenPosition.Begin<TweenPosition> (gameObject, during);
		if (tw != null) {
			tw.SetStartToCurrentValue();
			tw.to = -v;
			bIsMoveToObj = true;
			MyTool.TweenSetOneShotOnFinish( tw , MoveToGameObjEnd ); 
			
		}
	}
	// 單位死亡
	public void OnStageUnitDeadEvent( int nCharID , int nNum = 0)
	{
		// auto close all say window. 
        // bug .. auto close in dead end
//		TalkSayEndEvent sayevt = new TalkSayEndEvent();
//		sayevt.nChar = 0;		
//		GameEventManager.DispatchEvent ( sayevt  );

		if( m_bIsSkipMode )
		{
			DelUnit( nCharID , nNum );
			return;
		}

        int nCount = 0;
		foreach (KeyValuePair< int ,Panel_unit > pair in IdentToUnit) {
			if( pair.Value!= null )
			{
				if( pair.Value.CharID == nCharID )
				{
					if( pair.Value.bIsDead == false )
					{
						pair.Value.SetDead();
                        nCount++;
                    }
                    if (nNum > 0 && nCount >= nNum) {
                        break;
                    }
				}
			}
		}
	}
	// add star
	public void AddStar( int nStar=1 )
	{
		if (nStar == 0) {
			nStar = 1;
		}
		GameDataManager.Instance.nStars +=nStar;
		string sMsg = string.Format( "星星+ {0}" , nStar );
		BattleManager.Instance.ShowBattleMsg( null , sMsg );
	}

}
