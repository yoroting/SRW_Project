using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
using MYGRIDS;

public enum _CMD_TYPE // what kind of cmd list
{
	_SYS = 0,
	_CELL ,
	_ALLY 	,
	_ENEMY 	,
	_MENU 	 ,
	_WAITATK ,
	_WAITMOVE ,
	_COUNTER,		// _COUNTER for mob atk
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
	_SELF,
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
	_COUNTER,			// 反擊	

	_STAGEINFO ,			// 	
	_CANCEL ,			// 	
	_NEWGAME,
	_SAVE,
	_LOAD,
	_ROUNDEND,
	_GAMEEND,
	_OPTION,

	//================================
	_SUICIDE,
	_CHEAT,
	_SYSCHEAT,
	_WIN,
	_LOST,
	_KILLALLE,			//全敵人死亡
}


public class cCMD{
	
	private bool hadInit = false;
	public bool HadInit{ get{ return hadInit; } }
	
	public List<_CMD_ID>[] CmdlistArray ;
	
	public MyGrids Grids;				// main grids . only one
	
	
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
		CmdlistArray [idx].Add ( _CMD_ID._STAGEINFO ); 
		CmdlistArray [idx].Add ( _CMD_ID._SAVE ); 
		CmdlistArray [idx].Add ( _CMD_ID._LOAD ); 
		CmdlistArray [idx].Add ( _CMD_ID._ROUNDEND ); 
		CmdlistArray [idx].Add ( _CMD_ID._OPTION ); 
	//	CmdlistArray [idx].Add ( _CMD_ID._GAMEEND );  // 改成在 option 內 啟動
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		if (Debug.isDebugBuild) {
			CmdlistArray [idx].Add (_CMD_ID._SYSCHEAT);
//			CmdlistArray [idx].Add (_CMD_ID._WIN);
//			CmdlistArray [idx].Add (_CMD_ID._LOST);
//			CmdlistArray [idx].Add (_CMD_ID._KILLALLE);
			
		}
		
		
		// ally
		idx = (int)_CMD_TYPE._ALLY;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add ( _CMD_ID._INFO ); 
		//		CmdlistArray [idx].Add ( _CMD_ID._MOVE ); 
		CmdlistArray [idx].Add ( _CMD_ID._ATK ); 
		CmdlistArray [idx].Add ( _CMD_ID._SKILL ); 
		CmdlistArray [idx].Add ( _CMD_ID._ABILITY ); 
		//	CmdlistArray [idx].Add ( _CMD_ID._DEF ); 
		CmdlistArray [idx].Add ( _CMD_ID._SCHOOL ); 
		//CmdlistArray [idx].Add ( _CMD_ID._WAIT );  // no need wait here
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		if (Debug.isDebugBuild ) {
			CmdlistArray [idx].Add (_CMD_ID._SUICIDE);
			CmdlistArray [idx].Add (_CMD_ID._CHEAT);
		}
		
		// enemy
		idx = (int)_CMD_TYPE._ENEMY;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add (  _CMD_ID._INFO  );
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL );

		if (Debug.isDebugBuild) {
			CmdlistArray [idx].Add (_CMD_ID._SUICIDE);
			CmdlistArray [idx].Add (_CMD_ID._CHEAT);
			CmdlistArray [idx].Add (_CMD_ID._ATK);
			CmdlistArray [idx].Add (_CMD_ID._SKILL);
		}
		
		// 
		idx = (int)_CMD_TYPE._MENU;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 
		
		// wait sel a target to atk
		idx = (int)_CMD_TYPE._WAITATK;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		//		CmdlistArray [idx].Add ( _CMD_ID._ATK ); 
		CmdlistArray [idx].Add ( _CMD_ID._SKILL ); 
		CmdlistArray [idx].Add ( _CMD_ID._WAIT ); 
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		if (Debug.isDebugBuild) {
			CmdlistArray [idx].Add ( _CMD_ID._ABILITY );  // god for debug
		}
		// wait sel a pos
		idx = (int)_CMD_TYPE._WAITMOVE;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add ( _CMD_ID._WAIT ); 
		//CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 
		
		// counter 
		idx = (int)_CMD_TYPE._COUNTER;
		CmdlistArray [idx] = new List<_CMD_ID> (); // 反及
		CmdlistArray [idx].Add ( _CMD_ID._COUNTER ); 
	//	CmdlistArray [idx].Add ( _CMD_ID._ABILITY );  // 反擊不可放 精神指令。避免麻煩
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
	
	//	public _CMD_ID 		eLastCMDID;		// Last cmd ID
	public _CMD_TYPE	eLASTCMDTYPE;		// LAST Cmd type
	
	public _CMD_TYPE	eNEXTCMDTYPE;		// NEXT Cmd type
	public int nTarIdent;
	
	public int nOrgGridX;
	public int nOrgGridY;
	
	public int nTarGridX;
	public int nTarGridY;

	public int nCMDGridX;
	public int nCMDGridY;

	public int nSkillID;
	public int nAbilityID;
	
	public int nAOEID;				// if have aoe
	public int nItemID;
	
	public _CMD_TARGET 	eCMDAOETARGET;		// record Aoe target type
	
	
	public void Clear()
	{
		nCmderIdent = 0;
		eCMDTYPE = _CMD_TYPE._SYS;
		eCMDSTATUS = _CMD_STATUS._NONE;
		eCMDTARGET  = _CMD_TARGET._ALL;
		
		eCMDID 	  = _CMD_ID._NONE;
		//	eLastCMDID = _CMD_ID._NONE;
		
		eLASTCMDTYPE = _CMD_TYPE._SYS;;		// LAST Cmd type
		eNEXTCMDTYPE = _CMD_TYPE._SYS;
		
		nTarIdent = 0;		
		nOrgGridX = 0;
		nOrgGridY = 0;
		
		nTarGridX = 0;
		nTarGridY = 0;

//		nCMDGridX = 0;
//		nCMDGridY = 0;

		nSkillID = 0;
		nAbilityID = 0;
		nItemID = 0;
		
		nAOEID = 0;
		eCMDAOETARGET = _CMD_TARGET._ALL;
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

