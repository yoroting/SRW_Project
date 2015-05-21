using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _SRW;
using MYGRIDS;
using MyClassLibrary;			// for parser string



public class Panel_StageUI : MonoBehaviour {




	public GameObject BackGroundObj; // back ground
	public GameObject TilePlaneObj; // plane of all tiles sprite

	cMyGrids	Grids;				// main grids
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




	//Dictionary< int , GameObject > EnemyPool;			// EnemyPool this should be group pool


	Dictionary< int , GameObject > IdentToObj;			// allyPool
	Dictionary< int , UNIT_DATA > UnitDataPool;			// ConstData pool

	// ScreenRatio
	// float fUIRatio;


	void Awake( ){	

		//UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);	
		//fUIRatio = (float)mRoot.activeHeight / Screen.height;

		//float ratio = (float)mRoot.activeHeight / Screen.height;

		// UI Event
		// UIEventListener.Get(BackGroundObj).onClick += OnBackGroundClick;
	
		Grids = new cMyGrids ();
		StageData = new STAGE_DATA();
		EvtPool = new Dictionary< int , STAGE_EVENT >();			// add event id 

		WaitPool = new List< STAGE_EVENT >();					// check ok. waitinf to execute event
		EvtCompletePool = new Dictionary< int , int >();

		IdentToObj = new Dictionary< int , GameObject >();
		UnitDataPool = new Dictionary< int , UNIT_DATA >();
		// Debug Code jump to stage
		GameDataManager.Instance.nStageID = 1;
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
		GameDataManager.Instance.nActiveFaction  = 0;

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
	
	// Update is called once per frame
	void Update () {

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


		// block other event
		if( IsAnyActionRunning() == true )
			return;

		//=== Unit Action ====
		// Atk / mov / other action need to preform first
		RunUnitAction();

		//==================	
		RunEvent( );				// this will throw many unit action

		if( IsRunningEvent() == true  )
			return; // block other action  

		//===================		// this will throw unit action or trig Event
		if ( RunFactionAI( GameDataManager.Instance.nActiveFaction ) == false )
		{
			// no ai to run. go to next faction
			GameDataManager.Instance.NextFaction();
		}

		//

	}

	void OnDestroy()
	{
	
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
			Debug.Log(str);

			// avoid re open
//			if( PanelManager.Instance.CheckUIIsOpening( "Panel_CMDUI" ) == false  )
			{
				GameObject obj = PanelManager.Instance.GetOrCreatUI( "Panel_CMDSYSUI" );
				if (obj != null) {
					NGUITools.SetActive(obj, true );
					Vector3 vLoc = this.gameObject.transform.localPosition ;
					//UICamera.mainCamera.ScreenPointToRay
				//	UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);					
				//	float ratio = (float)mRoot.activeHeight / Screen.height;

					vLoc.x = MyTool.ScreenToLocX( Input.mousePosition.x );
					vLoc.y = MyTool.ScreenToLocY( Input.mousePosition.y );

					obj.transform.localPosition = vLoc;// MousePosition;

//					Panel_CmdSysUI co = obj.GetComponent< Panel_CmdSysUI >();
//					if( co )
//					{					
//					}
				}
			}
		}
	}

	bool LoadScene( int nScnid )
	{
		SCENE_NAME scn = ConstDataManager.Instance.GetRow<SCENE_NAME> ( nScnid );
		if (scn == null)
			return false;
		string filename = "Assets/StreamingAssets/scn/"+scn.s_MODLE_ID+".scn";
		
		bool bRes = Grids.Load( filename );
		
		// start to create sprite
		for( int i = -Grids.hW ; i <= Grids.hW ; i++ ){
			for( int j = -Grids.hH ; j <= Grids.hH ; j++ )
			{			
				_TILE t = Grids.GetValue( i , j  );
				
				GameObject cell = GetTileCellPrefab( i , j , t ); 
				
			}
		}
		// reget the drag limit 
		Resize();

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
			float locx =0, locy =0;
			Grids.GetRealXY(ref locx , ref locy , new iVec2( x , y ) );
			
			Vector3 pos = new Vector3( locx , locy , 0 );
			if( cell != null ){
				cell.transform.localPosition = pos; 
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
	// Check any action is running

	bool IsAnyActionRunning()
	{
		if( PanelManager.Instance.CheckUIIsOpening( "Panel_Talk" ) == true )
			return true;

		return false;
	}

	// UnitAction
	void RunUnitAction()
	{


	}


	// Faction AI
	bool RunFactionAI( int nFaction )
	{
		// our faction don't need AI process
		if( nFaction == 0 )
			return true; // player is playing 

		// change faction if all unit moved or dead.
		FactionUnit unit = GameDataManager.Instance.GetFaction( nFaction );
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
		if( EvtPool.Count <= 0 )
			return ;

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

	bool ConditionAllDead( int nFactID )
	{
		// assign id
		FactionUnit unit = GameDataManager.Instance.GetFaction( nFactID );
		if( unit != null )
		{
			return (unit.memLst.Count<=0) ;
		}
		return true;
	}

	bool ConditionUnitDead( int nFactID ,int nID )
	{
		// assign id
		FactionUnit unit = GameDataManager.Instance.GetFaction( nFactID );
		if( unit != null )
		{
			foreach( int no in  unit.memLst )
			{
				GameObject obj = this.IdentToObj[ no ];
				if( obj != null )
				{


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
				GameObject obj = AddChar( charid , func.At( 1) , func.At( 2 ) );
				if( obj != null )
				{
					Panel_unit unitobj = obj.GetComponent<Panel_unit>();
					//set as ally


					GameDataManager.Instance.AddFactionMember( 0 , unitobj.pUnitData.n_Ident ); // global game data

					IdentToObj.Add( unitobj.pUnitData.n_Ident , obj  ) ;// stage gameobj
				}
			}
			else if( func.sFunc == "POPMOB" )
			{
				int charid = func.At( 0 );
				GameObject obj = AddChar( charid , func.At( 1) , func.At( 2 ) );
				if( obj != null )
				{
					Panel_unit unitobj = obj.GetComponent<Panel_unit>();
					//set as enemy

					GameDataManager.Instance.AddFactionMember( 1 , unitobj.pUnitData.n_Ident );		// global game data

					IdentToObj.Add( unitobj.pUnitData.n_Ident , obj  ) ; // stage gameobj
				}
			}
			else if( func.sFunc == "TALK"  )
			{
				Talk( func.At( 0 ) );
			}
			else if( func.sFunc == "BGM"  )
			{
				int nID = func.At( 0 );
				// change bgm 
				GameSystem.PlayBGM ( nID );


			}
		}
	}


		 
	GameObject AddChar( int nCharID , int x , int y )
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

		// position
		obj.transform.localPosition =  MyTool.SnyGridtoLocalPos( x , y , ref Grids ) ; 



		// all ready
		NGUITools.SetActive( obj , true );
		return obj;
	}

	void DelChar()
	{


	}

	void Talk( int nTalkID  )
	{
		GameDataManager.Instance.nTalkID = nTalkID;
		// start talk UI
		GameObject obj = PanelManager.Instance.GetOrCreatUI( Panel_Talk.Name );
	}

}
