using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Panel_Mainten : MonoBehaviour {
	public const string Name = "Panel_Mainten";
	//public GameObject btnUnit;
	public GameObject btnSave;
	public GameObject btnLoad;
    public GameObject btnItem;    
	public GameObject btnGameEnd;

    public GameObject btnNextStage;

    public GameObject lblStars;
    public GameObject lblMoney;

    public GameObject GridUnitList;
    public GameObject CharUnit;




    public GameObject btnCheat;

	public GameObject lblStoryName;
 //   public GameObject scrollviewList;
 //   UIScrollView scrollview;

    // Use this for initialization
    void OnEnable()
	{
		GameSystem.PlayBGM ( 7 );

       
        SetStoryName();
        // re list

        // clear all
        CharUnit.RecycleAll();
        UIGrid grid = GridUnitList.GetComponent<UIGrid>();
        if (grid != null)
        {
            MyTool.DestoryGridItem(grid);
            //while (grid.transform.childCount > 0)
            //{
            //    DestroyImmediate(grid.transform.GetChild(0).gameObject);
            //}
        }
        // ReloadUnitList();
        //
        Resources.UnloadUnusedAssets();
     //   System.GC.Collect();			// Free memory resource here
    }

	void Start () {
		UIEventListener.Get(btnNextStage).onClick += OnNextStageClick; // for trig next lineev
		//UIEventListener.Get(btnUnit).onClick += OnUnitClick; // for trig next lineev
		UIEventListener.Get(btnSave).onClick += OnSaveClick; // for trig next lineev
		UIEventListener.Get(btnLoad).onClick += OnLoadClick; // for trig next lineev
        UIEventListener.Get(btnItem).onClick += OnItemClick; //
        //UIEventListener.Get(btnGameEnd).onClick += OnEndClick; // for trig next lineev
        UIEventListener.Get(btnGameEnd).onClick += OnSetupClick; // for trig next lineev

        UIEventListener.Get(btnCheat).onClick += OnCheatClick; // cheat

        CharUnit.CreatePool(5);

        if (CharUnit != null)
        {
            CharUnit.SetActive(false);
        }
        //if (scrollviewList != null) {
        //    scrollview = scrollviewList.GetComponent<UIScrollView>();
        //}

    }

    // Update is called once per frame
    void Update () {
        if (GameDataManager.Instance != null)
        {
            MyTool.SetLabelInt(lblStars, GameDataManager.Instance.nStars);
            MyTool.SetLabelInt(lblMoney, GameDataManager.Instance.nMoney);
        }
    }

	public void OnPopReady()
	{
		// on ready alpha done
		GameDataManager.Instance.ePhase = _SAVE_PHASE._MAINTEN;		// save to mainta phase

	//	GameSystem.bFXPlayMode = true;								// start play fx
		// close stage UI!!
		if (PanelManager.Instance.CheckUIIsOpening (Panel_StageUI.Name)) {


			// free here waill cause some  StartCoroutine of stageUI break 
			if( cSaveData.IsLoading()== false ){
				 PanelManager.Instance.DestoryUI( Panel_StageUI.Name ); 
			//PanelManager.Instance.CloseUI( Panel_StageUI.Name );
			// need to free all stage resource

			// close loading
				PanelManager.Instance.CloseUI( "Panel_Loading");
			}

		}

        ReloadUnitList();

    }
	//public void ChooseNextStage()
	//{
	//	GameDataManager.Instance.nStoryID += 1;
	//}

	IEnumerator EnterStory( int nStoryID )
	{
        PanelManager.Instance.OpenUI("Panel_Loading");

        // back up char pool
        GameDataManager.Instance.PrepareEnterStage();

        yield return new WaitForEndOfFrame();

        GameDataManager.Instance.nStoryID = nStoryID;
		yield return  new WaitForEndOfFrame();

		PanelManager.Instance.OpenUI( StoryUIPanel.Name );
		yield return  new WaitForEndOfFrame();
		
		
		PanelManager.Instance.DestoryUI( Name );  			// close main 
		yield break;
		
	}


	void OnNextStageClick( GameObject go )
	{
        //	GameDataManager.Instance.nStoryID += 1;
        //GameDataManager.Instance.nStageID += 1;
        //		PanelManager.Instance.OpenUI ( StoryUIPanel.Name );
        //		PanelManager.Instance.CloseUI ( Name );			// close this ui

        //		ChooseNextStage();

        if (GameDataManager.Instance.nStoryID == 0)
        {
            GameDataManager.Instance.nStoryID = 1;
        }

        GameSystem.PlaySound(141);
        //  StartCoroutine ( EnterStory( GameDataManager.Instance.nStoryID ) );
        Panel_Loading.StartLoad(null, Panel_Loading._LOAD_TYPE._STORY);

        PanelManager.Instance.CloseUI(Name);            // close this ui

    }
	void OnUnitClick( GameObject go )
	{

	}
	void OnSaveClick( GameObject go )
	{
		Panel_SaveLoad.OpenSaveMode ( _SAVE_PHASE._MAINTEN );
        GameSystem.BtnSound();
        //	cSaveData.Save ( 1 , _SAVE_PHASE._MAINTEN );
    }
	void OnLoadClick( GameObject go )
	{
		Panel_SaveLoad.OpenLoadMode ( _SAVE_PHASE._MAINTEN );
        GameSystem.BtnSound();
        //	cSaveData.Load ( 1 , _SAVE_PHASE._MAINTEN );
        //StartCoroutine ( SaveLoading( save) 

    }
    void OnItemClick(GameObject go)
    {
        Panel_ItemList.Open(0);
        GameSystem.BtnSound();
    }

    void OnSetupClick(GameObject go)
    {
        Panel_SystemSetting.OpenUI();
        GameSystem.BtnSound();
    }
    


    void OnEndClick( GameObject go )
	{
		PanelManager.Instance.OpenUI( MainUIPanel.Name );

		PanelManager.Instance.CloseUI( Name );
        GameSystem.BtnSound(3);
    }

	void OnCheatClick( GameObject go )
	{
		PanelManager.Instance.OpenUI( Panel_SysCheat.Name );
        GameSystem.BtnSound();

    }

//	//===========================================================
//	IEnumerator SaveLoading( cSaveData save )
//	{
//		//GameDataManager.Instance.nStoryID = nStoryID;
//		//GameDataManager.Instance.nStageID = save.n_StageID;
		
//		PanelManager.Instance.OpenUI( "Panel_Loading");

//    //    System.Threading.Thread.Sleep(5000);

//		yield return  new WaitForEndOfFrame();
		
//		if (save.ePhase == _SAVE_PHASE._MAINTEN) {
//			//PanelManager.Instance.OpenUI ( Panel_Mainten.Name );

//			RestoreBySaveData( save );

//		} else if (save.ePhase == _SAVE_PHASE._STAGE) {
			
//			PanelManager.Instance.OpenUI( Panel_StageUI.Name );  // don't run start() during open
////			Panel_StageUI.Instance.bIsRestoreData = true;
//			yield return  new WaitForEndOfFrame();
//			Panel_StageUI.Instance.RestoreBySaveData ( save );
//			yield return  new WaitForEndOfFrame ();
//		}		
//		// close loadint UI
//		PanelManager.Instance.CloseUI( "Panel_Loading");

//		cSaveData.SetLoading (false);

//		if (save.ePhase != _SAVE_PHASE._MAINTEN) {
//			PanelManager.Instance.DestoryUI (Name);  			// close main 
//		}
//		yield break;
//	}

	public void LoadSaveGame( cSaveData save )
	{
		if (save  == null)
			return;
        //   PanelManager.Instance.OpenUI("Panel_Loading");

        Panel_Loading.StartLoad(save, Panel_Loading._LOAD_TYPE._SAVE_DATA);
        //StartCoroutine ( SaveLoading( save) );

        PanelManager.Instance.CloseUI(Name); // close self
        //   PanelManager.Instance.CloseUI("Panel_Loading");
    }

	public bool RestoreBySaveData( cSaveData save  )
	{
      
        //	cSaveData save = GameDataManager.Instance.SaveData;
        if (save == null)
			return false;
		
		
		System.GC.Collect ();			// Free memory resource here

		if (save.ePhase == _SAVE_PHASE._STAGE) {
			// restore to mainten ui

		}	
	
	//	SetStoryName ();
		GameDataManager.Instance.ePhase = _SAVE_PHASE._MAINTEN;     // save to stage phase

        GameSystem.PlayBGM(7);

        SetStoryName();
        // re list
        ReloadUnitList();


        return true;
	}

	public void SetStoryName( )
	{
		if( lblStoryName != null ){
			MyTool.SetLabelText( lblStoryName , MyTool.GetStoryName(  GameDataManager.Instance.nStoryID ) );
		}
	}

    public void ReloadUnitList( int nCharID = 0 ) // 0- all
    {
        //if (nCharID != 0) {


        //    // sort grid resort

        //    return;
        //}
        // remove leave unit
        STAGE_DATA StageData = ConstDataManager.Instance.GetRow<STAGE_DATA>(GameDataManager.Instance.nStoryID);
        if (StageData != null)
        {
            // create avoid list
            char[] split = { ';', ' ', ',' };
            string[] strLeave = StageData.s_LEAVE_CHAR.Split(split);
            for (int i = 0; i < strLeave.Length; i++)
            {
                int nLeave = 0;
                if (int.TryParse(strLeave[i], out nLeave))
                {
                    if (0 == nLeave)
                        continue;
                   //leaveList.Add(nAvoid);
                   GameDataManager.Instance.EnableStorageUnit(nLeave, false);
                }
            }
        }



        // clear all
        CharUnit.RecycleAll();
        

        UIGrid grid = GridUnitList.GetComponent<UIGrid>();
        if (grid != null) {
            MyTool.DestoryGridItem(grid);
            //while (grid.transform.childCount > 0)
            //{
            //    DestroyImmediate(grid.transform.GetChild(0).gameObject);
            //}
        }

        // sort by mar
     
        var items = from pair in GameDataManager.Instance.StoragePool
                    orderby pair.Value.GetMar() descending select pair;


        // release all unit
        //foreach (var pair in GameDataManager.Instance.StoragePool)
        foreach (var pair in items)
        {
            if (pair.Value == null)
                continue;


            if (pair.Value.bEnable == false)
            {
                if (Config.SHOW_LEAVE == false) { 
                    continue;
                }
            }

            GameObject obj = CharUnit.Spawn(GridUnitList.transform);
            if (obj != null)
            {
                obj.SetActive(true);
                Mainten_Unit unit = obj.GetComponent<Mainten_Unit>();
                if (unit != null)
                {
                    unit.ReSize();
                    unit.SetData(pair.Value , 0  );
                }
            }
        }

        grid.repositionNow = true;  // for re pos
    }

}
