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

	public enum _CMD_STATUS
	{
		_WAIT = 0,
		_MOVE ,
		_TARGET ,
	}

	public enum _CMD_ID  // 
	{
		_NONE = 0,			// 
		_MOVE ,			// 
		_ATK ,			// 
		_DEF ,			// 	
		_SKILL ,			// 	
		_ABILITY ,			// 	
		_ITEM ,			//  No use		
		_INFO ,			// 	
		_CANCEL ,			// 	
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

public partial class GameScene
{
	//public delegate AudioClip GetAudioClipDelegate (string audioPath);
	
	private bool hadInit = false;
	public bool HadInit{ get{ return hadInit; } }
	
	public cMyGrids	Grids;				// main grids . only one

	
	//	private Dictionary<int, AudioChannelBase> channels = new Dictionary<int, AudioChannelBase>();
	
	public void Initial( ){
		if (hadInit)return;

		hadInit = true;
		//this.GetAudioClipFunc = getAudioClipFunc;
		//UnitPool = new Dictionary< int , UNIT_DATA >();
		//CampPool = new Dictionary< _CAMP , cCamp >();
		Grids = new cMyGrids();
	}
	
	private static GameScene instance;
	public static GameScene Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new GameScene();
				instance.Clear();
			}
			
			return instance;
		}
	}
	
	public void Clear()
	{

	}
}

public class cCMD{

	private bool hadInit = false;
	public bool HadInit{ get{ return hadInit; } }
	
	public cMyGrids	Grids;				// main grids . only one
	
	
	//	private Dictionary<int, AudioChannelBase> channels = new Dictionary<int, AudioChannelBase>();
	
	public void Initial( ){
		if (hadInit)return;
		
		hadInit = true;
		//this.GetAudioClipFunc = getAudioClipFunc;
		//UnitPool = new Dictionary< int , UNIT_DATA >();
		//CampPool = new Dictionary< _CAMP , cCamp >();
	}

	private static cCMD instance;
	public static cCMD Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new cCMD();
				instance.Clear();
			}
			
			return instance;
		}
	}

	public int nCmderIdent;			// Operatr char ident
	public _CMD_STATUS 	eCMDSTAT;		// cmd status
	public _CMD_ID 		eCMDID;			// cmd ID
	
	public int nTarIdent;
	
	public int nOrgGridX;
	public int nOrgGridY;
	
	public int nTarGridX;
	public int nTarGridY;
	
	public int nSkillID;
	public int nAvilityID;
	public int nItemID;
	
	public void Clear()
	{
		nCmderIdent = 0;
		eCMDSTAT  = _CMD_STATUS._WAIT;
		eCMDID 	  = _CMD_ID._NONE;
		nTarIdent = 0;		
		nOrgGridX = 0;
		nOrgGridY = 0;
		
		nTarGridX = 0;
		nTarGridY = 0;
		
		nSkillID = 0;
		nAvilityID = 0;
		nItemID = 0;
		
		
	}
}

