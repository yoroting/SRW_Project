using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
using _SRW;
using MYGRIDS;
// All SRW enum list
/// <summary>預設存在的 Channel Type</summary>



public partial class BattleManager
{
	//public delegate AudioClip GetAudioClipDelegate (string audioPath);

	private bool hadInit;
	public bool HadInit{ get{ return hadInit; } }



//	private Dictionary<int, AudioChannelBase> channels = new Dictionary<int, AudioChannelBase>();

	public void Initial( int fileindex =0 ){
		hadInit = true;

		bIsBattle = false;
		//this.GetAudioClipFunc = getAudioClipFunc;
	//	UnitPool = new Dictionary< int , UNIT_DATA >();
	//	CampPool = new Dictionary< _CAMP , cCamp >();
	}

	private static BattleManager instance;
	public static BattleManager Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new BattleManager();
				instance.Clear();
			}
			
			return instance;
		}
	}

	public void Clear()
	{

	}

	public bool IsBattlePhase()
	{
		if (bIsBattle)
			return true;

		return false;
	}



	public void Run()
	{
		//
		switch( nPhase )
		{
		case 0:			// atack pre show 
			ShowBattleMsg( nAtkerID , "attack" );
			nPhase++;
			break;
		case 1:			// def pre show 
			ShowBattleMsg( nDeferID , "counter" );
			nPhase++;
			break;
		case 2:			// atack assist pre show 
			nPhase++;
			break;
		case 3:			// def assist pre show 
			nPhase++;
			break;
		case 4:			// figher

			ShowBattleFX(nDeferID , "CFXM4 Hit B (Orange, CFX Blend)"  );
			nPhase++;
			break;
		case 5:			// fight end . show exp

			nPhase++;
			break;
		case 6:			// close all 
			nPhase++;

			bIsBattle = false;
			break;

		}

		return ;
	}


	//===================================================
	public bool bIsBattle { get; set; } 

	public int nAtkerID{ get; set; } 
	public int nDeferID{ get; set; } 

	public int nAtkerSkillID{ get; set; } 
	public int nDeferSkillID{ get; set; } 

	public bool bIsDefenceMode { get; set; } 		// 


	public int nBattleID{ get; set; } 


	int nPhase = 0; // 



	public void Play()
	{
		nPhase = 0;
		bIsBattle = true;
	}

	public void ShowBattleMsg( int nIdent , string msg )
	{
		Panel_unit unit = Panel_StageUI.Instance.GetUnitByIdent( nIdent ); 
		ShowBattleMsg (unit , msg );

	}

	public void ShowBattleMsg( Panel_unit unit , string msg )
	{
		Vector3 v = new Vector3 (0, 0, 0);
		if (unit != null) {
			// show in screen center
			v = unit.transform.position;
		}

		GameObject go = ResourcesManager.CreatePrefabGameObj ( Panel_StageUI.Instance.MaskPanelObj , "prefab/BattleMsg" );
		if (go != null) {
			go.transform.position = v;
			UILabel lbl = go.GetComponentInChildren<UILabel>();
			if( lbl != null )
			{
				lbl.text = msg;
			}
		}

	}

	public void ShowBattleFX( int nIdent , string fx )
	{
		Panel_unit unit = Panel_StageUI.Instance.GetUnitByIdent( nIdent ); 
		if (unit == null)
			return;

		//string path = "FX/Cartoon FX/" + fx;
		//GameObject go = ResourcesManager.CreatePrefabGameObj ( unit.gameObject , fx );
		GameObject go = GameSystem.PlayFX ( unit.gameObject , fx  );
		if (go != null) {

		}
	}

	// Operation Token ID 
	public int nOpCellX{ get; set; } 				//
	public int nOpCellY{ get; set; } 				//
	//
	public int nRound{ get; set; } 

};

