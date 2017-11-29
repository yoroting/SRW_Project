using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item_ability : MonoBehaviour {

    public UILabel lblName;
    public UILabel lblValue;
    myUiTip m_Tip;
    public int nID; // ability id

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
      //  UIEventListener.Get(this.gameObject).onClick = OnAbilityClick; // for trig next line
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetSkillD(int id, int type)
    {
        nID = id;
        lblName.text = MyTool.GetSkillName(nID);        
        SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nID);
        if (skl != null)
        {
                
            lblValue.text = skl.n_SP.ToString();
        }
        m_Tip.SetTip(id, myUiTip._TIP_TYPE._SKILL);
        

    }

    

}
