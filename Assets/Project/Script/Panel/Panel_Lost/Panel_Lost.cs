﻿using UnityEngine;
using System.Collections;

public class Panel_Lost : MonoBehaviour {

	public const string Name = "Panel_Lost";
	
	public GameObject SpritObj;

	void OnEnable()
	{
		Panel_StageUI.Instance.bIsStageEnd = true;
		GameDataManager.Instance.EndStage ();   // 處理戰場結束的資料回存
	}

	// Use this for initialization
	void Start () {
		UIEventListener.Get(SpritObj).onClick += OnCloseBtnClick; // for trig next lineev
		
		
		GameSystem.PlayBGM ( 6 ); // lost music
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnDisable () {
	//	UIEventListener.Get(SpritObj).onClick -= OnCloseBtnClick; // for trig next lineev
	}
	
	void OnCloseBtnClick(GameObject go)
	{	
		// open main ten ui
		PanelManager.Instance.OpenUI ( Panel_Mainten.Name );
		
		// Go to Mainten Ui 
		PanelManager.Instance.DestoryUI ( Name );
		
	}
}
