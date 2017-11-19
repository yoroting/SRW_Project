using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMsg : MonoBehaviour {
    static public int nDropCount = 0;

    public GameObject sprMoney;
    public GameObject lblMoney;

    public GameObject lblItem;
    public GameObject lblItemValue;


    public GameObject sprStar;
    public GameObject lblStar;

    float fUpdateTime = 0.0f;
    // Use this for initialization
    void Start () {
        nDropCount++;
    }

    void OnEnable()
    {
        transform.localRotation = new Quaternion();
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        fUpdateTime = 0.0f;
        //fOriginY = transform.localPosition.y; // 此時會是 上次的數值未更新
        // clear all twr
        //		nValueCount++;
      
    }

    // Update is called once per frame
    void Update () {
        // 增加自動消失機制
        fUpdateTime += Time.deltaTime;
        if (fUpdateTime > 3.0f)
        {
            TweenAlpha[] tws = this.gameObject.GetComponents<TweenAlpha>();
            if (tws.Length == 0)
            {
                TweenAlpha twa = TweenAlpha.Begin<TweenAlpha>(this.gameObject, 0.5f);
                if (twa != null)
                {
                    twa.delay = 0.0f;
                    twa.from = 1.0f;
                    twa.to = 0.0f;

                    MyTool.TweenSetOneShotOnFinish(twa, OnAlphaFinish);
                }
                Debug.LogErrorFormat("don't close DropMsg correct");
            }
        }
    }

    public void SetData(int nExp , int nMoney )
    {
        if (nExp < 0)
            nExp = 0;
        MyTool.SetLabelText(lblMoney , string.Format( "+{0}" , nMoney) );

        MyTool.SetLabelText(lblItemValue, string.Format("+{0}", nExp));

        //  經驗為負時，不顯示
        //if( nExp < 0　)
        sprMoney.SetActive(true);
        //{
        //    lblItem.SetActive(false);
        //    lblItemValue.SetActive(false);
        //}

        sprStar.SetActive(false);
    }

    public void SetStar(int nStar)
    {
        MyTool.SetLabelText(lblStar, string.Format("+ {0}", nStar));
        //Sprite 換成　星星
        //UISprite sp = sprMoney.GetComponent<UISprite>();
        //if (sp != null) {
        //    sp.spriteName = "icon_star";
        //}

        sprMoney.SetActive( false );
        lblItem.SetActive(false);
      //  lblItemValue.SetActive(false);
    }

    public void OnAlphaFinish()
    {
        nDropCount--;
        NGUITools.Destroy(this.gameObject);

    }
}
