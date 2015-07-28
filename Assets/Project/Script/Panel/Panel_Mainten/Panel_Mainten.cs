using UnityEngine;
using System.Collections;

public class Panel_Mainten : MonoBehaviour {
	public const string Name = "Panel_Mainten";
	public GameObject btnNextStage;


	// Use this for initialization
	void Start () {
		UIEventListener.Get(btnNextStage).onClick += OnNextStageClick; // for trig next lineev
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPopReady()
	{
		// on ready
		GameDataManager.Instance.ePhase = _SAVE_PHASE._MAINTEN;		// save to mainta phase
		// close stage UI!!
		if (PanelManager.Instance.CheckUIIsOpening (Panel_StageUI.Name)) {

			 PanelManager.Instance.DestoryUI( Panel_StageUI.Name ); // don't destory .. this is singoleten obj.. may be need to free singolten 
			//PanelManager.Instance.CloseUI( Panel_StageUI.Name );
			// need to free all stage resource
		}
	}

	IEnumerator EnterStory( int nStoryID )
	{
		GameDataManager.Instance.nStoryID = nStoryID;
		
		PanelManager.Instance.OpenUI( "Panel_Loading");
		
		yield return  new WaitForEndOfFrame();
		
		PanelManager.Instance.OpenUI( StoryUIPanel.Name );
		yield return  new WaitForEndOfFrame();
		
		
		PanelManager.Instance.CloseUI( Name );  			// close main 
		yield break;
		
	}


	void OnNextStageClick( GameObject go )
	{
		GameDataManager.Instance.nStoryID += 1;
		//GameDataManager.Instance.nStageID += 1;
//		PanelManager.Instance.OpenUI ( StoryUIPanel.Name );
//		PanelManager.Instance.CloseUI ( Name );			// close this ui

		StartCoroutine ( EnterStory( GameDataManager.Instance.nStoryID ) );

	}
}
