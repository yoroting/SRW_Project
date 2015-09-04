﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;			// for parser string

public class Panel_Talk : MonoBehaviour {
	public const string Name = "Panel_Talk";

	public GameObject Tex_BackGround;
//	public GameObject TalkWindow_Up;
//	public GameObject TalkWindow_Down;
	public GameObject Skip_Button;

	public GameObject AVG_Obj;			//右邊人像
	public GameObject NameObj;				// 名稱物件

	public GameObject TalkWindow_new;		//對話框框

	// AVG 用的新物件
	private Dictionary<int, SRW_AVGObj> m_idToFace; // 臉部管理
	public SRW_TextBox  TalkWindow;

	private int nLastPopType;			// for auto pop box

	STAGE_TALK m_cStageTalk;				// talk data class

	private int m_nTalkIdx;					// 文字目前在哪一行



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

		if( TalkWindow_new == null ){
			Debug.LogError( " Err! Talk UI no Talk window");
		}

		TalkWindow = TalkWindow_new.GetComponent<SRW_TextBox>();
		m_idToFace = new Dictionary<int, SRW_AVGObj > ();

		m_cScript = new cTextArray ();

		nTweenObjCount = 0;

		UIEventListener.Get(this.gameObject).onClick += OnPanelClick; // for trig next line
		UIEventListener.Get(Skip_Button).onClick += OnSkipClick; // for trig next line

		// templete
//		TalkWindow_Up.SetActive( false );
//		TalkWindow_Down.SetActive( false );

		// hide talk window
		TalkWindow_new.SetActive( false );
		NameObj.SetActive ( false );
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
	
