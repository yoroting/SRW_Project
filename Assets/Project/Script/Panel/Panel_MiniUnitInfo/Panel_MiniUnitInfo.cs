using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel_MiniUnitInfo : MonoBehaviour {

	public const string Name = "Panel_MiniUnitInfo";
	public GameObject lblName;
	public GameObject lblMar;
	public GameObject lblCP;
	public GameObject lblHP;
    public GameObject lblDef;
    public GameObject [] sprCP;

    public GameObject BuffGrid;
	// Buff List

	// open info by identify
//	static public int nCharIdent;
	static public GameObject OpenUI( cUnitData pData )
	{
	//	nCharIdent = nIdent;
		//GameDataManager.Instance.nInfoIdent = pCmder.Ident ();	
		GameObject go = PanelManager.Instance.OpenUI ( Panel_MiniUnitInfo.Name ); // set data in onenable

		Panel_MiniUnitInfo pInfo = MyTool.GetPanel< Panel_MiniUnitInfo > (go);
		if (pInfo != null) {
			pInfo.SetData( pData );
		}

		return go;
	}
	static public void CloseUI(  )
	{
		if (PanelManager.Instance) {
			PanelManager.Instance.CloseUI (Name);
		}
	}

	// Use this for initialization
	void Start () {
        UIEventListener.Get(this.gameObject).onClick += OnCloseClick; // click to close
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable()
	{

	}

	public void SetData( cUnitData pUnitData )
	{
		if (pUnitData == null)
			return;
		pUnitData.UpdateAllAttr ();
		pUnitData.UpdateAttr ();
		pUnitData.UpdateBuffConditionAttr ();
        string sName = MyTool.GetCharName(pUnitData.n_CharID);
        if (Config.GOD) {
            sName += ("("+ pUnitData.n_Ident + ")") ;
        }

        MyTool.SetLabelText (lblName, sName );
		MyTool.SetLabelInt (lblMar, (int)pUnitData.GetMar ());
		MyTool.SetLabelInt (lblCP, pUnitData.n_CP );
		MyTool.SetLabelInt (lblHP, pUnitData.n_HP );
        MyTool.SetLabelInt( lblDef, pUnitData.n_DEF );
        

        // set cp 
        int idx = 0;
        foreach (GameObject o in sprCP)
        {
            //o.SetActive(idx++ < pUnitData.n_CP);
            o.SetActive(false); // 全關
        }
        

		// set buff 
		MyTool.DestoryGridItem ( BuffGrid );
		
		foreach ( KeyValuePair< int , cBuffData > pair in pUnitData.Buffs.Pool ) {
			if( pair.Value.nTime == 0 ) // never 0  ( 被動能力 buff)
				continue;
			if( pair.Value.tableData.n_HIDE > 0 )
				continue;
			
			GameObject go = ResourcesManager.CreatePrefabGameObj( BuffGrid , "Prefab/Bufficon" ); 
			if( go == null )
				continue;


            MyTool.SetBuffIcon( go , pair.Value.nID, pair.Value.nTime, pair.Value.GetUITime() );
			//BuffIcon icon = go.GetComponent< BuffIcon >();
			//if( icon != null )
			//	icon.SetBuffData( pair.Value.nID , pair.Value.nTime , pair.Value.nNum  );
			//	UIEventListener.Get(go).onClick += OnBuffClick; // 
		}
		//==============
		UIGrid grid = BuffGrid.GetComponent<UIGrid > ();
		if (grid != null) {
			grid.repositionNow = true ;
			grid.Reposition();
		}


	}

    void OnCloseClick(GameObject go)
    {
        go.SetActive(false);
    }
}
