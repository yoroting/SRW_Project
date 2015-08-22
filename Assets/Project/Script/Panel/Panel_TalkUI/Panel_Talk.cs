using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;			// for parser string

public class Panel_Talk : MonoBehaviour {
	public const string Name = "Panel_Talk";

	public GameObject Tex_BackGround;
	public GameObject TalkWindow_Up;
	public GameObject TalkWindow_Down;
	public GameObject Skip_Button;


	public GameObject TalkWindow_new;
	//public GameObject StartButton;
	private Dictionary<int, SRW_TextBox> m_idToObj; // 管理 產生的 Prefab 物件 

	private int nLastPopType;			// for auto pop box

	STAGE_TALK m_cStageTalk;				// talk data class

//	public bool bSkipMode;					// skip mode will change some behavior
	private int m_nTalkIdx;					// 文字目前在哪一行
	//private List<string> m_cTextList;		// 內容集合


	private int 	  m_nScriptIdx;			// cur script index
	private cTextArray m_cScript;			// 腳本集合

	// script pause;
	bool m_bClickScript;
	bool m_bIsClosing ;

	// tween check
	private int nTweenObjCount;
	// Declare a delegate type for processing a book:
	public  void OnTweenNotifyEnd( )
	{
		if( --nTweenObjCount < 0 )
		{
			nTweenObjCount = 0;
		}
	}


	void Awake(){
		m_idToObj = new Dictionary<int, SRW_TextBox > ();
	//	m_cTextList = new List<string> ();
		m_cScript = new cTextArray ();

		nTweenObjCount = 0;

		UIEventListener.Get(this.gameObject).onClick += OnPanelClick; // for trig next line
		UIEventListener.Get(Skip_Button).onClick += OnSkipClick; // for trig next line

		// templete
		TalkWindow_Up.SetActive( false );
		TalkWindow_Down.SetActive( false );
	//	TalkWindow_new.SetActive( false );
	//	m_idToObj.Add(  0 , TalkWindow_Up );
	//	m_idToObj.Add(  1 , TalkWindow_Down );
		// for fast debug 
		//ConstDataManager.Instance.isLazyMode = false;
		//StartCoroutine(ConstDataManager.Instance.ReadDataStreaming("pcz/", Config.COMMON_DATA_NAMES));

		// cmd event
		GameEventManager.AddEventListener(  TalkSayEvent.Name , OnTalkSayEvent );
		GameEventManager.AddEventListener(  TalkSetCharEvent.Name , OnTalkSetCharEvent );
		GameEventManager.AddEventListener(  TalkSayEndEvent.Name , OnTalkSayEndEvent );

		GameEventManager.AddEventListener(  TalkBackGroundEvent.Name , OnTalkBackGroundEvent );
		GameEventManager.AddEventListener(  TalkDeadEvent.Name , OnTalkDeadEvent );
		GameEventManager.AddEventListener(  TalkShakeEvent.Name , OnTalkShakeEvent );

	//	CharSay( 1  , 9 );
		// cause some mob pop in talk event. player can't skip talk event to avoid bug
	//	NGUITools.SetActive( Skip_Button , false );
//#if UNITY_EDITOR
	//	NGUITools.SetActive( Skip_Button , true );	
//#endif 

	}
	// Use this for initialization
	void Start () {
//		TalkWindow_Up.SetActive (false);
//		TalkWindow_Down.SetActive (false);
		//SetScript ( GameDataManager.Instance.nTalkID ); 
	}

	void Clear()
	{
		foreach( KeyValuePair<int , SRW_TextBox > pair in m_idToObj )
		{
			if( pair.Value != null )
			{
				NGUITools.Destroy( pair.Value.gameObject );	
			}
		}
		m_idToObj.Clear ();

	}


	void OnEnable () {
		// clear all
		Clear ();
		if (Tex_BackGround != null) {
			Tex_BackGround.SetActive( false );
		}
		SetScript ( GameDataManager.Instance.nTalkID ); 

		//MyTool.SetAlpha (this.gameObject, 1.0f);
		TweenAlpha tw = TweenAlpha.Begin<TweenAlpha>( this.gameObject , 0.2f );
		if (tw != null) {
			//MyTool.SetAlpha (TilePlaneObj, 0.0f);
			tw.from = 0.0f;
			tw.to = 1.0f;
			//tw.onFinished = null ;
		}

		m_bIsClosing = false;
		//bSkipMode = false;
	}

