using UnityEngine;
using System.Collections;

public class SRW_AVGObj : MonoBehaviour {

	//public GameObject	_FaceTexObj;  // Face Texture
	public UITexture _FaceTexObj;  // Face Texture
	public int CharID { set; get; }
    public int FaceID { set; get; }
    public bool bIsZoom = true;    
    public bool bIsShaking = false;
	public bool bIsDeading = false;
    public bool bIsReplacing = false;
    public int nLayout;


	static public Color clrEnable 	  = new Color( 1.0f , 1.0f , 1.0f )   ;
	static public Color clrDisEnable  = new Color( 0.65f , 0.65f , 0.65f );
    // Use this for initialization

    void SetMaterialParamCB(Material mat)
    {       
        //  mat.SetFloat("_xx", 1.0f);
        TweenGrayLevel tw = _FaceTexObj.GetComponent<TweenGrayLevel>();
        if (tw)
        {
            mat.SetFloat("_GrayLevelScale", tw.fPoint); // 防止 tranform move 後，NGUI reset mat 而沒有  graylevel
        }
        else {
    //        mat.SetFloat("_GrayLevelScale", 0 ); // no gray
        }
      //  mat.SetFloat("_GrayLevelScale", 0.5f );
    }

    void Awake(){ // construct
		CharID = 0;
        FaceID = 0;
        if (_FaceTexObj == null)
        {
            _FaceTexObj = this.gameObject.GetComponent<UITexture>();
        }
        _FaceTexObj.onRender = SetMaterialParamCB; // 設定render shader
    }

	void OnEnable()
	{
		transform.localRotation = new Quaternion();
        transform.localScale = Vector3.one;
		if (_FaceTexObj != null) {
			_FaceTexObj.alpha = 1.0f;
            // clear face tween com
            // 由於 是 create new prefab 。所以 mat 都是 新的
        //    UITexture texture = _FaceTexObj.GetComponent<UITexture>();
      //      texture.drawCall.dynamicMaterial.SetFloat("_GrayLevelScale", 0.0f);

            //UITweener[] tws = _FaceTexObj.GetComponents<UITweener>();
            //foreach (UITweener tw in tws)
            //{
            //    Destroy(tw);
            //}
        }

        ClearFlag();
	}


	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void ClearFlag() {
        bIsZoom = false;
        bIsShaking = false;
        bIsDeading = false;
    }

	public bool IsEnd()
	{
        //		if( nTweenObjCount > 0 )
        //			return false;
        //		if( m_sPopText.Length > 0 )
        //			return false;
        //		if( m_lstsContextWait.Count > 0 )
        //			return false;
        //		
        if (gameObject.activeSelf == false){
            return true;  // disable is end too
        }


        if (bIsZoom)
            return false;
		if (bIsShaking)
			return false;
		if (bIsDeading)
			return false;
        if(bIsReplacing)
            return false;

        return true;
	}

    public void SetChar(int nCharId) // 0 - close obj
    {
        if (nCharId <= 0)
        {
            ClearFlag();
            NGUITools.SetActive(_FaceTexObj.gameObject, false);
            NGUITools.SetActive(this.gameObject, false);
            return;
        }

        CharID = nCharId;
        SetFace(GameDataManager.Instance.GetUnitFaceID(CharID));

    }

    public void SetFace( int nFaceId )
	{
        // 

        // set face texture
       // int nFaceID = GameDataManager.Instance.GetUnitFaceID(CharID);   
       		if( _FaceTexObj != null  )
			{
				
					NGUITools.SetActive( this.gameObject ,  true );
                    NGUITools.SetActive(_FaceTexObj.gameObject, true);
                //	DynamicAssetBundleLoader.LoadTexture(tex,DynamicAssetBundleLoader.SSAssetType.Card, "CARD_" + card.PicName);
                //					string url = "Art/char/" + charData.s_FILENAME +"_L";
                //Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
                //					Texture t= Resources.Load( url , typeof(Texture) ) as Texture; ;
                _FaceTexObj.mainTexture = MyTool.GetCharTexture(nFaceId, 1 );			
					//tex.MakePixelPerfect();
					
					TweenHeight twH = TweenHeight.Begin<TweenHeight>( this.gameObject , 0.2f );
					if( twH )
					{
						twH.from = 0;
						twH.to =  _FaceTexObj.height;
						twH.SetOnFinished( OnTweenNotifyEnd );
						nTweenObjCount++;
					}
			}
        FaceID = nFaceId;
    }
    public void ReplaceChar(int nCharId)
    {
        if (CharID == nCharId)
        {
            return;
        }
        CharID = nCharId;

        ReplaceFace( GameDataManager.Instance.GetUnitFaceID(CharID) );
    }

