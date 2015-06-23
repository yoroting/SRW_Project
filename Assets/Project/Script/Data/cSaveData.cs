using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class cSaveData{
	 public int nVersion=1;
     public int n_ID;
	 public int n_StoryID;
	 public int n_StageID;

	 public int n_Round;

     public int n_Money;
     //public int n_TOTALEXP;
     public int n_MAX_ACT;
     public int n_MAX_BATTLE;
     public int n_MAX_COST;

	 string sFileName;
	 public cSaveData( int nIdx )
	 {
		sFileName = "savedata" + nIdx.ToString() +".dat";

		CharPool = new Dictionary< int ,cUnitSaveData > ();
	 }

	Dictionary< int ,cUnitSaveData > CharPool;

	
	public bool Load()
	{
		FileStream fileStream = new FileStream(sFileName, FileMode.Open);
		if (fileStream == null) return false;
		BinaryReader bReader = new BinaryReader(fileStream);
		if (bReader == null) return false;

		try
		{
		}
		finally
		{
			fileStream.Close();               
		}
		// create path findere map
		
		//InitializePathFindMap (); // maybe move out later
		
		return true;
	}

	public bool Save(string sFileName)
	{
		FileStream fileStream = new FileStream(sFileName, FileMode.OpenOrCreate);
		BinaryWriter bWriter = new BinaryWriter(fileStream);
		
		
		try
		{

		}
		finally
		{
			fileStream.Close();               
		}
		// create path findere map
			
		//InitializePathFindMap (); // maybe move out later			
		return true;
	}
}


public class cUnitSaveData{
	public int n_ID;
}
