using UnityEngine;
using System.Collections;

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

// Char Cmd Event
public class StageCharMoveEvent: GameEvent {
	public const string Name = "StageCharMoveEvent";
	public int nIdent = 0;
	public int nX = 0;
	public int nY = 0;

	public StageCharMoveEvent(){
		name = Name;		// important.  must have this line as key
	}
}