	// Update is called once per frame
	void Update () {

		if (m_bIsClosing)
			return;

		// pause when unit is animate
		if( Panel_StageUI.Instance.IsAnyActionRunning() == true ) // wait all tween / fx / textbox / battle msg finish / unit move
				return;							// don't check event run finish here.

		//if (ActionManager.Instance.Run () == true)
		//		return;


		if (IsAllEnd () == false)
			return;
		// prcess script
		if (m_bClickScript) {
			NextLine ();
		}
	}

	void OnDestroy () {
		// cmd event
		GameEventManager.RemoveEventListener(  TalkSayEvent.Name , OnTalkSayEvent );
		GameEventManager.RemoveEventListener(  TalkSetCharEvent.Name , OnTalkSetCharEvent );
		GameEventManager.RemoveEventListener(  TalkSayEndEvent.Name , OnTalkSayEndEvent );

		GameEventManager.RemoveEventListener(  TalkBackGroundEvent.Name , OnTalkBackGroundEvent );
		GameEventManager.RemoveEventListener(  TalkDeadEvent.Name , OnTalkDeadEvent );
		GameEventManager.RemoveEventListener(  TalkShakeEvent.Name , OnTalkShakeEvent );


	}
	// Base Panel click
	void OnPanelClick(GameObject go)
	{
		if (IsAllEnd())
		{
			m_bClickScript = true; // go next script
		}
		else{
			// don't change script. only text box go next line
			foreach( KeyValuePair<int, SRW_TextBox> pair in m_idToObj  )
			{
				pair.Value.OnTextBoxClick( pair.Value.gameObject ) ;

			}
			//private Dictionary<int, SRW_TextBox> m_idToObj; // 管理 產生的 Prefab 物件

		}
	}
	void OnSkipClick(GameObject go)
	{
		//if (IsAllEnd())
		Panel_StageUI.Instance.m_bIsSkipMode = true;

		while( m_nScriptIdx< m_cScript.GetMaxCol ())
		{
			NextLine ( ); // 
		}
		EndTalk();

		Panel_StageUI.Instance.m_bIsSkipMode = false;
	}

	// close talk panel
	void EndTalk()
	{
		// if stage is end .. open main ten ui
		if (Panel_StageUI.Instance.bIsStageEnd == true) {
			PanelManager.Instance.OpenUI ( Panel_Mainten.Name );
		}			 

		TweenAlpha tw = TweenAlpha.Begin<TweenAlpha>( this.gameObject , 1.0f );
		if (tw != null) {
			//MyTool.SetAlpha (TilePlaneObj, 0.0f);
			tw.from = 1.0f;
			tw.to = 0.0f;
			MyTool.TweenSetOneShotOnFinish( tw , EndTalkFinish );
		}

		m_bIsClosing = true;
	}

	void EndTalkFinish()
	{
		if (m_bIsClosing) {
			m_bIsClosing = false;
			PanelManager.Instance.CloseUI (Panel_Talk.Name);
		}
	}

	public SRW_TextBox SelTextBoxObjByType( int nType )
	{
		if (m_idToObj.ContainsKey (nType) == false) {
			GameObject obj = ResourcesManager.CreatePrefabGameObj (this.gameObject, "Prefab/SRW_TEXTBOX");
			if( obj )
			{
				// insert to map
				NGUITools.SetActive( obj , true );
				
//				m_idToObj.Add( nType , obj );

				// setup Type
				SRW_TextBox boxobj =  obj.GetComponent<SRW_TextBox>( );
				if( boxobj )
				{
					boxobj.ChangeLayout( nType );
				}
				m_idToObj.Add( nType , boxobj );
				nLastPopType = nType;
				return boxobj ;
			}

		}
		else {
			return  m_idToObj[ nType ];
			//m_idToObj.TryGetValue( nType , out obj  );
		}


		return null;
	}

