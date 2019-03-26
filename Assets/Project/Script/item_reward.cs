using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item_reward : MonoBehaviour
{
    public UILabel m_lblTitle;
    public UILabel m_lblContent;
    public UISprite m_spMoney;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        transform.localRotation = Quaternion.identity;//  new Quaternion();
        transform.localScale = Vector3.one;//new Vector3(1.0f, 1.0f, 1.0f);

        m_spMoney.gameObject.SetActive( false );


    }


    public void SetMoney( string sSitle , int nMoney)
    {
        m_lblTitle.text = sSitle;

        m_spMoney.gameObject.SetActive(true);

        m_lblContent.text = "＋"+nMoney.ToString();

        GameDataManager.Instance.nEarnMoney += nMoney;
        GameDataManager.Instance.nMoney += nMoney;

    }

}
