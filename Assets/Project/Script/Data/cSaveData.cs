using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using JsonFx.Json;

public class cUnitBaseData{
	public int n_ID;
}

public class cSaveData{
	 public int nVersion=1;
     public int n_ID;
	 public int n_StoryID;
	 public int n_StageID;

	 public int n_Round;
     public int n_Money;
   

//	 string sFileName;

//	 public cSaveData( int nIdx )
//	 {
//		sFileName = "savedata" + nIdx.ToString() +".dat";
//
//		CharPool = new Dictionary< int ,cUnitBaseData > ();
//		ItemPool = new List<int> ();
//	 }

	public Dictionary< int ,cUnitBaseData > CharPool;
	public List<int>					ItemPool;


	static public string GetKey( int Idx )
	{
		return "save" + Idx.ToString() ;
	}

	static public cSaveData Load( int Idx )
	{
		string sKeyName = GetKey( Idx );
		string sJson = PlayerPrefs.GetString ( sKeyName , "" );
		if (string.IsNullOrEmpty (sJson))
			return null;
		// ---- DESERIALIZATION ----
		
		JsonReaderSettings readerSettings = new JsonReaderSettings();
		readerSettings.TypeHintName = "__type";
		
		JsonReader reader = new JsonReader(sJson, readerSettings);
		
		cSaveData save = (cSaveData)reader.Deserialize();
		//parameters = (Dictionary<string, object>)reader.Deserialize();
		return save;
	}

	public bool Save( int Idx )
	{
		string sKeyName = GetKey( Idx );
		// ---- SERIALIZATION ----
		
		JsonWriterSettings writerSettings = new JsonWriterSettings();
		writerSettings.TypeHintName = "__type";
		
		StringBuilder json = new StringBuilder();
		JsonWriter writer = new JsonWriter(json, writerSettings);
		writer.Write(this);


		PlayerPrefs.SetString (sKeyName , json.ToString() );
		PlayerPrefs.Save ();
		return true;
	}
}



