using UnityEngine;
using System.Collections;

public class Panel_SaveLoad : MonoBehaviour {
	public const string Name = "Panel_SaveLoad";

	public GameObject lblTitleObj;
	public GameObject GridObj;
	public GameObject CancelButton;

	public GameObject ItemSaveDataObj;


	public _SAVE_PHASE		ePhase;

	int  n_MaxRecords = 4;



	// Use this for initialization
	void Start () {
		if (ItemSaveDataObj != null) {
			ItemSaveDataObj.SetActive( false );
		}

		UIEventListener.Get(CancelButton).onClick += OnCancelClick; // 	


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	static public Panel_SaveLoad OpenSaveMode( _SAVE_PHASE phase )
	{
		Panel_SaveLoad panel = MyTool.GetPanel< Panel_SaveLoad >(  PanelManager.Instance.OpenUI (Name) ) ;
		if (panel != null) {
			panel.SetSave( phase );
		}

		return panel;
	}
	static public Panel_SaveLoad OpenLoadMode( _SAVE_PHASE phase )
	{
		Panel_SaveLoad panel = MyTool.GetPanel< Panel_SaveLoad >(  PanelManager.Instance.OpenUI (Name) ) ;
		if (panel != null) {
			panel.SetLoad( phase );
		}

		return panel;
	}


	public void SetSave ( _SAVE_PHASE phase )
	{
		ePhase = phase;
		MyTool.SetLabelText (lblTitleObj, "紀錄");

		MyTool.DestoryGridItem ( GridObj );

		for (int i = 1; i <= 4; i++) {
			GameObject go = ResourcesManager.CreatePrefabGameObj( GridObj , "Prefab/Item_SaveData" ); 
			if( go == null )
				continue;

			UIEventListener.Get(go).onClick += OnSaveClick; // 			
			
			Item_SaveData obj = go.GetComponent<Item_SaveData >();
			if( obj != null ){
				obj.SetData( i );
			}
		}


	}
	public void SetLoad (  _SAVE_PHASE phase )
	{
		ePhase = phase;
		MyTool.SetLabelText (lblTitleObj, "讀取");
		MyTool.DestoryGridItem ( GridObj );

		for (int i = 1; i <= 4; i++) {
			GameObject go = ResourcesManager.CreatePrefabGameObj( GridObj , "Prefab/Item_SaveData" ); 

			if( go == null )
				continue;

			UIEventListener.Get(go).onClick += OnLoadClick; // 
			

			Item_SaveData obj = go.GetComponent<Item_SaveData >();
			if( obj != null ){
				obj.SetData( i );
			}
		}

	}
	void OnCancelClick(GameObject go)
	{
		PanelManager.Instance.CloseUI (Name);
	}

	void OnSaveClick(GameObject go)
	{
		Item_SaveData obj = go.GetComponent<Item_SaveData >();
		if (obj != null) {
			cSaveData.Save( obj.nID , ePhase );

			PanelManager.Instance.CloseUI (Name);
		}
	}

	void OnLoadClick(GameObject go)
	{
		Item_SaveData obj = go.GetComponent<Item_SaveData >();
		if (obj != null) {
			cSaveData.Load( obj.nID , ePhase );

			PanelManager.Instance.CloseUI (Name);
		}
	}
}
