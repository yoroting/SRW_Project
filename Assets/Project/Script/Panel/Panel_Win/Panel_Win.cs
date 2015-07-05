using UnityEngine;
using System.Collections;

public class Panel_Win : MonoBehaviour {
	public const string Name = "Panel_Win";

	public GameObject SpritObj;


	void OnEnable()
	{
		Panel_StageUI.Instance.bIsStageEnd = true;
	}
	// Use this for initialization
	void Start () {
		UIEventListener.Get(SpritObj).onClick += OnCloseBtnClick; // for trig next lineev


		GameSystem.PlayBGM ( 5 );
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDisable () {
	//	UIEventListener.Get(SpritObj).onClick -= OnCloseBtnClick; // for trig next lineev
	}
	
	void OnCloseBtnClick(GameObject go)
	{	
		// open main ten ui
		PanelManager.Instance.OpenUI ( Panel_Mainten.Name );
		// Go to Mainten Ui 
		PanelManager.Instance.DestoryUI ( Name );

	}
}
