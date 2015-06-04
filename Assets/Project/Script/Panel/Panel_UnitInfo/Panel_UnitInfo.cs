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

		// change face	
		UITexture tex = FaceObj.GetComponent<UITexture>();

		if (tex != null) {
			string url = "Assets/Art/char/" + data.cCharData.s_FILENAME + "_L.png";
			//Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
			Texture t = Resources.LoadAssetAtPath (url, typeof(Texture)) as Texture;
			tex.mainTexture = t;				
		}
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

