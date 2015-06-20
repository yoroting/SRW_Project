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

		// close stage UI!!
		if (PanelManager.Instance.CheckUIIsOpening (Panel_StageUI.Name)) {

			// PanelManager.Instance.DestoryUI( Panel_StageUI.Name ); // don't destory .. this is singoleten obj.. may be need to free singolten 

		}



	}


	void OnNextStageClick( GameObject go )
	{
		//GameDataManager.Instance.nStageID += 1;
		PanelManager.Instance.OpenUI ( Panel_StageUI.Name );


	}
}
