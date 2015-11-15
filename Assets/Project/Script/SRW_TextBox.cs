using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;			// for parser string

//Text Box 於6/7改用 Ngui Textline 的方式呈現。
//好處是由 textline 幫忙管理換行，與文字變色。內容不會錯
//缺點是沒有 自己控制的『平滑』效果。會感覺文字都是忽然跳出來的。 很不舒服
//要解決本問題，必須改寫 NGUI 的TEXT LINE LIST。
//眼前先不處理，因為將來可能有人寫出更平順的元件可以替換。到時候在考慮吧

//還有個解決方案： 
//純使佣 UILABEL。 並在const text 控制好顯示文字長度。。每行顯示時都會清掉前面文字。
//如此可以兼顧到 平滑 ,文字變色, \n
//10 :41 改用此方案

public class SRW_TextBox : MonoBehaviour {

	public GameObject	_FaceTexObj;  // Face Texture
	public GameObject	_TextLineObj; // text show


	public int CharID { set; get; }
	//public List<string>		m_lstsContextAll; // all ine of text
	public List<string>		m_lstsContextWait; //  wait to pop
	public string		m_sPopText;
	public string		m_sShowText;
	int	m_nMaxCharNum;			//每最大
	int m_nMaxLineCount;
	int m_nCurLineCount;
	int m_nTextSpeed = 0;


	public bool bIsShaking = false;
	public bool bIsDeading = false;




	bool	m_bOnClickMode;		//
	public void SetClickMode()
	{
		m_bOnClickMode = true;
		UIEventListener.Get (_TextLineObj).onClick += OnTextBoxClick; // click event
	}

	public void ClearText()
	{	
		m_nCurLineCount = 0;
		m_sPopText = "";
		m_sShowText = "";
		UILabel lbl = _TextLineObj.GetComponentInChildren <UILabel>();
		if( lbl ){
			lbl.text = "";
		}
		UITextList list= _TextLineObj.GetComponent< UITextList > ();
		if( list )
			list.Clear ();
		//	m_lstsContextAll.Clear();
		m_lstsContextWait.Clear();
	}

	public void SetEnable( bool bActive )
	{
		ClearText ();
		this.gameObject.SetActive (bActive);

	}

	void Awake(){ // construct
		Debug.Log ("TextBox:awake");

		if (_TextLineObj != null) {

			//
			UILabel lbl = _TextLineObj.GetComponentInChildren <UILabel> ();
			if (lbl) {
				if (lbl.fontSize > 0) {
					m_nMaxCharNum = (lbl.width) / (int)(lbl.fontSize + lbl.floatSpacingX);
					//lbl.text = m_sShowText;
				}
				m_nMaxLineCount = lbl.maxLineCount;
			}
		}
		if( _FaceTexObj != null  )
		{
			NGUITools.SetActive( _FaceTexObj , false );
		}
	//	m_lstsContextAll  = new List<string> ();
		m_lstsContextWait = new List<string> ();
		nTweenObjCount = 0;
		m_bOnClickMode = false;
		ClearText ();

		//SetClickMode ();
	}

