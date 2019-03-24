using UnityEngine;
using System.Collections;

public class Panel_SaveLoad : MonoBehaviour {
	public const string Name = "Panel_SaveLoad";

	public GameObject lblTitleObj;
    public GameObject lblPageObj;
    public GameObject GridObj;
	public GameObject CancelButton;

	public GameObject ItemSaveDataObj;


	public _SAVE_PHASE		ePhase;

    int n_Mode = 0;     // 0- save , 1- load
	int  n_MaxRecords = 4; // 一頁多少資料

    int n_Page = 0;  // current
    int n_MaxPage = 99;

    


    // Use this for initialization
    void Start () {
		if (ItemSaveDataObj != null) {
			ItemSaveDataObj.SetActive( false );
		}

		UIEventListener.Get(CancelButton).onClick = OnCancelClick; // 	


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
        n_Mode = 0; // save mode
        ePhase = phase;
        UIGrid grid = GridObj.GetComponent<UIGrid>();
        
        MyTool.SetLabelText (lblTitleObj, "紀錄");
		MyTool.DestoryGridItem (grid);
        Reload();
        //for (int i = 1; i <= 4; i++) {
        //	GameObject go = ResourcesManager.CreatePrefabGameObj( GridObj , "Prefab/Item_SaveData" ); 
        //	if( go == null )
        //		continue;

        //	UIEventListener.Get(go).onClick = OnSaveClick; // 			

        //	Item_SaveData obj = go.GetComponent<Item_SaveData >();
        //	if( obj != null ){
        //		obj.SetData( i );
        //	}
        //}
        MyTool.ResetScrollView(grid);

    }
	public void SetLoad (  _SAVE_PHASE phase )
	{
        n_Mode = 1; // Load mode
        ePhase = phase;
        UIGrid grid = GridObj.GetComponent<UIGrid>();
        MyTool.SetLabelText (lblTitleObj, "讀取");
		MyTool.DestoryGridItem (grid);
        Reload();

        //      for (int i = 1; i <= 4; i++) {
        //	GameObject go = ResourcesManager.CreatePrefabGameObj( GridObj , "Prefab/Item_SaveData" ); 

        //	if( go == null )
        //		continue;

        //	UIEventListener.Get(go).onClick = OnLoadClick; // 


        //	Item_SaveData obj = go.GetComponent<Item_SaveData >();
        //	if( obj != null ){
        //		obj.SetData( i );
        //	}
        //}
        MyTool.ResetScrollView(grid);
    }
	void OnCancelClick(GameObject go)
	{
        GameSystem.BtnSound(1);
        PanelManager.Instance.CloseUI (Name);
       
    }

	void OnSaveClick(GameObject go)
	{        
        GameSystem.BtnSound();
        Item_SaveData obj = go.GetComponent<Item_SaveData >();
		if (obj != null) {
			cSaveData.Save( obj.nID , ePhase );

			PanelManager.Instance.CloseUI (Name);
		}
	}

    void OnLoadClick(GameObject go)
    {   
        if (cSaveData.IsLoading()){ return; }

        GameSystem.BtnSound();
        Item_SaveData obj = go.GetComponent<Item_SaveData >();
		if (obj != null) {
			if( cSaveData.Load( obj.nID , ePhase ) ){
				PanelManager.Instance.CloseUI (Name);
			}
		}
	}


    public void OnNextClick()
    {
        //邊界不處理
        if (n_Page >= (n_MaxPage - 1))
        {
            GameSystem.BtnSound(2);
            return;
        }
        GameSystem.BtnSound();
        if (++n_Page >= n_MaxPage)
        {
            n_Page = n_MaxPage-1;
        }
        Reload();



    }
    public void OnPrevClick()
    {
        if (n_Page <= 0)
        {
            GameSystem.BtnSound(2);
            return;
        }
        GameSystem.BtnSound(0);
        if (--n_Page < 0)
        {
            n_Page = 0;
        }
        Reload();

    }

    void Reload()
    {
        string spage = string.Format( "{0}/{1}", n_Page+1 , n_MaxPage );
        MyTool.SetLabelText(lblPageObj, spage);
        //  n_MaxPage;
        UIGrid grid = GridObj.GetComponent<UIGrid>();
        MyTool.DestoryGridItem(grid);
        int nShift = n_Page * n_MaxRecords;
        for (int i = 1; i <= n_MaxRecords; i++)
        {
            int nIdx = i + (nShift);

            GameObject go = ResourcesManager.CreatePrefabGameObj(GridObj, "Prefab/Item_SaveData");

            if (go == null)
                continue;

            if(n_Mode == 0 )     // save
                UIEventListener.Get(go).onClick = OnSaveClick; // 
            else       // LOAD
                UIEventListener.Get(go).onClick = OnLoadClick; // 


            Item_SaveData obj = go.GetComponent<Item_SaveData>();
            if (obj != null)
            {
                obj.SetData( nIdx );
            }
        }
        MyTool.ResetScrollView(grid);
    }
}
