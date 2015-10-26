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


    // Script unit
    public class cTextFunc
    {
        public cTextFunc()
        {
            sFunc = "";                      // avoid null
		//	nArg  = new List< int >();         // 摻數集合
			sArg  = new List< string >();         // 摻數集合
        }
        public string   sFunc;        // function name
		//List< int >     nArg;           // arg list
		List< string >  sArg;           // arg list

		public int GetArnNum() { return sArg.Count; }
//        public int At(int nIdx) {           // 取得指定位置 參數
//            if (nIdx >= nArg.Count)
//                return 0;
//			return nArg[nIdx] ;
//        }
		public int I(int nIdx) {           // 取得指定位置 參數
			if (nIdx >= sArg.Count)
				return 0;
			int i = 0;
			int.TryParse( sArg[nIdx] , out i ); // some param is string

			return i ;
		}

		public float F(int nIdx) {           // 取得指定位置 參數
			if (nIdx >= sArg.Count)
				return 0.0f;
			float f = 0.0f;
			float.TryParse( sArg[nIdx] , out f ); // some param is string
			
			return f ;
		}

		public string S(int nIdx) {           // 取得指定位置 參數
			if (nIdx >= sArg.Count)
				return "";
			return sArg[nIdx];
		}

        public void SetArg( string arg ) 
        {
			//nArg.Clear();
			sArg.Clear();
            char[] split = { ',' };
			string[] list = arg.Split(split );
            foreach (string s in list)
            {
				string ts = s.Trim();
				sArg.Add( ts );
			//	int res = 0;
			//	int.TryParse( ts , out res ); // some param is string
			//	nArg.Add( res );
            }
        }
    }

    // text line 
    public class CTextLine
    {
        public CTextLine()
        {
            m_kTextPool = new List<string>();
        }
        public int GetRowNum() { return m_kTextPool.Count; }
        public List<string> m_kTextPool;

        public string GetString(int nRow)
        {
            if (nRow < 0 || (nRow >= m_kTextPool.Count))
                return null;
            return m_kTextPool[nRow];
        }

        // 動態產生 ary list
        public List<cTextFunc> GetFuncList()
        {
            List<cTextFunc> funclist = new List<cTextFunc>();

            for (int i = 0; i < m_kTextPool.Count; i++)
            {
                string sFunc = m_kTextPool[i];
                string sParam = m_kTextPool[++i];
                if (sFunc == null || sParam == null) {
                    break;
                }
                cTextFunc func = new cTextFunc();
                func.sFunc = sFunc.ToUpper();

                func.SetArg(sParam);
                funclist.Add (func );
            
            }

            return funclist;
        }
    }


    // Text Arry
    public class cTextArray
    {
        

        //==============================
		public cTextArray( )
		{ 
			char[] rowChars = { '\n' };	
			char[] colChars = { '(', ')',';', '\t' };

			m_sRawToken =  new string( rowChars );
			m_sColToken =  new string( colChars );
			m_kTextLinePool = new List<CTextLine>();
		}

		public cTextArray(char[] cRawToken, char[] cColToken  )
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

            sText = sText.Replace(" ", "");     // 過濾空白字元 防止錯誤
            sText = sText.Replace("()", "(0)"); // 無參數時傳0為參數。以吻和 parser 時 一個命令，一個 param 的結構


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


		// Get param char ary
		public static List<string> GetParamLst( string s )
		{		
			char [] cCol = { ',' , ':' };
			string [] rows = s.Split( cCol );
			List<string> lst = new List<string>();
			foreach( string s2 in rows )
			{
				if( string.IsNullOrEmpty( s2) == false )
				lst.Add( s2.Trim() );
			}
			return lst;
		}

        public static int[] GetParamIntAry(string s)
        {
            int [] ary = new int []{0};

            List<string> lst = GetParamLst(s);
            int c = 0;
            foreach ( string s2 in lst )
            {
                if (string.IsNullOrEmpty(s2) == false)
                {
					int arg;
					int.TryParse( s2.Trim() , out arg );
					ary[c++] = arg;                    
                }
            }
            return ary;
        }

        private List< CTextLine> m_kTextLinePool;

        private string m_sRawToken;
	    private string m_sColToken;



    }


}