    public void ReplaceFace(int nFaceId)
    {
        if (FaceID == nFaceId)
        {
            return;
        }
        FaceID = nFaceId;
      //  int nFaceID = GameDataManager.Instance.GetUnitFaceID(CharID);
       

            NGUITools.SetActive(this.gameObject, true);
            
           
            //tex.MakePixelPerfect();
            bIsReplacing = true;
            TweenRotation twr = gameObject.AddComponent<TweenRotation>();  //TweenRotation.Begin<TweenRotation>(this.gameObject, 0.2f);
            if (twr != null)
            {
                twr.duration = 0.3f;
                twr.SetStartToCurrentValue();
                twr.to = new Vector3(0.0f, 270.0f, 0.0f);//Math.PI
                MyTool.TweenSetOneShotOnFinish(twr, OnTwReplaceFace); // for once only
              
            }
            TweenRotation twr2 = gameObject.AddComponent<TweenRotation>();  //TweenRotation.Begin<TweenRotation>(this.gameObject, 0.2f);
            if (twr2 != null)
            {
                twr2.delay    = 0.3f;
                twr2.duration = 0.4f;
                twr2.from = new Vector3(0.0f, 270.0f, 0.0f);//Math.PI
                twr2.to = new Vector3(0.0f, 360.0f, 0.0f);//Math.PI
                MyTool.TweenSetOneShotOnFinish(twr2, OnTwReplaceRotateEnd); // for once only

            }

            // twR.from = Vector3.n();
            //TweenHeight twH = TweenHeight.Begin<TweenHeight>(this.gameObject, 0.2f);
            //if (twH)
            //{
            //    twH.from = 0;
            //    twH.to = _FaceTexObj.height;
            //    twH.SetOnFinished(OnTweenNotifyEnd);
            //    nTweenObjCount++;
            //}

    }

    public void OnTwReplaceFace()
    {
        if (_FaceTexObj != null)
        {
            NGUITools.SetActive(_FaceTexObj.gameObject, true);
            // int nFaceID = GameDataManager.Instance.GetUnitFaceID(CharID);
            if (_FaceTexObj != null)
            {
                _FaceTexObj.mainTexture = MyTool.GetCharTexture(FaceID, 1);
            }
        }
    }
    public void OnTwReplaceRotateEnd()
    {
        // clear all move tw
        TweenRotation[] tws = gameObject.GetComponents<TweenRotation>();
        foreach (TweenRotation tw in tws)
        {
            Destroy(tw);
        }


        // reset pos
        gameObject.transform.localRotation = Quaternion.identity;
        bIsReplacing = false;
    }

    public void ChangeLayout( int layout = 0 )
	{  
        //	UITexture tex = GetComponentInChildren<UITexture>();
        //	UILabel lbl = GetComponentInChildren <UILabel> ();
        nLayout = layout;

        if (layout == 0) { // left
			// base pos
			Vector3 vPos = new Vector3( -240 ,0 , 0 );
			this.transform.localPosition = vPos; 
//			if( this.gameObject )
//			{
//				Vector3 vTexPos = _FaceTexObj.transform.localPosition;
//				vTexPos.x = -450;
//				_FaceTexObj.transform.localPosition = vTexPos;				
//				UITexture uiTex = this.gameObject.GetComponent<UITexture>(); 
				if( _FaceTexObj != null ){
					_FaceTexObj.flip = UIBasicSprite.Flip.Horizontally;
				}

//			}
		}
		else {				// right
			Vector3 vPos = new Vector3( 240 ,0 , 0 );
			this.transform.localPosition = vPos; 
			
//			if( this.gameObject )
//			{
//				Vector3 vTexPos = _FaceTexObj.transform.localPosition;
//				vTexPos.x = 450;
//				_FaceTexObj.transform.localPosition = vTexPos;				

//				UITexture uiTex = this.gameObject.GetComponent<UITexture>(); 
				if( _FaceTexObj != null ){
					_FaceTexObj.flip = UIBasicSprite.Flip.Nothing;
				}

//			}
		}

        ZoomIn();
	}

