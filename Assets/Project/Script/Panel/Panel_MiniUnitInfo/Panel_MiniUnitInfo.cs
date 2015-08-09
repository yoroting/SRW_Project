using UnityEngine;
using System.Collections;

public class Panel_MiniUnitInfo : MonoBehaviour {

	public const string Name = "Panel_MiniUnitInfo";

	private cUnitData pUnitData;
	// open info by identify
	static public int nCharIdent;
	static public GameObject OpenUI( int nIdent )
	{
		nCharIdent = nIdent;
		//GameDataManager.Instance.nInfoIdent = pCmder.Ident ();
		
		GameObject go = PanelManager.Instance.OpenUI ( Panel_MiniUnitInfo.Name ); // set data in onenable
		
		return go;
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable()
	{
		pUnitData = GameDataManager.Instance.GetUnitDateByIdent( nCharIdent );
		if (pUnitData == null) {
			//OnCloseClick( this.gameObject );
			return ;
		}
		
		int nCharId = pUnitData.n_CharID;
	
		ReloadData();
	}

	void ReloadData()
	{
		if (pUnitData == null)
			return;

	}
}
