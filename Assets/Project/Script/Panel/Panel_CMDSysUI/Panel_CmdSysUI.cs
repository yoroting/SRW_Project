using UnityEngine;
using System.Collections;

public class Panel_CmdSysUI : MonoBehaviour
{
	public const string Name = "Panel_CmdSysUI";

	public GameObject InfoButton;
	public GameObject SaveButton;
	public GameObject LoadButton;
	public GameObject RoundEndButton;
	public GameObject GameEndButton;
    public GameObject SystemSettingButton;
    public GameObject CancelButton;

	void Awake()
	{
		UIEventListener.Get(InfoButton).onClick += OnInfoButtonClick;
		UIEventListener.Get(SaveButton).onClick += OnSaveButtonClick;;
		UIEventListener.Get(LoadButton).onClick += OnLoadButtonClick;;
		UIEventListener.Get(RoundEndButton).onClick += OnRoundEndButtonClick;;
        UIEventListener.Get(SystemSettingButton).onClick = OnSystemSettingClick; ;

        UIEventListener.Get(GameEndButton).onClick += OnGameEndButtonClick;
		UIEventListener.Get(CancelButton).onClick += OnCancelButtonClick;;

	}
	// Use this for initialization
	void Start () {
//		Vector3 vLoc = this.gameObject.transform.localPosition ;	
//		vLoc.x = MyTool.ScreenToLocX( Input.mousePosition.x );// (UICamera.lastHit.point.x *Screen.width);
//		vLoc.y = MyTool.ScreenToLocY( Input.mousePosition.y ); //(UICamera.lastHit.point.y *Screen.height);			
//		this.gameObject.transform.localPosition =  vLoc;

//		Vector3 v23 =  UICamera.currentCamera.WorldToScreenPoint( this.gameObject.transform.position );
//		if( v23.x > 0 )
//		{
//		}

		//CheckBorderLimit();
	}
	
	// Update is called once per frame
	void Update () {
		// avoid out screen

	}


	//click
	void OnInfoButtonClick(GameObject go)
	{
		 // 查情報
	}
	void OnSaveButtonClick(GameObject go)
	{
	}
	void OnLoadButtonClick(GameObject go)
	{
	}
	void OnRoundEndButtonClick(GameObject go)
	{
		GameDataManager.Instance.NextCamp();
	}
	void OnGameEndButtonClick(GameObject go)
	{
		// 結束遊戲
	}
	void OnCancelButtonClick(GameObject go)
	{
		// 取消
		PanelManager.Instance.CloseUI( Name );
	}
    void OnSystemSettingClick(GameObject go)
    {
        // 取消
        PanelManager.Instance.CloseUI(Name);
    }
    
}
