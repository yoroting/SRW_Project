using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
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

	public enum _CMD_TYPE // what kind of cmd list
	{
		_SYS = 0,
		_CELL ,
		_ALLY 	,
		_ENEMY 	,
		_MENU 	 ,
		_WAITATK ,
		_WAITMOVE ,
		_COUNTER,
		_MAX ,
	}

	public enum _CMD_STATUS // what kind of cmd list
	{
		_NONE=0,
		_WAIT_CMDID,
		_WAIT_TARGET,

	}

	public enum _CMD_TARGET  // target type
	{
		_ALL = 0,
		_POS ,
		_UNIT,
	}

	public enum _CMD_ID  // list of cmd btn id
	{
		_NONE = 0,			// 
		_MOVE ,			// 
		_ATK ,			// 
		_DEF ,			// 	
		_SKILL ,			// 	
		_ABILITY ,			// 	
		_SCHOOL ,			// 	
		_ITEM ,			//  No use		
		_WAIT,			//
		_INFO ,			// 	
		_CANCEL ,			// 	
		_NEWGAME,
		_SAVE,
		_LOAD,
		_ROUNDEND,
		_GAMEEND,
		_OPTION,
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
		EvtDonePool = new Dictionary< int , int > ();			// record event complete round 
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

	public void ResetStage()
	{
		nRound = 0;
		nActiveCamp = _CAMP._PLAYER;
	//	UnitPool.Clear ();
		CampPool.Clear ();
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
		// weakup current camp first for remove unit mask
		StageWeakUpCampEvent cmd = new StageWeakUpCampEvent ();
		cmd.nCamp = nActiveCamp;
		GameEventManager.DispatchEvent ( cmd );

		// 
		bool bRoundChange = false;
		if( nActiveCamp == _CAMP._PLAYER )
		{
			nActiveCamp = _CAMP._ENEMY;
			nRoundStatus = _ROUND_STATUS._START;

			bRoundChange = false;
		}
		else if( nActiveCamp == _CAMP._ENEMY )
		{
			nActiveCamp = _CAMP._PLAYER; //
			nRound++;
			nRoundStatus = _ROUND_STATUS._START;

			bRoundChange =  true;
		}

		// open . round change panel ui
		PanelManager.Instance.OpenUI( Panel_Round.Name );


		return bRoundChange;	 
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
	// Event Status
	public Dictionary< int , int > EvtDonePool;			// record event complete round 


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

	public List<_CMD_ID>[] CmdlistArray ;

	public cMyGrids	Grids;				// main grids . only one
	
	
	//	private Dictionary<int, AudioChannelBase> channels = new Dictionary<int, AudioChannelBase>();
	
	public void Initial( ){
		if (hadInit)return;
		
		hadInit = true;

		CmdlistArray = new List<_CMD_ID>[ (int)_CMD_TYPE._MAX ];

		int idx = 0;
		// create cmd list
		idx = (int)_CMD_TYPE._SYS;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add ( _CMD_ID._NEWGAME ); 
		CmdlistArray [idx].Add ( _CMD_ID._LOAD );
		CmdlistArray [idx].Add ( _CMD_ID._OPTION );

		// cell
		idx = (int)_CMD_TYPE._CELL;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add ( _CMD_ID._INFO ); 
		CmdlistArray [idx].Add ( _CMD_ID._SAVE ); 
		CmdlistArray [idx].Add ( _CMD_ID._LOAD ); 
		CmdlistArray [idx].Add ( _CMD_ID._ROUNDEND ); 
		CmdlistArray [idx].Add ( _CMD_ID._GAMEEND ); 
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		// ally
		idx = (int)_CMD_TYPE._ALLY;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add ( _CMD_ID._INFO ); 
		CmdlistArray [idx].Add ( _CMD_ID._MOVE ); 
		CmdlistArray [idx].Add ( _CMD_ID._ATK ); 
		CmdlistArray [idx].Add ( _CMD_ID._SCHOOL ); 
		CmdlistArray [idx].Add ( _CMD_ID._ABILITY ); 
		CmdlistArray [idx].Add ( _CMD_ID._DEF ); 
		CmdlistArray [idx].Add ( _CMD_ID._SCHOOL ); 
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		// enemy
		idx = (int)_CMD_TYPE._ENEMY;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add (  _CMD_ID._INFO  );
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL );

		//
		idx = (int)_CMD_TYPE._MENU;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		// wait sel a target to atk
		idx = (int)_CMD_TYPE._WAITATK;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add ( _CMD_ID._ATK ); 
		CmdlistArray [idx].Add ( _CMD_ID._SCHOOL ); 
		CmdlistArray [idx].Add ( _CMD_ID._WAIT ); 
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		// wait sel a pos
		idx = (int)_CMD_TYPE._WAITMOVE;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add ( _CMD_ID._WAIT ); 
		//CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		// counter 
		idx = (int)_CMD_TYPE._COUNTER;
		CmdlistArray [idx] = new List<_CMD_ID> (); // 反及
		CmdlistArray [idx].Add ( _CMD_ID._ATK ); 
		CmdlistArray [idx].Add ( _CMD_ID._ABILITY ); 
		CmdlistArray [idx].Add ( _CMD_ID._SKILL ); 
		CmdlistArray [idx].Add ( _CMD_ID._DEF ); 
		//CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		// initial cmd data
		Clear ();
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
	public _CMD_TYPE	eCMDTYPE;		// Cmd type
	public _CMD_STATUS  eCMDSTATUS;		// current cmd status
	public _CMD_TARGET 	eCMDTARGET;		// cmd status
	public _CMD_ID 		eCMDID;			// current cmd ID

	public _CMD_ID 		eLastCMDID;		// Last cmd ID

	public _CMD_TYPE	eNEXTCMDTYPE;		// NEXT Cmd type
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
		eCMDTYPE = _CMD_TYPE._SYS;
		eCMDSTATUS = _CMD_STATUS._NONE;
		eCMDTARGET  = _CMD_TARGET._ALL;

		eCMDID 	  = _CMD_ID._NONE;
		eLastCMDID = _CMD_ID._NONE;

		eNEXTCMDTYPE = _CMD_TYPE._SYS;

		nTarIdent = 0;		
		nOrgGridX = 0;
		nOrgGridY = 0;
		
		nTarGridX = 0;
		nTarGridY = 0;
		
		nSkillID = 0;
		nAvilityID = 0;
		nItemID = 0;
		
		
	}

	public List<_CMD_ID> GetCmdList( _CMD_TYPE eType)
	{
		int itype = (int)eType;
		if (itype < 0 || itype >= CmdlistArray.Length) {
			return null;
		}
		return CmdlistArray [itype];
	}
}

