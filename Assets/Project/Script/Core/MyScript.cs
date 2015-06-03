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
			if( func.sFunc == "ALLDEAD" )
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
				int charid = func.At( 0 );
				StagePopCharEvent evt = new StagePopCharEvent ();
				evt.nCharID = func.At( 0 );
				evt.nX		= func.At( 1 );
				evt.nY		= func.At( 2 );
				GameEventManager.DispatchEvent ( evt );
			}
			else if( func.sFunc == "POPMOB" )
			{
				int charid = func.At( 0 );
				StagePopMobEvent evt = new StagePopMobEvent ();
				evt.nCharID = func.At( 0 );
				evt.nX		= func.At( 1 );
				evt.nY		= func.At( 2 );
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
			else if( func.sFunc  == "SCLOSE") 
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
			else if( func.sFunc  == "DELCHAR") 
			{
				int charid = func.At( 0 );
				StageDelCharEvent evt = new StageDelCharEvent ();
				evt.nCharID = func.At( 0 );
				GameEventManager.DispatchEvent ( evt );
			}
			else if( func.sFunc  == "DELMOB") 
			{
				int charid = func.At( 0 );
				StageDelMobEvent evt = new StageDelMobEvent ();
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
