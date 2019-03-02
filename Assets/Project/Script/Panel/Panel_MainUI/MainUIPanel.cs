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
	public GameObject GameEndButton;
	void Awake(){
#if DEBUG && UNITY_EDITOR
        // Avoid Const data null for debug
#endif
        //screen size
      //  Screen.SetResolution(Config.WIDTH, Config.HEIGHT, false);


		// UI Event
		UIEventListener.Get(StartButton).onClick += OnStartButtonClick;
		UIEventListener.Get(LoadButton).onClick += OnLoadButtonClick;
		UIEventListener.Get(GalleryButton).onClick += OnGalleryButtonClick;
		UIEventListener.Get(SetUpButton).onClick += OnSetUpButtonClick;
        UIEventListener.Get(GameEndButton).onClick += OnGameEndButtonClick;
        // Game Event
        //GameEventManager.AddEventListener( "startgame" , OnStartGameEvent );

        // start bgm
        /*1.拿到特定的table
			DataTable table = ConstDataManager.Instance.GetTable( X );//		 可以傳Name或tableID
			foreach(ConstDataRow row in table.RowList)
		  2.ConstDataRow row = ConstDataManager.Instance.GetRow("SKILL", 1);	
		*/


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



    }

	// set up startup 
	void OnEnable()
	{
		GameDataManager.Instance.ePhase = _SAVE_PHASE._STARTUP;

        // 先關閉音效

        // pre load FX
        

        // pre load music 
        GameSystem.PlayBGM(5); // 勝利 BGM


        // 播放  mian BGM
        GameSystem.PlayBGM ( 1 );

       

    }

	// Use this for initialization
	void Start () {

        // Cal cache value again to avoid gamesystem ceatate with no UI
        //UIRoot mRoot = NGUITools.FindInParents<UIRoot>(UICamera.currentCamera.gameObject);
        //if (mRoot != null)
        //{
        //    MyTool.fScnRatio = (float)mRoot.activeHeight / Screen.height;
        //}
        //Output the current screen window height in the console
        Debug.Log("Screen Height : " + Screen.height);
        Debug.Log("Screen Width : " + Screen.width);

    }

	// release game event 
	void OnDestroy()
	{
		UIEventListener.Get(StartButton).onClick -= OnStartButtonClick;
		UIEventListener.Get(LoadButton).onClick -= OnLoadButtonClick;
		UIEventListener.Get(GalleryButton).onClick -= OnGalleryButtonClick;
		UIEventListener.Get(SetUpButton).onClick -= OnSetUpButtonClick;
        UIEventListener.Get(GameEndButton).onClick -= OnGameEndButtonClick;
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
//		Debug.Log("Start click ");
        // 回到第0 關
        //	GameDataManager.Instance.nStoryID = Config.StartStory; //回到第一關

        //	PanelManager.Instance.OpenUI( "Panel_Loading");

        // clear all game data		

        GameObject obj = PanelManager.Instance.OpenUI(Panel_CreateName.Name );


        // StartGame();
        // 進入第一關
        //      GameDataManager.Instance.Initial();
        //      GameDataManager.Instance.ClearStorageUnit ();
        //GameDataManager.Instance.ResetStage ();

        //StartCoroutine ( EnterStory( Config.StartStory ) );



        GameSystem.BtnSound();
    }

    void OnLoadButtonClick(GameObject go)
	{
		// When load button clicked do :
	//	Debug.Log("Load");

		Panel_SaveLoad.OpenLoadMode ( _SAVE_PHASE._STARTUP );
        //cSaveData.Load (1, _SAVE_PHASE._STARTUP );
        GameSystem.BtnSound();
    }

	void OnGalleryButtonClick(GameObject go)
	{
        // When gallery button clicked do :
        //		Debug.Log("Gallery");
        GameSystem.BtnSound();
    }

	void OnSetUpButtonClick(GameObject go)
	{
        // When setup button clicked do :
        //	Debug.Log("SetUp");
        Panel_SystemSetting.OpenUI();
        GameSystem.BtnSound();
    }
    void OnGameEndButtonClick(GameObject go )
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif

    }

    public IEnumerator SaveLoading( cSaveData save )
	{
        //GameDataManager.Instance.nStoryID = nStoryID;
        //GameDataManager.Instance.nStageID = save.n_StageID;

        //PanelManager.Instance.OpenUI( "Panel_Loading");
        System.Threading.Thread.Sleep(1000);
        yield return  new WaitForEndOfFrame();

		if (save.ePhase == _SAVE_PHASE._MAINTEN) {
			Panel_Mainten panel  = MyTool.GetPanel<Panel_Mainten>( PanelManager.Instance.OpenUI ( Panel_Mainten.Name ) );
			yield return  new WaitForEndOfFrame();

			panel.RestoreBySaveData( save );


		} else if (save.ePhase == _SAVE_PHASE._STAGE) {

			PanelManager.Instance.OpenUI( Panel_StageUI.Name );  // don't run start() during open
//			Panel_StageUI.Instance.bIsRestoreData = true;
			yield return  new WaitForEndOfFrame();

			Panel_StageUI.Instance.RestoreBySaveData ( save );
		}
        System.Threading.Thread.Sleep(1000);
        yield return  new WaitForEndOfFrame();

		PanelManager.Instance.CloseUI( Name );
		cSaveData.SetLoading (false);

        //	PanelManager.Instance.CloseUI( "Panel_Loading");
        System.Threading.Thread.Sleep(1000);
        yield break;
		
	}

	public void LoadSaveGame( cSaveData save )
	{
		if (save  == null)
			return;

        Panel_Loading.StartLoad( save, Panel_Loading._LOAD_TYPE._SAVE_DATA );



        //    Loading = MyTool.GetPanel<Panel_Loading>(PanelManager.Instance.OpenUI("Panel_Loading"));
        //if (Loading != null)
        //{
        //    Loading.save = save;
        //}

        ;
    //    StartCoroutine (  SaveLoading( save  ) );
    //    PanelManager.Instance.CloseUI("Panel_Loading");
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


        PanelManager.Instance.CloseUI( Name );  			// close main 
        GameSystem.BtnSound();
    }

    public void StartGame()
    {
        // 進入第一關
        GameDataManager.Instance.Initial();
        GameDataManager.Instance.ClearStorageUnit();
        GameDataManager.Instance.ResetStage();

      //  StartCoroutine(EnterStory(Config.StartStory));
        GameSystem.PlaySound(157);

        Panel_Loading.StartLoad(null, Panel_Loading._LOAD_TYPE._STORY);
        PanelManager.Instance.CloseUI(Name);            // close this ui

    }
}
