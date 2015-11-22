using UnityEngine;
using System.Collections;

public class Panel_char : MonoBehaviour {

	public GameObject FaceObj;
    bool bIsAlphaing;
    bool bIsGraying;
    bool bIsMoveing;

    public void OnEnable()
	{
        bIsGraying = false;
        bIsGraying = false;
        bIsMoveing = false;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool IsIdle()
    {
        if (bIsGraying  || bIsAlphaing || bIsMoveing  ) {
            return false;
        }


        return true;
    }

    public void SetFace(int nCharId)
    {
        CHARS charData = ConstDataManager.Instance.GetRow<CHARS>(nCharId);
        if (charData != null)
        {
            // charge face text				
            UITexture tex =  FaceObj.GetComponent<UITexture>();

            if (tex)
            {
                if (tex != null)
                {
                    //	DynamicAssetBundleLoader.LoadTexture(tex,DynamicAssetBundleLoader.SSAssetType.Card, "CARD_" + card.PicName);
                    //string texpath = "char/" +charData.s_FILENAME +"_S";
                    string url = "Art/char/" + charData.s_FILENAME + "_S";
                    //Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
                    //Texture t= Resources.Load( url , typeof(Texture) ) as Texture; ;
                    tex.mainTexture = MyTool.GetCharTexture( nCharId );  //Resources.Load(url, typeof(Texture)) as Texture; ;
                    //tex.mainTexture = Resources.Load( texpath) as Texture; 
                    //tex.MakePixelPerfect();
                }
            }
        }

    }


    public void Moveto( float fX , float fY , float during = 1.5f)
    {
        TweenPosition t = TweenPosition.Begin(this.gameObject, during , new Vector3(fX, fY, transform.localPosition.z)); //直接移動
        if (t != null)
        {
            bIsMoveing = true;
            t.SetStartToCurrentValue();
            t.SetOnFinished(OnTweenMoveEnd);
          //  nTweenObjCount++;
        }
    }

    public void StartAlpha( float from , float to , float during = 1.5f ) {

        TweenAlpha tObj = TweenAlpha.Begin(this.gameObject, during, to);
        if (tObj != null)
        {
            bIsAlphaing = true;
            tObj.from = from;
            tObj.SetOnFinished(OnTweenAlphaEnd);
           // nTweenObjCount++;
        }

    }

    public void StartGray()
	{
		if (FaceObj == null)
			return;

		UITexture texture = FaceObj.GetComponent< UITexture >(); 
		
		//TweenGrayLevel.Begin <TweenGrayLevel>(  texture , 1.0f  );
		TweenGrayLevel tw = TweenGrayLevel.Begin <TweenGrayLevel >( texture.gameObject, 1.0f);
		if (tw) {
            bIsGraying = true;
            tw.from = 0.0f;
			tw.to   = 1.0f;
			MyTool.TweenSetOneShotOnFinish( tw , OnDead );
		}

	}


	public void OnDead( )
	{
        bIsGraying = false;
        //GameEventManager.DispatchEvent ( evt );
    }

    public void OnTweenAlphaEnd()
    {
        bIsAlphaing = false;
    }
    public void OnTweenMoveEnd()
    {
        bIsMoveing = false;
    }
    
}
