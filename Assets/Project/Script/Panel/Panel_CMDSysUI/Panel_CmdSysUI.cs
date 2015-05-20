using UnityEngine;
using System.Collections;

public class Panel_CmdSysUI : MonoBehaviour {

	public GameObject InfoButton;
	public GameObject SaveButton;
	public GameObject LoadButton;
	public GameObject RoundEndButton;
	public GameObject GameEndButton;
	public GameObject CancelButton;

	void Awake()
	{
		UIEventListener.Get(InfoButton).onClick += OnInfoButtonClick;
		UIEventListener.Get(SaveButton).onClick += OnSaveButtonClick;;
		UIEventListener.Get(LoadButton).onClick += OnLoadButtonClick;;
		UIEventListener.Get(RoundEndButton).onClick += OnRoundEndButtonClick;;
		UIEventListener.Get(GameEndButton).onClick += OnGameEndButtonClick;
		UIEventListener.Get(CancelButton).onClick += OnCancelButtonClick;;

	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
	}
	void OnGameEndButtonClick(GameObject go)
	{
		// 結束遊戲
	}
	void OnCancelButtonClick(GameObject go)
	{
		// 取消
		PanelManager.Instance.CloseUI( "Panel_CMDSYSUI" );
	}
}