	public SRW_TextBox  SelTextBoxObjByCharID( int nCharid )
	{
		//
		foreach( KeyValuePair < int ,SRW_TextBox > pair in m_idToObj )
		{
			if( pair.Value != null )
			{
				if( pair.Value.CharID == nCharid )
				{
					return pair.Value;
				}
			}
		}
		// if this is not exist. create new
		int nType =0;
		if( m_idToObj.ContainsKey (0) == false )
		{
			nType = 0;
		}
		else if( m_idToObj.ContainsKey (1) == false )
		{
			nType = 1;
		}
		else{ // auto destory 0 . and create

			nType = (nLastPopType==0)?1:0; 
			CloseBox( nType , 0 );
		}
		nLastPopType = nType;
		// create plane
		GameObject obj = ResourcesManager.CreatePrefabGameObj (this.gameObject, "Prefab/SRW_TEXTBOX");
		if( obj )
		{
			// insert to map
			NGUITools.SetActive( obj , true );
			
			//m_idToObj.Add( nType , obj );
			
			// setup Type
			SRW_TextBox boxobj =  obj.GetComponent<SRW_TextBox>( );
			if( boxobj )
			{
				boxobj.ChangeLayout( nType );
				boxobj.ChangeFace( nCharid );
			}
			m_idToObj.Add( nType , boxobj );
			
			return boxobj ;
		}

		return null;
	}
//	public void SetTextBoxActive( int nType , bool bActive )
//	{
//		if (nType == 0) {
//			TalkWindow_Up.SetActive( bActive );
//		}
//		else if (nType == 1) {
//			TalkWindow_Down.SetActive( bActive );
//		}
//	}

	public void OnTweenAlphaEnd()
	{

	}

	public void SetBackground( int nSceneID )
	{
		if (Tex_BackGround == null)
			return;

		SCENE_NAME scene = ConstDataManager.Instance.GetRow<SCENE_NAME> ( nSceneID );
		if (scene == null)
			return;


		NGUITools.SetActive( Tex_BackGround ,  true );
	
		string url = "Art/BG/" + scene.s_SCNEN_BACK;

		Texture t= Resources.Load( url , typeof(Texture) ) as Texture; ;

		UITexture tex = Tex_BackGround.GetComponent<UITexture>();
		tex.mainTexture = t;				
		//tex.MakePixelPerfect();

		TweenAlpha twA = TweenAlpha.Begin<TweenAlpha> (Tex_BackGround , 1.0f); 
		
	
		if( twA )
		{
			twA.from = 0.0f;
			twA.to =  1.0f;
			twA.SetOnFinished( OnTweenAlphaEnd );

		}

		Tex_BackGround.SetActive( true );



	}


	public void SetScript( int nScriptID )
	{
		m_nScriptIdx = 0; // current execute script

		 m_cStageTalk = ConstDataManager.Instance.GetRow<STAGE_TALK> ( nScriptID );
		if (m_cStageTalk == null)
			return;

		// change Back Tex
		if ( m_cStageTalk.n_SCENE_ID > 0 ) 
		{
			SetBackground( m_cStageTalk.n_SCENE_ID );
			// load texture of sceneID

		}

		// change BGM
		if (m_cStageTalk.n_TALK_BGM > 0) {
			GameSystem.PlayBGM( m_cStageTalk.n_TALK_BGM );
		}

		m_nScriptIdx = 0;

		m_cScript.SetText ( m_cStageTalk.s_CONTEXT );
		// for test
		//m_cScript.SetText( "SETCHAR(0,2);SAY(0,1)\nSETCHAR(1,1);SAY(1,2)\nSAY(0,3)\nSAY(1,4)\nSAY(0,5)\nCLOSE(0,0)\nSAY(1,6)\nSETCHAR(0,20);SAY(0,7)\nSAY(1,8)\nSAY(1,9)");
		//m_cScript.SetText( "SETCHAR(1,1);SAY(1,9)");
		//m_cScript.SetText( "SAY(0,3)");
		//m_cScript.SetText( "SAY(1,9)\nCLOSE(1,0)");
		// need get script for const data
		NextLine();
	}

	// script go next line
	public void NextLine( )
	{
		if (m_nScriptIdx >= m_cScript.GetMaxCol ())
		{
			EndTalk();
			return;
		}

		//ParserScript ( m_cScript.GetTextLine( m_nScriptIdx++ )  );
		MyScript.Instance.ParserScript ( m_cScript.GetTextLine( m_nScriptIdx++ )   );

		m_bClickScript = false;
	}

