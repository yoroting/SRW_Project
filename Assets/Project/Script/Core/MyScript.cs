using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
using MYGRIDS;
using _SRW;
using MyClassLibrary;			// for parser string
// 
public class MyScript {

	private static MyScript instance;
	public static MyScript Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new MyScript();
			}
			
			return instance;
		}
	}

	public bool CheckEventCondition( CTextLine line )
	{
		if( line == null )
			return false;
		
		List<cTextFunc> funcList =line.GetFuncList();
		foreach( cTextFunc func in funcList )
		{
			if( func.sFunc == "GO" )
			{
				if( ConditionGO( ) == false )
				{
					return false;
				}	
			}
			else if( func.sFunc == "ALLDEAD" )
			{
				if( ConditionAllDead( func.At(0) ) == false )
				{
					return false;
				}				
			}
			else if( func.sFunc == "DEAD"  )
			{
				if( ConditionUnitDead( func.At(0), func.At(1) ) == false )
				{
					return false;
				}				
			}
			else if( func.sFunc == "ROUND"  )
			{
				if( ConditionRound( func.At(0) ) == false )
				{
					return false;
				}				
			}		
			else if( func.sFunc == "AFTER"  )
			{
				if( ConditionAfter( func.At(0),func.At(1)  ) == false )
				{
					return false;
				}				
			}
			else if( func.sFunc == "COMBAT"  )
			{
				if( ConditionCombat( func.At(0),func.At(1)  ) == false )
				{
					return false;
				}		
			}
			else if( func.sFunc == "DIST"  )
			{
				if( ConditionDist( func.At(0),func.At(1) ,func.At(2) ) == false )
				{
					return false;
				}	
			}
			else{
				Debug.LogError( string.Format( "Error-Can't find script cond func '{0}'" , func.sFunc ) );
			}
		}
		return true;
	}

	// Check event
	bool CheckEventCanRun( STAGE_EVENT evt )
	{
		cTextArray sCond = new cTextArray( );
		sCond.SetText( evt.s_CONDITION );
		// check all line . if one line success . this event check return true
		
		int nCol = sCond.GetMaxCol();
		for( int i= 0 ; i <nCol ; i++ )
		{
			if( CheckEventCondition( sCond.GetTextLine( i ) ) )
			{
				return true;
			}
		}
		return false;
	}

	// Condition
	// condition check 
	bool ConditionGO(  ) // always active
	{
		return true;
	}
	
	bool ConditionAllDead( int nCampID )
	{
		// assign id
		cCamp unit = GameDataManager.Instance.GetCamp( (_CAMP)nCampID );
		if( unit != null )
		{
			return (unit.memLst.Count<=0) ;
		}
		return true;
	}
	
	bool ConditionUnitDead( int nCampID ,int nCharID )
	{
		// assign id
		cCamp camp = GameDataManager.Instance.GetCamp( (_CAMP)nCampID );
		if( camp != null )
		{
			foreach( int no in  camp.memLst )
			{
				Panel_unit unit =  Panel_StageUI.Instance.GetUnitByIdent( no ); //this.IdentToUnit[ no ];
				if( unit != null )
				{
					if( unit.CharID ==  nCharID )
					{
						return false;
					}
				}
			}
		}
		return true;
	}
	
	bool ConditionRound( int nID )
	{
		//	if( GameDataManager.Instance.nRoundStatus != 0 )
		//		return false;
		
		return (GameDataManager.Instance.nRound >=nID ) ;
	}
	bool ConditionAfter( int nID , int nRound  )
	{
		if( GameDataManager.Instance.EvtDonePool.ContainsKey( nID )  ){
			int nCompleteRound = GameDataManager.Instance.EvtDonePool[ nID ];
			
			return ( GameDataManager.Instance.nRound >= ( nCompleteRound + nRound ) );
		}
		return false;
	}
	bool ConditionCombat( int nChar1 , int nChar2  )
	{
		if( BattleManager.Instance.bIsBattle )
		{
			Panel_unit atker = Panel_StageUI.Instance.GetUnitByIdent( BattleManager.Instance.nAtkerID );
			Panel_unit defer = Panel_StageUI.Instance.GetUnitByIdent( BattleManager.Instance.nDeferID );
			int atkerid = 0;
			int deferid = 0;
			if( atker!=null )
			{
				atkerid = atker.CharID;
			}
			if( defer!=null )
			{
				deferid = defer.CharID;
			}

			if( nChar1 != 0 )
			{
				if( (atkerid!=nChar1) && ( deferid!=nChar1) )
				{
					return false;
				}
			}
			if( nChar2 != 0 )
			{
				if( (atkerid!=nChar2) && ( deferid!=nChar2) )
				{
					return false;
				}
			}
			return true;

		}

		return false;
	}
	bool ConditionDist( int nChar1 , int nChar2 , int nDist )
	{
		// don't check suring cmd
		if (cCMD.Instance.eCMDSTATUS != _CMD_STATUS._NONE)
			return false;

		//check range
		Panel_unit unit1 = Panel_StageUI.Instance.GetUnitByCharID (nChar1);
		Panel_unit unit2 = Panel_StageUI.Instance.GetUnitByCharID (nChar2);
		if ((unit1 != null) && (unit2 != null)) {
			int dist = unit1.Loc.Dist( unit2.Loc );
			if( dist <= nDist )
			{
				return true;
			}
		}
		return false;
	}
	
	//------------------
	// Stage Run
	//-----------------
	public void ParserScript( CTextLine line )
	{
		List<cTextFunc> funcList =line.GetFuncList();
		foreach( cTextFunc func in funcList )
		{
			if( func.sFunc == "POPCHAR" )
			{
				StagePopUnitEvent evt = new StagePopUnitEvent ();
				evt.eCamp   = _CAMP._PLAYER;
				evt.nCharID = func.At( 0 );
				evt.nX		= func.At( 1 );
				evt.nY		= func.At( 2 );
				GameEventManager.DispatchEvent ( evt );
			}
			else if( func.sFunc == "POPMOB" )
			{
				StagePopUnitEvent evt = new StagePopUnitEvent ();
				evt.eCamp   = _CAMP._ENEMY;
				evt.nCharID = func.At( 0 );
				evt.nX		= func.At( 1 );
				evt.nY		= func.At( 2 );
				GameEventManager.DispatchEvent ( evt );
			}
			else if( func.sFunc == "POP" )
			{
				StagePopUnitEvent evt = new StagePopUnitEvent ();
				evt.eCamp 	= (_CAMP)func.At( 0 );
				evt.nCharID = func.At( 1 );
				evt.nX		= func.At( 2 );
				evt.nY		= func.At( 3 );
				GameEventManager.DispatchEvent ( evt );

			}
			else if( func.sFunc == "TALK"  ) // open talkui
			{
				#if UNITY_EDITOR
				//	return ;
				#endif
				int nID = func.At( 0 );
				GameSystem.TalkEvent( nID );
			}
			else if( func.sFunc == "BGM"  )
			{
				int nID = func.At( 0 );
				// change bgm 
				GameSystem.PlayBGM ( nID );
			}
			else if( func.sFunc == "SAY" )
			{
				TalkSayEvent evt = new TalkSayEvent();
				//evt.nType  = func.At(0);
				evt.nChar  = func.At(0);
				evt.nSayID = func.At(1);

				//Say( func.At(0), func.At(1) );
				GameEventManager.DispatchEvent ( evt  );
			}
			else if( func.sFunc == "SETCHAR" )
			{
				TalkSetCharEvent evt = new TalkSetCharEvent();
				evt.nType  = func.At(0);
				evt.nChar  = func.At(1);
				
				//Say( func.At(0), func.At(1) );
				GameEventManager.DispatchEvent ( evt  );
			}		
			else if( func.sFunc == "CHANGEBACK") 
			{
				
			}
			else if( func.sFunc  == "SAYEND") 
			{
				TalkSayEndEvent evt = new TalkSayEndEvent();
				//evt.nType = func.At(0);
				evt.nChar = func.At(0);
				GameEventManager.DispatchEvent ( evt  );
//				CloseBox( func.At(0), func.At(1) );
			}
			// stage event
			else if( func.sFunc  == "STAGEBGM") 
			{
				GameEventManager.DispatchEvent ( new StageBGMEvent()  );
				
			}

			else if( func.sFunc  == "POPGROUP")  //  pop a group of mob
			{
				
			}
			else if( func.sFunc  == "DELUNIT") 
			{			
				StageDelUnitEvent evt = new StageDelUnitEvent ();
				//evt.eCamp = (_CAMP)func.At( 0 );
				evt.nCharID = func.At( 0 );
				GameEventManager.DispatchEvent ( evt );
			}
			else 
			{
				Debug.LogError( string.Format( "Error-Can't find script func '{0}'" , func.sFunc ) );
			}
		}
	}

	public void ParserStoryScript( CTextLine line )
	{


	}
}
