using UnityEngine;
using Playcoo.Common;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameSystem : MonoBehaviour {

	// 目前在進行那個模式
	public enum eStageStatus
	{
		STORY 	=0 , // 故事中
		STAGE 	=1 , // 戰場上
		PREPARE =2 , // 整備
		TALK 	=3 , // 對話 
		BATTLE	= 4, // 戰鬥

	};
	// 操作 ststus
	public enum eOPStatus
	{
		NONE = 0,
		SEL_ALLY =1,
		SEL_ENEMY=2,

	};

	public static bool isApplicationQuit = false;
	
	public static string SystemLogFormat(string log){
		return "<b><color=orange>[sys]" + log + "</color></b>";
	}
	
	protected virtual void Awake () 
	{
		DontDestroyOnLoad(this.gameObject);
		
		// Lock all other device orientation in the beginning
		Screen.orientation = ScreenOrientation.AutoRotation;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		
		//設定背景載入優先度
		Application.backgroundLoadingPriority = ThreadPriority.Normal;
		
		Debug.developerConsoleVisible = true;
		
		// set target frame rate
		#if DEBUG && UNITY_EDITOR
		Application.targetFrameRate = -1;
		#else
		Application.targetFrameRate = 35;
		#endif
		
		
		//初始化音效系統
		Debug.Log(SystemLogFormat("開始初始化 Manager: " + typeof(ResourcesManager).ToString()));
		AudioManager.Instance.Initial(ResourcesManager.LoadClip);
		
		//初始化 ConstData 系統
		Debug.Log(SystemLogFormat("開始初始化 Manager: " + typeof(ConstDataManager).ToString()));
		//ConstData只讀有註冊並設定lazyMode
		//		ConstDataManager.Instance.useUnregistedTables = false;
		ConstDataManager.Instance.isLazyMode = false;
		StartCoroutine(ConstDataManager.Instance.ReadDataStreaming("pcz/", Config.COMMON_DATA_NAMES));
		
		//初始化 PanelManager
		Debug.Log(SystemLogFormat("開始初始化 Manager: " + typeof(PanelManager).ToString()));
		PanelManager.Instance.Initial(
			"Panel_Blocker",
			new Dictionary<string, string[]>(){
				{ "5Main", new string[]{ "Panel_MainUI", /*"Panel_BattleList", */ } },
			}
		);
		// 資料管理器
		Debug.Log(SystemLogFormat("開始初始化 Manager: " + typeof(GameDataManager).ToString()));
		GameDataManager.Instance.Initial (0);
		//Const 管理器
	}
	
	void Start()
	{
		if(Application.loadedLevel == 1)
		{
			ResourcesManager.LoadLevel("5Main");
		}
		//ResourcesManager.LoadLevel(Application.loadedLevel + 1);
	}
	
	void OnLevelWasLoaded(int levelIndex)
	{
		Debug.Log("OnLevelWasLoaded: " + Application.loadedLevelName);
	}
	
	// ios does not dispatch
	void OnApplicationQuit()
	{
		#if DEBUG
		Debug.Log("OnApplicationQuit");
		#endif
		isApplicationQuit = true;
	}
	// 目前操作

	public static void PlayBGM( int nBGMIdx )
	{
		if( nBGMIdx <=0 )
			AudioManager.Instance.Stop(AudioChannelType.BGM);
		// 播放  mian BGM
		DataRow row = ConstDataManager.Instance.GetRow("MUSIC", nBGMIdx );
		if( row != null )
		{
			string strFile = row.Field< string >("s_FILENAME");
			if( !String.IsNullOrEmpty( strFile ))
			{
				string audioPath = ResourcesManager.GetAudioClipPath( AudioChannelType.BGM ,  strFile );
				AudioManager.Instance.Play( AudioChannelType.BGM ,  audioPath );				
				
			}
		}
	}

	public static GameObject CreateCharacterGameObj( GameObject parent )
	{
		GameObject preObj = Resources.Load( "Panel/Panel_char" ) as GameObject;
		if (preObj != null) {
			return  NGUITools.AddChild ( parent, preObj);
		}
		return null;
	}
	// 目前的紀錄狀態
	public PLAYER_DATA			m_cPlayerData;
}
