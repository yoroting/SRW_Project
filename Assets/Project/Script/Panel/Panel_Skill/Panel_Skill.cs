using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;			// for parser string
//using _SRW;

public class Panel_Skill : MonoBehaviour {
	public const string Name = "Panel_Skill";

	public GameObject OkBtn;
	public GameObject CloseBtn;
	public GameObject SkillGrid;
	public GameObject ScrollView;
	public GameObject SkillContent;

	public GameObject SkillSprite;
	public GameObject CastNote;


	public _SKILL_TYPE eSkillType;

	public int 	nOpSkillID;			// current select skill ID

	int nOpIdent;
	int nOpCharID;

	cUnitData pData;
	Dictionary<GameObject  , SKILL > sklPool;


	//bool bLongPress;		// is long press?
	private float _longClickDuration = 1.0f;
	float _lastPress = -1f;

	void Awake()
	{
		sklPool = new Dictionary<GameObject  , SKILL > ();
		
		UIEventListener.Get(OkBtn).onClick += OnOkClick; // for trig next line
		UIEventListener.Get(CloseBtn).onClick += OnCloseClick; // for trig next line

		CastNote.SetActive( true );

		nOpSkillID = 0;
	}

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
	
		if (_lastPress > 0) {
			if( Time.realtimeSinceStartup - _lastPress > _longClickDuration )
			{
				CastSkill();


			}
		}
	}
	// 


	public void SetData( cUnitData data , _SKILL_TYPE eType )
	{
		ClearData ();
		eSkillType = eType;
		//
		nOpIdent  = data.n_Ident;
		nOpCharID = data.n_CharID;

		pData = data;

		DataTable pTable = ConstDataManager.Instance.GetTable < SKILL > ();
		if (pTable == null) 
			return;

		List< SKILL > sklLst = new List< SKILL > ();



		if (eType == _SKILL_TYPE._SKILL) {

			int ISch = pData.nActSch [0];
			int ESch = pData.nActSch [1];
			int ELv = pData.GetSchoolLv (ESch);

			foreach( SKILL skl in pTable )
			{
				if( skl.n_SCHOOL == 0 )	// == 0 is ability
					continue;

				if( skl.n_PASSIVE == 1 )
					continue;

				// cheat code for god
				if( Config.GOD ){
					sklLst.Add( skl );
					continue;
				}

				// normal 
				if( skl.n_SCHOOL != ESch )
					continue;
				if( skl.n_LEVEL_LEARN > ELv )
					continue;

				sklLst.Add( skl );
			}
		}
		else if (eType == _SKILL_TYPE._ABILITY ) 		
		{
			int CLv = pData.n_Lv;
			foreach( SKILL skl in pTable )
			{
				if( skl.n_SCHOOL > 0 ) // > 0 is school
					continue;
				if( skl.n_PASSIVE == 1 )
					continue;

				if( Config.GOD ){
					sklLst.Add( skl );
					continue;
				}
				// check char have this ability

				if( skl.n_LEVEL_LEARN > CLv )
					continue;
				
				sklLst.Add( skl );
			}
		}


		// Create UI item

		foreach( SKILL skl in sklLst )
		{
				// add this skill
			GameObject go = ResourcesManager.CreatePrefabGameObj( SkillGrid , "Prefab/Item_Skill" ); 
			if( go == null )
				continue;

			Item_Skill item = go.GetComponent<Item_Skill> ();
			item.SetItemData( skl.s_NAME , skl.n_RANGE , skl.n_MP );
			item.SetScrollView( ScrollView );

		//	UIEventListener.Get(go).onClick += OnSkillClick; // for trig next line
			UIEventListener.Get(go).onPress += OnSkillPress; // 

			sklPool.Add(  go , skl );
		}

		// default to 1 st skill
		SetSkill (null);
		//foreach (KeyValuePair<GameObject  , SKILL> pair in sklPool) {
		//	SetSkill( pair.Value );			// set to first 
		//	break;
		//}

		// for grid re pos
		UIGrid grid = SkillGrid.GetComponent<UIGrid>(); 
		grid.repositionNow = true;		// need this for second pop to re pos

		CastNote.SetActive( false ); 
	}

	void ClearData()
	{
		sklPool.Clear ();

		UIGrid grid = SkillGrid.GetComponent<UIGrid>(); 
		if (grid == null) {
			return ;
		}

		List< Transform > lst = grid.GetChildList ();
		//List< GameObject > CmdBtnList = MyTool.GetChildPool( NGuiGrids );
		
		if (lst != null) {
			foreach (Transform t in lst) {
			
				///UIEventListener.Get(obj).onClick -= OnCMDButtonClick;;  // no need.. destory soon
				NGUITools.Destroy (t.gameObject);
			}
		}
	}

	public static Panel_Skill OpenUI( int nIdent , _SKILL_TYPE eType  )
	{
		cUnitData data = GameDataManager.Instance.GetUnitDateByIdent ( nIdent );
		if (data == null)
			return null;
		GameObject go = PanelManager.Instance.OpenUI (Name );
		if (go == null) 
			return null;

		Panel_Skill pUI = MyTool.GetPanel<Panel_Skill>( go );
		pUI.SetData( data , eType );
		return pUI;
	}

	void SetSkill( SKILL skl )
	{
		SkillSprite.SetActive ( (skl != null) );
		if (skl == null) {

			return;
		}
		MyTool.SetLabelText (SkillContent, skl.s_NAME);
		nOpSkillID = skl.n_ID;

		CastNote.SetActive (true);
	}

	void CastSkill(  )
	{
		Panel_CMDUnitUI panel = MyTool.GetPanel<Panel_CMDUnitUI> ( PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name) );
		if (panel != null) {
			panel.SetSkillID( nOpSkillID );
		}
		
		PanelManager.Instance.DestoryUI ( Name );
	}

	void OnOkClick(GameObject go)
	{

		// use skill to atk
		//GameDataManager.Instance
		// send skill ok command
		CastSkill ();


		//PanelManager.Instance.CloseUI ( Name ); // close SKILL UI
	}

	void OnCloseClick(GameObject go)
	{
		Panel_CMDUnitUI panel = MyTool.GetPanel<Panel_CMDUnitUI> ( PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name) );
		if (panel != null) {
			panel.SetSkillID( 0  ); // 0 is cancel
		}
		PanelManager.Instance.CloseUI ( Name );

	}

	void OnSkillClick(GameObject go)
	{
		SKILL skl = null;
		if (sklPool.TryGetValue (go, out skl) == false) {
			return ;
		}
		// show skill detail
		SetSkill ( skl );

		// 
		OnOkClick (go);
	}
	void OnSkillPress(GameObject go , bool pressed )
	{

		if (pressed) {
			_lastPress = Time.realtimeSinceStartup; 

			SKILL skl = null;
			if (sklPool.TryGetValue (go, out skl) == false) {
				return ;
			}
			// show skill detail
			SetSkill ( skl );
		}
		else 
		{ 
			if (Time.realtimeSinceStartup - _lastPress > _longClickDuration) 
			{
				// don't send ok cmd

//				SKILL skl = null;
//				if (sklPool.TryGetValue (go, out skl) == false) {
//					return ;
//				}
//				// show skill detail
//				SetSkill ( skl );
			}
			else{ // normal click . if cast note is not disappear
				//if( CastNote.activeSelf ){
				//	OnOkClick (go);
				//}
				//_lastPress = -1.0f;		// reset key
			}

			_lastPress = -1.0f;		// reset key
		} 
	}



}
