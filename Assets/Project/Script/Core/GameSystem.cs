using UnityEngine;
using Playcoo.Common;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameSystem : MonoBehaviour {
	
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
}
