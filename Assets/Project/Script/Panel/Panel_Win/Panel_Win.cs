using UnityEngine;
using System.Collections;

public class Panel_Win : MonoBehaviour {
	public const string Name = "Panel_Win";

	public GameObject SpritObj;


	void OnEnable()
	{
		Panel_StageUI.Instance.EndStage ();
		//Panel_StageUI.Instance.bIsStageEnd = true;

		// GameDataManager.Instance.EndStage ();   // 處理戰場結束的資料回存

		// hide stage 

		GameSystem.PlayBGM ( 5 );
	}
	// Use this for initialization
	void Start () {
		UIEventListener.Get(SpritObj).onClick += OnCloseBtnClick; // for trig next lineev



	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDisable () {
	//	UIEventListener.Get(SpritObj).onClick -= OnCloseBtnClick; // for trig next lineev
	}
	
	void OnCloseBtnClick(GameObject go)
	{	
		// check mission complete
		Panel_StageUI.Instance.CheckMissionComplete (); // last check for mission complete


		// if it have talk event. play it
		Panel_StageUI.Instance.ShowStage (false);
		// 決定 下一個story
		STAGE_DATA stage = ConstDataManager.Instance.GetRow< STAGE_DATA > ( GameDataManager.Instance.nStageID );
		if (stage != null) {
			GameDataManager.Instance.nStoryID = stage.n_NEXT_STORY;

			if( stage.n_WIN_TALK > 0 ){
				GameSystem.TalkEvent( stage.n_WIN_TALK );
			}
			else{
				// open main ten ui directly
				PanelManager.Instance.OpenUI ( Panel_Mainten.Name );

			}

		}


		// Go to Mainten Ui 
		PanelManager.Instance.DestoryUI ( Name );

	}
}
