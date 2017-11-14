using UnityEngine;
using System.Collections;

public class Skill_Simple : MonoBehaviour {

	public GameObject lblName;

	public int nID;
    public int nIndex;
	public int nType; // 0- ability , 1 - skill , 2- Item , 3- fate
    myUiTip m_Tip;

    void Awake()
    {

        m_Tip = this.gameObject.GetComponent<myUiTip>();
        if (m_Tip == null)
        {
            m_Tip = this.gameObject.AddComponent<myUiTip>();
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
        nID = 0;
        nIndex = -1;
        nType = 0;
    }


    public void ReSize()
    {

        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }
    public void SetSkillD(int id , int type )
    {
        nID = id;
        nType = type;

        //MyTool.SetLabelText(lblName, MyTool.GetItemName(nID));
       

        if (type == 0)
        {
            string sname = MyTool.GetSkillName(nID);
            SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nID);
            if (skl != null)
            {
                sname += " " + skl.n_SP;
            }
            MyTool.SetLabelText(lblName, sname); // set
            
            m_Tip.SetTip(id, myUiTip._TIP_TYPE._SKILL);

        }
        else {
            string sname = MyTool.GetBuffName(nID);
            MyTool.SetLabelText(lblName, sname); // set

            m_Tip.SetTip(id, myUiTip._TIP_TYPE._BUFF);
        }
       
    }

}
