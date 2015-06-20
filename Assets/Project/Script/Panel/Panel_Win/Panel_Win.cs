using UnityEngine;
using System.Collections;

public class Panel_Win : MonoBehaviour {
	public const string Name = "Panel_Win";

	public GameObject SpritObj;

	// Use this for initialization
	void Start () {
		UIEventListener.Get(SpritObj).onClick += OnCloseBtnClick; // for trig next lineev


		GameSystem.PlayBGM ( 5 );
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	void OnCloseBtnClick(GameObject go)
	{	
		// open main ten ui
		PanelManager.Instance.OpenUI ( Panel_Mainten.Name );

		// Go to Mainten Ui 
		PanelManager.Instance.DestoryUI ( Name );

	}
}
