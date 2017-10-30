using UnityEngine;
using System.Collections;

public class BattleValue : MonoBehaviour {

//	static public int nValueCount=0;  // no more need
//	float fOffset = 16.0f;
	float fOriginY = 0.0f;
    public int nMode = 0; // 顯示方式， 0- 正常， 1- 爆擊
    float fUpdateTime = 0.0f;

	void OnEnable()
	{
		transform.localRotation = new Quaternion(); 
		transform.localScale = new Vector3( 1.0f, 1.0f , 1.0f);
        fUpdateTime = 0.0f;
        //fOriginY = transform.localPosition.y; // 此時會是 上次的數值未更新
        // clear all twr
        //		nValueCount++;
    }
	// Use this for initialization
	void Start () {
	//	nValueCount++;
	}
	
	// Update is called once per frame
	void Update () {

        // 防呆，避免沒正確關閉 訊息
        fUpdateTime += Time.deltaTime;
        if (fUpdateTime > 3.0f) {
            TweenAlpha[] tws = this.gameObject.GetComponents<TweenAlpha>();
            if (tws.Length == 0 ) {
                TweenAlpha twa = TweenAlpha.Begin<TweenAlpha>(this.gameObject, 0.5f);
                if (twa != null)
                {
                    twa.delay = 0.0f;
                    twa.from = 1.0f;
                    twa.to = 0.0f;

                    MyTool.TweenSetOneShotOnFinish(twa, OnAlphaFinish);
                }
                Debug.LogErrorFormat("don't close BattleValue correct");
            }

        }
    }

    // 這裡是 spwan 後， 座標位置正確
    public void SetMode( int mode = 0 )
    {
        nMode = mode;
        //float x = transform.localPosition.x;
        fOriginY = transform.localPosition.y;
        float y = fOriginY;
        // MyTool.DestoryTweens(this.gameObject); // 不會立刻刪除物件，將造成新建立的 tween 於update 前被刪除

        float delaytime = 0.0f;

        if (nMode == 0)
        {
            TweenY twy = TweenY.Begin<TweenY>(gameObject, 0.5f);
            if (twy != null)
            {
                twy.from = y;
                twy.to = y + 64;
                twy.style = UITweener.Style.Once;
            }
            delaytime = 0.5f;
        }
        else if (nMode == 1)
        {
            TweenScale tws = TweenScale.Begin<TweenScale>(gameObject, 0.2f);
            if (tws != null)
            {
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                tws.SetStartToCurrentValue();
                //tws.from = new Vector3(0.5f, 0.5f, 0.5f);
                tws.to = new Vector3(1.0f, 1.0f, 1.0f);
                tws.style = UITweener.Style.Once;
            }

            TweenY twy = TweenY.Begin<TweenY>(gameObject, 0.5f);
            if (twy != null)
            {
                twy.delay = 0.5f;
                twy.from = y;
                twy.to = y + 64;
                twy.style = UITweener.Style.Once;
            }


            delaytime = 1f;
        }



        // re start to twa
        //		TweenAlpha twa = gameObject.GetComponent<TweenAlpha>(); // TweenAlpha.Begin<TweenAlpha> (this.gameObject, 1.0f );
        //		if (twa != null) {
        //		//	twa.Play();
        //		}

        MyTool.SetAlpha(this.gameObject, 1.0f);
        TweenAlpha twa = TweenAlpha.Begin<TweenAlpha>(this.gameObject, 0.5f);
        if (twa != null)
        {
            twa.delay = delaytime;
            //			twa.from = 1.0f;
            twa.to = 0.0f;

            MyTool.TweenSetOneShotOnFinish(twa, OnAlphaFinish);
        }
    }


    void PlayTweenY( )
	{
		TweenY twy = TweenY.Begin<TweenY> ( gameObject , 0.25f);
		if (twy != null) {
			twy.from = fOriginY ; 
			twy.to = fOriginY + fOriginY;

			MyTool.TweenSetOneShotOnFinish( twy , OnTweenXFinish );

			if( twy.direction == AnimationOrTween.Direction.Forward )
			{

			}
			else {

			}

		}
	}

	public void OnTweenXFinish()
	{
		TweenY twy = TweenY.Begin<TweenY> ( gameObject , 0.25f);
		if (twy != null) {
			twy.PlayReverse();
		}
	}

	public void OnAlphaFinish()
	{
//		nValueCount--;
		this.Recycle ();        
        MyTool.DestoryTweens(this.gameObject); // 於 關閉前把全部 alpha 刪除是最好的

    }
}
