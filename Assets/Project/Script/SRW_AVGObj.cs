using UnityEngine;
using System.Collections;

public class SRW_AVGObj : MonoBehaviour {

	//public GameObject	_FaceTexObj;  // Face Texture
	public UITexture _FaceTexObj;  // Face Texture
	public int CharID { set; get; }
	public bool bIsShaking = false;
	public bool bIsDeading = false;

	static public Color clrEnable 	  = new Color( 1.0f , 1.0f , 1.0f )   ;
	static public Color clrDisEnable  = new Color( 0.65f , 0.65f , 0.65f );
	// Use this for initialization

	void Awake(){ // construct
		CharID = 0;
		_FaceTexObj = this.gameObject.GetComponent<UITexture>(); 

	}

	void OnEnable()
	{
		transform.localRotation = new Quaternion(); 
		transform.localScale = new Vector3( 1.0f, 1.0f ,1.0f);
		if (_FaceTexObj != null) {
			_FaceTexObj.alpha = 1.0f;
		}
	}


	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
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
		if (bIsShaking)
			return false;
		if (bIsDeading)
			return false;
		
		return true;
	}
	public void ChangeFace( int nCharId )
	{
		if( nCharId <= 0 ){			
			NGUITools.SetActive( this.gameObject ,  false );
			return ;
		}
		// 
		if( nCharId == CharID ){
			return ;
		}
		CharID = nCharId;
		
		// set face texture
		
//		CHARS charData = ConstDataManager.Instance.GetRow<CHARS>( nCharId );
		//DataRow row = ConstDataManager.Instance.GetRow("CHARS", nCharId );
		//if( row != null )
//		if( charData != null)
			//DataRow row = ConstDataManager.Instance.GetRow("CHARS", nCharId );
			//if( row != null )
		{	
			//	CHAR_DATA charData = new CHAR_DATA();
			//	charData.FillDatabyDataRow( row );
			// charge face text
			
			//UITexture tex = _FaceTexObj.GetComponentInChildren<UITexture>();
			//UITexture tex = this.gameObject.GetComponent<UITexture>();
			if( _FaceTexObj != null  )
			{
				
					NGUITools.SetActive( this.gameObject ,  true );
					//	DynamicAssetBundleLoader.LoadTexture(tex,DynamicAssetBundleLoader.SSAssetType.Card, "CARD_" + card.PicName);
//					string url = "Art/char/" + charData.s_FILENAME +"_L";
					//Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
//					Texture t= Resources.Load( url , typeof(Texture) ) as Texture; ;
                    _FaceTexObj.mainTexture = MyTool.GetCharTexture(nCharId , 1 );			
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
		}
	}
	
	public void ChangeLayout( int layout = 0 )
	{
		//	UITexture tex = GetComponentInChildren<UITexture>();
		//	UILabel lbl = GetComponentInChildren <UILabel> ();

		
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
	}

	public void Speak( bool bSpeak )
	{
		if( _FaceTexObj != null ){
			if( bSpeak ){
				_FaceTexObj.color = clrEnable;
			}
			else {
				_FaceTexObj.color = clrDisEnable;
			}
		}
	}
	
	public void SetShake(  )
	{
		TweenShake tws = TweenShake.Begin (this.gameObject, 2.0f, 40.0f);
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
		TweenGrayLevel tw = TweenGrayLevel.Begin <TweenGrayLevel >( this.gameObject, 2.0f);
		if (tw) {
			bIsDeading = true;
			tw.from = 0.0f;
			tw.to   = 1.0f;
			MyTool.TweenSetOneShotOnFinish( tw , OnGrayEnd );
			//			tw.style = UITweener.Style.Once; // PLAY ONCE
			//			tw.SetOnFinished( OnDead );
			
		}
	}
	public void FadeOut( )
	{
		_FaceTexObj.alpha = 1.0f;
		TweenAlpha twa = TweenAlpha.Begin<TweenAlpha> (this.gameObject, 0.5f);
		if (twa != null) {
			twa.to = 0.0f;
		}
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
	
	public void OnGrayEnd()
	{	
		bIsDeading = false;
		
		// remove char
		TalkSayEndEvent evt = new TalkSayEndEvent();
		evt.nChar = this.CharID;
		GameEventManager.DispatchEvent ( evt  );
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
