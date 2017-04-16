using UnityEngine;
using System.Collections;

public class EnhanceItem : MonoBehaviour
{


    // school scroll View
    //    public UIScrollView ExtScrollView;
    //public UIScrollView IntScrollView;
    public GameObject lblName;
    public GameObject lblLv;
    public GameObject lblRank;
    public GameObject btnAdd;


    public int nSchoolID;
    public int nLv;
    public int nType;

    // Use this for initialization
    void Start()
    {

      //  UIEventListener.Get(btnAdd).onClick += OnAddClick;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void onEnable()
    {
        ReSize();
    }

    public void ReSize()
    {
        transform.localScale = Vector3.one;

        transform.localRotation = Quaternion.identity;
    }

    public void SetData( int schid, int lv)
    {        
        SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(schid);
        if (sch == null)
        {
            return;
        }
        nSchoolID = schid;
        nLv = lv;
        nType = sch.n_TYPE;
        MyTool.SetLabelText(lblName, MyTool.GetSchoolName(schid));
        MyTool.SetLabelInt(lblLv, lv);
        MyTool.SetLabelFloat(lblRank, sch.f_RANK);


    }

   
}
