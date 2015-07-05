using UnityEngine;
using System.Collections;

public class Panel_CheckBox : MonoBehaviour {
	public const string Name = "Panel_CheckBox";
	public GameObject lblText;
	public GameObject btnOK;
	public GameObject btnNO;

	int nCheckType ; 
	// Use this for initialization
	void Start () {
		UIEventListener.Get(btnOK).onClick += OnOkClick;
		UIEventListener.Get(btnNO).onClick += OnCloseClick;
	}

	void OnEnable(){

	}


	// Update is called once per frame
	void Update () {
	
	}

	public void SetAoeCheck()
	{
		nCheckType = 1;
		SetMsg ( "確定施展嗎？" );


	}

	public void SetMsg( string msg ){
		MyTool.SetLabelText ( lblText , msg );
	}


	void OnOkClick( GameObject go )
	{
		// 
		if (nCheckType == 1) {
			MyTool.CMDUI().AOE_OK();
		}

		PanelManager.Instance.CloseUI (Name );
	}

	void OnCloseClick( GameObject go )
	{
		if (nCheckType == 1) {			
			MyTool.CMDUI().AOE_Cancel();
		}


		PanelManager.Instance.CloseUI (Name );
	}
}
