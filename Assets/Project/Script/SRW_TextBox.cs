using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;			// for parser string


public class SRW_TextBox : MonoBehaviour {

	public GameObject	_FaceTexObj;  // Face Texture
	public GameObject	_TextLineObj; // text show
	//public List<string>		m_lstsContextAll; // all ine of text
	public List<string>		m_lstsContextWait; //  wait to pop
	public string		m_sPopText;
	public string		m_sShowText;
	int	m_nMaxCharNum;			//每最大
	int m_nMaxLineCount;
	int m_nCurLineCount;
	int m_nTextSpeed = 0;

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
	//	m_lstsContextAll.Clear();
		m_lstsContextWait.Clear();
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

	//	m_lstsContextAll  = new List<string> ();
		m_lstsContextWait = new List<string> ();
		nTweenObjCount = 0;
		m_bOnClickMode = false;
		ClearText ();
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
		//AddText ("[ff0000]問世間情是何物，直教人生死相許？[-]\n天南地北雙飛客，老翅幾回寒暑。\n歡樂趣，離別苦，是中更有痴兒女。\n君應有語，渺萬里層雲，\n千山暮雪，只影向誰去。");
		//AddText ("哎呀你什麼都不知道，我師傅綽號[ff0000]赤練仙子[-]，殺人不扎眼。她把我全家都殺光了。總之就是很危險，傻蛋你快想辦法吧。");
		//AddText("本遊戲的防禦系統較特別。每個角色身上都有防禦值，被攻擊時都會先扣除防禦值。\n當防禦值為零時才會真的扣除到生命值。生命值歸零則角色撤退不會真正死亡。\n防禦值的回復要靠同伴支援或防禦指令。同一回合內不受到任何攻擊則能全額回復。");
		//ChangeFace (0);
		//m_sPopText = "[ff0000]012345678[-]901234567890123456789012345678901234567890123456789</color>";
	}

	// Update is called once per frame
	void Update () {
		if (nTweenObjCount > 0)
			return;

		if (m_nTextSpeed++ < 1) {
			return;
		}
		m_nTextSpeed = 0;

		// start to pop text
		if (m_sPopText.Length > 0) {
			ProcessText (1);
		}else {
			NextLine(); // go to next line
		}
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

		return true;
	}

	// click for next line
	public void OnTextBoxClick(GameObject go)
	{
		if( m_sPopText != null )
		{
			if( m_sPopText.Length > 0  )
			{
				NextLine();
			}
			else{
				if( m_nCurLineCount >= m_nMaxLineCount ){
					// change page

					//m_sShowText = "";
					// manual remove first line
					int nPos = m_sShowText.IndexOf("\n");
					m_sShowText = m_sShowText.Substring( nPos +1 ); 
					m_nCurLineCount --; 

					NextLine();
				}
			}
		}
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
		foreach (string s in sary) {
			if( s == null || s == " " || s == "\t")
				continue;
			if(m_nMaxCharNum == 0 )
			{
				m_lstsContextWait.Add( s );
				continue;
			}

			//string sTemp = strRemain+s; // cur line
			int idx = 0;
			while( idx < s.Length )
			{
				int nLen = s.Length-idx;
				if( nLen > m_nMaxCharNum)
				{
					nLen = m_nMaxCharNum ;
				}

				int nOffset = 0; // tag len of maxchar
				int idx2 = idx; // temp idx
				do{
					if( idx2 >= s.Length )
						break;

					int n  = s.IndexOf("[" , idx2 );

					if(  idx2 < s.Length ){ 
						string s2 = s.Substring( idx2 );  // for debug
					}
					if( n > 0 ){
						string debug =  s.Substring( n );  // for debug
					}

					// check tag during this line
					if( (n>=idx2) && n <= (m_nMaxCharNum+nOffset)  ) // check '[' is vaild
					{
						int nEnd = s.IndexOf("]" , n );
						if( nEnd < 0 )
							break;

						int shift = nEnd-n+1;

						nOffset += shift;
						nLen +=shift;   // for sub str

						idx2 =nEnd+1;   // idx2 move next to ']' for while

						// due to the tag always need a close "[-]". calcul it at the same time to avoid [-] in 17 byte and cut line soon
					//	int n2  = s.IndexOf("[" , idx2 );  // special check if end [ is end line
					//	idx2 = n2+3;
					//	nOffset +=3;
					//	nLen+=3;
					//	int nl2 = n2-nEnd;
					}
					else{
						break; // the first '[' out of maxchar

					}
					// check break;
					if( (idx2 - nOffset) > m_nMaxCharNum )
						break;

				}while( true );				

				if( (idx+nLen) > (s.Length+nOffset) )
				{
					// last string
					m_lstsContextWait.Add( s.Substring( idx ) );
					break;
				}
				// modify nlen for [xx ] tag


				m_lstsContextWait.Add( s.Substring( idx , nLen ) );
				idx +=nLen;

			}//while( idx < s.Length )
			//m_lstsContextWait.Add( s );
		}

		NextLine ();
		//m_sPopText = sText;
	}
	void NextLine()
	{
		if (m_lstsContextWait == null)
			return;

		// ensure all text poped
		ProcessText ();

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

		if (m_lstsContextWait.Count > 0) {
			if( m_sShowText.Length > 0 && (m_sShowText[m_sShowText.Length-1 ]!= '\n') )
				m_sShowText += "\n";  			// change line and avoid double '\n'

			m_sPopText = m_lstsContextWait[0];
			m_lstsContextWait.RemoveAt( 0 );
			m_nCurLineCount++;
		}
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
			
			UITexture tex = _FaceTexObj.GetComponentInChildren<UITexture>();
			
			if( tex )
			{
				if(tex != null){
					//	DynamicAssetBundleLoader.LoadTexture(tex,DynamicAssetBundleLoader.SSAssetType.Card, "CARD_" + card.PicName);
					//string texpath = "char/" +charData.s_FILENAME +"_S";
					string url = "Assets/Art/char/" + charData.s_FILENAME +"_S.png";
					//Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
					Texture t= Resources.LoadAssetAtPath( url , typeof(Texture) ) as Texture; ;
					tex.mainTexture = t;
					//tex.mainTexture = Resources.Load( texpath) as Texture; 
					//tex.MakePixelPerfect();
				}
			}
		}
	}

	public void ChangeLayout( int layout = 0 )
	{
		UITexture tex = GetComponentInChildren<UITexture>();
	//	UILabel lbl = GetComponentInChildren <UILabel> ();

		if (layout == 0) {
			// base pos
			Vector3 vPos = new Vector3( 86 ,200 , 0 );
			this.transform.localPosition = vPos; 

			if( tex != null )
			{
				TweenX t = TweenX.Begin<TweenX>( this.gameObject , 2f );
				if( t != null )
				{
					t.from = -810 ;
					t.to =  85 ;
					t.SetOnFinished( OnTweenNotifyEnd );
					nTweenObjCount++;
				}
			}

		

		}
		else {
			Vector3 vPos = new Vector3( -62 ,-200 , 0 );
			this.transform.localPosition = vPos; 
			
			if( tex != null )
			{
				Vector3 vTexPos = tex.transform.localPosition;
				vTexPos.x = 400;
				tex.transform.localPosition = vTexPos;

				TweenX t = TweenX.Begin<TweenX>(this.gameObject , 0.5f );
				if( t != null )
				{
					t.from =  820 ;
					t.to =  -80;
					t.SetOnFinished( OnTweenNotifyEnd );
					nTweenObjCount++;
				}
			}
		}
	}

	// untility
	int 	nTweenObjCount;
	public  void OnTweenNotifyEnd( )
	{
		if( --nTweenObjCount < 0 )
		{
			nTweenObjCount = 0;
		}
	}
}
