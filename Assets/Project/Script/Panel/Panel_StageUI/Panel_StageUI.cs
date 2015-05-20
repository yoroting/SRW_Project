using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _SRW_CMD;
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
	STAGE_EVENT						NextEvent;
	private cTextArray 				m_cScript;			// 劇本 腳本集合
	private int						m_nFlowIdx;			// 腳本演到哪段
	bool							IsEventEnd;			// 


	Dictionary< int , int > EvtCompletePool;			// record event complete round 

	Dictionary< int , GameObject > EnemyPool;			// EnemyPool
	Dictionary< int , GameObject > AllyPool;			// EnemyPool


	void Awake( ){

		if( Config.LoadConstData == false )
		{
			//初始化 ConstData 系統
		//	Debug.Log(SystemLogFormat("開始初始化 Manager: " + typeof(ConstDataManager).ToString()));
			//ConstData只讀有註冊並設定lazyMode
			//		ConstDataManager.Instance.useUnregistedTables = false;
			ConstDataManager.Instance.isLazyMode = false;
			StartCoroutine(ConstDataManager.Instance.ReadDataStreaming("pcz/", Config.COMMON_DATA_NAMES));		
			Config.LoadConstData = true;
		}


		// UI Event
		// UIEventListener.Get(BackGroundObj).onClick += OnBackGroundClick;
	
		Grids = new cMyGrids ();
		StageData = new STAGE_DATA();
		EvtPool = new Dictionary< int , STAGE_EVENT >();			// add event id 
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
		SCENE_NAME scn = ConstDataManager.Instance.GetRow<SCENE_NAME> (  StageData.n_SCENE_ID );
		if (scn == null)
			return;
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
		// change bgm 
		GameSystem.PlayBGM ( scn.n_BGM );

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
		//==================	
	//	RunEvent( );

	
	}

	void OnDestroy()
	{
	}



	void OnCellClick(GameObject go)
	{
		UnitCell unit = go.GetComponent<UnitCell>() ;
		if( unit != null ){
			string str = string.Format( "CellOnClick( {0},{1}) " , unit.X() , unit.Y() );
			Debug.Log(str);

			// avoid re open
//			if( PanelManager.Instance.CheckUIIsOpening( "Panel_CMDUI" ) == false  )
			{
				GameObject obj = PanelManager.Instance.GetOrCreatUI( "Panel_CMDSYSUI" );
				if (obj != null) {
//				Vector3 vLoc = obj.transform.localPosition;
//				vLoc.x = UICamera.lastHit.point.x ; 
//				vLoc.y = UICamera.lastHit.point.y ; 
//				obj.transform.localPosition = vLoc;

				// set cmd list type
//					Panel_CmdUI cmdUI = obj.GetComponent<Panel_CmdUI>();
//					if( cmdUI != null )
//					{
//						cmdUI.CreateCMDList( ( _CMD_TYPE) 1 ) ; // Cmd list Type
//					}
				}
			}
		}

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
	// Check event
	bool CheckEvent( STAGE_EVENT evt )
	{
		cTextArray sCond = new cTextArray( );
		sCond.SetText( evt.s_CONDITION );
		// check all line . if one line success . this event check return true

		int nCol = sCond.GetMaxCol();
		for( int i= 0 ; i <nCol ; i++ )
		{
			if( CheckEventCondition( sCond.GetTextLine( nCol ) ) )
			{
				return true;
			}
		}
		return false;
	}

	bool CheckEventCondition( CTextLine line )
	{
		List<cTextFunc> funcList =line.GetFuncList();
		foreach( cTextFunc func in funcList )
		{
			if( func.sFunc == "ENEMYDEAD" )
			{
				if( ConditionEnemyDead( func.At(0) ) == false )
				{
					return false;
				}				
			}
			else if( func.sFunc == "ALLYDEAD"  )
			{
				if( ConditionAllyDead( func.At(0) ) == false )
				{
					return false;
				}				
			}
			else if( func.sFunc == "ROUNDSTART"  )
			{
				if( ConditionRoundStart( func.At(0) ) == false )
				{
					return false;
				}				
			}
			else if( func.sFunc == "ROUNDEND"  )
			{
				if( ConditionRoundEnd( func.At(0) ) == false )
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
	bool ConditionEnemyDead( int nID )
	{
		// 0 == all dead
		if(0 == nID){
			return (EnemyPool.Count==0);

		}
		// assign id
		if( EnemyPool.ContainsKey( nID ) ){
			return false;
		}
		return true;
	}

	bool ConditionAllyDead( int nID )
	{
		// 0 == all dead
		if(0 == nID){
			return (AllyPool.Count==0);
			
		}
		// assign id
		if( AllyPool.ContainsKey( nID ) ){
			return false;
		}
		return true;
	}

	bool ConditionRoundStart( int nID )
	{
		if( GameDataManager.Instance.nRoundStatus != 0 )
			return false;

		return (GameDataManager.Instance.nRound >=nID ) ;
	}

	bool ConditionRoundEnd( int nID )
	{
		if( GameDataManager.Instance.nRoundStatus != 1 )
			return false;
		return (GameDataManager.Instance.nRound >=nID);
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
		if( NextEvent == null )
		{
			// get next event to run
			foreach( KeyValuePair< int ,STAGE_EVENT > pair in EvtPool ) 
			{
				if( CheckEvent( pair.Value ) ){		// check if this event need run
					NextEvent = pair.Value ; 		// run in next loop
					EvtPool.Remove( pair.Key );		// remove key , never check it again
					
					EcecuteEvent();					// parser event to run
					break;							// run one event once time only
				}
			}
		}
		else  //run event
		{
			//NextLine();					// parser event to run
			if( IsNextEventCompleted() )
			{
				// clear event for next
				NextEvent = null;
				IsEventEnd = false;
			}
		}
		
	}

	void EcecuteEvent(  )
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
		NextLine();			// script to run
	}

	void NextLine()
	{
		if( IsEventEnd == true )
			return;
		//if( m_nFlowIdx  )
		CTextLine line = m_cScript.GetTextLine( m_nFlowIdx );
		if( line != null )
		{
			ParserScript( line );
			m_nFlowIdx++;
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
			if( func.sFunc == "ADDCHAR" )
			{


			}
			else if( func.sFunc == "TALK"  )
			{


			}

		}
	}
}
