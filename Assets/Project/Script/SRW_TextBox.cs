using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;			// for parser string


public class SRW_TextBox : MonoBehaviour {

	public GameObject	_FaceTexObj;  // Face Texture
	public GameObject	_TextLineObj; // text show
	public List<string>		m_lstsContextAll; // all ine of text
	public string		m_sPopText;
	public string		m_sShowText;



	public void ClearText()
	{
		m_sShowText = "";
		UILabel lbl = _TextLineObj.GetComponentInChildren <UILabel>();
		if( lbl ){
			lbl.text = "";
		}
	
	}

	// Use this for initialization
	void Start () {
		nTweenObjCount = 0;
		m_lstsContextAll = new List<string> ();;
		m_sPopText = "";
		m_sShowText = "";

		ClearText ();
	
		GameObject obj = this.gameObject as GameObject;
		if (obj.GetComponent<TweenWidth> () != null) {
			nTweenObjCount --;
		}

		UI2DSprite ui2d = obj.GetComponent<UI2DSprite>();
		if(ui2d != null )
		{

	//		var renderer = obj.GetComponent<Renderer>();
	//		float width = renderer.bounds.size.x;


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
		AddText ("[ff0000]012345678[-]901\n23 45 678 901 23456789012345678901234567890123456789");
		//m_sPopText = "[ff0000]012345678[-]901234567890123456789012345678901234567890123456789</color>";
	}
	
	// Update is called once per frame
	void Update () {
		if (nTweenObjCount > 0)
			return;
		// start to pop text
		if (m_sPopText.Length > 0 ) {
			ProcessText( 1 );
		}
	}

	public void AddText( string sText )
	{
		if( string.IsNullOrEmpty(sText) )
			return;
		if (string.IsNullOrEmpty (m_sPopText) == false) {
			m_sShowText += m_sPopText;
		}
		// change poptext
		m_sPopText = sText;
		// push m_sShowText to line
		//m_lstsContextAll.Add( m_sShowText );
		m_sShowText = "";
		//check how many line in this text


	}


	void ProcessText( int nByte=0 )
	{
		if( string.IsNullOrEmpty(m_sPopText) )
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
		UILabel lbl = _TextLineObj.GetComponentInChildren <UILabel>();
		if( lbl ){
			lbl.text = m_sShowText;
		}
		m_sPopText = strTmp;
		
	}

	public void ChangeFace( int nCharIdx , int mirrior =0 )
	{


	}

	public void ChangeLayout( int layout  )
	{
	
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
