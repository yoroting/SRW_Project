using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace _SRW_CMD
{
	public enum _CMD_TYPE
	{
		_SYS = 0,
		_CELL =1	,
		_ALLY =2	,
		_ENEMY =3	,
		_MENU =4	
	}

	public enum _CMD_ID  // 
	{
		_MOVE = 0,			// 
		_ATK =1,			// 
		_DEF =2,			// 	
		_SKILL =3,			// 	
		_ABILITY =4,			// 	
		_ITEM =5,			//  use		
		_INFO =6,			// 	
		_CANCEL =7,			// 	
	}
}//_SRW_CMD
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
	public int nRoundStatus{ get; set; }    // 0- start , 1- end

	public int nActiveFaction{ get; set; }  // 

	// 目前的紀錄狀態
	public PLAYER_DATA			cPlayerData;

};
