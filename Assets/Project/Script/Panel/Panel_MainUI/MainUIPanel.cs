using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;

public class MainUIPanel : BasicPanel {

	public GameObject StartButton;
	public GameObject LoadButton;
	public GameObject GalleryButton;
	public GameObject SetUpButton;

	void Awake(){
		// UI Event
		UIEventListener.Get(StartButton).onClick += OnStartButtonClick;
		UIEventListener.Get(LoadButton).onClick += OnLoadButtonClick;
		UIEventListener.Get(GalleryButton).onClick += OnGalleryButtonClick;
		UIEventListener.Get(SetUpButton).onClick += OnSetUpButtonClick;

		// Game Event
		GameEventManager.AddEventListener( "startgame" , OnStartGameEvent );

	}

	// Use this for initialization
	void Start () {

	}

	// release game event 
	void OnDestroy()
	{
		GameEventManager.RemoveEventListener( "startgame" , OnStartGameEvent );

	}


	void OnStartButtonClick(GameObject go)
	{
		// When start button clicked do :
		Debug.Log("Start click ");
		GameEventManager.DispatchEventByName( "startgame"  , 1 );  



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
	public void OnStartGameEvent(GameEvent evt)
	{
		Debug.Log("Startevent");
		// open story panel 
		GameObject obj = PanelManager.Instance.GetOrCreatUI( "Panel_StoryUI" );
		if (obj != null) {
			PanelManager.Instance.CloseUI( "Panel_MainUI" );

		}
			string s1 = "linetext( 1 );LineText(\t2)\nPopChar(3 ,4);PopChar( 4 , 5 ,6)。\nSysText(  \"test\" )。";
		
			char[] colChars = { ' ', '(', ')',';', ',', '\t' };
			char[] rowChars = { '\n' };	
			cTextArray txt = new cTextArray(rowChars , colChars );
			txt.SetText( s1 ); // 將猜解各字串

	}

}
