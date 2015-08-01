using UnityEngine;
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

		cSaveData.Load (1, _SAVE_PHASE._STARTUP , this.gameObject );
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


	public IEnumerator SaveLoading( cSaveData save )
	{
		//GameDataManager.Instance.nStoryID = nStoryID;
		//GameDataManager.Instance.nStageID = save.n_StageID;
		
		PanelManager.Instance.OpenUI( "Panel_Loading");
		
		yield return  new WaitForEndOfFrame();
		
		if (save.ePhase == _SAVE_PHASE._MAINTEN) {
			PanelManager.Instance.OpenUI ( Panel_Mainten.Name );			
		} else if (save.ePhase == _SAVE_PHASE._STAGE) {

			PanelManager.Instance.OpenUI( Panel_StageUI.Name );  // don't run start() during open
//			Panel_StageUI.Instance.bIsRestoreData = true;
		}
		
		yield return  new WaitForEndOfFrame();
		
		if (save.ePhase == _SAVE_PHASE._STAGE) {// need wait a frame to load
				Panel_StageUI.Instance.RestoreBySaveData ( save );
				//yield return  new WaitForEndOfFrame ();
		}
		
	
		PanelManager.Instance.CloseUI( Name );  			// close main 

		cSaveData.SetLoading (false);
		yield break;
		
	}

	public void LoadSaveGame( cSaveData save )
	{
		if (save  == null)
			return;
		StartCoroutine (  SaveLoading( save  ) );

//		if (save.ePhase == _SAVE_PHASE._MAINTEN) {
//			
//			PanelManager.Instance.OpenUI ( Panel_Mainten.Name );
//
//		} else if (save.ePhase == _SAVE_PHASE._STAGE) {
//			
//			StartCoroutine ( LoadStage( cSaveData save) );
//			//Panel_StageUI.Instance.RestoreBySaveData ();
//
//		}


//		PanelManager.Instance.CloseUI( Name );  			// close main 

	}
}
