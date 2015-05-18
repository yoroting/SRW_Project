using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MYGRIDS;
using MyClassLibrary;			// for parser string

public class Panel_StageUI : MonoBehaviour {

	public GameObject BackGroundObj; // back ground
	public GameObject TilePlaneObj; // plane of all tiles sprite

	public bool ReadCompleted ;

	cMyGrids	Grids;				// main grids
	STAGE_DATA	StageData;

	// drag canvas limit								
	float fMinOffX ;
	float fMaxOffX ;
	float fMinOffY ;
	float fMaxOffY ;

	Dictionary< int , STAGE_EVENT > EvtPool;			// add event id 
	STAGE_EVENT						NextEvent;
	private cTextArray 				m_cScript;			// 劇本 腳本集合
	private int						m_nFlowIdx;			// 腳本演到哪段
	bool							IsEventEnd;			// 

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
		ReadCompleted = false;
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

		// 
		ReadCompleted = true;

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
		RunEvent( );

	
	}

	void OnDestroy()
	{
		ReadCompleted = false;
	}



	void OnCellClick(GameObject go)
	{
		UnitCell unit = go.GetComponent<UnitCell>() ;
		if( unit != null ){
			string str = string.Format( "CellOnClick( {0},{1}) " , unit.X() , unit.Y() );
			Debug.Log(str);
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

	bool CheckEvent( STAGE_EVENT evt )
	{

		return false;
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
		//if( m_nFlowIdx  )
		cTextArray.CTextLine line = m_cScript.GetTextLine( m_nFlowIdx );
		if( line != null )
		{
			ParserScript( line );
			m_nFlowIdx++;
		}
		else{
			IsEventEnd = true; // all script complete
		}
	}

	bool IsEventCompleted()
	{


		return true ;
	}

	void RunEvent(  )
	{
		if( NextEvent == null )
		{
			foreach( KeyValuePair< int ,STAGE_EVENT > pair in EvtPool ) 
			{
				if( CheckEvent( pair.Value ) ){
					NextEvent = pair.Value ; 		// run in next loop
					EvtPool.Remove( pair.Key );		// remove key , never check it again

					EcecuteEvent();					// parser event to run
				}
			}
		}
		else  //run event
		{
			//NextLine();					// parser event to run
			if( IsEventCompleted() )
			{
				// clear event for next
				NextEvent = null;
				IsEventEnd = false;
			}
		}

	}
	void ParserScript( cTextArray.CTextLine line )
	{
		for( int i = 0 ; i < line.GetRowNum() ; i++ )
		{
			string s = line.GetString( i ).ToUpper();
			if( s == "ADDCHAR" )
			{

			}
			else if( s == "TALKEVENT" )
			{
				string sp= line.GetString( ++i );	if( sp == null ) return; //  null = error 
				//List<string> lst = cTextArray.GetParamLst( sp );
				
				// only 1 par default value
				int nID = 0;
				nID = int.Parse( sp.Trim() );

				// 
			}
		}
	}
}