	public void Speak( bool bSpeak )
	{
        if ( _FaceTexObj != null ){
			if( bSpeak ){
//				_FaceTexObj.color = clrEnable;
                _FaceTexObj.depth = 3; // spealer is more front

                TweenScale tw = TweenScale.Begin<TweenScale>(_FaceTexObj.gameObject, 0.2f);
                if (tw != null) {
                    tw.SetStartToCurrentValue();
                    tw.to = Vector3.one * 1.1f;
                }
            }
			else {
//				_FaceTexObj.color = clrDisEnable;
                _FaceTexObj.depth = 2;
                TweenScale tw = TweenScale.Begin<TweenScale>(_FaceTexObj.gameObject, 0.2f);
                if (tw != null)
                {
                    tw.SetStartToCurrentValue();
                    tw.to = Vector3.one * 1.0f;
                }
            }
		}
	}
	
	public void SetShake(  )
	{
        // 強制結束 zoomin 並設定到正確位置。不然之後的shake 會造成座標錯誤
        if (bIsZoom == true ) {
            bIsZoom = false;
            TweenPosition tw = gameObject.GetComponent<TweenPosition>();
            if (tw != null) {
                tw.transform.localPosition = tw.to;
                tw.enabled = false;
            }
        }

        TweenShake tws = TweenShake.Begin (this.gameObject, 1.5f, 30.0f);
		if( tws )
		{
			bIsShaking = true;
			tws.shakeX = true;
			tws.shakeY = true;
			MyTool.TweenSetOneShotOnFinish( tws , OnShakeEnd );
			//Destroy( tws , 2.1f ); // 
		}
		
	}
	public void SetDead(  )
	{
       // TweenGrayLevel tw = TweenGrayLevel.Begin<TweenGrayLevel>( _FaceTexObj.gameObject , 2.0f);
        TweenGrayLevel tw = GrayLevelHelper.StartTweenGrayLevel(_FaceTexObj, 2.0f );
        if (tw) {
			bIsDeading = true;
			tw.from = 0.0f;
			tw.to   = 1.0f;
			MyTool.TweenSetOneShotOnFinish( tw , OnDeadEnd);
			//			tw.style = UITweener.Style.Once; // PLAY ONCE
			//			tw.SetOnFinished( OnDead );
			
		}
	}

    public void SetColor( Color color )
    {
        _FaceTexObj.color = color;
    }
    public void SetGray( int nDisable )
    {
        // 灰階在 GO移動時會變回彩色的原因是 NGUI 內部在 tranform 改變時，會偷換 material 來整理 drawcall
        // 連帶使 shader 還原。

        UITexture texture = _FaceTexObj.GetComponent<UITexture>();
//        texture.drawCall.dynamicMaterial.SetFloat("_GrayLevelScale", 1.0f);
        texture.material = new Material(Shader.Find("Custom/GrayLevel")); // 更換shader
        texture.material.SetFloat("_GrayLevelScale", 1.0f );

        return;
       //// TweenGrayLevel tw = TweenGrayLevel.Begin<TweenGrayLevel>(_FaceTexObj.gameObject, 2.0f);
       // TweenGrayLevel tw = GrayLevelHelper.StartTweenGrayLevel(_FaceTexObj, 0.0f);
       // if (tw)
       // {
       //     if (nDisable == 1) // full color
       //     {
       //         tw.from = 1.0f;
       //         tw.to = 0.0f;
       //     }
       //     else { // default is gray
       //         tw.from = 0.0f;
       //         tw.to = 1.0f;                
       //     }
       //     tw.duration = 5.1f;
       //     //tw.ResetToBeginning();
       //     tw.Play();
            

       //    // MyTool.TweenSetOneShotOnFinish(tw, OnGrayEnd);
       //     //			tw.style = UITweener.Style.Once; // PLAY ONCE
       //     //			tw.SetOnFinished( OnDead );

       // }
    }

