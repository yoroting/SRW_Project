using UnityEngine;
using System.Collections;

public class ItemList_Item : MonoBehaviour {

    public GameObject lblName;
    public GameObject lblCount;
//    public UILabel lblName;
//    public UILabel lblCount;
    public UILabel lblContent;
    public UILabel lblType;


    public int m_nItemID;
    public int m_nCount;
    public int m_nType;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        m_nItemID = 0;
        m_nCount = 0;
        m_nType = 0;

        MyTool.SetLabelText(lblName , "——— 空 ———" );
        MyTool.SetLabelInt(lblCount, m_nCount);

        lblContent.text = "";

        ReSize();
    }


    public void ReSize()
    {

        transform.localScale = Vector3.one;

        transform.localRotation = Quaternion.identity;
    }

    public void SetData(int itemid , int count )
    {
        m_nItemID = itemid;
        m_nCount = count;

        MyTool.SetLabelText(lblName, MyTool.GetItemName(m_nItemID));
        MyTool.SetLabelInt( lblCount, m_nCount);
        if (itemid == 0) {
            MyTool.SetLabelText(lblCount, "——");
        }

        lblContent.text = MyTool.GetItemTip(m_nItemID);

        ITEM_MISC item = ConstDataManager.Instance.GetRow<ITEM_MISC>(itemid);   //GameDataManager.Instance.GetConstSchoolData ( nSchool );
        if (item == null)
            return;
        m_nType = item.n_TAG_LOOT;
        string sType = "";
        switch (m_nType)
        {
            case 1:  sType = "武"; break;
            case 2: sType = "防"; break;
            case 3: sType = "飾"; break;
            default: sType = "無"; break;
        }
        lblType.text = sType;

    }

}