	// Use this for initialization
	void Start () {
		// first time to run
	
		// avoid blocked
		GameObject obj = this.gameObject as GameObject;
		if (obj.GetComponent<TweenWidth> () != null) {
			nTweenObjCount --;
		}

		UI2DSprite ui2d = obj.GetComponent<UI2DSprite>();
		if(ui2d != null )
		{

			TweenWidth t = TweenWidth.Begin<TweenWidth>( this.gameObject , 0.5f );
			if( t != null )
			{
				t.from = 0;
				t.to =  ui2d.width;
				t.SetOnFinished( OnTweenNotifyEnd );
				nTweenObjCount++;
			}
		}
		// test code 

		//UITextList list = _TextLineObj.GetComponent<UITextList> (); 
		//list.Add ("1嗚～先解決那個敵人再說。先[ff0000]點擊[-]我的頭像看看");
		//list.Add ("2嗚～先解決那個敵人再說。先[ff0000]點擊[-]我的頭像看看");
		//list.Add ("3嗚～先解決那個敵人再說。先[ff0000]點擊[-]我的頭像看看");
		//int i = list.CountPooled ();
		//UILabel lbl = list.textLabel;

		//list.Add ("4嗚～先解決那個敵人再說。先[ff0000]點擊[-]我的頭像看看");



//		UILabel lbl = _TextLineObj.GetComponentInChildren <UILabel>();
		//if( lbl ){
//			lbl.text = "嗚～先解決那個敵人再說。先[ff0000]點擊[-]我的頭像看看。\n嗚～先解決那個敵人再說。先[ff0000]點擊[-]我的頭像看看\n嗚～先解決那個敵人再說。先[ff0000]點擊[-]我的頭像看看。\n嗚～先解決那個敵人再說。先[ff0000]點擊[-]我的頭像看看\n";
//		}
//		AddText ("嗚～先解決那個敵人再說。先[ff0000]點擊[-]我的頭像看看。\n2嗚～先解決那個敵人再說。先[ff0000]點擊[-]我的頭像看看\n3嗚～先解決那個敵人再說。先[ff0000]點擊[-]我的頭像看看。\n4嗚～先解決那個敵人再說。先[ff0000]點擊[-]我的頭像看看\n");
		//AddText ("嗚～先解決那個敵人再說。先[ff0000]點擊[-]我的頭像看看。");
		//AddText ("[ff0000]問世間情是何物，直教人生死相許？[-]\n天南地北雙飛客，老翅幾回寒暑。\n歡樂趣，離別苦，是中更有痴兒女。\n君應有語，渺萬里層雲，\n千山暮雪，只影向誰去。");
		//AddText ("哎呀你什麼都不知道，我師傅綽號[ff0000]赤練仙子[-]，殺人不扎眼。她把我全家都殺光了。總之就是很危險，傻蛋你快想辦法吧。");
		//AddText("本遊戲的防禦系統較特別。每個角色身上都有防禦值，被攻擊時都會先扣除防禦值。\n當防禦值為零時才會真的扣除到生命值。生命值歸零則角色撤退不會真正死亡。\n防禦值的回復要靠同伴支援或防禦指令。同一回合內不受到任何攻擊則能全額回復。");
		//ChangeFace (0);
		//m_sPopText = "[ff0000]012345678[-]901234567890123456789012345678901234567890123456789</color>";
	}

	// Update is called once per frame
	void Update () {

		// Config.TextSpeed = 45; // text line

		Config.TextSpeed = 3; 	 // lable 

		// text pop speed
		if (++m_nTextSpeed < Config.TextSpeed ) {
			return;
		}
		if (nTweenObjCount > 0)
			return;

		m_nTextSpeed = 0;

		// use text list
// new method use text list
//		if (m_sPopText.Length > 0) {
//			//m_sPopText = ""; // clear here for last line have some delay time to read			
//			UITextList list = _TextLineObj.GetComponent<UITextList> (); 
//			list.Clear();		// clear line
//
//			list.Add ( m_sPopText );
//			//list.Add ( m_sPopText ); // for test scroll value
//
//			list.scrollValue = 1.0f;		// go to end line
//
//			m_sPopText = "";
//		} 
//		NextLine(); // go next line
		// new method end



// mark old
		// start to pop text
		if (m_sPopText.Length > 0) {
			ProcessText (1);
		}else {
			// NextLine(); // go to next line
			// close auto next line in 10/26
		}
// old end
	}

	// is all performance end
	public bool IsEnd()
	{
		if( nTweenObjCount > 0 )
			return false;
		if( m_sPopText.Length > 0 )
			return false;
		if( m_lstsContextWait.Count > 0 )
			return false;

		if (bIsShaking)
			return false;
		if (bIsDeading)
			return false;

		return true;
	}

	// click for next line
	public void OnTextBoxClick(GameObject go)
	{
		m_nTextSpeed = Config.TextSpeed;
// new method
//		NextLine();
//		return;
// new end 

// mark old
		if( m_sPopText != null )
		{
			if( m_sPopText.Length > 0  )
			{
				ProcessText( ); //  fast close current line

			}

			if( m_nCurLineCount >= m_nMaxLineCount ){
					// change page

					//m_sShowText = "";
					// manual remove first line
					int nPos = m_sShowText.IndexOf("\n");
					m_sShowText = m_sShowText.Substring( nPos +1 ); 
					m_nCurLineCount --; 

			
			}

			NextLine();
		}
// old end
	}

