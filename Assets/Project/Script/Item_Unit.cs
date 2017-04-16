using UnityEngine;
using System.Collections;

public class Item_Unit : MonoBehaviour {

    public GameObject lblName;
    public GameObject btnEquip;

    public int nID;
    public int nIndex;
    //   public int nType; // 0- ability , 1 - skill , 2- Item , 3- fate

    void Awake()
    {
        nID = 0;
        nIndex = -1;
        // 子物件的 awake 在母物件後。所以要母物件 在 awkae 外 設定index
        //    nType = 0;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnEnable()
    {
        ReSize();
    }


    public void ReSize()
    {

        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }

    public void SetItemID( int id )
    {
        nID = id;
        MyTool.SetLabelText(lblName, MyTool.GetItemName(nID));
    }
    public void SetItemSlot(int index )
    {
        nIndex = index;
    }
    public void SetItemCallBack( UIEventListener.VoidDelegate func  )
    {
        UIEventListener.Get(btnEquip).onClick = func;
    }

}
