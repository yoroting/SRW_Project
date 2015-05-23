using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using _SRW;
using MYGRIDS;
// All SRW enum list
namespace _SRW
{
	public enum _CAMP
	{
		_PLAYER=0,
		_ENEMY =1,
		_FRIEND=2,
	}

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
public class cCamp
{
	public _CAMP CampID { set; get; }
	public List<int> memLst;

	public cCamp()
	{
		memLst = new List<int>();
	}
}
/// <summary>預設存在的 Channel Type</summary>

public partial class GameDataManager 
{
	//public delegate AudioClip GetAudioClipDelegate (string audioPath);

	private bool hadInit;
	public bool HadInit{ get{ return hadInit; } }



//	private Dictionary<int, AudioChannelBase> channels = new Dictionary<int, AudioChannelBase>();

	public void Initial( int fileindex =0 ){
		hadInit = true;
		//this.GetAudioClipFunc = getAudioClipFunc;
		UnitPool = new Dictionary< int , UNIT_DATA >();
		CampPool = new Dictionary< _CAMP , cCamp >();
	}

	private static GameDataManager instance;
	public static GameDataManager Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new GameDataManager();
				instance.Clear();
			}
			
			return instance;
		}
	}

	public void Clear()
	{

	}

	//===================================================
	public int nStoryID{ get; set; } 
	public int nStageID{ get; set; } 
	public int nTalkID{ get; set; } 
	public int nBattleID{ get; set; } 

	// Operation Token ID 
	public int nOpCharIdent{ get; set; } 				//
	public int nOpMobIdent{ get; set; } 				//
	public int nOpFriendIdent{ get; set; } 				//

	public int nOpCellX{ get; set; } 				//
	public int nOpCellY{ get; set; } 				//
	//
	public int nRound{ get; set; } 
	public _SRW._ROUND_STATUS nRoundStatus{ get; set; }    // 0- start ,1- running, 2- end

	// Camp
	public _CAMP nActiveCamp{ get; set; }  // 


	// Camp
	public Dictionary< _CAMP , cCamp > CampPool;			// add Camp
	public cCamp GetCamp( _CAMP nCampID )
	{
		if( CampPool.ContainsKey( nCampID ) )
		{
			return CampPool[ nCampID ];
		}
		return null;
	}

	public void AddCampMember( _CAMP nCampID , int nMemIdent )
	{
		if( CampPool.ContainsKey( nCampID ) ){
			cCamp unit = CampPool[ nCampID ];
			if( unit != null ){
				if( unit.memLst.Contains( nMemIdent ) == false  )
				{
					unit.memLst.Add( nMemIdent );
				}
			}
		}
		else{
			cCamp unit = new cCamp();
			unit.CampID = nCampID;
			unit.memLst.Add( nMemIdent );
			CampPool.Add( nCampID , unit );
		}

	}
	public void DelCampMember( _CAMP nCampID , int nMemIdent )
	{
		if( CampPool.ContainsKey( nCampID ) ){
			cCamp unit = CampPool[ nCampID ];
			if( unit != null )
			{
				unit.memLst.Remove( nMemIdent );
			}
		}
	}
	// switch to next Camp. return true if round change
	public bool NextCamp()
	{
		if( nActiveCamp == _CAMP._PLAYER )
		{
			nActiveCamp++;
			nRoundStatus = _ROUND_STATUS._START;
			return false;
		}
		else if( nActiveCamp == _CAMP._ENEMY )
		{
			nActiveCamp = _CAMP._PLAYER; //
			nRound++;
			nRoundStatus = _ROUND_STATUS._START;
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
