using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// All SRW enum list
namespace _SRW
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

	public enum _ROUND_STATUS
	{
		_START =0,
		_RUNNING , 
		_END     ,

	}
}//_SRW_CMD

//
public class FactionUnit
{
	public int FactionID { set; get; }
	public List<int> memLst;

	public FactionUnit()
	{
		memLst = new List<int>();
	}
}
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
		UnitPool = new Dictionary< int , UNIT_DATA >();
		FactionPool = new Dictionary< int , FactionUnit >();
	}



	public int nStoryID{ get; set; } 
	public int nStageID{ get; set; } 
	public int nTalkID{ get; set; } 
	public int nBattleID{ get; set; } 


	//
	public int nRound{ get; set; } 
	public _SRW._ROUND_STATUS nRoundStatus{ get; set; }    // 0- start ,1- running, 2- end
	public int nActiveFaction{ get; set; }  // 


	// Faction
	public Dictionary< int , FactionUnit > FactionPool;			// add faction
	public FactionUnit GetFaction( int nFactionID )
	{
		if( FactionPool.ContainsKey( nFactionID ) )
		{
			return FactionPool[ nFactionID ];
		}
		return null;
	}

	public void AddFactionMember( int nFactionID , int nMemIdent )
	{
		if( FactionPool.ContainsKey( nFactionID ) ){
			FactionUnit unit = FactionPool[ nFactionID ];
			if( unit != null ){
				if( unit.memLst.Contains( nMemIdent ) == false  )
				{
					unit.memLst.Add( nMemIdent );
				}
			}
		}
		else{
			FactionUnit unit = new FactionUnit();
			unit.FactionID = nFactionID;
			unit.memLst.Add( nMemIdent );
			FactionPool.Add( nFactionID , unit );
		}

	}
	public void DelFactionMember( int nFactionID , int nMemIdent )
	{
		if( FactionPool.ContainsKey( nFactionID ) ){
			FactionUnit unit = FactionPool[ nFactionID ];
			if( unit != null )
			{
				unit.memLst.Remove( nMemIdent );
			}
		}
	}
	// switch to next faction. return true if round change
	public bool NextFaction()
	{
		if( nActiveFaction == 0 )
		{
			nActiveFaction++;
			nRoundStatus = _SRW._ROUND_STATUS._START;
			return false;
		}
		else if( nActiveFaction == 1 )
		{
			nActiveFaction = 0; //
			nRound++;
			nRoundStatus = _SRW._ROUND_STATUS._START;
			return true;
		}
		return true;	 
	}


	// 目前的紀錄狀態
	public PLAYER_DATA			cPlayerData;



	// don't public this
	int nSerialNO;		// object serial NO
	Dictionary< int , UNIT_DATA > UnitPool;			// add event id 

 	int GenerSerialNO( ){ return ++nSerialNO ; }
	int GenerMobSerialNO( ){ return (++nSerialNO)*(-1) ; }


	public UNIT_DATA CreateChar( int nCharID )
	{
		UNIT_DATA unit = new UNIT_DATA();
		unit.n_Ident = GenerSerialNO( );
		unit.n_CharID = nCharID;
		UnitPool.Add( unit.n_Ident , unit );
		return unit;
	}

	public UNIT_DATA CreateMob( int nCharID )
	{
		UNIT_DATA unit = new UNIT_DATA();
		unit.n_Ident = GenerMobSerialNO() ; 
		unit.n_CharID = nCharID;		
		UnitPool.Add( unit.n_Ident , unit );
		return unit;
	}

	public void DelUnit( int nIdent )
	{
		UnitPool.Remove( nIdent );

	}
	public void DelUnit( UNIT_DATA unit )
	{
		if( unit != null )
			UnitPool.Remove( unit.n_Ident );
	}
};
