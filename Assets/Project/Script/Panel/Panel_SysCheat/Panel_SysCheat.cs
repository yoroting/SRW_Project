using UnityEngine;
using System.Collections;

public class Panel_SysCheat : MonoBehaviour {

	public const string Name = "Panel_SysCheat";

	public GameObject CloseBtn;            
	public GameObject GodChk;           // God Mode
	public GameObject KillChk;           // Kill mode
	public GameObject MobAIChk;           // Kill mode
	public GameObject MoneyInput;           // Kill mode
	public GameObject MoneyBtn;           // Set money

	// Use this for initialization
	void Start () {
		UIEventListener.Get(GodChk).onClick += OnGODClick; 
		UIEventListener.Get(KillChk).onClick += OnKillClick; 
		UIEventListener.Get(MobAIChk).onClick += OnMobAIClick; 
		UIEventListener.Get(CloseBtn).onClick += OnCloseClick; 

		//UIEventListener.Get(MoneyInput).onSubmit += OnMoneySubmit; 
		UIEventListener.Get(MoneyBtn).onClick += OnMoneyClick; 
		


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

}
