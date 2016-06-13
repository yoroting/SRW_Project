using UnityEngine;
using System.Collections;
//using _SRW;

// start Stage
//public class StoryStartStageEvent: GameEvent {
//	public const string Name = "StoryStartStageEvent";
//
//	public int StageID = 0; //used for substage
//	public StoryStartStageEvent(){
//		name = Name;		// important.  must have this line as key
//	}
//}


// Stage event 
public class StageBGMEvent: GameEvent {
	public const string Name = "StageBGMEvent";
	
	public int nID = 0; 
	public StageBGMEvent(){
		name = Name;		// important.  must have this line as key
	}
}


public class StagePopUnitEvent: GameEvent {
	public const string Name = "StagePopUnitEvent";

	public int nCharID = 0; 
	public _CAMP eCamp =  _CAMP._PLAYER ; 
	public int nX	   ; 
	public int nY	   ; 
	public int nValue1 ; 
	public int nValue2 ;
    public int nRadius;


    public StagePopUnitEvent(){
		name = Name;		// important.  must have this line as key
	}
}
public class StageDelUnitEvent: GameEvent {
	public const string Name = "StageDelUnitEvent";
	public _CAMP eCamp=  _CAMP._PLAYER ;
	public int nCharID = 0; 
	public int nDelType = 0; 
	public StageDelUnitEvent(){
		name = Name;		// important.  must have this line as key
	}
}



public class StagePopGroupEvent: GameEvent {
	public const string Name = "StagePopGroupEvent";
	public _CAMP eCamp=  _CAMP._ENEMY ;
//	public int nGroupID = 0; 
	public int nLeaderCharID; 
	public int nCharID ; 

	public int stX	   ; 
	public int stY	   ;
	public int edX	   ; 
	public int edY	   ; 

	public int nPopType ;  //分布方式 aggign a AOE type 
//	public int nValue1 ; 
//	public int nValue2 ; 
	
	public StagePopGroupEvent(){
		name = Name;		// important.  must have this line as key
	}
}

public class StageDelUnitByIdentEvent: GameEvent {
	public const string Name = "StageDelUnitByIdentEvent";
	public _CAMP eCamp=  _CAMP._PLAYER ;
	public int nIdent = 0; 

	public StageDelUnitByIdentEvent(){
		name = Name;		// important.  must have this line as key
	}
}


public class StageCharMoveEvent: GameEvent {
	public const string Name = "StageCharMoveEvent";
	public int nIdent = 0;
	public int nX = 0;
	public int nY = 0;
	public int nCharID =0; 
	public StageCharMoveEvent(){
		name = Name;		// important.  must have this line as key
	}
}


public class StageShowMoveRangeEvent: GameEvent {
	public const string Name = "StageShowMoveRangeEvent";
	public int nIdent = 0;
	public int nX = 0;
	public int nY = 0;
	
	public StageShowMoveRangeEvent(){
		name = Name;		// important.  must have this line as key
	}
}

public class StageShowAttackRangeEvent: GameEvent {
	public const string Name = "StageShowAttackRangeEvent";
	public int nIdent = 0;
	public int nX = 0;
	public int nY = 0;
	
	public StageShowAttackRangeEvent(){
		name = Name;		// important.  must have this line as key
	}
}

public class StageRestorePosEvent: GameEvent {
	public const string Name = "StageRestorePosEvent";
	public int nIdent = 0;
	public int nX = 0;
	public int nY = 0;
	
	public StageRestorePosEvent(){
		name = Name;		// important.  must have this line as key
	}
}
// unit action finished
//public class StageUnitActionFinishEvent: GameEvent {
//	public const string Name = "StageUnitActionFinishEvent";
//	public int nIdent = 0;
//	public StageUnitActionFinishEvent(){
//		name = Name;		// important.  must have this line as key
//	}
//}
public class StageMoveToUnitEvent: GameEvent {
	public const string Name = "StageMoveToUnitEvent";
	public int nAtkCharID = 0;
	public int nDefCharID = 0;
	
	public StageMoveToUnitEvent(){
		name = Name;		// important.  must have this line as key
	}
}

