using UnityEngine;
using System.Collections;

public class BattleMsg : MonoBehaviour {
	static public int nMsgCount=0;
    float fUpdateTime = 0.0f;
    public UILabel m_lblText;

    // Use this for initialization
    void Awake()
    {
        nMsgCount++;  // call when construct
    }

    void Start () {
		// nMsgCount++;   // call when first update ( enable/disable)
	}

    void OnDestroy() // 直接 關閉 場警對話，會有 pop的道具沒關到         
    {
        nMsgCount--;
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
        // 防呆，避免沒正確關閉 訊息
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
                Debug.LogErrorFormat("don't close BattleMsg correct");
            }
        }
    }

	public void OnAlphaFinish()
	{
	//	nMsgCount--;
		NGUITools.Destroy ( this.gameObject );

	}


    public void SetText( string str , int nType = 0 )
    {
        m_lblText.text = str;

    }
    public void SetDepth(int nDepth )
    {
        MyTool.SetDepth( gameObject, nDepth );

        UILabel lbl = this.gameObject.GetComponentInChildren<UILabel>();
        if (lbl != null)
        {
            MyTool.SetDepth(lbl.gameObject, nDepth +1 );
        }
    }

}
