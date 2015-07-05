﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;
//using Playcoo.Common;

public class MainUIPanel : BasicPanel {
	public const string Name = "Panel_MainUI";

	public GameObject StartButton;
	public GameObject LoadButton;
	public GameObject GalleryButton;
	public GameObject SetUpButton;

	void Awake(){
#if DEBUG && UNITY_EDITOR
		// Avoid Const data null for debug
#endif
		// UI Event
		UIEventListener.Get(StartButton).onClick += OnStartButtonClick;
		UIEventListener.Get(LoadButton).onClick += OnLoadButtonClick;
		UIEventListener.Get(GalleryButton).onClick += OnGalleryButtonClick;
		UIEventListener.Get(SetUpButton).onClick += OnSetUpButtonClick;

		// Game Event
		//GameEventManager.AddEventListener( "startgame" , OnStartGameEvent );

		// start bgm
		/*1.拿到特定的table
			DataTable table = ConstDataManager.Instance.GetTable( X );//		 可以傳Name或tableID
			foreach(ConstDataRow row in table.RowList)
		  2.ConstDataRow row = ConstDataManager.Instance.GetRow("SKILL", 1);	
		*/

		// 播放  mian BGM
		GameSystem.PlayBGM ( 1 );
//		DataRow row = ConstDataManager.Instance.GetRow("MUSIC", 1);
//		if( row != null )
//		{
//			string strFile = row.Field< string >("s_FILENAME");
//			if( !String.IsNullOrEmpty( strFile ))
//			{
//				string audioPath = ResourcesManager.GetAudioClipPath( AudioChannelType.BGM ,  strFile );
//				AudioManager.Instance.Play( AudioChannelType.BGM ,  audioPath );
//			}
//		}

		// Cal cache value again to avoid gamesystem ceatate with no UI
		UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);	
		if (mRoot != null) {
			MyTool.fScnRatio = (float)mRoot.activeHeight / Screen.height;
		}


	}

	// Use this for initialization
	void Start () {

	}

	// release game event 
	void OnDestroy()
	{
		UIEventListener.Get(StartButton).onClick -= OnStartButtonClick;
		UIEventListener.Get(LoadButton).onClick -= OnLoadButtonClick;
		UIEventListener.Get(GalleryButton).onClick -= OnGalleryButtonClick;
		UIEventListener.Get(SetUpButton).onClick -= OnSetUpButtonClick;

	//	GameEventManager.RemoveEventListener( "startgame" , OnStartGameEvent );

	}


	IEnumerator EnterStory( int nStoryID )
	{
		GameDataManager.Instance.nStoryID = nStoryID;
	
		PanelManager.Instance.OpenUI( "Panel_Loading");

		yield return  new WaitForEndOfFrame();

		PanelManager.Instance.OpenUI( StoryUIPanel.Name );
		yield return  new WaitForEndOfFrame();


		PanelManager.Instance.CloseUI( Name );  			// close main 
		yield break;

	}

	void OnStartButtonClick(GameObject go)
	{
		// When start button clicked do :
		Debug.Log("Start click ");
		// 回到第0 關
	//	GameDataManager.Instance.nStoryID = Config.StartStory; //回到第一關

	//	PanelManager.Instance.OpenUI( "Panel_Loading");

		StartCoroutine ( EnterStory( Config.StartStory ) );

		//StartCoroutine ( "EnterStory" );


		//GameObject obj = PanelManager.Instance.OpenUI( StoryUIPanel.Name );
	}

	void OnLoadButtonClick(GameObject go)
	{
		// When load button clicked do :
		Debug.Log("Load");
	}

	void OnGalleryButtonClick(GameObject go)
	{
		// When gallery button clicked do :
		Debug.Log("Gallery");
	}

	void OnSetUpButtonClick(GameObject go)
	{
		// When setup button clicked do :
		Debug.Log("SetUp");
	}

	// Game event 
	// this func never use
//	public void OnStartGameEvent(GameEvent evt)
//	{
//		Debug.Log("Startevent");
//		// setup global stage =1;
//
//		// 回到第0 關
//		GameDataManager.Instance.nStoryID = Config.StartStory;  //設定為 第一關
//		// open story panel 
//
//
//		GameObject obj = PanelManager.Instance.OpenUI( StoryUIPanel.Name );
//		if (obj != null) {
//			PanelManager.Instance.CloseUI( "Panel_MainUI" ); 
//		}
//
////			string s1 = "linetext( 1 );LineText(\t2)\nPopChar(3 ,4);PopChar( 4 , 5 ,6)。\nSysText(  \"test\" )。";
//		
////			char[] colChars = { ' ', '(', ')',';', ',', '\t' };
////			char[] rowChars = { '\n' };	
////			cTextArray txt = new cTextArray(rowChars , colChars );
////			txt.SetText( s1 ); // 將猜解各字串
////		string prePath = "Panel/Panel_char";
////		GameObject preObj = Resources.Load( prePath ) as GameObject;
////		if( preObj != null )
////		{
////			GameObject char1obj  = NGUITools.AddChild( obj , preObj );
////			if( char1obj != null )
////			{
////				Vector3 v = new Vector3( -150 , 50 ,0 );
////				char1obj.transform.localPosition = v;
////			}
//			
//			
//			
////			GameObject char2obj  = NGUITools.AddChild( obj , preObj );
////			if( char2obj != null )
////			{
////				Vector3 v = new Vector3( 50 , +150 ,0 );
////				char2obj.transform.localPosition = v;				
////			}
////		}
//
//
//	}

}
