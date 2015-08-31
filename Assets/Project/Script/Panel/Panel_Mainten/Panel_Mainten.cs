using UnityEngine;
using System.Collections;

public class Panel_Mainten : MonoBehaviour {
	public const string Name = "Panel_Mainten";
	public GameObject btnUnit;
	public GameObject btnSave;
	public GameObject btnLoad;
	public GameObject btnNextStage;
	public GameObject btnGameEnd;

	public GameObject btnCheat;

	public GameObject lblStoryName;

	// Use this for initialization
	void OnEnable()
	{
		GameSystem.PlayBGM ( 7 );
	}

	void Start () {
		UIEventListener.Get(btnNextStage).onClick += OnNextStageClick; // for trig next lineev
		UIEventListener.Get(btnUnit).onClick += OnUnitClick; // for trig next lineev
		UIEventListener.Get(btnSave).onClick += OnSaveClick; // for trig next lineev
		UIEventListener.Get(btnLoad).onClick += OnLoadClick; // for trig next lineev
		UIEventListener.Get(btnGameEnd).onClick += OnEndClick; // for trig next lineev

		UIEventListener.Get(btnCheat).onClick += OnCheatClick; // cheat
	}
	
	// Update is called once per frame
	void Update () {
		SetStoryName ();
	}

	public void OnPopReady()
	{
		// on ready
		GameDataManager.Instance.ePhase = _SAVE_PHASE._MAINTEN;		// save to mainta phase
		// close stage UI!!
		if (PanelManager.Instance.CheckUIIsOpening (Panel_StageUI.Name)) {


			// free here waill cause some  StartCoroutine of stageUI break 
			if( cSaveData.IsLoading()== false ){
				 PanelManager.Instance.DestoryUI( Panel_StageUI.Name ); 
			//PanelManager.Instance.CloseUI( Panel_StageUI.Name );
			// need to free all stage resource

			// close loading
				PanelManager.Instance.CloseUI( "Panel_Loading");
			}

		}
	}
	public void ChooseNextStage()
	{
		GameDataManager.Instance.nStoryID += 1;
	}

	IEnumerator EnterStory( int nStoryID )
	{
		GameDataManager.Instance.nStoryID = nStoryID;
		
		PanelManager.Instance.OpenUI( "Panel_Loading");
		
		yield return  new WaitForEndOfFrame();
		
		PanelManager.Instance.OpenUI( StoryUIPanel.Name );
		yield return  new WaitForEndOfFrame();
		
		
		PanelManager.Instance.DestoryUI( Name );  			// close main 
		yield break;
		
	}


	void OnNextStageClick( GameObject go )
	{
	//	GameDataManager.Instance.nStoryID += 1;
		//GameDataManager.Instance.nStageID += 1;
//		PanelManager.Instance.OpenUI ( StoryUIPanel.Name );
//		PanelManager.Instance.CloseUI ( Name );			// close this ui

//		ChooseNextStage();
		StartCoroutine ( EnterStory( GameDataManager.Instance.nStoryID ) );

	}
	void OnUnitClick( GameObject go )
	{

	}
	void OnSaveClick( GameObject go )
	{
		Panel_SaveLoad.OpenSaveMode ( _SAVE_PHASE._MAINTEN );
	//	cSaveData.Save ( 1 , _SAVE_PHASE._MAINTEN );
	}
	void OnLoadClick( GameObject go )
	{
		Panel_SaveLoad.OpenLoadMode ( _SAVE_PHASE._MAINTEN );
	//	cSaveData.Load ( 1 , _SAVE_PHASE._MAINTEN );
		//StartCoroutine ( SaveLoading( save) 

	}
	void OnEndClick( GameObject go )
	{
		PanelManager.Instance.OpenUI( MainUIPanel.Name );

		PanelManager.Instance.CloseUI( Name );
	}

	void OnCheatClick( GameObject go )
	{
		PanelManager.Instance.OpenUI( Panel_SysCheat.Name );
	
	}

	//===========================================================
	IEnumerator SaveLoading( cSaveData save )
	{
		//GameDataManager.Instance.nStoryID = nStoryID;
		//GameDataManager.Instance.nStageID = save.n_StageID;
		
		PanelManager.Instance.OpenUI( "Panel_Loading");
		
		yield return  new WaitForEndOfFrame();
		
		if (save.ePhase == _SAVE_PHASE._MAINTEN) {
			//PanelManager.Instance.OpenUI ( Panel_Mainten.Name );
			RestoreBySaveData( save );

		} else if (save.ePhase == _SAVE_PHASE._STAGE) {
			
			PanelManager.Instance.OpenUI( Panel_StageUI.Name );  // don't run start() during open
//			Panel_StageUI.Instance.bIsRestoreData = true;
			yield return  new WaitForEndOfFrame();
			Panel_StageUI.Instance.RestoreBySaveData ( save );
			yield return  new WaitForEndOfFrame ();
		}		
		// close loadint UI
		PanelManager.Instance.CloseUI( "Panel_Loading");

		cSaveData.SetLoading (false);

		if (save.ePhase != _SAVE_PHASE._MAINTEN) {
			PanelManager.Instance.DestoryUI (Name);  			// close main 
		}

		yield break;
		
	}

	public void LoadSaveGame( cSaveData save )
	{
		if (save  == null)
			return;
		StartCoroutine ( SaveLoading( save) );		

	}

	public bool RestoreBySaveData( cSaveData save  )
	{
	//	cSaveData save = GameDataManager.Instance.SaveData;
		if (save == null)
			return false;
		
		
		System.GC.Collect ();			// Free memory resource here

		if (save.ePhase == _SAVE_PHASE._STAGE) {
			// restore to mainten ui

		}	
	
	//	SetStoryName ();
		GameDataManager.Instance.ePhase = _SAVE_PHASE._MAINTEN;		// save to stage phase

		return true;
	}

	public void SetStoryName( )
	{
		if( lblStoryName != null ){
			MyTool.SetLabelText( lblStoryName , MyTool.GetStoryName(  GameDataManager.Instance.nStoryID ) );
		}
	}
}
