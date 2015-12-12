using UnityEngine;
using System.Collections;

public class ItemList_School : MonoBehaviour {

    public GameObject lblStar;
    public GameObject lblName;
    public GameObject lblLv;

    public int nSchoolID;
    public int nLv;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

    public void SetData(int schoolid, int lv)
    {
        nSchoolID = schoolid;
        nLv = lv;
        SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(schoolid);
        if (sch != null)
        {
            MyTool.SetLabelInt(lblStar, sch.n_RANK);
        }

        MyTool.SetLabelText(lblName, MyTool.GetSchoolName(schoolid));
        MyTool.SetLabelInt(lblLv, lv);
    }
}