		if(AVG_Obj != null ) {
			AVG_Obj.SetActive( false );
		}
		GameDataManager.Instance.nTalkID = 1;
	}
	// Use this for initialization
	void Start () {
//		TalkWindow_Up.SetActive (false);
//		TalkWindow_Down.SetActive (false);
		//SetScript ( GameDataManager.Instance.nTalkID ); 
	}

	void Clear()
	{
		if( TalkWindow != null ){
			TalkWindow.gameObject.SetActive( false );
		}

		//AVG_FaceR.i
		if( m_idToFace != null ){
			foreach( KeyValuePair<int , SRW_AVGObj > pair in m_idToFace )
			{
				if( pair.Value != null )
				{
					NGUITools.Destroy( pair.Value.gameObject );	
				}
			}
			m_idToFace.Clear();
		}
	}


	void OnEnable () {
		// clear all
		Clear ();
		if (Tex_BackGround != null) {
			Tex_BackGround.SetActive( false );
		}
		SetScript ( GameDataManager.Instance.nTalkID ); 

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
		if( (Panel_StageUI.Instance!=null) && (Panel_StageUI.Instance.IsAnyActionRunning()==true) ) // wait all tween / fx / textbox / battle msg finish / unit move
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
			TalkWindow.OnTextBoxClick( TalkWindow.gameObject  );
		}
	}
	void OnSkipClick(GameObject go)
	{
		if (m_bIsClosing == true)
			return;


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
		if ( (Panel_StageUI.Instance!= null) && (Panel_StageUI.Instance.bIsStageEnd == true) ) {
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
	public SRW_AVGObj SelAVGObjByType( int nType , int nCharID  )
	{
		if (m_idToFace.ContainsKey (nType) == false) {
			GameObject obj = ResourcesManager.CreatePrefabGameObj (this.gameObject, "Prefab/SRW_AVGObj");
			if( obj )
			{
				// insert to map
				NGUITools.SetActive( obj , true );
				
					// setup Type
				SRW_AVGObj boxobj =  obj.GetComponent<SRW_AVGObj>( );
				if( boxobj )
				{
					boxobj.ChangeFace( nCharID );
					boxobj.ChangeLayout( nType );
				}
				m_idToFace.Add( nType , boxobj );
				nLastPopType = nType;
				return boxobj ;
			}
			
		}
		else {
			return  m_idToFace[ nType ];
			//m_idToObj.TryGetValue( nType , out obj  );
		}
		
		
		return null;
	}
	
	public SRW_AVGObj  SelAVGObjByCharID( int nCharid )
	{
		//
		foreach( KeyValuePair < int ,SRW_AVGObj > pair in m_idToFace )
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
		if( m_idToFace.ContainsKey (0) == false )
		{
			nType = 0;
		}
		else if( m_idToFace.ContainsKey (1) == false )
		{
			nType = 1;
		}
		else{ // auto destory 0 . and create			
			nType = (nLastPopType==0)?1:0; 
			CloseBox( nType , 0 );
		}
		nLastPopType = nType;
		// create plane
		GameObject obj = ResourcesManager.CreatePrefabGameObj (this.gameObject, "Prefab/SRW_AVGObj");
		if( obj )
		{
			// insert to map
			NGUITools.SetActive( obj , true );
			
			// setup Type
			SRW_AVGObj boxobj =  obj.GetComponent<SRW_AVGObj>( );
			if( boxobj )
			{
				boxobj.ChangeFace( nCharid );
				boxobj.ChangeLayout( nType );
			}
			m_idToFace.Add( nType , boxobj );			
			return boxobj ;
		}		
		return null;
	}

	public void OnTweenAlphaEnd()
	{

	}

	public void SetName( int nCharID )
	{
		if( nCharID == 0 ){
			NameObj.SetActive( false );
			return ;
		}

		NameObj.SetActive( true );
		string name = MyTool.GetCharName( nCharID );
		UILabel lbl = NameObj.GetComponentInChildren< UILabel >();
		if( lbl != null ){
			lbl.text = name;
		}


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


	public bool IsAllEnd()
	{
		// check both box is end
		if( TalkWindow.IsEnd() == false )
			return false;

		// face anime
		foreach( KeyValuePair < int ,SRW_AVGObj > pair in m_idToFace )
		{
			if( pair.Value != null )
			{
				if( pair.Value.IsEnd() == false  )
				{
					return false;
				}
			}
		}

		// stage is not complete
		if (Panel_StageUI.Instance != null ) {
			if( Panel_StageUI.Instance.IsAnyActionRunning() ){
				return false;
			}
		
		}

		return true;
	}

	public void SpeakAll( bool bSpeak )
	{
		foreach( KeyValuePair < int ,SRW_AVGObj> pair in m_idToFace )
		{
			if( pair.Value != null )
			{
				pair.Value.Speak( bSpeak );
			}
		}
	}
	
	public void CharSay( int nCharID , int nSayTextID )
	{
		SpeakAll( false );

		SetName( nCharID ); // name 
		SRW_AVGObj avgobj = SelAVGObjByCharID( nCharID );// face 
		if( avgobj!= null ){
			avgobj.Speak( true );
		}

		//SRW_TextBox obj = SelTextBoxObjByCharID (nCharID) ;
		SRW_TextBox obj =  TalkWindow;// SelTextBoxObjByCharID (nCharID) ;
		if (obj == null)
			return;

		obj.gameObject.SetActive( true );
		obj.ClearText(); // clear text first
		string s = GameSystem.GetTalkText ( nSayTextID );
		string sText = "";
		if (string.IsNullOrEmpty (s)) {
			sText = string.Format("CharSay null text in textid{0} ", nSayTextID);
		} else {

			sText = s.Replace ( "$F" , Config.PlayerFirst ); // replace player name
			sText = sText.Replace ( "$N" , Config.PlayerName ); // replace player name			

		}
		obj.ClearText();
		obj.AddText(sText);
	}

	public void CharEnd( int nCharID  )
	{
		 // 0 = close all
		if (nCharID == 0) {
			foreach( KeyValuePair < int ,SRW_AVGObj> pair in m_idToFace )
			{
				if( pair.Value != null )
				{
					NGUITools.Destroy( pair.Value.gameObject );
				}
			}
			m_idToFace.Clear();

			return;
		}

		foreach( KeyValuePair < int ,SRW_AVGObj > pair in m_idToFace )
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
		SRW_AVGObj obj = SelAVGObjByType (nType , nCharID ) ;
//		if (obj) {	
//			obj.ChangeFace (nCharID);
//		}

	}
	public void CharShake( int nCharID  )
	{
		foreach( KeyValuePair < int ,SRW_AVGObj > pair in m_idToFace )
		{
			if( pair.Value != null )
			{
				if( pair.Value.CharID == nCharID )
				{
					pair.Value.SetShake();
				}
			}
		}
	}

	public void CharDead( int nCharID  )
	{
		foreach( KeyValuePair < int ,SRW_AVGObj > pair in m_idToFace )
		{
			if( pair.Value != null )
			{
				if( pair.Value.CharID == nCharID )
				{		
					pair.Value.SetDead();
				}
			}
		}

	}

	public void CloseBox( int nType , int nCloseType )
	{
		// this may create and destory
		if( m_idToFace.ContainsKey( nType ) )
		{
			NGUITools.Destroy( m_idToFace[nType].gameObject );
			m_idToFace.Remove( nType );
		}

	}
	// widget func


}
