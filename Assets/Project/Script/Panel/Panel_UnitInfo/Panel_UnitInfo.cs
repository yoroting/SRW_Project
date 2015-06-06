using UnityEngine;
using System.Collections;

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

	public GameObject CloseBtnObj;

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

		SCHOOL inSch = GameDataManager.Instance.GetConstSchoolData( data.nActSch[0] ); // int 
		MyTool.SetLabelText( IntSchObj ,inSch.s_NAME );

		SCHOOL exSch = GameDataManager.Instance.GetConstSchoolData( data.nActSch[1] ); // ext 
		MyTool.SetLabelText( ExtSchObj ,exSch.s_NAME );

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

}

