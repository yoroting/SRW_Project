using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
using _SRW;
using MYGRIDS;
// All SRW enum list
/// <summary>預設存在的 Channel Type</summary>

public class cFightResult
{
	public int AtkIdent { set; get;}
	public int DefIdent { set; get;}
	public int AtkSchool { set; get;}
	public int DefSchool { set; get;}
	public int AtkSkill { set; get;}
	public int DefSkill { set; get;}

	// hp modify
	public int AtkHp { set; get;}
	public int DefHp { set; get;}

	// change Pos 

};

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



	public void RunBattle()
	{
		//
		//Panel_unit unDef = Panel_StageUI.Instance.GetUnitByIdent( nDeferID ); 

		switch( nPhase )
		{
		case 0:	// prepare for event check
			nPhase++;
			break;
		case 1:			// atack pre show 
			ShowBattleMsg( nAtkerID , "attack" );
			nPhase++;
			break;
		case 2:			// def pre show 
			//ShowBattleMsg( nDeferID , "counter" );
			 // show assist
			nPhase++;
			break;
		case 3:			// atack assist pre show 
			nPhase++;
			break;
		case 4:			// def assist pre show 
			nPhase++;
			break;
		case 5:			// atk -> def 
		//	DoAttackEvent( nAtkerID , nDeferID );
			Panel_unit unitAtk = Panel_StageUI.Instance.GetUnitByIdent( nAtkerID );
			//Panel_unit unitDef = Panel_StageUI.Instance.GetUnitByIdent( nDeferID );
			if( unitAtk != null )
			{
				unitAtk.ActionAttack( nDeferID );
			}

			nPhase++;
			break;
		case 6:			//  def -> atk
			if( bDefMode  == false )
			{
				Panel_unit unitDef = Panel_StageUI.Instance.GetUnitByIdent( nDeferID );
				if( unitDef != null )
				{
					unitDef.ActionAttack( nAtkerID );
				}
			}
			nPhase++;
			break;
		case 7: 	// fight end . show exp

			nPhase++;
			break;
		case 8:			// close all 
			nPhase++;

			// cmd finish
			StageUnitActionFinishEvent cmd = new StageUnitActionFinishEvent ();
			cmd.nIdent = nAtkerID;
			GameEventManager.DispatchEvent ( cmd );

			bIsBattle = false;

			// Do Counter

			break;

		}

		return ;
	}


	//===================================================
	public bool bIsBattle { get; set; } 
	public bool bPause { get; set; } 

	public int nAtkerID{ get; set; } 
	public int nDeferID{ get; set; } 

	public bool bDefMode{ get; set; } 

	public int nAtkerSkillID{ get; set; } 
	public int nDeferSkillID{ get; set; } 

	public bool bIsDefenceMode { get; set; } 		// 


	public int nBattleID{ get; set; } 

	//===================================================
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

	public void ShowBattleResValue( int nIdent , int nValue , int nMode)
	{
		Panel_unit unit = Panel_StageUI.Instance.GetUnitByIdent( nIdent ); 
		if (unit == null)
			return;
		ShowBattleResValue ( unit.gameObject , nValue , nMode );
	}
	public void ShowBattleResValue( GameObject obj , int nValue , int nMode )
	{	
		Vector3 v = new Vector3 (0, 0, 0);
		if ( obj != null) {
			// show in screen center
			v = obj.transform.position;
		}
		
		GameObject go = ResourcesManager.CreatePrefabGameObj ( Panel_StageUI.Instance.MaskPanelObj , "Prefab/BattleValue" );
		//string path = "Prefab/BattleValue";
		//GameObject go = ResourcesManager.CreatePrefabGameObj ( obj , path );
		//GameObject go = GameSystem.PlayFX ( unit.gameObject , fx  );
		if (go != null) {
			go.transform.position = v;
			UILabel lbl = go.GetComponent< UILabel >();
			if( lbl )
			{
				if( nValue > 0 )
				{
					// heal 
					
					lbl.gradientTop = new Color( 0.0f, 1.0f , 0.0f );
				}
				else{ 
					// damage
					lbl.gradientTop = new Color( 1.0f, 0.0f , 0.0f );
				}
				lbl.text = nValue.ToString();
			}
		}
	}


	public cFightResult CalAttackResult( int nAtker , int nDefer )
	{
		cUnitData pAtker = GameDataManager.Instance.GetUnitDateByIdent( nAtker ); 	//Panel_StageUI.Instance.GetUnitByIdent( nAtker ); 
		cUnitData pDefer = GameDataManager.Instance.GetUnitDateByIdent( nDefer );	//Panel_StageUI.Instance.GetUnitByIdent( nDefer ); 
		if ( (pAtker == null) || (pDefer == null) )
			return null;

		// create result
		cFightResult res = new cFightResult ();
		res.AtkIdent = nAtker;
		res.DefIdent = nDefer;

		// buff effect
		float AtkMarPlus = 0.0f;
		float DefMarPlus = 0.0f;
		int AtkPowPlus = 0;
		int DefPowPlus = 0;
		int AtkPlus = 0;



		float AtkMar =  pAtker.GetMar() + AtkMarPlus;
		float DefMar =  pDefer.GetMar() + DefMarPlus ;

		// 1 mar = 0.5% hit rate
		float HitRate = ((AtkMar-DefMar) + Config.HIT) / 200.0f; // add base rate
		if( HitRate < 0.0f )
			HitRate = 0.0f;


		int AtkPow =  pAtker.GetPow() + AtkPowPlus;
		int DefPow =  pDefer.GetPow() + DefPowPlus ;

		int PowDmg = (int)(HitRate*(pAtker.GetPow()-pDefer.GetPow())); // 

		if( PowDmg > 0 ){
			res.DefHp -= PowDmg;
	
		}
		else if( PowDmg < 0 ){
			res.AtkHp = PowDmg; // it is neg value already
	

		}

		// buff effect
		int Atk = pAtker.GetAtk() + AtkPlus;
		int DefAC = 0; // armor

		float fAtkDmg 	= (HitRate*Atk) - DefAC  ; 



		fAtkDmg = (fAtkDmg<0)? 0: fAtkDmg;
		if( bDefMode )
		{
			fAtkDmg = (fAtkDmg*Config.DefReduce /100.0f);
		}

		res.DefHp -= (int)(fAtkDmg);

		return res;
	}

	// Operation Token ID 
	public int nOpCellX{ get; set; } 				//
	public int nOpCellY{ get; set; } 				//
	//


};

