using UnityEngine;
using System.Collections;

public class ItemList_Item : MonoBehaviour {

    public GameObject lblName;
    public GameObject lblCount;

    public int nItemID;
    public int nCount;

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

    public void SetData(int itemid , int count )
    {
        nItemID = itemid;
        nCount  = count;



        MyTool.SetLabelText(lblName, MyTool.GetItemName(nItemID));
        MyTool.SetLabelInt( lblCount, nCount);
        if (itemid == 0) {
            MyTool.SetLabelText(lblCount, "- -");
        }

    }

}
