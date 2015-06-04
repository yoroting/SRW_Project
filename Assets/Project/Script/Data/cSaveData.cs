using System.Collections.Generic;

public class cSaveData{
     public int n_ID;
	 public int n_Round;
     public int n_Money;
     //public int n_TOTALEXP;
     public int n_MAX_ACT;
     public int n_MAX_BATTLE;
     public int n_MAX_COST;

	 public cSaveData()
	 {
		CharPool = new Dictionary< int ,cUnitSaveData > ();
	 }

	Dictionary< int ,cUnitSaveData > CharPool;

}


public class cUnitSaveData{
	public int n_ID;
}
