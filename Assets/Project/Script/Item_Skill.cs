using UnityEngine;
using System.Collections;

public class Item_Skill : MonoBehaviour {
	public GameObject NameObj;
	public GameObject RangeObj;
	public GameObject CostObj;

	public bool bEnable = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetItemData( string name , int nRange , int nCost )
	{
		MyTool.SetLabelText ( NameObj , name );
		MyTool.SetLabelInt ( RangeObj , nRange );
		MyTool.SetLabelInt ( CostObj , nCost );

	}

	public void SetScrollView( GameObject go )
	{
		UIDragScrollView dsv = this.GetComponent<UIDragScrollView> ();
		if (dsv != null) {
			dsv.scrollView = go.GetComponent< UIScrollView >();
		}
	}

	public void SetEnable( bool enable )
	{
		bEnable = enable;
		Color c = new Color (1.0f, 1.0f, 1.0f);
		if (bEnable == false) {
			c.g = 0.0f; c.b = 0.0f; 
		}
 
		if (NameObj != null) {
			MyTool.SetLabelColor ( CostObj , c );
		}
	}

}

