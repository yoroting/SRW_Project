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
        btnOK.SetActive(true);
        btnNO.SetActive(true);
    }


	// Update is called once per frame
	void Update () {
	
	}

	public void SetAoeCheck()
	{
		nCheckType = 1;
		SetMsg ( "確定施展嗎？" );


	}
	public void SetRoundEndCheck()
	{
		nCheckType = 2;
		SetMsg ( "直接結束本回合？" );
	}

    public void SetMessageCheck( int nMsgID )
    {   
        string msg = MyTool.GetMsgText(nMsgID);
        SetMsg(msg);
    }

    public void SetMessageCheck(string sMsg)
    {
        nCheckType = 3;

        btnNO.SetActive(false);
        SetMsg(sMsg);
    }

    public void SetMsg( string msg ){
		MyTool.SetLabelText ( lblText , msg );
	}


	void OnOkClick( GameObject go )
	{
		if (nCheckType == 1) {		// check type
			MyTool.CMDUI().AOE_OK();
		}
		else if (nCheckType == 2) { //round end
			MyTool.CMDUI().RoundEndCmd();
		}
		PanelManager.Instance.CloseUI (Name );
	}

	void OnCloseClick( GameObject go )
	{
		if (nCheckType == 1) {			
			MyTool.CMDUI().AOE_Cancel();
		}
		else if (nCheckType == 2) { //round end
		}

		PanelManager.Instance.CloseUI (Name );
	}
}
