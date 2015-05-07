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
	void Awake(){
		Debug.Log ("TextBox:awake");
		UIEventListener.Get(_TextLineObj).onClick += OnTextBoxClick; // click event

		//
		UILabel lbl = _TextLineObj.GetComponentInChildren <UILabel>();
		if( lbl ){
			if( lbl.fontSize > 0 ){
				m_nMaxCharNum = (lbl.width) /(int)(lbl.fontSize+lbl.floatSpacingX);
			//lbl.text = m_sShowText;
			}
			m_nMaxLineCount = lbl.maxLineCount;
		}
	//	m_lstsContextAll  = new List<string> ();
		m_lstsContextWait = new List<string> ();
	}

	// Use this for initialization
	void Start () {
		nTweenObjCount = 0;

		ClearText ();
	
		// avoid blocked
		GameObject obj = this.gameObject as GameObject;
		if (obj.GetComponent<TweenWidth> () != null) {
			nTweenObjCount --;
		}

		UI2DSprite ui2d = obj.GetComponent<UI2DSprite>();
		if(ui2d != null )
		{

			TweenWidth t = TweenWidth.Begin<TweenWidth>( this.gameObject , 1.0f );
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
		//AddText ("哎呀你什麼都不知道，我師傅綽號\n[ff0000]赤練仙子[-]，殺人不扎眼。她把我全家都殺光了。總之就是很危險，傻蛋你快想辦法吧。");
		AddText("本遊戲的防禦系統較特別。每個角色身上都有防禦值，被攻擊時都會先扣除防禦值。\n當防禦值為零時才會真的扣除到生命值。生命值歸零則角色撤退不會真正死亡。\n防禦值的回復要靠同伴支援或防禦指令。同一回合內不受到任何攻擊則能全額回復。");
		//ChangeFace (0);
		//m_sPopText = "[ff0000]012345678[-]901234567890123456789012345678901234567890123456789</color>";
	}
	
	// Update is called once per frame
	void Update () {
		if (nTweenObjCount > 0)
			return;
		// start to pop text
		if (m_sPopText.Length > 0) {
			ProcessText (1);
		}else {
			PopNextText();

		}
	}

	// click for next line
	void OnTextBoxClick(GameObject go)
	{
		if( m_sPopText != null )
		{
			if( m_sPopText.Length > 0  )
			{
				PopNextText();
			}
			else{
				if( m_nCurLineCount >= m_nMaxLineCount ){
					// change page

					//m_sShowText = "";
					// remove first line
					int nPos = m_sShowText.IndexOf("\n");
					m_sShowText = m_sShowText.Substring( nPos +1 ); 
					m_nCurLineCount --; 
					PopNextText();
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
		//m_lstsContextAll.Add( m_sShowText );
		m_sShowText = "";
		//check how many line in this text
		string [] sary = sText.Split( "\n".ToCharArray() );
		foreach (string s in sary) {
			if( s == null || s == " " || s == "\t")
				continue;

			int idx = 0;
			while( idx < s.Length )
			{
				int nLen = m_nMaxCharNum;
				int nOffset = 0;
				int idx2 = idx; // temp idx
				do{
					string s2 = s.Substring( idx2 ); 

					int n  = s.IndexOf("[" , idx2 );
					// check tag during this line
					int st2 = n-idx2;
					if( st2>=0 && st2 <= (m_nMaxCharNum+nOffset)  ) // check '[' is vaild
					{
						int nEnd = s.IndexOf("]" , n );
						int nofflen = nEnd-n+1;

						nOffset += nofflen;
						nLen +=nofflen;   // for sub str
						idx2 +=nofflen;   // for while

						// due to the tag always need a close "[-]". calcul it at the same time to avoid [-] in 17 byte and cut line soon
						int n2  = s.IndexOf("[-]" , idx2 );
						idx2 = n2+3;
						nOffset +=3;
						nLen+=3;
					}
					else{
						break;

					}
					// check break;

				}while( true );				

				if( (idx+nLen) > (s.Length+nOffset) )
				{
					// last string
					m_lstsContextWait.Add( s.Substring( idx ) );
					break;
				}
				// modify nlen for [xx ] tag

				//string sub = s.Substring( idx , idx+m_nMaxCharNum );
				m_lstsContextWait.Add( s.Substring( idx , nLen ) );
				idx +=nLen;
				//m_lstsContextWait.Add( sub );
			}
			//m_lstsContextWait.Add( s );
		}

		PopNextText ();
		//m_sPopText = sText;
	}
	void PopNextText()
	{
		if (m_lstsContextWait == null)
			return;

		// ensure all text poped
		ProcessText ();

		// stop when end
		if( m_nCurLineCount >= m_nMaxLineCount )
			return ;

		if (m_lstsContextWait.Count > 0) {
			//if( m_sShowText.Length > 0)
			//	m_sShowText += "\n";  			// change line
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
			strSub = m_sPopText.Substring( 0 , nByte) ;
			
			int nLen = m_sPopText.Length-nByte ;
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
		if(strTmp.Length == 0 )
			m_sShowText += "\n";
		//
		UILabel lbl = _TextLineObj.GetComponentInChildren <UILabel>();
		if( lbl ){
			lbl.text = m_sShowText;
		}
		m_sPopText = strTmp;
		
	}

	public void ChangeFace( int nCharIdx )
	{
		UITexture tex = _FaceTexObj.GetComponentInChildren<UITexture>();
		
		if( tex )
		{
			if(tex != null){
				//	DynamicAssetBundleLoader.LoadTexture(tex,DynamicAssetBundleLoader.SSAssetType.Card, "CARD_" + card.PicName);
				//string texpath = "char/" +charData.s_FILENAME +"_S";
				string url = "Assets/Art/char/" + "00_0001" +"_S.png";
				//Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
				Texture t= Resources.LoadAssetAtPath( url , typeof(Texture) ) as Texture; ;
				tex.mainTexture = t;
				//tex.mainTexture = Resources.Load( texpath) as Texture; 
	//			tex.MakePixelPerfect();
			}
		}

	}

	public void ChangeLayout( int layout = 0 )
	{
		if (layout == 1) {

		}
		else {

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
