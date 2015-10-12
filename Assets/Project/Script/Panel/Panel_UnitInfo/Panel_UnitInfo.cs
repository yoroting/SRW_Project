using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel_UnitInfo : MonoBehaviour {

	public const string Name = "Panel_UnitInfo";

	public GameObject FaceObj;
	public GameObject NameObj;

	public GameObject MarObj;
	public GameObject HpObj;
	public GameObject MpObj;
	public GameObject SpObj;
	public GameObject MovObj;

	public GameObject AtkObj;
	public GameObject DefObj;
	public GameObject PowObj;

	public GameObject IntSchObj;
	public GameObject ExtSchObj;

	public GameObject LvObj;
	public GameObject ExpObj;


	public GameObject AbilityGrid;
	public GameObject SkillGrid;
	public GameObject ItemGrid;
	public GameObject FateGrid;
	public GameObject BuffGrid;
	public GameObject PassGrid; // 被動


	public GameObject CloseBtnObj;

	private cUnitData pUnitData;

	// open info by identify
	static public int nCharIdent;
	static public GameObject OpenUI( int nIdent )
	{
		nCharIdent = nIdent;
		//GameDataManager.Instance.nInfoIdent = pCmder.Ident ();
		
		GameObject go = PanelManager.Instance.OpenUI ( Panel_UnitInfo.Name ); // set data in onenable

		return go;
	}

	void Awake()
	{
		UIEventListener.Get(CloseBtnObj).onClick += OnCloseClick; // for trig next line

	}



	void OnEnable()
	{
		pUnitData = GameDataManager.Instance.GetUnitDateByIdent( nCharIdent );
		if (pUnitData == null) {
			OnCloseClick( this.gameObject );
			return ;
		}

		int nCharId = pUnitData.n_CharID;
		//CHARS chars = data.cCharData;
		// change face	
		UITexture tex = FaceObj.GetComponent<UITexture>();
		if (tex != null) {
			string url = "Art/char/" + pUnitData.cCharData.s_FILENAME + "_L";
			//Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
			Texture t = Resources.Load(url, typeof(Texture)) as Texture;
			tex.mainTexture = t;				
		}


		// name 
		//string name = pUnitData.cCharData.s_NAME;
		MyTool.SetLabelText( NameObj , MyTool.GetCharName( nCharId ) );


		ReloadData();
	}

	void OnDisable()
	{
		Panel_Tip.CloseUI (); // auto close tip

	}

	void ReloadData()
	{
		if( pUnitData == null )
			return;

		pUnitData.UpdateAllAttr ();
		pUnitData.UpdateAttr ();
		pUnitData.UpdateBuffConditionAttr ();
		// lv
		MyTool.SetLabelInt( LvObj , pUnitData.n_Lv );
		// exp 
		MyTool.SetLabelInt( ExpObj , pUnitData.n_EXP);
		// mar
		MyTool.SetLabelFloat( MarObj , pUnitData.GetMar() );
		// HP
		int nMaxHp = pUnitData.GetMaxHP();
		MyTool.SetLabelText( HpObj , string.Format( "{0}/{1}" , pUnitData.n_HP , nMaxHp ) );
		// MP
		int nMaxMp = pUnitData.GetMaxMP();
		MyTool.SetLabelText( MpObj , string.Format( "{0}/{1}" , pUnitData.n_MP , nMaxMp ) );
		// SP
		int nMaxSp = pUnitData.GetMaxSP();
		MyTool.SetLabelText( SpObj , string.Format( "{0}/{1}" , pUnitData.n_SP , nMaxSp ) );
		// mov
		MyTool.SetLabelInt( MovObj , pUnitData.GetMov() );
		// atk 
		MyTool.SetLabelInt( AtkObj , pUnitData.GetAtk() );
		// def 
		int nMaxDef = pUnitData.GetMaxDef();
		MyTool.SetLabelText( DefObj , string.Format( "{0}/{1}" , pUnitData.n_DEF , nMaxDef ) );
		
		// pow
		MyTool.SetLabelInt( PowObj , pUnitData.GetPow() );
		
		// school name
		
		//SCHOOL inSch = GameDataManager.Instance.GetConstSchoolData( data.nActSch[0] ); // int 
		MyTool.SetLabelText( IntSchObj , MyTool.GetUnitSchoolFullName( nCharIdent , pUnitData.GetIntSchID() )  );
		
		//SCHOOL exSch = ConstDataManager.Instance.GetRow<SCHOOL>( pUnitData.nActSch[1] );//   GameDataManager.Instance.GetConstSchoolData( pUnitData.nActSch[1] ); // ext 
		MyTool.SetLabelText( ExtSchObj , MyTool.GetUnitSchoolFullName( nCharIdent , pUnitData.GetExtSchID() ) );


		// Set ability
		UpdateAbility ();
		// set skill
		UpdateSkill ();
		
		// set item 
		UpdateItem ();
		// set fate
		//UpdateFate ();
		// set buff
		UpdateBuff ();
		//
		UpdatePass ();
	}

	// Use this for initialization
	void Start () {
	

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCloseClick( GameObject go )
	{
		PanelManager.Instance.CloseUI( Name );
	}


	void UpdateAbility()
	{
		int nCharlv = pUnitData.n_Lv;

		MyTool.DestoryGridItem ( AbilityGrid );

		foreach (KeyValuePair< int , int > pair in pUnitData.AbilityPool ) {
			if( pair.Value > nCharlv )
				continue;
			GameObject go = ResourcesManager.CreatePrefabGameObj( AbilityGrid , "Prefab/Skill_simple" ); 
			if( go == null )
				continue;



			UIEventListener.Get(go).onClick += OnAbilityClick; // 


			Skill_Simple obj = go.GetComponent<Skill_Simple >();
			if( obj != null ){
				obj.nID = pair.Key;
				obj.nType = 0; // 0 is ability
				MyTool.SetLabelText( obj.lblName , MyTool.GetSkillName( pair.Key ) );
			}
			
		}
		//UIGrid grid = AbilityGrid.GetComponent<UIGrid>(); 
		//grid.repositionNow = true;		// need this for second pop to re pos

	}
	void UpdateSkill()
	{
		//int nExtSchool = pUnitData.nActSch [cAttrData._EXTSCH];

		MyTool.DestoryGridItem ( SkillGrid );


		foreach ( int nID in pUnitData.SkillPool ) {
			int nSkillID = nID;
			nSkillID = pUnitData.Buffs.GetUpgradeSkill( nSkillID ); // Get upgrade skill

			SKILL skl = ConstDataManager.Instance.GetRow< SKILL > ( nSkillID );

			if(skl.n_SCHOOL == 0 )
				continue;

			GameObject go = ResourcesManager.CreatePrefabGameObj( SkillGrid , "Prefab/Skill_simple" ); 
			if( go == null )
				continue;
			
			UIEventListener.Get(go).onClick += OnSkillClick; // 		


			Skill_Simple obj = go.GetComponent<Skill_Simple >();
			if( obj != null ){
				obj.nID = nSkillID;
				obj.nType = 1; // 0 is ability
				MyTool.SetLabelText( obj.lblName , MyTool.GetSkillName( nSkillID) );
			}
			
		}
//		UIGrid grid = SkillGrid.GetComponent<UIGrid>(); 
//		grid.repositionNow = true;		// need this for second pop to re pos


	}
	void UpdateItem()
	{
		MyTool.DestoryGridItem ( ItemGrid );
		foreach (int itemid in pUnitData.Items) {
			if( itemid <= 0 )
				continue;

			GameObject go = ResourcesManager.CreatePrefabGameObj( ItemGrid , "Prefab/Skill_simple" ); 
			if( go == null )
				continue;
			
			UIEventListener.Get(go).onClick += OnItemClick; // 
			
			
			Skill_Simple obj = go.GetComponent<Skill_Simple >();
			if( obj != null ){
				obj.nID = itemid;
				obj.nType = 2; // 2 is item
				MyTool.SetLabelText( obj.lblName , MyTool.GetItemName( itemid ) );
			}
	

			//ITEM_MISC item = ConstDataManager.


		}
//		int item0 = pUnitData.Items[ (int)_ITEMSLOT._SLOT0  ]; 
//		int item1 = pUnitData.Items[ (int)_ITEMSLOT._SLOT1  ]; 



	}

	void UpdateBuff()
	{
		MyTool.DestoryGridItem ( BuffGrid );

		foreach ( KeyValuePair< int , cBuffData > pair in pUnitData.Buffs.Pool ) {
			if( pair.Value.nTime == 0 ) // never 0  
				continue;
			if( pair.Value.tableData.n_HIDE > 0 )
				continue;

			GameObject go = ResourcesManager.CreatePrefabGameObj( BuffGrid , "Prefab/Bufficon" ); 
			if( go == null )
				continue;		
			
			BuffIcon icon = go.GetComponent< BuffIcon >();
			if( icon != null )
				icon.SetBuffData( pair.Value.nID , pair.Value.nNum  );
		//	UIEventListener.Get(go).onClick += OnBuffClick; // 

		}
	}

	void UpdatePass ()
	{
		MyTool.DestoryGridItem ( PassGrid );

		foreach ( KeyValuePair< int , cBuffData > pair in pUnitData.Buffs.Pool ) {
			if( pair.Value.nTime != 0 ) // only 0  
				continue;
			if( pair.Value.tableData.n_HIDE > 0 )
				continue;

			GameObject go = ResourcesManager.CreatePrefabGameObj( PassGrid , "Prefab/Skill_simple" ); 
			if( go == null )
				continue;	
			UIEventListener.Get(go).onClick += OnBuffClick; // this is a buff

			Skill_Simple obj = go.GetComponent<Skill_Simple >();
			if( obj != null ){
				obj.nID = pair.Value.nID;
				obj.nType = 0; // 0 is ability
				MyTool.SetLabelText( obj.lblName , MyTool.GetBuffName (obj.nID ) ); // this is buff name
			}
			
		}

	}

	// onclick event
	void OnAbilityClick( GameObject go )
	{
		Skill_Simple obj = go.GetComponent<Skill_Simple >();
		if (obj != null) {
			Panel_Tip.OpenSkillTip( obj.nID );
			//Panel_Tip.OpenUI( MyTool.GetSkillName( obj.nID )   ); 
		}
		
	}
	void OnSkillClick( GameObject go )
	{		
		Skill_Simple obj = go.GetComponent<Skill_Simple >();
		if (obj != null) {
			Panel_Tip.OpenSkillTip( obj.nID );
			//Panel_Tip.OpenUI( MyTool.GetSkillName( obj.nID )   ); 
		}
	}

	void OnItemClick( GameObject go )
	{
		Skill_Simple obj = go.GetComponent<Skill_Simple >();
		if (obj != null) {
			Panel_Tip.OpenItemTip( obj.nID );
			//Panel_Tip.OpenUI( MyTool.GetSkillName( obj.nID )   ); 
		}
	}

	void OnBuffClick( GameObject go )
	{
		Skill_Simple obj = go.GetComponent<Skill_Simple >();
		if (obj != null) {
			Panel_Tip.OpenBuffTip( obj.nID );
			//Panel_Tip.OpenUI( MyTool.GetSkillName( obj.nID )   ); 
		}
	}

}

