using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel_MiniUnitInfo : MonoBehaviour {

	public const string Name = "Panel_MiniUnitInfo";
	public GameObject lblName;
	public GameObject lblMar;
	public GameObject lblCP;
	public GameObject lblHP;

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
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable()
	{

	}

	void SetData( cUnitData pUnitData )
	{
		if (pUnitData == null)
			return;
		pUnitData.UpdateAllAttr ();
		pUnitData.UpdateAttr ();
		pUnitData.UpdateBuffConditionAttr ();

		MyTool.SetLabelText (lblName, MyTool.GetCharName (pUnitData.n_CharID));
		MyTool.SetLabelInt (lblMar, (int)pUnitData.GetMar ());
		MyTool.SetLabelInt (lblCP, pUnitData.n_CP );
		MyTool.SetLabelInt (lblHP, pUnitData.n_HP );

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
			
			BuffIcon icon = go.GetComponent< BuffIcon >();
			if( icon != null )
				icon.SetBuffData( pair.Value.nID , pair.Value.nNum  );
			//	UIEventListener.Get(go).onClick += OnBuffClick; // 
		}
		//==============
		UIGrid grid = BuffGrid.GetComponent<UIGrid > ();
		if (grid != null) {
			grid.repositionNow = true ;
			grid.Reposition();
		}


	}


}
