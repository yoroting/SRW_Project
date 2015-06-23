using UnityEngine;
using System.Collections;

public class Panel_Tip : MonoBehaviour {

	public const string Name = "Panel_Tip";
	public int nTipType;

	public int nTipID;

	//public GameObject spBG;
	public GameObject lblText;

	// Use this for initialization
	void Start () {
		UIEventListener.Get(this.gameObject).onClick += OnBuffClick; // 
	}
	
	// Update is called once per frame
	void Update () {

	}

	static public void OpenUI( string str )
	{
		Panel_Tip pTip = MyTool.GetPanel< Panel_Tip > (PanelManager.Instance.OpenUI (Name));
		if (pTip) {

			MyTool.SetLabelText ( pTip.lblText , str );
		}

	}

	void OnBuffClick( GameObject go )
	{

		PanelManager.Instance.CloseUI ( Name );
	}

}
