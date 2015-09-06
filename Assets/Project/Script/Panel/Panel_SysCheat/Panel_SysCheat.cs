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
	public GameObject MobAIChk;           // Kill mode
	public GameObject MoneyInput;           // Kill mode
	public GameObject MoneyBtn;           // Set money


	public GameObject StoryPoplist;           // StoryPoplist
//	public GameObject StoryInput;           // Kill mode
//	public GameObject StoryBtn;           // Set money

	// pop mob
	public GameObject CharIdInput;           // pop char id
	public GameObject CampPoplist;           // sel camp
	public GameObject PopMobBtn;           // Set money
	public int nPopCharID;


	// Use this for initialization
	void Start () {
		UIEventListener.Get(GodChk).onClick += OnGODClick; 
		UIEventListener.Get(KillChk).onClick += OnKillClick; 
		UIEventListener.Get(MobAIChk).onClick += OnMobAIClick; 
		UIEventListener.Get(CloseBtn).onClick += OnCloseClick; 
		UIEventListener.Get(WinBtn).onClick += OnWinClick; 
		UIEventListener.Get(LostBtn).onClick += OnLostClick; 

		UIEventListener.Get(AllDieBtn).onClick += OnAllDieClick; 
		//UIEventListener.Get(MoneyInput).onSubmit += OnMoneySubmit; 
		UIEventListener.Get(MoneyBtn).onClick += OnMoneyClick; 

		//UIEventListener.Get(StoryPoplist).onSubmit += OnJumpStory; 
		UIEventListener.Get(PopMobBtn).onClick += OnPopMobClick; 

		///
		UIPopupList popList = StoryPoplist.GetComponent<UIPopupList>();
		if (popList != null) {		
			//添加触发事件
		//	EventDelegate.Add (popList.onChange, label.SetCurrentSelection);
			EventDelegate.Add (popList.onChange, StoryComboboxChange);

			popList.Clear ();
			DataTable storyTable = ConstDataManager.Instance.GetTable ("STAGE_STORY");
			if (storyTable != null) {
				foreach (STAGE_STORY s in  storyTable) {
					popList.AddItem ( MyTool.GetStoryName (s.n_ID), s );	
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


		}

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
		UIToggle ai = KillChk.GetComponent<UIToggle> ();
		ai.value =Config.MOBAI;

		UIInput min = MoneyInput.GetComponent<UIInput> ();
		min.value = GameDataManager.Instance.nMoney.ToString();


		// switch story in mainten only
		if (GameDataManager.Instance.ePhase == _SAVE_PHASE._MAINTEN) {
			StoryPoplist.SetActive( true );
		} else {
			StoryPoplist.SetActive( false );
		}

	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnCloseClick(GameObject go)
	{
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
				if( pair.Value.eCampID == _CAMP._ENEMY ){
					pair.Value.AddHp( -2099999999 );
				}
			}
		}
		Panel_StageUI.Instance.CheckUnitDead (true);

		PanelManager.Instance.CloseUI ( Name ); 
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

		StagePopUnitEvent evt = new StagePopUnitEvent ();
		evt.eCamp   = camp;
		evt.nCharID = nPopCharID;
		evt.nX		= cCMD.Instance.nCMDGridX;
		evt.nY		= cCMD.Instance.nCMDGridY;
		Panel_StageUI.Instance.OnStagePopUnitEvent( evt ); 

	}


	//跳關
	public void StoryComboboxChange()
	{
		UIPopupList popList = StoryPoplist.GetComponent<UIPopupList>();
		if (popList != null) {	
			string s = popList.value;
			STAGE_STORY st = popList.data as STAGE_STORY;
			if( st != null ){
				GameDataManager.Instance.nStoryID = st.n_ID;

			}
		}

//		UIInput min = MoneyInput.GetComponent<UIInput> ();
//		int money = 0;
//		if (int.TryParse (min.value, out money)) {
//			GameDataManager.Instance.nMoney = money;
//		}
	}

}
