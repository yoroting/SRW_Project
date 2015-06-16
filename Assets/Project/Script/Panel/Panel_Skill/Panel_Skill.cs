using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;			// for parser string
using _SRW;

public class Panel_Skill : MonoBehaviour {
	public const string Name = "Panel_Skill";

	public GameObject OkBtn;
	public GameObject CloseBtn;
	public GameObject SkillGrid;
	public GameObject ScrollView;
	public GameObject SkillContent;

	public int 	nOpSkillID;			// current select skill ID

	int nOpIdent;
	int nOpCharID;

	cUnitData pData;
	Dictionary<GameObject  , SKILL > sklPool;


	void Awake()
	{
		sklPool = new Dictionary<GameObject  , SKILL > ();
		
		UIEventListener.Get(OkBtn).onClick += OnOkClick; // for trig next line
		UIEventListener.Get(CloseBtn).onClick += OnCloseClick; // for trig next line

		nOpSkillID = 0;
	}

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
	
	}
	// 


	public void SetData( cUnitData data )
	{
		ClearData ();

		//
		nOpIdent  = data.n_Ident;
		nOpCharID = data.n_CharID;

		pData = data;

		int ISch =  pData.nActSch[0];
		int ESch =  pData.nActSch[1];
		int ELv = pData.GetSchoolLv (ESch);

		DataTable pTable = ConstDataManager.Instance.GetTable < SKILL >();
		if (pTable == null) 
			return;

	//	List<SKILL> lst = pTable.ListRows<SKILL> ();
		foreach( SKILL skl in pTable )
		{
			// all open for trace grid
		//	if( skl.n_SCHOOL != ESch )
		//		continue;
		//	if( skl.n_LEVEL_LEARN > ELv )
		//		continue;

			// add this skill
			GameObject go = ResourcesManager.CreatePrefabGameObj( SkillGrid , "Prefab/Item_Skill" ); 
			if( go == null )
				continue;

			Item_Skill item = go.GetComponent<Item_Skill> ();
			item.SetItemData( skl.s_NAME , skl.n_RANGE , skl.n_MP );
			item.SetScrollView( ScrollView );

			UIEventListener.Get(go).onClick += OnSkillClick; // for trig next line

			sklPool.Add(  go , skl );
		}

		// default to 1 st skill

		foreach (KeyValuePair<GameObject  , SKILL> pair in sklPool) {
			SetSkill( pair.Value );			// set to first 
			break;
		}

		// for grid re pos
		UIGrid grid = SkillGrid.GetComponent<UIGrid>(); 
		grid.repositionNow = true;		// need this for second pop to re pos

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

	public static Panel_Skill OpenUI( int nIdent )
	{
		cUnitData data = GameDataManager.Instance.GetUnitDateByIdent ( nIdent );
		if (data == null)
			return null;
		GameObject go = PanelManager.Instance.OpenUI (Name );
		if (go == null) 
			return null;

		Panel_Skill pUI = MyTool.GetPanel<Panel_Skill>( go );
		pUI.SetData( data );
		return pUI;
	}

	void OnOkClick(GameObject go)
	{

		// use skill to atk
		//GameDataManager.Instance
		// send skill ok command


		Panel_CMDUnitUI panel = MyTool.GetPanel<Panel_CMDUnitUI> ( PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name) );
		if (panel != null) {
			panel.SetSkillID( nOpSkillID );
		}

		PanelManager.Instance.CloseUI ( Name ); // close CMD UI
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
	}
	void SetSkill( SKILL skl )
	{
		if (skl == null)
			return;
		MyTool.SetLabelText (SkillContent, skl.s_NAME);
		nOpSkillID = skl.n_ID;
	}

}