	//目前無法處理跨行 變色的拆行處理。如與到變色關鍵字剛好在換行。直接整個關鍵字下移一行
	public void AddText( string sText )
	{
		if( string.IsNullOrEmpty(sText) )
			return;
		if (string.IsNullOrEmpty (m_sPopText) == false) {
			m_sShowText += m_sPopText;
		}
		// change poptext
		// push m_sShowText to line
	
		m_sShowText = "";
		//check how many line in this text
		string [] sary = sText.Split( "\n".ToCharArray() );

// new method to use text list
//		foreach (string s in sary) {
//			if (s == null || s == " " || s == "\t")
//				continue;
//			if (s.IndexOf ("//") >= 0) // have common
//				continue;			    // giveup this line
//
//			m_lstsContextWait.Add( s );
//		}
//		m_nTextSpeed = Config.TextSpeed;
//		NextLine ();
//		return;
// new method end


		// float text version 2
		foreach (string s in sary) {
            if (string.IsNullOrEmpty(s))
                continue;
			if( s == " " || s == "\t")
				continue;
			if( s.IndexOf("//") >= 0 ) // have common
				continue;			    // giveup this line
			m_lstsContextWait.Add( s );
		}


//// old method to float text 
//
//		foreach (string s in sary) {
//			if( s == null || s == " " || s == "\t")
//				continue;
//			if( s.IndexOf("//") >= 0 ) // have common
//				continue;			    // giveup this line
//
//			if(m_nMaxCharNum == 0 )
//			{
//				m_lstsContextWait.Add( s );
//				continue;
//			}
//
//			//string sTemp = strRemain+s; // cur line
//			int idx = 0;
//			while( idx < s.Length )
//			{
//				int nLen = s.Length-idx;
//				if( nLen > m_nMaxCharNum)
//				{
//					nLen = m_nMaxCharNum ;
//				}
//
//				int nOffset = 0; // tag len of maxchar
//				int idx2 = idx; // temp idx
//				do{
//					if( idx2 >= s.Length )
//						break;
//
//					int n  = s.IndexOf("[" , idx2 );
//			// check tag during this line
//					if( (n>=idx2) && n <= (m_nMaxCharNum+nOffset)  ) // check '[' is vaild
//					{
//						int nEnd = s.IndexOf("]" , n );
//						if( nEnd < 0 )
//							break;
//
//						int shift = nEnd-n+1;
//
//						nOffset += shift;
//						nLen +=shift;   // for sub str
//
//						idx2 =nEnd+1;   // idx2 move next to ']' for while
//
//						// due to the tag always need a close "[-]". calcul it at the same time to avoid [-] in 17 byte and cut line soon
//				}
//					else{
//						break; // the first '[' out of maxchar
//
//					}
//					// check break;
//					if( (idx2 - nOffset) > m_nMaxCharNum )
//						break;
//
//				}while( true );				
//
//				if( (idx+nLen) > (s.Length+nOffset) )
//				{
//					// last string
//					m_lstsContextWait.Add( s.Substring( idx ) );
//					break;
//				}
//				// modify nlen for [xx ] tag
//
//
//				m_lstsContextWait.Add( s.Substring( idx , nLen ) );
//				idx +=nLen;
//
//			}//while( idx < s.Length )	
//		}
// float text ver 1 end

		NextLine ();
		// old method end
		//m_sPopText = sText;
	}


	public void NextLine()
	{
		if (m_lstsContextWait == null || m_lstsContextWait.Count == 0 )
			return;

		if( m_sPopText.Length > 0 ){
			m_nTextSpeed = Config.TextSpeed+1;
			return;
		}

		m_sPopText = m_lstsContextWait[0];
		m_lstsContextWait.RemoveAt( 0 );
		m_nCurLineCount++;



		// goto end line
//  new method start
//		return;
// new method end

		// ensure all text poped
// mark old
		m_sShowText = "";
		ProcessText ( 1 );

		// stop when end
		if (m_nMaxLineCount > 0 && m_nCurLineCount >= m_nMaxLineCount)		
		{
			if( this.m_bOnClickMode )
				return;

			// auto remove uppest line for match max line
			int nPos = m_sShowText.IndexOf("\n"); // remove first line
			m_sShowText = m_sShowText.Substring( nPos +1 ); 
			m_nCurLineCount --; 
		}

		if ( string.IsNullOrEmpty( m_sPopText) &&  m_lstsContextWait.Count > 0) {
			if( m_sShowText.Length > 0 && (m_sShowText[m_sShowText.Length-1 ]!= '\n') )
				m_sShowText += "\n";  			// change line and avoid double '\n'

			m_sPopText = m_lstsContextWait[0];
			m_lstsContextWait.RemoveAt( 0 );
			m_nCurLineCount++;
		}

// old end
	}

