using UnityEngine;
using System.Collections;

public class Panel_Win : MonoBehaviour {
	public const string Name = "Panel_Win";

	public GameObject SpritObj;


	void OnEnable()
	{
		Panel_StageUI.Instance.bIsStageEnd = true;
		GameDataManager.Instance.EndStage ();   // 處理戰場結束的資料回存

		// 決定 下一個story
		STAGE_DATA stage = ConstDataManager.Instance.GetRow< STAGE_DATA > ( GameDataManager.Instance.nStageID );
		if (stage != null) {
			GameDataManager.Instance.nStoryID = stage.n_NEXT_STORY;
		}


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
