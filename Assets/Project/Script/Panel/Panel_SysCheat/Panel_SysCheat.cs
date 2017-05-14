using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel_SysCheat : MonoBehaviour {

	public const string Name = "Panel_SysCheat";

	public GameObject CloseBtn;            
	public GameObject WinBtn;            
	public GameObject LostBtn;            
	public GameObject AllDieBtn;            

	public GameObject GodChk;           // God Mode
	public GameObject KillChk;           // Kill mode
	public GameObject MobAIChk;           // mob aI
    public GameObject ShowLeaveChk;           // show leave aI

    // Money
    public GameObject MoneyInput;           // Kill mode
	public GameObject MoneyBtn;           // Set money
    public GameObject MoneyInfo;           // MoneyInfo

    public GameObject StoryPoplist;           // StoryPoplist
//	public GameObject StoryInput;           // Kill mode
//	public GameObject StoryBtn;           // Set money

	// star
	public GameObject StarInput;           // star input
	public GameObject StarBtn;           	// star input

	// pop mob
	public GameObject CharIdInput;           // pop char id
	public GameObject CampPoplist;           // sel camp
	public GameObject PopMobBtn;           // Set money
	public int nPopCharID;

	// event trig
	public GameObject EventPoplist;           // sel Event
	public GameObject TrigEventBtn;           // trig Event

    // Item
    public GameObject ItemPoplist;           // sel Event
    public GameObject AddItemBtn;           // trig Event

    // roundob
    public GameObject RoundInput;
    public GameObject RoundBtn;              // star input
                                            // Use this for initialization
    void Start () {
		UIEventListener.Get(GodChk).onClick = OnGODClick; 
		UIEventListener.Get(KillChk).onClick = OnKillClick; 
		UIEventListener.Get(MobAIChk).onClick = OnMobAIClick;
        UIEventListener.Get(ShowLeaveChk).onClick = OnShowLeaveClick;
        UIEventListener.Get(CloseBtn).onClick = OnCloseClick; 
		UIEventListener.Get(WinBtn).onClick = OnWinClick; 
		UIEventListener.Get(LostBtn).onClick = OnLostClick; 

		UIEventListener.Get(AllDieBtn).onClick = OnAllDieClick; 
		//UIEventListener.Get(MoneyInput).onSubmit += OnMoneySubmit; 
		UIEventListener.Get(MoneyBtn).onClick = OnMoneyClick; 

		//UIEventListener.Get(StoryPoplist).onSubmit += OnJumpStory; 
		UIEventListener.Get(PopMobBtn).onClick = OnPopMobClick; 	
		//
		UIEventListener.Get(TrigEventBtn).onClick = OnTrigEventClick;
        UIEventListener.Get(AddItemBtn).onClick = OnAddItemClick;

        ///
        
        //      UIPopupList popList = StoryPoplist.GetComponent<UIPopupList>();
        //if (popList != null) {		
        //	//添加触发事件
        ////	EventDelegate.Add (popList.onChange, label.SetCurrentSelection);
        //	EventDelegate.Add (popList.onChange, StoryComboboxChange);

        //	popList.Clear ();
        //	DataTable storyTable = ConstDataManager.Instance.GetTable ("STAGE_STORY");
        //	if (storyTable != null) {
        //		foreach (STAGE_STORY s in  storyTable) {
        //			popList.AddItem ( MyTool.GetStoryName (s.n_ID), s );
        //		}
        //	}
        //}
        UIScrollablePopupList popList = StoryPoplist.GetComponent<UIScrollablePopupList>();
        if (popList != null)
        {
            //添加触发事件
            //	EventDelegate.Add (popList.onChange, label.SetCurrentSelection);
          EventDelegate.Add(popList.onChange, StoryComboboxChange);
            DataTable storyTable = ConstDataManager.Instance.GetTable("STAGE_STORY");
            if (storyTable != null)
            {
                popList.Clear();
                foreach (STAGE_STORY s in storyTable)
                {
                   // popList.items.Add(MyTool.GetStoryName(s.n_ID));
                      popList.AddItem(MyTool.GetStoryName(s.n_ID), s);
                }
            }
        }
       
        // camp
        UIPopupList campList = CampPoplist.GetComponent<UIPopupList>();
		if (campList != null) {		
			//添加触发事件
			//	EventDelegate.Add (popList.onChange, label.SetCurrentSelection);
			//EventDelegate.Add (popList.onChange, StoryComboboxChange);
			
			campList.Clear ();
			 
			campList.AddItem(_CAMP._PLAYER.ToString()  );
			campList.AddItem(_CAMP._ENEMY.ToString() );
			campList.AddItem(_CAMP._FRIEND.ToString());

            // set select value
            campList.value = _CAMP._PLAYER.ToString();
        }

        // Item List
        UIPopupList itemList = ItemPoplist.GetComponent<UIPopupList>();
        if (itemList != null)
        {
            //添加触发事件
            //	EventDelegate.Add (popList.onChange, label.SetCurrentSelection);
            //  EventDelegate.Add(popList.onChange, StoryComboboxChange);

            itemList.Clear();
            DataTable Table = ConstDataManager.Instance.GetTable("ITEM_MISC");
            if (Table != null)
            {
                foreach ( ITEM_MISC item in Table)
                {
                    itemList.AddItem(MyTool.GetItemName(item.n_ID), item.n_ID);
                }
            }
        }
        // round 
        UIEventListener.Get(RoundBtn).onClick = OnSetRoundClick;

        //		foreach(DataTable table in tableList)
        //		{
        //			popList.AddItem(table.Name);
        //		}; 
    }

	void OnEnable () {      
        UIToggle god = GodChk.GetComponent<UIToggle> ();
		god.value =Config.GOD;

		UIToggle kill = KillChk.GetComponent<UIToggle> ();
		kill.value =Config.KILL_MODE;

		UIToggle ai = MobAIChk.GetComponent<UIToggle> ();
		ai.value =Config.MOBAI;

        UIToggle showleave = ShowLeaveChk.GetComponent<UIToggle>();
        showleave.value = Config.SHOW_LEAVE;

        UIInput min = MoneyInput.GetComponent<UIInput> ();
		min.value = GameDataManager.Instance.nMoney.ToString();

		UIInput star = StarInput.GetComponent< UIInput> ();
		star.value = GameDataManager.Instance.nStars.ToString();

        UIInput round = RoundInput.GetComponent<UIInput>();
        round.value = GameDataManager.Instance.nRound.ToString();

        // switch story in mainten only
        if (GameDataManager.Instance.ePhase == _SAVE_PHASE._MAINTEN) {
			StoryPoplist.SetActive( true );

            //Fill value
            UIScrollablePopupList storypopList = StoryPoplist.GetComponent<UIScrollablePopupList>();
            if (storypopList != null)
            {
                storypopList.value = MyTool.GetStoryName(GameDataManager.Instance.nStoryID);
            }

        } else {
			StoryPoplist.SetActive( false );
		}
		// Event list
		UIPopupList EvtList = EventPoplist.GetComponent<UIPopupList>();
		if (EvtList != null) {		
			EvtList.Clear();

			//int nStageID = GameDataManager.Instance.nStageID;
			STAGE_DATA StageData = ConstDataManager.Instance.GetRow<STAGE_DATA>(GameDataManager.Instance.nStageID);
			if (StageData == null)
			{
				Debug.LogFormat("stageloding:StageData fail with ID {0}  ", GameDataManager.Instance.nStageID);
				return;
			}

			//===
			char[] split = { ';',' ',',' };		
			// mission
			string[] strMission = StageData.s_MISSION.Split(split);
			for (int i = 0; i < strMission.Length; i++)
			{
				int nMissionID ;
				if( int.TryParse( strMission[i],  out nMissionID ) ){
					if( nMissionID ==0 )
						continue;
                 
                        EvtList.AddItem(nMissionID.ToString(), nMissionID);
				}
			}
			
			// event
			string[] strEvent = StageData.s_EVENT.Split(split);
			for (int i = 0; i < strEvent.Length; i++)
			{
				int nEventID;
				if( int.TryParse( strEvent[i],  out nEventID ) ){
					if( nEventID ==0 )
						continue;
                    string sTmp = nEventID.ToString();
                    // check mission complete
                    if (GameDataManager.Instance.EvtDonePool.ContainsKey(nEventID))
                    {
                        int nCompleteRound = GameDataManager.Instance.EvtDonePool[nEventID];
                        sTmp += "-" + nCompleteRound;
                    }

                    EvtList.AddItem(sTmp.ToString(), nEventID);
				}
			}
		}
        // Debug Info

        UILabel pMoneyInfo = MoneyInfo.GetComponent< UILabel  >();
        if ( pMoneyInfo != null)
        {
            pMoneyInfo.text = string.Format( "+ {0} / - {1} " , GameDataManager.Instance.nEarnMoney , GameDataManager.Instance.nSpendMoney );
        }


    }

	// Update is called once per frame
	void Update () {
	
	}

	void OnCloseClick(GameObject go)
	{
        // reenable in main
        if (GameDataManager.Instance.ePhase == _SAVE_PHASE._MAINTEN)
        {
          //  Panel_Mainten p = PanelManager.Instance.JustGetUI<Panel_Mainten>(Panel_Mainten.Name);            
        }

        PanelManager.Instance.CloseUI( Name );
	}

	public void OnGODClick(GameObject go)
	{
		UIToggle ui = go.GetComponent<UIToggle> ();
		Config.GOD =  ui.value ;

	}

	public void OnKillClick(GameObject go)
	{
		UIToggle ui = go.GetComponent<UIToggle> ();
		Config.KILL_MODE =  ui.value ;
	}

	public void OnMobAIClick(GameObject go)
	{
		UIToggle ui = go.GetComponent<UIToggle> ();
		Config.MOBAI =  ui.value ;
	}

    public void OnShowLeaveClick(GameObject go)
    {
        UIToggle ui = go.GetComponent<UIToggle>();
        Config.SHOW_LEAVE = ui.value;


        if ( PanelManager.Instance.CheckUIIsOpening( Panel_Mainten.Name ) ) {
            Panel_Mainten panel = MyTool.GetPanel< Panel_Mainten >( PanelManager.Instance.OpenUI(Panel_Mainten.Name) );
            if (panel != null)
            {
                panel.ReloadUnitList();
            }
        }
    }

	public void OnMoneyClick(GameObject go)
	{
		UIInput min = MoneyInput.GetComponent<UIInput> ();
		int money = 0;
		if (int.TryParse (min.value, out money)) {
			GameDataManager.Instance.nMoney = money;
		}
	}

	public void OnWinClick(GameObject go)
	{
		PanelManager.Instance.OpenUI (Panel_Win.Name);
		PanelManager.Instance.CloseUI ( Name ); 
	}
	public void OnLostClick(GameObject go)
	{
		PanelManager.Instance.OpenUI (Panel_Lost.Name);
		PanelManager.Instance.CloseUI ( Name ); 
	}

	public void OnAllDieClick(GameObject go)
	{
		//PanelManager.Instance.OpenUI (Panel_Lost.Name);
		foreach (KeyValuePair< int , cUnitData> pair in GameDataManager.Instance.UnitPool) {
			if( pair.Value != null ){
				if( pair.Value.eCampID == _CAMP._ENEMY && (pair.Value.IsTag( _UNITTAG._PEACE )== false) )
                {
					pair.Value.AddHp( -2099999999 );
                    cUnitData pCmdData = pair.Value;
                    // drop
                    if (pCmdData.eCampID == _CAMP._ENEMY)
                    {
                        int money = Config.BaseMobMoney;
                        money = (int)(money * pCmdData.cCharData.f_DROP_MONEY);
                        money = MyTool.ClampInt(money, 0, money);
                        BattleManager.Instance.nDropMoney += money;
                        GameDataManager.Instance.nEarnMoney += money;

                        // check drop item
                        if (pCmdData.n_DropItemID > 0)
                        {
                            BattleManager.Instance.nDropItemPool.Add(pCmdData.n_DropItemID);
                        }
                    }
                }
			}
		}
		Panel_StageUI.Instance.CheckUnitDead (true);

		PanelManager.Instance.CloseUI ( Name ); 
	}

	public void OnSetStarClick(GameObject go)
	{
		int nStar = 0;
		UIInput input = StarInput.GetComponent< UIInput> ();
		if (int.TryParse (input.value, out  nStar)) {
			GameDataManager.Instance.nStars = nStar;
		}
	}


    public void OnSetRoundClick(GameObject go)
    {
        int nRound = 0;
        UIInput input = RoundInput.GetComponent<UIInput>();
        if (int.TryParse(input.value, out nRound))
        {
            GameDataManager.Instance.nRound = nRound;
        }
    }
    public void OnPopMobClick(GameObject go)
	{
		UIInput input = CharIdInput.GetComponent< UIInput> ();
		int.TryParse (input.value , out  nPopCharID );
		if (nPopCharID <= 0)
			return;

		_CAMP camp = _CAMP._PLAYER;
		UIPopupList popList = CampPoplist.GetComponent<UIPopupList>();
		if (popList != null) {	
			if( popList.value.ToString() == _CAMP._PLAYER.ToString()  ){
				camp = _CAMP._PLAYER;
			}
			else if( popList.value.ToString() == _CAMP._ENEMY.ToString()  ){
				camp = _CAMP._ENEMY;
			}
			else if( popList.value.ToString() == _CAMP._FRIEND.ToString()  )
			{
				camp = _CAMP._FRIEND;
			}
		}

		if( camp == _CAMP._PLAYER )
		{
            // enable char in mainten mode 
            if (GameDataManager.Instance.ePhase == _SAVE_PHASE._MAINTEN)
            {
                GameDataManager.Instance.EnableStorageUnit(nPopCharID, true); // add char

                Panel_Mainten p = PanelManager.Instance.JustGetUI<Panel_Mainten>(Panel_Mainten.Name);
                // MyTool.GetPanel<>
                if (p != null) {
                    p.ReloadUnitList();
                }
                
                return;
            }

            GameDataManager.Instance.RemoveStorageUnit( nPopCharID ); // to avoid re use storage data
		}


		StagePopUnitEvent evt = new StagePopUnitEvent ();
		evt.eCamp   = camp;
		evt.nCharID = nPopCharID;
		evt.nX		= cCMD.Instance.nCMDGridX;
		evt.nY		= cCMD.Instance.nCMDGridY;
		Panel_StageUI.Instance.OnStagePopUnitEvent( evt ); 

	}

	// trig event
	public void OnTrigEventClick(GameObject go)
	{
		UIPopupList popList = EventPoplist.GetComponent<UIPopupList>();
		if (popList != null) {	
			string s = popList.value;
			int nEventID ;
			if( int.TryParse( s , out nEventID ) )
			{
				Panel_StageUI.Instance.TrigEventToRun(nEventID);
			}
		}
		PanelManager.Instance.CloseUI( Name );
	}
    public void OnTestEventClick(GameObject go)
    {
        UIPopupList popList = EventPoplist.GetComponent<UIPopupList>();
        if (popList != null)
        {
            string s = popList.value;
            int nEventID;
            if (int.TryParse(s, out nEventID))
            {
                // get event data

                // Panel_StageUI.Instance.TrigEventToRun(nEventID);
                STAGE_EVENT evt = ConstDataManager.Instance.GetRow<STAGE_EVENT> (nEventID);
                if (evt != null ) {
                    MyScript.Instance.CheckEventCanRun(evt);
                }

            }
        }
        PanelManager.Instance.CloseUI(Name);
    }

    //跳關
    public void StoryComboboxChange()
	{
        UIScrollablePopupList popList = StoryPoplist.GetComponent<UIScrollablePopupList>();
		if (popList != null) {	
//			string s = popList.value;
			STAGE_STORY st = popList.data as STAGE_STORY;
			if( st != null ){
				GameDataManager.Instance.nStoryID = st.n_ID;

			}
		}

        if (GameDataManager.Instance.ePhase == _SAVE_PHASE._MAINTEN)
        {
            Panel_Mainten p = PanelManager.Instance.JustGetUI<Panel_Mainten>(Panel_Mainten.Name);
            if (p != null)
            {
                p.SetStoryName();
            }
        }


        //		UIInput min = MoneyInput.GetComponent<UIInput> ();
        //		int money = 0;
        //		if (int.TryParse (min.value, out money)) {
        //			GameDataManager.Instance.nMoney = money;
        //		}
    }
    // Add Item
    public void OnAddItemClick(GameObject go)
    {
        UIPopupList popList = ItemPoplist.GetComponent<UIPopupList>();
        if (popList != null)
        {
            if (popList.data != null)
            {
                int nItemID = (int)popList.data;

                GameDataManager.Instance.AddItemtoBag(nItemID);
            }
            
        }
        //PanelManager.Instance.CloseUI(Name);
    }

    public void OnDelItemClick(GameObject go)
    {
        UIPopupList popList = ItemPoplist.GetComponent<UIPopupList>();
        if (popList != null)
        {
            if (popList.data != null)
            {
                int nItemID = (int)popList.data;

                GameDataManager.Instance.RemoveItemfromBag(nItemID);
            }

        }
        //PanelManager.Instance.CloseUI(Name);
    }
}