	void ProcessText( int nByte=0 )
	{
		if( string.IsNullOrEmpty(m_sPopText) )
			return;
		if (m_sPopText.Length == 0)
			return;

		string strSub;
		string strTmp;
		
		if( nByte > 0 )
		{
			int nOffset = 0;
			int nSt = m_sPopText.IndexOf( "[");
			if( nSt >= 0 && nSt <nByte  )
			{
				int nEnd = m_sPopText.IndexOf( "]" , nSt );
				nOffset = nEnd-nSt +1 ;
				nByte += nOffset;
			}

			// avoid crash
			if( nByte > m_sPopText.Length )
				nByte = m_sPopText.Length;

			strSub = m_sPopText.Substring( 0 , nByte) ;

			int nLen = (m_sPopText.Length-nByte);
			if( nLen > 0 )
				strTmp = m_sPopText.Substring(  nByte ,m_sPopText.Length-nByte ) ;
			else
				strTmp = "";
		}
		else{
			strSub = m_sPopText;
			strTmp = "";
			
		}

		m_sShowText += strSub;
		// 
		//if(strTmp.Length == 0 )
		//	m_sShowText += "\n";
		//
		UILabel lbl = _TextLineObj.GetComponentInChildren <UILabel>();
		if( lbl ){
			lbl.text = m_sShowText;
		}
		m_sPopText = strTmp;
		
	}

	public void ChangeFace( int nCharId )
	{
		if( nCharId <= 0 ){

			NGUITools.SetActive( _FaceTexObj ,  false );
			return ;
		}
		// 
		if( nCharId == CharID )
		{
			return ;
		}
		CharID = nCharId;

		// set face texture

		CHARS charData = ConstDataManager.Instance.GetRow<CHARS>( nCharId );
		//DataRow row = ConstDataManager.Instance.GetRow("CHARS", nCharId );
		//if( row != null )
		if( charData != null)
		//DataRow row = ConstDataManager.Instance.GetRow("CHARS", nCharId );
		//if( row != null )
		{	
		//	CHAR_DATA charData = new CHAR_DATA();
		//	charData.FillDatabyDataRow( row );
			// charge face text
			
			//UITexture tex = _FaceTexObj.GetComponentInChildren<UITexture>();
			UITexture tex = _FaceTexObj.GetComponent<UITexture>();
			if( tex )
			{
				if(tex != null){
					NGUITools.SetActive( _FaceTexObj ,  true );
					//	DynamicAssetBundleLoader.LoadTexture(tex,DynamicAssetBundleLoader.SSAssetType.Card, "CARD_" + card.PicName);
					//string texpath = "char/" +charData.s_FILENAME +"_S";
					string url = "Art/char/" + charData.s_FILENAME +"_S";
					//Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
					Texture t= Resources.Load( url , typeof(Texture) ) as Texture; ;
					tex.mainTexture = t;				
					//tex.MakePixelPerfect();

					TweenHeight twH = TweenHeight.Begin<TweenHeight>( _FaceTexObj , 0.2f );
					if( twH )
					{
						twH.from = 0;
						twH.to =  tex.height;
						twH.SetOnFinished( OnTweenNotifyEnd );
						nTweenObjCount++;
					}

				}
			}
		}
	}

	public void ChangeLayout( int layout = 0 )
	{
	//	UITexture tex = GetComponentInChildren<UITexture>();
	//	UILabel lbl = GetComponentInChildren <UILabel> ();


		if (layout == 0) {
			// base pos
			Vector3 vPos = new Vector3( 94 ,200 , 0 );
			this.transform.localPosition = vPos; 
			if( _FaceTexObj )
			{
				Vector3 vTexPos = _FaceTexObj.transform.localPosition;
				vTexPos.x = -450;
				_FaceTexObj.transform.localPosition = vTexPos;				
			}
		}
		else {
			Vector3 vPos = new Vector3( -100 ,-200 , 0 );
			this.transform.localPosition = vPos; 

			if( _FaceTexObj )
			{
				Vector3 vTexPos = _FaceTexObj.transform.localPosition;
				vTexPos.x = 450;
				_FaceTexObj.transform.localPosition = vTexPos;				
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
		TweenGrayLevel tw = TweenGrayLevel.Begin <TweenGrayLevel >( _FaceTexObj, 2.0f);
		if (tw) {
			bIsDeading = true;
			tw.from = 0.0f;
			tw.to   = 1.0f;
			MyTool.TweenSetOneShotOnFinish( tw , OnGrayEnd );
			//			tw.style = UITweener.Style.Once; // PLAY ONCE
			//			tw.SetOnFinished( OnDead );
			
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