// unit action finished
public class StageWeakUpCampEvent: GameEvent {
	public const string Name = "StageWeakUpCampEvent";
	public _CAMP nCamp = _CAMP._PLAYER;
	public StageWeakUpCampEvent(){
		name = Name;		// important.  must have this line as key
	}
}


// Char Cmd UI Event
public class CmdCharMoveEvent: GameEvent {
	public const string Name = "CmdCharMoveEvent";
	public int nIdent = 0;
	public int nX = 0;
	public int nY = 0;
	
	public CmdCharMoveEvent(){
		name = Name;		// important.  must have this line as key
	}
}

// Talk UI EVent
public class TalkClickEvent : GameEvent
{
    public const string Name = "TalkClickEvent";
    public int nType;

    public TalkClickEvent()
    {
        name = Name;        // important.  must have this line as key
    }
}

public class TalkSayEvent: GameEvent {
	public const string Name = "TalkSayEvent";
	public int nChar = 0;
	public int nType  ;
	public int nSayID ;
    public int nReplaceID;
    public int nReplaceType;

    public TalkSayEvent(){
		name = Name;		// important.  must have this line as key
	}
}

public class TalkSetCharEvent: GameEvent {
	public const string Name = "TalkSetCharEvent";
	public int nChar = 0;
	public int nType  ;
    public int nReplaceID;
    public int nReplaceType;
    public TalkSetCharEvent(){
		name = Name;		// important.  must have this line as key
	}
}


// Talk UI EVent
public class TalkSayEndEvent: GameEvent {
	public const string Name = "TalkSayEndEvent";
	public int nChar = 0;
	public int nType  ;
	
	public TalkSayEndEvent(){
		name = Name;		// important.  must have this line as key
	}
}
public class TalkBackGroundEvent: GameEvent {
	public const string Name = "TalkBackGroundEvent";
	public int nBackGroundID = 0;
    public int nSoundID = 0;
    public int nType =0 ;
	
	public TalkBackGroundEvent(){
		name = Name;		// important.  must have this line as key
	}
}
public class TalkDeadEvent: GameEvent {
	public const string Name = "TalkDeadEvent";
	public int nChar = 0;
    public int nSoundID = 0;
    public TalkDeadEvent(){
		name = Name;		// important.  must have this line as key
	}
}
public class TalkShakeEvent: GameEvent {
	public const string Name = "TalkShakeEvent";
	public int nChar = 0;
    public int nSoundID = 0;
    public TalkShakeEvent(){
		name = Name;		// important.  must have this line as key
	}
}
public class TalkFaceEvent : GameEvent
{
    public const string Name = "TalkFaceEvent";
    public int nChar = 0;
    public int nFaceID = 0;
    public TalkFaceEvent()
    {
        name = Name;        // important.  must have this line as key
    }
}
// Battle
public class StageBattleAttackEvent: GameEvent {
	public const string Name = "StageBattleAttackEvent";
	public int nAtkCharID = 0;
	public int nDefCharID = 0;
	public int nAtkSkillID = 0;
    public int nNum = 0;
    public int nResult = 0;
    public int nVar1 = 0;               // 節果表演變數
    public int nVar2 = 0;
    public StageBattleAttackEvent(){
		name = Name;		// important.  must have this line as key
	}
}

public class StageBattleCastEvent: GameEvent {
	public const string Name = "StageBattleCastEvent";
	public int nAtkCharID = 0;
	public int nDefCharID = 0;
	public int nAtkSkillID = 0;
	public int nTargetX = 0;
	public int nTargetY = 0;
    public int nResult = 0;

    public StageBattleCastEvent(){
		name = Name;		// important.  must have this line as key
	}
}

// cast buff on unit
//public class StageBattleCastBuffEvent: GameEvent {
//	public const string Name = "StageBattleCastBuffEvent";
//	public int nAtkCharID = 0;
//	public int nDefCharID = 0;
//	public int nAtkBuffID = 0;
//
//	public StageBattleCastBuffEvent(){
//		name = Name;		// important.  must have this line as key
//	}
//}


