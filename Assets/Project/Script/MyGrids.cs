using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MYGRIDS
{
    // tile type 的列舉
   public enum _TILE{
        _NULL       = 0,   // 無效
        _GREEN      = 1,    // 綠地
        _LAND       = 2,    // 平原
        _RIVER      = 3,    // 河流
        _LAKE       = 4,    // 湖
        _SNOW       = 5,    // 雪
        _SAND       = 6,    // 沙地
    }

    // int 的 2維向量。將來可能會使用到
    public class iVec2
    {
        public int X { set; get; }
        public int Y { set; get; }

        public iVec2() { }
        public iVec2(int x, int y) { X = x; Y = y; }
        public iVec2( iVec2 v ) { X = v.X; Y = v.Y; }


        // get cell key string 
        public string GetKey()
        {
            return String.Format("({0},{1})", X, Y);
        }
        public static string GetKey( int nX , int nY )
        {
            return String.Format("({0},{1})", nX, nY );
        }    


        // 棋盤格 的距離。 不是 sqrt 的平方根，而是一格一格逐步走的距離
        public int Dist(int x, int y)
        {
            int dist = Math.Abs(x - X) + Math.Abs(y - Y);
            return dist;

        }

        public int Dist( iVec2 vec2 )
        {
            int dist = Math.Abs(vec2.X - X) + Math.Abs(vec2.Y - Y);
            return dist;

        }


        // 比對 座標。看是否同一點
        public bool Collision(iVec2 v2)
        {         
            return ( X==v2.X && Y==v2.Y);
        }


        public double GetAngleFromTwoPoint(iVec2 p1, iVec2 p2)
        {
            return AngleBetween( (this-p1) , (this-p2) );
        }

        public static double AngleBetween(iVec2 vector1, iVec2 vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            return Math.Atan2(sin, cos) * (180 / Math.PI);
        }
      

        // v = 目的座標，　 E 的敵方座標
        public bool ZocCheck(iVec2 C, iVec2 E )
        {
            //距離短的的絕不會是 ZOC
            if (Dist(C) < Dist(E))
                return false;
            // 座標在相對45度以上 不會是ZOC
            double ang1 = GetAngleFromTwoPoint(C, E); ;

            if ( Math.Abs( ang1 ) > 45)
                return false;

            // 由目標點夾角判斷。這邊不知道對不對
//            double ang = C.GetAngleFromTwoPoint(this, E); // Math.Atan2(sin, cos) * (180 / Math.PI);
//            //
  //          ang = Math.Abs( ang );      // 
//            if (ang > 180)
//            {
//                ang = 360 - ang;
//            }


//            return ang < 40;
            return true;

        }

        // 移動座標
        public iVec2 MoveX ( int nX )
        {
            iVec2 v = new iVec2( this );
            v.X += nX;
            return v;
        }
        public iVec2 MoveY(int nY)
        {
            iVec2 v = new iVec2(this);
            v.Y += nY;
            return v;
        }
        public iVec2 MoveXY(int nX , int nY)
        {
            iVec2 v = new iVec2(this);
            v.X += nX;
            v.Y += nY;
            return v;
        }

        // operate +-*/
        // Overload + operator to add two iVec2 objects.
        public static iVec2 operator +(iVec2 v1, iVec2 v2)
        {
            iVec2 v3 = new iVec2();
            v3.X = v1.X + v2.X;
            v3.Y = v1.Y + v2.Y;
            return v3;
        }

        public static iVec2 operator -(iVec2 v1, iVec2 v2)
        {
            iVec2 v3 = new iVec2();
            v3.X = v1.X - v2.X;
            v3.Y = v1.Y - v2.Y;
            return v3;
        }

        public static iVec2 operator *(iVec2 v1, float f)
        {
            iVec2 v3 = new iVec2();
            v3.X = (int)(v1.X *f);
            v3.Y = (int)(v1.Y *f) ;
            return v3;
        }


        public static iVec2 operator /(iVec2 v1, float f)
        {
            if (f == 0.0f)
                return v1;

            iVec2 v3 = new iVec2();
            v3.X = (int)(v1.X / f);
            v3.Y = (int)(v1.Y / f);
            return v3;
        }

    }

    //定義一個矩行。最小 size = 1 ,1 。永不為零。永不反相
	public class iRect
    {   
        int nStX;
        int nStY;

        // 矩行
        int nEdX;
        int nEdY;

        public iRect( int stX , int stY )
        {
            nStX = stX;
            nStY = stY;    
            
            SetSize( 1 , 1);
        }

        public iRect(int stX, int stY  , int edX , int edY )
        {
            nStX = stX;
            nStY = stY;    
            nEdX = edX;
            nEdY = edY;
            if (nEdX <= nStX) {
                nEdX = nStX + 1; 
            }
            if (nEdY <= nStY)
            {
                nEdY = nStY + 1;
            }
        }



        public void SetSize( int W  , int H )
        {
            if (W < 1) W = 1;
            if (H < 1) H = 1;
            nEdX = W + nStX;
            nEdY = H + nStY;
        }

        public bool CheckCol(int x, int y)
        {
            if ((x >= nStX) && (x < nEdX) && (y >= nStY) && (y < nEdY) ) 
            {
                return true;
            }
            return false;
        }

        //取得矩行內的全部位置點
        public List<iVec2 > GetList()
        {
            List<iVec2> lst = new List<iVec2>();
            int w = nEdX - nStX;
            int h = nEdY - nStY;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    lst.Add( new iVec2( nStX+i , nStY+j )  );
                }            
            }
            return lst;
        }


    }

    // 各 Cell 的最小單位
	// 坐標系 必須有 0,0 存在才能對稱又吻合數學計算
	//  (-1,1) , (0,1) , (1,1) 
	//  (-1,0) , (0,0) , (1,0)
	//  (-1,-1), (0,-1), (1,-1)
	//

	public class cMyCell
    {
        public static int nVersion = 1 ; // cell version

        public int Value { set; get; }

        public iVec2 Loc { set; get; }
        public int X() { return Loc.X; }

        public IntPtr Ptr { set; get; }         // 自訂指標

        public cMyCell()
        {
            Loc = new iVec2();
            Value = 0;        
        }

        public cMyCell(int x, int y)
        {
            Loc = new iVec2( x , y );
            Value = 0;
        }

        public cMyCell( int x , int y , int value )
        { 
            Loc = new iVec2(x, y);
            Value = value;
        }

        // get cell name 
        public string GetKey()
        {
            return Loc.GetKey();
        }       

        public int Dist(int nX , int nY )
        {

            return Loc.Dist(nX, nY);
        }

        public int Dist( ref cMyCell cell )
        {

            return Loc.Dist(cell.Loc);
        }

        // File IO
        public bool Write(ref BinaryWriter bWriter )
        {

            bWriter.Write(Loc.X);
            bWriter.Write(Loc.Y);
            bWriter.Write(Value);


            return true;
        }

        public bool Read(ref BinaryReader bReader, int nVer )
        {
            // ver is bindata version

            Loc.X = bReader.ReadInt32();
            Loc.Y = bReader.ReadInt32();
            Value = bReader.ReadInt32();

            return true;
        }
    }

    // 一層 layer。要注意 layer 從0 開始
	public class cMyLayer
    {
        int nVersion;

        public int Z { set; get; }

        public int W { set; get; }
        public int H { set; get; }

        public bool bEnable { set; get; }

        public cMyLayer( int w , int h , int z)
        {
            //defaut value
            nVersion = 1; ; 

            W = w;
            if (W < 0) { W = 1; }

            H = h;
            if (H < 0) { H = 1; }

            Z = z;

            
            // create array
           //cMyCell [][] c = new cMyCell[W][];

           //for (int i = 0; i < W; i++  )
           //{
           //     c[ i ] = new cMyCell[H];
           //     for (int j = 0; j < H; j++)
           //     {
           //         c[i][j] = new cMyCell(i, j);
           //     }
           //}
           //Cells = c;
           // tile
           Tiles  = new Byte[W][]; // [x][y]
           for (int i = 0; i < W; i++)
           {
               Tiles[i] = new Byte[H];
           
           }
      

           bEnable = true; // complete
        }

        public _TILE GetValue( int x , int y ) 
        {
            if( x < 0 || x >= W || y < 0 || y >= H )
                return _TILE._NULL;

            _TILE type = (_TILE)Tiles[x][y];
            return type;

        }

        public bool SetValue(int x, int y, _TILE v)
        {
            if (x < 0 || x >= W || y < 0 || y >= H)
                return false;

            Tiles[x][y] = (byte)v ;
            return true;
        }

        //public cMyCell GetCell( int x , int y )
        //{
        //    if (Cells == null) return null;
        //    if (x < 0 || x >= W) return null;
        //    if (y < 0 || y >= H) return null;

        //    return Cells[x][y];
        
        //}

        //public int GetCellValue(int x, int y)
        //{
        //    cMyCell cell = GetCell( x , y );
        //    if (cell != null) {
        //        return cell.Value;
        //    }

        //    return -1;
        //}
        //public string GetCellKey(int x, int y)
        //{
        //    cMyCell cell = GetCell(x, y);
        //    if (cell != null)
        //    {
        //        return cell.GetKey();
        //    }

        //    return "";
        //}


        // File IO
        public bool Write( ref BinaryWriter bWriter )
        {
            bWriter.Write(nVersion);
            bWriter.Write(W);
            bWriter.Write(H);

            for (int i = 0; i < W; i++)
            {
                bWriter.Write( Tiles[i] , 0, H );
            }

            return true;
        }

        public bool Read(ref BinaryReader bReader )
        {
            int ver = bReader.ReadInt32();
            if (ver != nVersion){
                // version different 
            }

            // grid param
            W = bReader.ReadInt32();
            H = bReader.ReadInt32();

            Tiles = new Byte[W][]; // [x][y]

            for (int i = 0; i < W; i++)
            {
                //Byte [] h = new Byte[H];
               Tiles[i] = bReader.ReadBytes(H); 
            
            }


            return true;
        }


        // 全部cell 的實體
   //     public cMyCell[][] Cells { set; get; }  // [x][y]
        public Byte[][] Tiles { set; get; }  // [x][y]

      }

    

    //  整體的操作容器 。座標是以(0,0) 為座標系的( （- HW ～ HW ）
	public class cMyGrids 
    {
        int nVersion { set; get; }

        // singleton
        private static cMyGrids instance;

        public static cMyGrids Instance
        {
            get 
           {
                if (instance == null)
                {
                    instance = new cMyGrids();
                }
                return instance;
            }
        }

        //  W / H 
        public int MaxW
        {
            set;
            get;
            //set
            //{
            //    if (value < 1) MaxW = 1;
            //    else MaxW = value;

            //}

            //get { return MaxW; }

        }

        public int MaxH
        {
            set;
            get;
            //set
            //{
            //    if (value < 1) MaxH = 1; 
            //    else MaxH = value;
            //}

            //get { return MaxH; }
        }


        // 半寬
        public int hW
        {
            set;
            get;
        }
        // 半高
        public int hH
        {
            set;
            get;
        }


        public int LayerNum
        {
            set;
            get;
            //set
            //{
            //    if (value < 1) LayerNum = 1;
            //    else LayerNum = value;
            //}

            //get { return LayerNum; }
        }

        // Grid total W/H
        public int TotalW { set; get; }
        public int TotalH { set; get; }

        // cell 的 pix w/H
        public int PW { set; get; }
        public int PH { set; get; }

        public int hPW { set; get; }   // cell 的中心 pixel 
        public int hPH { set; get; }
    

        //成像上的偏移
        public int SX { set; get; }
        public int SY { set; get; }



        public cMyGrids()
        {
            nVersion = 1; // first ver

            // default value
            LayerNum = 1; // have 1 layer
            PW = 100;
            PH = 100;


            // default is min limit
            CreateLayer(1, 1); //預設一層
            //
            ThingPool = new Dictionary<string, cMyCell>();     // 地上物
        }


        // 建立 layer
        // 了確保　建立的必為　2的幕次方大小+1。所以　設定都是半徑
        public void CreateLayer(int nhalfW, int nhalfH)
        {
            hW = nhalfW;
            hH = nhalfH;
            MaxW = (2*hW) + 1 ;  // add 0 x-axie
            MaxH = (2*hH) + 1 ;  // add 0 y-axie
            


 //           Layer = new cMyLayer( );
//            for (int i = 0; i < nNum; i++  )
//           {
               Layer = new cMyLayer(MaxW, MaxH , 0 );
 //          }

            SetPixelWH( PW, PH );
        }

        public void SetPixelWH(int npW, int npH)
        {
            PW = npW;
            PH = npH;

            hPW = PW / 2;
            hPH = PH / 2;

            TotalW = MaxW * PW;
            TotalH = MaxH * PH;

            // 計算偏移( 描點在 地圖中央)
          //  SX = -(TotalW / 2);
          //  SY = -(TotalH / 2);

        }


        public void AddThing(int nX, int nY , int nValue )        {
            cMyCell cell = new cMyCell(nX, nY, nValue );

            ThingPool.Add( cell.GetKey() , cell);

        }

        public void RemoveTHing(int nX, int nY)
        {            
            ThingPool.Remove( iVec2.GetKey(nX, nY ) );
        }

        public void ReplaceTHing(int nX, int nY  ,int nValue )
        {
            string skey = iVec2.GetKey(nX, nY);
            if (ThingPool.ContainsKey(skey))
            {
                cMyCell cell = ThingPool[skey];
                cell.Value = nValue;
                ThingPool[skey] = cell;
            }
           
        }

        // 取得 當前座標的 tile
        public _TILE GetValue(int x, int y)
        {
            return Layer.GetValue( x +hW , y + hH );
        }
        public _TILE GetValue( iVec2 v)
        {
            return Layer.GetValue(v.X + hW, v.Y + hH);
        }

        // edit 使用
        public bool SetValue( int x, int y , _TILE value )
        {
            return Layer.SetValue(x + hW, y + hH, value );
        }
        public bool SetValue( iVec2 v , _TILE value )
        {
            return Layer.SetValue(v.X + hW, v.Y + hH, value );
        }

        // 座標轉換


        public float GetRealX( int nX )
        {   
            float fX = nX * PW;
            return fX;

        }
        public float GetRealY(int nY)
        {

            float fY = nY * PH;
            return fY;
        }
        public void GetRealXY( ref float fX , ref float fY , iVec2 v )
        {
            fX = GetRealX( v.X );
            fY = GetRealY( v.Y );
        }

        //螢幕顯示的座標
        public float GetScnX(int nX)
        {
            return GetRealX(nX) - SX;
        }

        public float GetScnY(int nY)
        {
            return GetRealY(nY) - SY;
        }


        // Math utility func 
        public List<iVec2> GetRangePool( iVec2 v , int dist ) { 
            // 取得指定座標 對應距離內的 pool
            List<iVec2> lst = new List<iVec2>();

            // 正向
            for (int i = 0; i <= dist; i++) // 0 不用計算
            {
                int x1 = v.X + i;           // 正
                if (x1 <= hW)
                {
                    for (int j = 0; j <= dist; j++) // 0 不用計算
                    {

                        if (i + j > dist)
                            continue;


                        int y1 = v.Y + j;           // 正
                        if (y1 <= hH) {
                            lst.Add(new iVec2(x1, y1));

                        }

                        if( j == 0 )                 // avoid 0 duplic
                            continue;

                        int y2 = v.Y - j;           // 反 
                        if (y2 >= -hH)
                        {
                            lst.Add(new iVec2(x1, y2));
                        }

                    }
                }

                if( i == 0)                      // avoid 0 duplic
                    continue;


                int x2 = v.X - i;           // 反
                if (x2 >= -hW)
                {
                    for (int j = 0; j <= dist; j++) // 0 不用計算
                    {
                        if( i + j > dist )
                            continue ;          // over dist 

                        int y1 = v.Y + j;           // 正
                        if (y1 <= hH)
                        {
                            lst.Add(new iVec2(x2, y1));

                        }

                        if( j == 0 )                 // avoid 0 duplic
                            continue;

                        int y2 = v.Y - j;           // 反 
                        if (y2 >= -hH)
                        {
                            lst.Add(new iVec2(x2, y2));
                        }

                    }
                    
                }

            }
            // 



            return lst;
        }

        // 根據不可行走的　tile 來過濾
        public List<iVec2> FilterBlockTile(ref List<iVec2> pool)
        {
            List<iVec2> lst = new List<iVec2>();

            foreach (iVec2 v in pool)
            {
                _TILE value = Layer.GetValue(  v.X  , v.Y ) ;
                if (value != _TILE._NULL ) // 只有合法的才保留　
                {
                    lst.Add( v );
                }
            }
            return lst;
        }

        // remove ignore point
        public List<iVec2> FilterPool(ref List<iVec2> pool, ref List<iVec2> ignore )
        {
            List<iVec2> lst = new List<iVec2>();
            foreach (iVec2 v in pool )
            {
                bool bCol = false;
                foreach (iVec2 v2 in ignore)
                {                     
                    if( v.Collision(v2 ) ){
                        bCol = true;
                        break;
                    }
                
                }
				//
                if (bCol != true ){
					lst.Add( v ); //     
				}                
            }

            return lst;
        }

        // 凡是被　ＺＯＣ影響到的　都移除 ， S = 自己, E 的敵人，　0=可行走，　Z= ZOC 
        //  ZOC： 敵人身後　不可行走
        // Ex1:
        //  0 0 0 S 0 0 0  
        //  0 0 0 0 0 0 0 
        //  0 0 0 E 0 0 0
        //  0 0 Z Z Z 0 0

        // Ex2:
        //  0 0 0 S 0 0 0  
        //  0 0 0 0 0 0 0 
        //  0 0 0 0 E Z Z
        //  0 0 0 0 Z Z Z
        //  0 0 0 0 Z Z Z


        // remove Zoc Block
        public List<iVec2> FilterZocPool( iVec2 self , ref List<iVec2> pool, ref List<iVec2> enemy )
        {
            List<iVec2> lst = new List<iVec2>();
            foreach (iVec2 v in pool)
            {
                bool bCol = false;
                foreach (iVec2 v2 in enemy )
                {
                    if (v.ZocCheck(v , v2 ))
                    {
                        bCol = true;
                        break;
                    }

                }

                if (bCol){
					continue;
                    //continue;　// 重複的過濾掉
				}

                lst.Add(v); // 
            }

            return lst;
        }


        // File I / O
        public bool Save(string sFileName)
        {
            FileStream fileStream = new FileStream(sFileName, FileMode.OpenOrCreate);
            BinaryWriter bWriter = new BinaryWriter(fileStream);


            try
            {                
                // read from file or write to file
                bWriter.Write( nVersion );
                bWriter.Write( MaxW );
                bWriter.Write( MaxH );
                bWriter.Write( LayerNum );
                if (Layer != null)
                {
                    bWriter.Write( true );
                    Layer.Write( ref bWriter );
                
                }

                // 地上物
                bWriter.Write( cMyCell.nVersion ); // record cell ver
                bWriter.Write( ThingPool.Count);
                foreach ( KeyValuePair< string ,  cMyCell>  pair in ThingPool )
                {
                    if (pair.Value != null)
                    {
                        bWriter.Write(true);
                        pair.Value.Write(ref bWriter);
                    }
                    else {
                        bWriter.Write(false);
                    }
                    
                }


            }
            finally
            {
                fileStream.Close();               
            }
            return true;
        }


        public bool Load(string sFileName)
        {
            FileStream fileStream = new FileStream(sFileName, FileMode.Open);
            if (fileStream == null) return false;
            BinaryReader bReader = new BinaryReader(fileStream);
            if (bReader == null) return false;

            //=================load ====
            try
            {
                // read from file or write to file
                int ver = bReader.ReadInt32();
                if (ver != nVersion) { 
                    // version different 
                
                }

                // grid param
                MaxW = bReader.ReadInt32();
				hW   = MaxW / 2 ; 
                MaxH = bReader.ReadInt32();
				hH   = MaxH / 2 ; 

                LayerNum = bReader.ReadInt32();

                // layer
                bool bLayer = bReader.ReadBoolean();
                if (bLayer)
                {
                    Layer.Read( ref bReader );                
                }

                // Build thing
                ThingPool.Clear();
                int nCellVer = bReader.ReadInt32();
                int nNum = bReader.ReadInt32();
                for (int i = 0; i < nNum; i++)
                {
                    bool bExist = bReader.ReadBoolean();

                    if (bExist == false)
                        continue;

                    cMyCell cell = new cMyCell();
                    cell.Read(ref bReader, nCellVer);
                  //  LstThing.Add(cell);

                    ThingPool.Add( cell.GetKey(), cell ); 
                }


            }
            finally
            {
                fileStream.Close();               
            }
            return true;

        }

        // Layer 的實體
        cMyLayer Layer;                                   //不公開。以免被誤操作。 （兩造 座標系不同）
        public Dictionary<string, cMyCell> ThingPool;     // 地上物 集合
    
    }

};