	// talk 
	void OnTalkSayEvent( GameEvent evt )
	{
		TalkSayEvent Evt = evt as TalkSayEvent;
		if (Evt == null)
			return;
		CharSay( Evt.nChar  , Evt.nSayID );


		// find obj to move
		Panel_unit unit = Panel_StageUI.Instance.GetUnitByCharID ( Evt.nChar );
		if (unit != null) {
			Panel_StageUI.Instance.MoveToGameObj (unit.gameObject, false);
		}
//		SetChar( Evt.nType , Evt.nChar );
//		Say( Evt.nType , Evt.nSayID );

	}

	void OnTalkSetCharEvent( GameEvent evt )
	{
		TalkSetCharEvent Evt = evt as TalkSetCharEvent;
		if (Evt == null)
			return;
		SetChar( Evt.nType , Evt.nChar );

	}


	// sayend
	void OnTalkSayEndEvent( GameEvent evt )
	{
		TalkSayEndEvent Evt = evt as TalkSayEndEvent;
		if (Evt == null)
			return;

		CharEnd( Evt.nChar );
	}

	// set back ground
	void OnTalkBackGroundEvent( GameEvent evt )
	{
		TalkBackGroundEvent Evt = evt as TalkBackGroundEvent;
		if (Evt == null)
			return;
		this.SetBackground ( Evt.nBackGroundID );
	}
	// set talk dead
	void OnTalkDeadEvent( GameEvent evt )
	{
		TalkDeadEvent Evt = evt as TalkDeadEvent;
		if (Evt == null)
			return;
		this.CharDead ( Evt.nChar );
	}
	void OnTalkShakeEvent( GameEvent evt )
	{
		TalkShakeEvent Evt = evt as TalkShakeEvent;
		if (Evt == null)
			return;
		this.CharShake ( Evt.nChar  );
	}

//	void ParserScript( CTextLine line )
//	{
//		//m_cTextList.Clear();
//		//m_nTextIdx = 0 ; // change text 
//		List<cTextFunc> funcList =line.GetFuncList();
//		foreach( cTextFunc func in funcList )
//		{
//			if( func.sFunc == "SAY" )
//			{
//				Say( func.At(0), func.At(1) );
//			}
//			else if( func.sFunc == "SETCHAR" )
//			{
//				SetChar( func.At(0), func.At(1) );
//			}		
//			else if( func.sFunc == "CHANGEBACK") 
//			{
//				
//			}
//			else if( func.sFunc  == "CLOSE") 
//			{
//				CloseBox( func.At(0), func.At(1) );
//			}
//			else if( func.sFunc  == "BGM") 
//			{
//				int id = func.At(0);
//				GameSystem.PlayBGM( id ); 
//				//CloseBox( func.At(0), func.At(1) );
//			}
//			// stage event
//			else if( func.sFunc  == "STAGEBGM") 
//			{
//				GameEventManager.DispatchEvent ( new StageBGMEvent()  );
//
//			}
//			// pop unit in stage
//			else if( func.sFunc  == "POPCHAR") 
//			{
//				int charid = func.At( 0 );
//				StagePopCharEvent evt = new StagePopCharEvent ();
//				evt.nCharID = func.At( 0 );
//				evt.nX		= func.At( 1 );
//				evt.nY		= func.At( 2 );
//				GameEventManager.DispatchEvent ( evt );
//			}
//			else if( func.sFunc  == "POPMOB") 
//			{
//				int charid = func.At( 0 );
//				StagePopMobEvent evt = new StagePopMobEvent ();
//				evt.nCharID = func.At( 0 );
//				evt.nX		= func.At( 1 );
//				evt.nY		= func.At( 2 );
//				GameEventManager.DispatchEvent ( evt );
//			}
//			else if( func.sFunc  == "POPGROUP")  //  pop a group of mob
//			{
//
//			}
//			else if( func.sFunc  == "DELCHAR") 
//			{
//				int charid = func.At( 0 );
//				StageDelCharEvent evt = new StageDelCharEvent ();
//				evt.nCharID = func.At( 0 );
//				GameEventManager.DispatchEvent ( evt );
//			}
//			else if( func.sFunc  == "DELMOB") 
//			{
//				int charid = func.At( 0 );
//				StageDelMobEvent evt = new StageDelMobEvent ();
//				evt.nCharID = func.At( 0 );
//				GameEventManager.DispatchEvent ( evt );
//			}
//			else{
//				// error 
//
//				Debug.Log( string.Format( "Error-Can't find script func '{0}'" , func.sFunc ) );
//			}
//		}
//
//
//
//	}

