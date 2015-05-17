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

