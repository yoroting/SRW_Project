using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using JsonFx.Json;
using System.ComponentModel;

[Serializable][JsonName("unit")]
public class cUnitBaseData{
	public int n_ID;
	[JsonName("char")] [DefaultValue(1)] public int n_CharID;
}
// 不斷調整後，SAVEDATA 變成了一個工具用物件

[Serializable][JsonName("save")]
public class cSaveData{
	[JsonName("ver")] [DefaultValue(1)] public int nVersion=1;
	[JsonName("idx")] [DefaultValue(0)] public int n_IDX;
	[JsonName("story")] [DefaultValue(0)] public int n_StoryID;
	[JsonName("stage")] [DefaultValue(0)] public int n_StageID;

	[JsonName("act")] [DefaultValue( _CAMP._PLAYER )] public _CAMP e_Camp ;
	[JsonName("round")] [DefaultValue(0)] public int n_Round;
	[JsonName("mon")] [DefaultValue(0)]public int n_Money;
   

//	 string sFileName;

//	 public cSaveData( int nIdx )
//	 {
//		sFileName = "savedata" + nIdx.ToString() +".dat";
//
//		CharPool = new Dictionary< int ,cUnitBaseData > ();
//		ItemPool = new List<int> ();
//	 }

	[JsonName("cpool")] public Dictionary< int ,cUnitBaseData > CharPool;
	[JsonName("ipool")]public List<int>					ItemPool;

	// write data to save
	public void SetData( int nIdx )
	{
		n_IDX = nIdx;

		//把所有要記錄的都寫在這
		n_StoryID = GameDataManager.Instance.nStoryID;
		n_StageID = GameDataManager.Instance.nStageID;
		n_Round = GameDataManager.Instance.nRound;
		e_Camp = GameDataManager.Instance.nActiveCamp;
		n_Money = GameDataManager.Instance.nMoney;
		// item list
	}

	//將遊戲還原到 紀錄的狀態
	public void RestoreData( )
	{
		GameDataManager.Instance.nStoryID = n_StoryID;
		GameDataManager.Instance.nStageID = n_StageID;
		GameDataManager.Instance.nRound   = n_Round;
		GameDataManager.Instance.nActiveCamp = e_Camp ;
		GameDataManager.Instance.nMoney = n_Money;
	}

	static public string GetKey( int Idx )
	{
		return "save" + Idx.ToString() ;
	}

	static public bool Load( int Idx )
	{
		string sKeyName = GetKey( Idx );
		string sJson = PlayerPrefs.GetString ( sKeyName , "" );
		if (string.IsNullOrEmpty (sJson))
			return false;
		// ---- DESERIALIZATION ----
		
		JsonReaderSettings readerSettings = new JsonReaderSettings();
		readerSettings.TypeHintName = "__type";
		
		JsonReader reader = new JsonReader(sJson, readerSettings);
		
		cSaveData save = (cSaveData)reader.Deserialize();
		save.RestoreData ();
		//parameters = (Dictionary<string, object>)reader.Deserialize();
		return true;
	}

	static public bool Save( int nID  )
	{
		cSaveData save = new cSaveData ();
		save.SetData ( nID );

		string sKeyName = GetKey( nID );
		// ---- SERIALIZATION ----
		
		JsonWriterSettings writerSettings = new JsonWriterSettings();
		writerSettings.TypeHintName = "__type";
		
		StringBuilder json = new StringBuilder();
		JsonWriter writer = new JsonWriter(json, writerSettings);
		writer.Write(save);

		PlayerPrefs.SetString (sKeyName , json.ToString() );
		PlayerPrefs.Save ();
		return true;
	}
}