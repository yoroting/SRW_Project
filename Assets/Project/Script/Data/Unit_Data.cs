using System.Collections;
using System.Collections.Generic;

// current stage runtime data
public class UNIT_DATA{
	public int n_Ident;		// auto create by game system
	public int n_CharID;
	public int n_EXP;
	public int n_X;
	public int n_Y;
	public int n_HP;
	public int n_DEF;

	public int n_ActID;
	// Buff list
}

//
public class cMobGroup
{
	public int nGroupID{ set; get; }
	public cMobGroup()
	{
		memList = new List< UNIT_DATA >{};

	}


	public List< UNIT_DATA > memList ;
}