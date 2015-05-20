using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _SRW_CMD;

public class Panel_CmdUI : MonoBehaviour {

	public GameObject nGrid;

	// Use this for initialization
	void Start () {
		Vector3 vLoc = this.gameObject.transform.localPosition ;
		//vLoc.x =  Input.mousePosition.x;
		//vLoc.y =  Input.mousePosition.y;
		vLoc.x = (UICamera.lastHit.point.x *Screen.width);
		vLoc.y = (UICamera.lastHit.point.y *Screen.height);


		this.gameObject.transform.localPosition = vLoc;

	}
	
	// Update is called once per frame
	void Update () {
		// avoid cmd out windows


	}

	// Create Cmd list type
	public void CreateCMDList( _CMD_TYPE nType )
	{
		// clear all child
		List<_CMD_ID> typeList = new List<_CMD_ID>();
		typeList.Add( _CMD_ID._CANCEL );

		// 
		switch( nType )
		{
		  case _CMD_TYPE._SYS:


			break;
		  case _CMD_TYPE._CELL:
			
			
			break;
		}

		//
		foreach( _CMD_ID cmdid in typeList )
		{
			int nArg1 = 0;
			int nArg2 = 0;
			int nArg3 = 0;	
			GameObject cBtn = CreateCMDButton(cmdid , nArg1 , nArg2 ,nArg3 );

		}
	}

	public GameObject CreateCMDButton( _CMD_ID nID , int nArg1 , int nArg2 , int nArg3 )
	{
		GameObject cPrefab = ResourcesManager.CreatePrefabGameObj(nGrid,"Prefab/ColorButton" );
		if( cPrefab != null )
		{
			UILabel lab = cPrefab.GetComponentInChildren< UILabel >();
			if( lab != null )
			{
				 // lab text
			}

			// param
			ColorButton btn = cPrefab.GetComponent<ColorButton>();
			if( btn != null)
			{
				btn.CMD_ID = nID;
				btn.nArg.Add( nArg1 );
				btn.nArg.Add( nArg2 );
				btn.nArg.Add( nArg3 );
			}
			// 
			UIEventListener.Get(cPrefab).onClick += OnButtonClick;

		}
		return cPrefab;
	}

	void OnButtonClick(GameObject go)
	{
		ColorButton btn = go.GetComponent<ColorButton>();
		if( btn != null)
		{
			if( btn.CMD_ID == _CMD_ID._CANCEL )
			{
				PanelManager.Instance.CloseUI( "Panel_CMDUI" );

			}
		}
	}
}
