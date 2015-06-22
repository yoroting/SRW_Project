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


	public GameObject CloseBtnObj;

	private cUnitData pUnitData;

	void Awake()
	{
		UIEventListener.Get(CloseBtnObj).onClick += OnCloseClick; // for trig next line

	}


	void OnEnable()
	{
		cUnitData data = GameDataManager.Instance.GetUnitDateByIdent( GameDataManager.Instance.nInfoIdent );
		if (data == null) {
			OnCloseClick( this.gameObject );
			return ;
		}
		pUnitData = data;

		int nCharId = data.n_CharID;
		//CHARS chars = data.cCharData;
		// change face	
		UITexture tex = FaceObj.GetComponent<UITexture>();
		if (tex != null) {
			string url = "Assets/Art/char/" + data.cCharData.s_FILENAME + "_L.png";
			//Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
			Texture t = Resources.LoadAssetAtPath (url, typeof(Texture)) as Texture;
			tex.mainTexture = t;				
		}


		// name 
		MyTool.SetLabelText( NameObj , data.cCharData.s_NAME );
		// lv
		MyTool.SetLabelInt( LvObj , data.n_Lv );
		// exp 
		MyTool.SetLabelInt( ExpObj , data.n_EXP);
		// mar
		MyTool.SetLabelFloat( MarObj , data.GetMar() );
		// HP
		int nMaxHp = data.GetMaxHP();
		MyTool.SetLabelText( HpObj , string.Format( "{0}/{1}" , data.n_HP , nMaxHp ) );
		// MP
		int nMaxMp = data.GetMaxMP();
		MyTool.SetLabelText( MpObj , string.Format( "{0}/{1}" , data.n_MP , nMaxMp ) );
		// SP
		int nMaxSp = data.GetMaxSP();
		MyTool.SetLabelText( SpObj , string.Format( "{0}/{1}" , data.n_SP , nMaxSp ) );
		// mov
		MyTool.SetLabelInt( MovObj , data.GetMov() );
		// atk 
		MyTool.SetLabelInt( AtkObj , data.GetAtk() );
		// def 
		int nMaxDef = data.GetMaxDef();
		MyTool.SetLabelText( DefObj , string.Format( "{0}/{1}" , data.n_DEF , nMaxDef ) );

		// pow
		MyTool.SetLabelInt( PowObj , data.GetPow() );

		// school name

		//SCHOOL inSch = GameDataManager.Instance.GetConstSchoolData( data.nActSch[0] ); // int 
		MyTool.SetLabelText( IntSchObj , MyTool.GetUnitSchoolFullName( GameDataManager.Instance.nInfoIdent , data.nActSch[0] )  );

		SCHOOL exSch = GameDataManager.Instance.GetConstSchoolData( data.nActSch[1] ); // ext 
		MyTool.SetLabelText( ExtSchObj , MyTool.GetUnitSchoolFullName( GameDataManager.Instance.nInfoIdent , data.nActSch[1] ) );

		// Set ability
		UpdateAbility ();
		// set skill
		UpdateSkill ();

		// set item 
		UpdateItem ();
		// set fate
		UpdateFate ();
		// set buff
		UpdateBuff ();

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


		foreach ( int skillid in pUnitData.SkillPool ) {

			GameObject go = ResourcesManager.CreatePrefabGameObj( SkillGrid , "Prefab/Skill_simple" ); 
			if( go == null )
				continue;		
			
			
			UIEventListener.Get(go).onClick += OnSkillClick; // 
			
			
			Skill_Simple obj = go.GetComponent<Skill_Simple >();
			if( obj != null ){
				obj.nID = skillid;
				obj.nType = 1; // 0 is ability
				MyTool.SetLabelText( obj.lblName , MyTool.GetSkillName( skillid ) );
			}
			
		}
//		UIGrid grid = SkillGrid.GetComponent<UIGrid>(); 
//		grid.repositionNow = true;		// need this for second pop to re pos


	}
	void UpdateItem()
	{

	}
	void UpdateFate()
	{

	}
	void UpdateBuff()
	{

	}


	// onclick event
	void OnAbilityClick( GameObject go )
	{

		
	}
	void OnSkillClick( GameObject go )
	{
		
	}
	void OnItemClick( GameObject go )
	{
		
	}
	void OnFateClick( GameObject go )
	{
		
	}
	void OnBuffClick( GameObject go )
	{
		
	}

}