	public bool IsAllEnd()
	{
		// check both box is end
		foreach( KeyValuePair<int , SRW_TextBox > pair in m_idToObj )
		{
			SRW_TextBox pBox = pair.Value;
			if( pBox && pBox.IsEnd() == false )
			{
					return false;
			}
		}
		// stage is not complete
		if (Panel_StageUI.Instance != null ) {
			if( Panel_StageUI.Instance.IsAnyActionRunning() ){
				return false;
			}
		
		}


		// cehck all tween is end
//		SRW_TextBox obj1 = SelTextBoxObjByType (1) ;
//		if (obj1 != null) {
//			if( obj1.IsEnd() == false )
//				return false;
//		}

		return true;
	}

	public void Say( int nType , int nSayTextID )
	{
	//	SetTextBoxActive ( nType , true ); // need active first to awake() to do some thing

		SRW_TextBox obj = SelTextBoxObjByType (nType) ;
		if (obj == null)
			return;

		// get Text form talk_text
		obj.ClearText(); // clear text first

		string s = GameSystem.GetTalkText ( nSayTextID );
		string sText = s.Replace ( "$F" , Config.PlayerFirst ); // replace player name
				sText = sText.Replace ( "$N" , Config.PlayerName ); // replace player name



		obj.AddText(sText);
		//SRW_TextBox pBox = obj.GetComponent<SRW_TextBox>();
		//if (pBox) {
		//	pBox.AddText (sText);
		//}
	}
	public void CharSay( int nCharID , int nSayTextID )
	{
		SRW_TextBox obj = SelTextBoxObjByCharID (nCharID) ;
		if (obj == null)
			return;
		obj.ClearText(); // clear text first
		string s = GameSystem.GetTalkText ( nSayTextID );
		string sText = "";
		if (string.IsNullOrEmpty (s)) {
			sText = string.Format("CharSay null text in textid{0} ", nSayTextID);
		} else {

			sText = s.Replace ( "$F" , Config.PlayerFirst ); // replace player name
			sText = sText.Replace ( "$N" , Config.PlayerName ); // replace player name			

		}
		obj.AddText(sText);
	}

	public void CharEnd( int nCharID  )
	{
		 // 0 = close all
		if (nCharID == 0) {
			foreach( KeyValuePair < int ,SRW_TextBox > pair in m_idToObj )
			{
				if( pair.Value != null )
				{
					NGUITools.Destroy( pair.Value.gameObject );
				}
			}
			m_idToObj.Clear();
			return;
		}

		foreach( KeyValuePair < int ,SRW_TextBox > pair in m_idToObj )
		{
			if( pair.Value != null )
			{
				if( pair.Value.CharID == nCharID )
				{
					//return pair.Value;
					CloseBox( pair.Key , 0 );
					return ;
				}
			}
		}
	}

	public void SetChar( int nType , int nCharID )
	{
		//SetTextBoxActive ( nType , true ); // need active first to awake() to do some thing
		
		//SRW_TextBox obj = SelTextBoxObjByType (nType) ;
		SRW_TextBox obj = SelTextBoxObjByType (nType) ;
		if (obj) {
			// get Text form talk_text
			//SRW_TextBox pBox = obj.GetComponent<SRW_TextBox> ();
			//if (pBox) {
			obj.ChangeFace (nCharID);
			//}
		}
		
	}
	public void CharShake( int nCharID  )
	{
		foreach( KeyValuePair < int ,SRW_TextBox > pair in m_idToObj )
		{
			if( pair.Value != null )
			{
				if( pair.Value.CharID == nCharID )
				{
					//return pair.Value;
					// CloseBox( pair.Key , 0 );
					pair.Value.SetShake();
				}
			}
		}
		
	}

	public void CharDead( int nCharID  )
	{
		foreach( KeyValuePair < int ,SRW_TextBox > pair in m_idToObj )
		{
			if( pair.Value != null )
			{
				if( pair.Value.CharID == nCharID )
				{
					//return pair.Value;
					// CloseBox( pair.Key , 0 );
					pair.Value.SetDead();
				}
			}
		}
	}

	public void CloseBox( int nType , int nCloseType )
	{
		SRW_TextBox obj = SelTextBoxObjByType (nType) ;
		if (obj) {
			NGUITools.Destroy( obj.gameObject );
			m_idToObj.Remove( nType );
		
		}
	}
	// widget func


}