    public void SetAlpha(float fAlpha)
    {
        UITexture texture = _FaceTexObj.GetComponent<UITexture>();
        //    texture.material = new Material(Shader.Find("Custom/GrayLevel"));
        //    texture.material.SetFloat("_GrayLevelScale", 1.0f);
        texture.alpha = fAlpha;
        return;
    }
        

    public void FadeIn()
    {
        _FaceTexObj.alpha = 0.0f;
        TweenAlpha twa = TweenAlpha.Begin<TweenAlpha>(this.gameObject, 0.2f);
        if (twa != null)
        {
            twa.to = 1.0f;
        }
    }
    public void FadeOut( )
	{
        _FaceTexObj.alpha = 1.0f;
		TweenAlpha twa = TweenAlpha.Begin<TweenAlpha> (this.gameObject, 0.2f);
		if (twa != null) {
			twa.to = 0.0f;
            twa.SetOnFinished(OnOutDestory);
        }
	}

    public void OnOutDestory() {

        bIsZoom = false;
        NGUITools.Destroy(gameObject); // delete this
    }

    public void ZoomIn()
    {
        float fY = 128.0f;
        Vector3 vPos;
        Vector3 vTar;

        //this.nLayout; // 0- left , 1 - right
        if (nLayout == 0)
        {
            vPos = new Vector3(-960, fY, 0);
            vTar = new Vector3(-240, fY, 0);
        }
        else {
            vPos = new Vector3(960, fY, 0);
            vTar = new Vector3(240, fY, 0);
        }
        this.gameObject.transform.localPosition = vPos;

        TweenPosition tw = TweenPosition.Begin<TweenPosition>(gameObject, 0.5f );
        if (tw) {
            bIsZoom = true;
            tw.from = vPos;
            tw.to = vTar;
            tw.SetOnFinished(OnZoomEnd);
        }

    }

    public void ZoomOut()
    {
        float fY = 128.0f;
        Vector3 vPos;
        Vector3 vTar;

        //this.nLayout; // 0- left , 1 - right
        if (nLayout == 0)
        {
            vTar = new Vector3(-960, fY, 0);
            vPos = new Vector3(-240, fY, 0);
        }
        else
        {
            vTar = new Vector3(960, fY, 0);
            vPos = new Vector3(240, fY, 0);
        }
        this.gameObject.transform.localPosition = vPos;

        TweenPosition tw = TweenPosition.Begin<TweenPosition>(gameObject, 0.5f);
        if (tw)
        {
            bIsZoom = true;
            tw.from = vPos;
            tw.to = vTar;
            tw.SetOnFinished(OnOutDestory);
        }

    }
    public void OnZoomEnd()
    {
        bIsZoom = false;

    }


    public void SetScale(  float fScale)
	{
		//_FaceTexObj.alpha = 1.0f;
		TweenScale tws = TweenScale.Begin<TweenScale> (this.gameObject, 0.5f);
		if (tws != null) {
			tws.to = new Vector3( fScale , fScale );
		}
	}

	public void OnShakeEnd()
	{	
		bIsShaking = false;
		// remove char
	}

    public void OnDeadEnd()
    {
        bIsDeading = false;

        // remove char
        TalkSayEndEvent evt = new TalkSayEndEvent();
        evt.nChar = this.CharID;
        GameEventManager.DispatchEvent(evt);
    }

 

	// untility
	int 	nTweenObjCount=0;
	public  void OnTweenNotifyEnd(  )
	{
		if( --nTweenObjCount < 0 )
		{
			nTweenObjCount = 0;
		}
	}
}
