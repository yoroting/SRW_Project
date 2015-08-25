using UnityEngine;
using System.Collections;

public class Panel_Cheat : MonoBehaviour {

	public const string Name = "Panel_Cheat";

	public GameObject CloseBtn;            
	public GameObject OkBtn;           // God Mode
	//
	public GameObject IntValueobj;
	public GameObject IntAddobj;
	public GameObject IntDelobj;

	public GameObject ExtValueobj;
	public GameObject ExtAddobj;
	public GameObject EXtDelobj;

	public GameObject HpValueobj;
	public GameObject HpAddobj;
	public GameObject HpDelobj;

	public GameObject MpValueobj;
	public GameObject MpAddobj;
	public GameObject MpDelobj;

	public GameObject SpValueobj;
	public GameObject SpAddobj;
	public GameObject SpDelobj;

	public GameObject CpValueobj;
	public GameObject CpAddobj;
	public GameObject CpDelobj;
		


	// Use this for initialization
	void Start () {
	
		UIEventListener.Get(CloseBtn).onClick += OnCloseClick; 
		UIEventListener.Get(OkBtn).onClick += OnOkClick; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable () {
		
//		UIToggle god = GodChk.GetComponent<UIToggle> ();
//		god.value =Config.GOD;
//		UIToggle kill = KillChk.GetComponent<UIToggle> ();
//		kill.value =Config.KILL_MODE;
//		UIToggle ai = KillChk.GetComponent<UIToggle> ();
//		ai.value =Config.MOBAI;
//		
//		UIInput min = MoneyInput.GetComponent<UIInput> ();
//		min.value = GameDataManager.Instance.nMoney.ToString();
		
		
	}


	void OnCloseClick(GameObject go)
	{
		PanelManager.Instance.CloseUI( Name );
	}

	void OnOkClick(GameObject go)
	{

	}

}
