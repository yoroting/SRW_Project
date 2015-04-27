using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MyClassLibrary
{

    // 針對指定的文字建立 2 維矩陣
	// Sample code:
	// using MyClassLibrary;
	// 
	//	string s1 = "linetext( 1 );LineText(\t2)\nPopChar(3 ,4);PopChar( 4 , 5 ,6)。\nSysText(  \"test\" )。";

	//	char[] colChars = { ' ', '(', ')',';', ',', '\t' };
	//	char[] rowChars = { '\n' };	
	//	cTextArray txt = new cTextArray(rowChars , colChars );
	//	txt.SetText( s1 ); // 將猜解各字串


    public class cTextArray
    {
        // text line 
        public  class CTextLine 
    	{
            public CTextLine()
            { 
                m_kTextPool = new List<string>();
            }
            public int GetRowNum() { return m_kTextPool.Count; }
		    public List< string >	m_kTextPool;

            public string GetString( int nRow ) {
                if (nRow < 0 || (nRow >= m_kTextPool.Count))
                    return null;
                return m_kTextPool[nRow]; }
	    }

        //==============================

        public cTextArray(char[] cRawToken, char[] cColToken )
        { 
             m_sRawToken =  new string( cRawToken );
             m_sColToken =  new string( cColToken );
             m_kTextLinePool = new List<CTextLine>();
        }
        public void Clear()
        {

            m_kTextLinePool.Clear();
        
        }

        // 設定文字 拆解 字串
        public void SetText( string sText)
        {
            Clear();
            if (String.IsNullOrEmpty(sText))
                return;        

            // 拆解 lines
            string[] lines = sText.Split( m_sRawToken.ToCharArray() );            
            foreach (string s in lines)
            {
                if (String.IsNullOrEmpty(s))
                    continue;

                //拆解 Rows
                CTextLine newline = new CTextLine();
                string [] rows = s.Split(m_sColToken.ToCharArray());
                foreach (string r in rows)
                {
                    if (String.IsNullOrEmpty(r))
                        continue;

                    newline.m_kTextPool.Add( r );
                }

                if (newline.GetRowNum() > 0 )
                    m_kTextLinePool.Add( newline );
            }
        }

        // 取得 有幾行
        public int GetMaxCol( ) { return m_kTextLinePool.Count; }
        // 取得指定 行數 中有幾列
        public int GetRowNumByLine(int nCol) {
            if (nCol < 0 || nCol >= m_kTextLinePool.Count )
                return 0;

           CTextLine text =  m_kTextLinePool[ nCol ];
           if (text == null)
               return 0;
           else
               return text.m_kTextPool.Count;
        }

        // 取得指定行 的內容
        public CTextLine GetTextLine(int nCol) {
            if (nCol < 0 || nCol >= m_kTextLinePool.Count)
                return null;

            return m_kTextLinePool[nCol];
        
        }

        //取得 指定 行列 的內容
        public string GetString(int nCol, int nRow ) { 
            CTextLine text =  GetTextLine( nCol );
            if (text== null)
                return null;                
            else
                return text.GetString(nRow);
        
        }

        //判斷是否為空
		public bool IsEmpty(int nRaw, int nCol)
        {
            if ( GetString(nRaw, nCol) != null)
                return false;
            return true;
        }
        // 在指定位置插入文字
		public void Insert(int nCol, string s)
        {
            if (nCol < 0)
                return;
            if (String.IsNullOrEmpty(s))
                return ;

            //拆解 Rows
            CTextLine newline = new CTextLine();
            string[] rows = s.Split(m_sColToken.ToCharArray());
            foreach (string r in rows)
            {
                if (String.IsNullOrEmpty(r))
                    continue;

                newline.m_kTextPool.Add(r);
            }

            if (newline.GetRowNum() > 0)
            {
                m_kTextLinePool.Insert(nCol, newline);
                
            }
        
        }

        private List< CTextLine> m_kTextLinePool;

        private string m_sRawToken;
	    private string m_sColToken;

    }
}
