﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>預設存在的 Channel Type</summary>

public class GameDataManager : Singleton<GameDataManager>
{
	//public delegate AudioClip GetAudioClipDelegate (string audioPath);

	private bool hadInit;
	public bool HadInit{ get{ return hadInit; } }



//	private Dictionary<int, AudioChannelBase> channels = new Dictionary<int, AudioChannelBase>();

	public void Initial( int fileindex =0 ){
		hadInit = true;
		//this.GetAudioClipFunc = getAudioClipFunc;
	}

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	//	AudioListener.volume = PlayerPrefs.GetFloat("volumeAudioManager", 1f);
	//	CreateChannel<BGMChannel>(AudioChannelType.BGM);
	//	CreateChannel<SoundEffectChannel>(AudioChannelType.SoundFX);
	}

	public int nStoryID{ get; set; } 
	public int nStageID{ get; set; } 
	public int nTalkID{ get; set; } 
	public int nBattleID{ get; set; } 


	//
	public int nRound{ get; set; } 
	public int nActiveFaction{ get; set; }  // 

	// 目前的紀錄狀態
	public PLAYER_DATA			cPlayerData;

};
