using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _SRW;



//public class Panel_CMDUnitUI : MonoBehaviour 
public class Panel_CMDUnitUI : Singleton<Panel_CMDUnitUI>
{

	public const string Name = "Panel_CMDUnitUI";



	//static public  cCommand CMD;							// for global operate
	public  cCMD CMD;							// for global operate

	public Panel_unit pUnit; 						// setup it

	public GameObject NGuiGrids;

	public GameObject InfoButton;
	public GameObject MoveButton;
	public GameObject AttackButton;
	public GameObject SkillButton;
	public GameObject SchoolButton;
	public GameObject CancelButton;


	bool bWaitMoveFinish ; 
	// widget Data

	void Clear()
	{
		pUnit = null;
		bWaitMoveFinish = false;
	}
	// Use this for initialization
	void Awake()
	{
		CMD = cCMD.Instance;

	

		UIEventListener.Get(InfoButton).onClick += OnInfoButtonClick;
		UIEventListener.Get(MoveButton).onClick += OnMoveButtonClick;;
		UIEventListener.Get(AttackButton).onClick += OnAttackButtonClick;;
		UIEventListener.Get(SkillButton).onClick += OnSkillButtonClick;;
		UIEventListener.Get(SchoolButton).onClick += OnSchoolButtonClick;
		UIEventListener.Get(CancelButton).onClick += OnCancelButtonClick;;


		//
		GameEventManager.AddEventListener(  CmdCharMoveEvent.Name , OnCmdCharMoveEvent );

		//==============================
		ClearCmdPool ();
	}

	void Start () {
		//Clear ();  This line will cause first open Ui clear data with setuped. don't clear here
	}
	
	// Update is called once per frame
	void Update () {
		if( pUnit == null )
		{
			//Debug.LogError( "ERR: UnitCMDUI with NULL Unit" );
		//	Clear();
			//nCmderIdent 	= 0;
		//	PanelManager.Instance.CloseUI( Name );
			return ;
		}

		if ( CMD.eCMDSTAT == _CMD_STATUS._TARGET) {  // sel target only
			// check if move end.
			if( pUnit.IsMoving() == false )
			{
				// show atk range
				if( bWaitMoveFinish == true )
				{
					bWaitMoveFinish = false;
					StageShowAttackRangeEvent evt = new StageShowAttackRangeEvent();
					evt.nIdent = CMD.nCmderIdent;
					GameEventManager.DispatchEvent ( evt );
				}

			}
		}
		else if (CMD.eCMDSTAT == _CMD_STATUS._MOVE) {  // move only
			
		}
		else // normal
		{
		}

	}

	void Destroy()
	{
	//	if (pUnit != null) {
	//		pUnit.OnSelected( false );
	//	}
	}

	void OnEnable() // before start
	{
		// create cmd by type
		CreateCMDList ( cCMD.Instance.eCMDTYPE );

	//	if (pUnit != null) {
	//		pUnit.OnSelected( true );
	//	}
	}

	void OnDisable()
	{
		ClearCmdPool ();
	//	if (pUnit != null) {
	//		pUnit.OnSelected( false );
	//	}
	}

	void ClearCmdPool()
	{
		if (NGuiGrids == null) {
			return ;
		}
		UIGrid grid = NGuiGrids.GetComponent<UIGrid>(); 
		if (grid == null) {
			return ;
		}
		List< Transform > lst = grid.GetChildList ();
		//List< GameObject > CmdBtnList = MyTool.GetChildPool( NGuiGrids );

		if (lst == null)
			return;

		foreach ( Transform t in lst) {

			///UIEventListener.Get(obj).onClick -= OnCMDButtonClick;;  // no need.. destory soon
			NGUITools.Destroy( t.gameObject );
		}

	}

	void CreateCMDList( _CMD_TYPE eType )
	{
		ClearCmdPool ();
		List< _CMD_ID >  cmdList =  cCMD.Instance.GetCmdList ( eType );
		if (cmdList == null)
			return;
	
		foreach( _CMD_ID id in cmdList )
		{
			GameObject obj = ResourcesManager.CreatePrefabGameObj( this.NGuiGrids , "Prefab/CMD_BTN" ); // create cmd and add to grid
			if( obj != null )
			{
				obj.name = MyTool.GetCMDNameByID( id );
				UILabel lbl = obj.GetComponentInChildren<UILabel> ();
				if( lbl != null )
				{
					lbl.text = obj.name ;
					// Load Label by const data
				}
				UIEventListener.Get(obj).onClick += OnCMDButtonClick;
			}
		}
		// update
		UIGrid grid = NGuiGrids.GetComponent<UIGrid>(); 
		grid.repositionNow = true;		// need this for second pop to re pos

		//NGuiGrids.SetActive (true);

	}

	public void SetCmder( Panel_unit unit )
	{
		//cancel old
		if (pUnit != null) {
			pUnit.OnSelected( false );
		}
		// clear
		Clear ();
		if( unit == null ){

			return ;
		}
		// setup origin param
		pUnit = unit;
		CMD.nCmderIdent = pUnit.Ident();
		// who will disable
		pUnit.OnSelected (true);

		CMD.nOrgGridX = pUnit.X();
		CMD.nOrgGridY = pUnit.Y();


	}
	public void SetTarget( Panel_unit unit )
	{
		CMD.nTarIdent = 0;
		if( unit != null ){
			CMD.nTarIdent = unit.Ident();
		}
		// trig attack event

		// close cmd ui
		Clear ();
		PanelManager.Instance.CloseUI( Name );
	}

	public void CancelCmd( )
	{
		Clear ();
		PanelManager.Instance.CloseUI( Name );
	}
	//click
	void OnCMDButtonClick(GameObject go)
	{
		string name = go.name;

		//_CMD_ID id = MyTool.GetCMDIDByName ( name );
		if (name == _CMD_ID._INFO.ToString ()) {
		}
		else if (name == _CMD_ID._MOVE.ToString ()) {
		}
		else if (name == _CMD_ID._ATK.ToString ()) {
		}
		else if (name == _CMD_ID._ABILITY.ToString ()) {
		}
		else if (name == _CMD_ID._CANCEL.ToString ()) {
			OnCancelButtonClick( go );
		}
	}

	void OnInfoButtonClick(GameObject go)
	{
		// 查情報

	}
	void OnMoveButtonClick(GameObject go)
	{
	}
	void OnAttackButtonClick(GameObject go)
	{
	}
	void OnSkillButtonClick(GameObject go)
	{
	}
	void OnSchoolButtonClick(GameObject go)
	{
		// 結束遊戲
	}
	void OnCancelButtonClick(GameObject go)	{
		if (pUnit != null) {
			pUnit.OnSelected (false);
		}

		// 取消
		Clear ();
//		nCharIdent 	= 0;
//		pUnit 		= null;
		PanelManager.Instance.CloseUI( Name );

		// send clear over
		Panel_StageUI.Instance.ClearOverCellEffect ();
	}

	// Game Event
	public void OnCmdCharMoveEvent(GameEvent evt)
	{
		CmdCharMoveEvent Evt = evt as CmdCharMoveEvent;
		if (Evt == null)
			return;
		int nIdent = Evt.nIdent;
		int nX =  Evt.nX;
		int nY =  Evt.nY;
		if (nIdent != CMD.nCmderIdent)
			return;
		// entry next phase
		CMD.eCMDSTAT  = _CMD_STATUS._TARGET; // sel target only
		bWaitMoveFinish = true;


	}
}
