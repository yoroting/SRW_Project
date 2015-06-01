﻿using UnityEngine;
using System.Collections;
using _SRW;

// start Stage
public class StoryStartStageEvent: GameEvent {
	public const string Name = "StoryStartStageEvent";

	public int StageID = 0; //used for substage
	public StoryStartStageEvent(){
		name = Name;		// important.  must have this line as key
	}
}


// Stage event 
public class StageBGMEvent: GameEvent {
	public const string Name = "StageBGMEvent";
	
	public int nID = 0; 
	public StageBGMEvent(){
		name = Name;		// important.  must have this line as key
	}
}

public class StagePopCharEvent: GameEvent {
	public const string Name = "StagePopCharEvent";
	
	public int nCharID = 0; 
	public int nX	   ; 
	public int nY	   ; 
	public int nValue1 ; 
	public int nValue2 ; 

	public StagePopCharEvent(){
		name = Name;		// important.  must have this line as key
	}
}

public class StagePopMobEvent: GameEvent {
	public const string Name = "StagePopMobEvent";
	
	public int nCharID = 0; 
	public int nX	   ; 
	public int nY	   ; 
	public int nValue1 ; 
	public int nValue2 ; 
	
	public StagePopMobEvent(){
		name = Name;		// important.  must have this line as key
	}
}

public class StagePopMobGroupEvent: GameEvent {
	public const string Name = "StagePopMobGroupEvent";
	
	public int nGroupID = 0; 
	public int nX	   ; 
	public int nY	   ; 
	public int nValue1 ; 
	public int nValue2 ; 
	
	public StagePopMobGroupEvent(){
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


public class StageDelCharEvent: GameEvent {
	public const string Name = "StageDelCharEvent";
	public int nCharID = 0; 
	public int nValue1 ; 
	public int nValue2 ; 
	
	public StageDelCharEvent(){
		name = Name;		// important.  must have this line as key
	}
}

public class StageDelMobEvent: GameEvent {
	public const string Name = "StageDelMobEvent";
	
	public int nCharID = 0;
	public int nValue1 ; 
	public int nValue2 ; 
	
	public StageDelMobEvent(){
		name = Name;		// important.  must have this line as key
	}
}


public class StageCharMoveEvent: GameEvent {
	public const string Name = "StageCharMoveEvent";
	public int nIdent = 0;
	public int nX = 0;
	public int nY = 0;

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
public class StageUnitActionFinishEvent: GameEvent {
	public const string Name = "StageUnitActionFinishEvent";
	public int nIdent = 0;
	public StageUnitActionFinishEvent(){
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
