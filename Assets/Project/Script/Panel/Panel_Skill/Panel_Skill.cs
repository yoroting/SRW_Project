using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;			// for parser string

public class Panel_Skill : MonoBehaviour {
	public const string Name = "Panel_Skill";

	public GameObject OkBtn;
	public GameObject CloseBtn;
	public GameObject SkillGrid;

	int nOpIdent;
	int nOpCharID;

	cUnitData pData;
	Dictionary<GameObject  , SKILL > sklPool;


	void Awake()
	{
		sklPool = new Dictionary<GameObject  , SKILL > ();
		
		UIEventListener.Get(OkBtn).onClick += OnOkClick; // for trig next line
		UIEventListener.Get(CloseBtn).onClick += OnCloseClick; // for trig next line
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
			if( skl.n_SCHOOL != ESch )
				continue;
			if( skl.n_LEVEL_LEARN > ELv )
				continue;

			// add this skill
			GameObject go = ResourcesManager.CreatePrefabGameObj( SkillGrid , "Prefab/Item_Skill" ); 
			if( go == null )
				continue;

			Item_Skill item = go.GetComponent<Item_Skill> ();
			item.SetItemData( skl.s_NAME , skl.n_RANGE , skl.n_MP );

			UIEventListener.Get(go).onClick += OnSkillClick; // for trig next line

			sklPool.Add(  go , skl );
		}




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
		
	}

	void OnCloseClick(GameObject go)
	{
		PanelManager.Instance.CloseUI ( Name );

	}

	void OnSkillClick(GameObject go)
	{
		SKILL skl = null;
		if (sklPool.TryGetValue (go, out skl) == false) {
			return ;
		}
		// show skill detail
	}


}
