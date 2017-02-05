using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SimpleAStarExample;
using System.ComponentModel;

namespace MYGRIDS
{
    // tile type 的列舉
    public enum _TILE
    {
        /// <summary>
        /// 無效
        /// </summary>
        [Description("_TILE")]
        _NULL = 0,
		_BORDER =1,    // 外框
        _GREEN = 2,    // 綠地( 平原/石路/河流 ) 
        _LAND  = 3,    // 平原( 石路 )
        _RIVER = 4,    // 河流( 平原/湖 ) 
        _LAKE = 5,    // 湖  ( 河流 )
        _SNOW = 6,    // 雪  ( 河流 )
        _SAND = 7,    // 沙地 ( 平原 )
        _DIRT = 8,    // 泥版 ( 平原/道路 )
        _ROAD = 9,	// 石路	

    };

    public enum _THING
    {
        _NULL = 0,   // 無效
        _TREE = 1,
        _BIGTREE = 2,
        _STONE = 3,
        _HILL = 4,
        _SNOWHILL = 5,
        _MOUNT = 6,
        _SNOWMOUNT = 7,
        _FIREMOUNT = 8,

        _WALLLT = 9,
        _WALLRT = 10,
        _WALLLD = 11,
        _WALLRD = 12,
        _WALLV = 13,
        _WALLH = 14,
        _HOUSE = 15,
        _VILLAGE = 16,
        _CASTLLE = 17,
        _DOOR_V = 18,
        _DOOR_H = 19,
        _BRIDGE_V = 20,
        _BRIDGE_H = 21,

    };

    public enum _DIR
    {
		// 4 way       
        _UP 	 = 0,
		_LEFT_UP = 1,
		_LEFT 	 = 2,
		_LEFT_DOWN  = 3,
		_DOWN 	 = 4,
		_RIGHT_DOWN = 5,
        _RIGHT   = 6,
		_RIGHT_UP = 7,
		
		// 8 way 

    };

    // int 的 2維向量。將來可能會使用到
    public class iVec2
    {
        public int X { set; get; }
        public int Y { set; get; }

        public iVec2() { }
        public iVec2(int x, int y) { X = x; Y = y; }
        public iVec2(iVec2 v) { X = v.X; Y = v.Y; }


        // get cell key string 
        public string GetKey()
        {
            return String.Format("({0},{1})", X, Y);
        }
        public static string GetKey(int nX, int nY)
        {
            return String.Format("({0},{1})", nX, nY);
        }


        // 棋盤格 的距離。 不是 sqrt 的平方根，而是一格一格逐步走的距離
        static public int Dist(int x1, int y1, int x2, int y2)
        {
            int dist = Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
            return dist;
        }


        public int Dist(int x, int y)
        {
            int dist = Math.Abs(x - X) + Math.Abs(y - Y);
            return dist;

        }

        public int Dist(iVec2 vec2)
        {
            int dist = Math.Abs(vec2.X - X) + Math.Abs(vec2.Y - Y);
            return dist;

        }
		static public iVec2 Move8Dir(int stx, int sty, _DIR dir )
		{
			iVec2 v = new iVec2 ( stx , sty );
			switch( dir )
			{
			case _DIR._UP:  v.Y++; 	break;
			case _DIR._DOWN: v.Y--; break;
			case _DIR._RIGHT: v.X++; break;
			case _DIR._LEFT: v.X--;	break;			

			case _DIR._RIGHT_UP: v.X++;v.Y++; break;
			case _DIR._RIGHT_DOWN: v.X++;v.Y--;break;
			case _DIR._LEFT_UP: v.X--;v.Y++; break;
			case _DIR._LEFT_DOWN: v.X--;v.Y--; break;
			}
			return v;
		}

        static public _DIR GetDir(int stx, int sty, int edx, int edy)
        {
            int nDiffX = edx - stx;
            int nDiffY = edy - sty;
            // 只處理4正維方向。斜角 不處理
            if (nDiffX == 0)
            {
                if (nDiffY > 0)
                {
                    return _DIR._UP;// up
                }
                else if (nDiffY < 0)
                {
                    return _DIR._DOWN;// down
                }
            }
            else if (nDiffY == 0)
            {
                if (nDiffX > 0)
                {
                    return _DIR._RIGHT; // right
                }
                else if (nDiffX < 0)
                {
                    return _DIR._LEFT; // left  
                }
            }
            else if (nDiffX > 0 && nDiffY > 0)
            {
                return _DIR._UP;// up
            }
            else if (nDiffX < 0 && nDiffY < 0)
            {
                return _DIR._DOWN;// down;
            }
            else if (nDiffX > 0 && nDiffY < 0)
            {
                return _DIR._RIGHT; // right
            }
            else if (nDiffX < 0 && nDiffY > 0)
            {
                return _DIR._LEFT; // left  
            }
			return _DIR._UP;
		}
		
		// get 8 way dir
		static public _DIR Get8Dir(int stx, int sty, int edx, int edy)
		{
			int nDiffX = edx - stx;
			int nDiffY = edy - sty;
			// 只處理4正維方向。斜角 不處理
			if (nDiffX == 0)
			{
				if (nDiffY > 0)
				{
					return _DIR._UP;// up
				}
				else if (nDiffY < 0)
				{
					return _DIR._DOWN;// down
				}
			}
			else if (nDiffY == 0)
			{
				if (nDiffX > 0)
				{
					return _DIR._RIGHT; // right
				}
				else if (nDiffX < 0)
				{
					return _DIR._LEFT; // left  
				}
			}
			else if (nDiffX > 0 && nDiffY > 0)
			{
				return _DIR._RIGHT_UP;// up
			}
			else if (nDiffX < 0 && nDiffY < 0)
			{
				return _DIR._LEFT_DOWN;// down;
			}
			else if (nDiffX > 0 && nDiffY < 0)
			{
				return _DIR._RIGHT_DOWN; // right
			}
			else if (nDiffX < 0 && nDiffY > 0)
			{
				return _DIR._LEFT_UP; // left  
			}
			return _DIR._UP; // UP as default
		}

        public _DIR GetDir(int x, int y)
        {
            return GetDir(X, Y, x, y);
        }

		public _DIR Get8Dir(int x, int y)
		{
			return Get8Dir (X, Y, x, y );
		}

        public void Rotate(_DIR dir)
        {

            switch (dir)
            {
                case _DIR._UP: // up
                    // no change
                    //return new iVec2( X , Y );
                    break;
                case _DIR._DOWN:// down
                    X *= -1;
                    Y *= -1;
                    //return new iVec2( X * -1 , Y *-1 );
                    break;
                case _DIR._RIGHT:
                    {// right
                        int nTmpX = X;
                        int nTmpY = Y;
                        X = nTmpY * 1;
                        Y = nTmpX * -1;
                        //return new iVec2( Y * 1 , X *-1 );
                    } break;
                case _DIR._LEFT:
                    { // left  
                        int nTmpX = X;
                        int nTmpY = Y;
                        //return new iVec2( Y * -1 , X *1 );
                        X = nTmpY * -1;
                        Y = nTmpX * 1;
                    } break;
            }

        }
        // 比對 座標。看是否同一點
        public bool Collision(iVec2 v2)
        {
            return (X == v2.X && Y == v2.Y);
        }
        // 比對 座標。看是否同一點
        public bool Collision(int nX , int nY)
        {
            return ( X == nX && Y == nY );
        }

        public double GetAngleFromTwoPoint(iVec2 p1, iVec2 p2)
        {
            return AngleBetween((this - p1), (this - p2));
        }

        public static double AngleBetween(iVec2 vector1, iVec2 vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            return Math.Atan2(sin, cos) * (180 / Math.PI);
        }


        // v = 目的座標，　 E 的敵方座標
        public bool ZocCheck(iVec2 C, iVec2 E)
        {
            // it wont zoc if the same point
            if (Collision(C) || Collision(E) || C.Collision(E))
                return false;
            //距離短的的絕不會是 ZOC
            int distc = Dist(C);
            int diste = Dist(E);
            if (distc <= diste)
                return false;
            // 座標在相對45度以上 不會是ZOC
            double ang1 = GetAngleFromTwoPoint(C, E); ;
            if (Math.Abs(ang1) > 26) // 45 is too large ...
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

        public List<iVec2> AdjacentList( int nRadius = 1 )
        {
            List<iVec2> lst = new List<iVec2>();
			if( nRadius <= 1){
            	lst.Add(new iVec2(X - 1, Y));
            	lst.Add(new iVec2(X, Y - 1));
            	lst.Add(new iVec2(X + 1, Y));
            	lst.Add(new iVec2(X, Y + 1));
				return lst;
			}
			// expan size
			// 正向
			for (int i = 0; i <= nRadius; i++) // 0 不用計算
			{
				int x1 = X + i;           // 正
				for (int j = 0; j <= nRadius; j++) // 0 不用計算
				{
					int tmp = i + j;
					if (tmp > nRadius || tmp < 1)
						continue;					
						
					int y1 = Y + j;           // 正
					lst.Add(new iVec2(x1, y1));

					if (j == 0)                 // avoid 0 duplic
						continue;
						
					int y2 = Y - j;           // 反 
					lst.Add(new iVec2(x1, y2));						
				}
				if (i == 0)                      // avoid 0 duplic
					continue;
				int x2 = X - i;           // 反
				for (int j = 0; j <= nRadius; j++) // 0 不用計算
				{
					int tmp = i + j;
					if (tmp > nRadius || tmp < 1)
						continue;          // over dist 						
					int y1 = Y + j;           // 正
					lst.Add(new iVec2(x2, y1));							
						
					if (j == 0)                 // avoid 0 duplic
						continue;
						
					int y2 = Y - j;           // 反 
					lst.Add(new iVec2(x2, y2));
				}
			}
            return lst;
        }
        // 移動座標
        public iVec2 MoveX(int nX)
        {
            iVec2 v = new iVec2(this);
            v.X += nX;
            return v;
        }
        public iVec2 MoveY(int nY)
        {
            iVec2 v = new iVec2(this);
            v.Y += nY;
            return v;
        }
        public iVec2 MoveXY(int nX, int nY)
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
            v3.X = (int)(v1.X * f);
            v3.Y = (int)(v1.Y * f);
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
        public int nStX;
        public int nStY;

        // 矩行
        public int nEdX;
        public int nEdY;

        public iRect(int stX, int stY)
        {
            nStX = stX;
            nStY = stY;

            SetSize(1, 1);
        }

        public iRect(int stX, int stY, int edX, int edY)
        {
            //
            SetRect(stX, stY, edX, edY);
            if (nEdX <= nStX)
            {
                nEdX = nStX + 1;
            }
            if (nEdY <= nStY)
            {
                nEdY = nStY + 1;
            }
        }

        public void SetRect(int stX, int stY, int edX, int edY)
        {
            nStX = stX < edX ? stX : edX;
            nStY = stY < edY ? stY : edY;
            nEdX = stX > edX ? stX : edX;
            nEdY = stY > edY ? stY : edY;
        }

        public void SetSize(int W, int H)
        {
            if (W < 1) W = 1;
            if (H < 1) H = 1;
            nEdX = (W-1) + nStX;
            nEdY = (H-1) + nStY;
        }

        public bool CheckInside(int x, int y)
        {
            if ((x >= nStX) && (x <= nEdX) && (y >= nStY) && (y <= nEdY))
            {
                return true;
            }
            return false;
        }

        //取得矩行內的全部位置點
        public List<iVec2> GetList()
        {
            List<iVec2> lst = new List<iVec2>();
            
            for (int i = nStX; i <= nEdX; i++)
            {
                for (int j = nStY; j <= nEdY; j++)
                {
                    lst.Add(new iVec2( i, j));
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
        public static int nVersion = 1; // cell version

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
            Loc = new iVec2(x, y);
            Value = 0;
        }

        public cMyCell(int x, int y, int value)
        {
            Loc = new iVec2(x, y);
            Value = value;
        }

        // get cell name 
        public string GetKey()
        {
            return Loc.GetKey();
        }

        public int Dist(int nX, int nY)
        {

            return Loc.Dist(nX, nY);
        }

        public int Dist(ref cMyCell cell)
        {

            return Loc.Dist(cell.Loc);
        }

        // File IO
        public bool Write(ref BinaryWriter bWriter)
        {

            bWriter.Write(Loc.X);
            bWriter.Write(Loc.Y);
            bWriter.Write(Value);


            return true;
        }

        public bool Read(ref BinaryReader bReader, int nVer)
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

        public cMyLayer(int w, int h, int z)
        {
            //defaut value
            nVersion = 1; ;

            W = w;
            if (W < 0) { W = 1; }

            H = h;
            if (H < 0) { H = 1; }

            Z = z;

            Tiles = new Byte[W][]; // [x][y]
            for (int i = 0; i < W; i++)
            {
                Tiles[i] = new Byte[H];

            }

            bEnable = true; // complete
        }

        public _TILE GetValue(int x, int y)
        {
            if (x < 0 || x >= W || y < 0 || y >= H)
                return _TILE._NULL;

            _TILE type = (_TILE)Tiles[x][y];
            return type;

        }

        public bool SetValue(int x, int y, _TILE v)
        {
            if (x < 0 || x >= W || y < 0 || y >= H)
                return false;

            Tiles[x][y] = (byte)v;
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
        public bool Write(ref BinaryWriter bWriter)
        {
            bWriter.Write(nVersion);
            bWriter.Write(W);
            bWriter.Write(H);

            for (int i = 0; i < W; i++)
            {
                bWriter.Write(Tiles[i], 0, H);
            }

            return true;
        }

        public bool Read(ref BinaryReader bReader)
        {
            int ver = bReader.ReadInt32();
            if (ver != nVersion)
            {
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

    /// <summary>
    /// 地上物的資料
    /// </summary>
    public class MyThing
    {
        public cMyCell Cell = new cMyCell();
        /// <summary>
        /// 在哪一層地表
        /// </summary>
        public int Layer = 1;
    }

    //  整體的操作容器 。座標是以(0,0) 為座標系的( （- HW ～ HW ）
    public class MyGrids
    {
        public const int Version = 2; // 2 : support background text file

        // singleton
        private static MyGrids instance;
        public static MyGrids Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MyGrids();
                }
                return instance;
            }
        }
		public string sBackGround;			//背景圖標示
        public int MaxW;
        public int MaxH;
        /// <summary>
        /// 半寬
        /// </summary>
        public int hW;
        /// <summary>
        /// 半高
        /// </summary>
        public int hH;

        public int LayerNum;

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

        public MyGrids()
        {
            // default value
            LayerNum = 1; // have 1 layer
            PW = 100;
            PH = 100;

            // default is min limit
            CreateLayer(1, 1); //預設一層
            //
            ThingPool = new Dictionary<string, List<MyThing>>();     // 地上物
			//
			sBackGround = "null";
        }


        // 建立 layer
        // 了確保　建立的必為　2的倍數+1。所以　設定都是半徑
        public void CreateLayer(int nhalfW, int nhalfH)
        {
            hW = nhalfW;
            hH = nhalfH;
            MaxW = (2 * hW) + 1;  // add 0 x-axie
            MaxH = (2 * hH) + 1;  // add 0 y-axie

            Layer = new cMyLayer(MaxW, MaxH, 0);

            SetPixelWH(PW, PH);
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


        public void AddThing(int nX, int nY, int nValue, int layer)
        {
            MyThing newThing = new MyThing();
            newThing.Cell = new cMyCell(nX, nY, nValue);
            newThing.Layer = layer;

            List<MyThing> thingList = null;
            if (ThingPool.TryGetValue(newThing.Cell.GetKey(), out thingList))
            {
                thingList.Add(newThing);
            }
            else
            {
                thingList = new List<MyThing>();
                thingList.Add(newThing);

                ThingPool.Add(newThing.Cell.GetKey(), thingList);
            }
        }

        public List<MyThing> GetThing(int x, int y)
        {
            string key = iVec2.GetKey(x, y);
            if (ThingPool.ContainsKey(key))
                return ThingPool[key];

            return null;
        }

        public void RemoveThing(int nX, int nY)
        {
            ThingPool.Remove(iVec2.GetKey(nX, nY));
        }

        public void ReplaceThing(int x, int y, int value, int layer)
        {
            List<MyThing> thingList = null;
            string skey = iVec2.GetKey(x, y);
            if (ThingPool.TryGetValue(skey, out thingList))
            {
                MyThing thing = thingList.Find(item => item.Layer == layer);
                if (thing != null)
                    thing.Cell.Value = value;
            }

        }

        // 取得 當前座標的 tile
        public _TILE GetValue(int x, int y)
        {
            return Layer.GetValue(x + hW, y + hH);
        }
        public _TILE GetValue(iVec2 v)
        {
            return Layer.GetValue(v.X + hW, v.Y + hH);
        }

        // edit 使用
        public bool SetValue(int x, int y, _TILE value)
        {
            return Layer.SetValue(x + hW, y + hH, value);
        }
        public bool SetValue(iVec2 v, _TILE value)
        {
            return Layer.SetValue(v.X + hW, v.Y + hH, value);
        }

        // 座標轉換
        public float GetRealX(int nX)
        {
            float fX = nX * PW;
            return fX;

        }
        public float GetRealY(int nY)
        {

            float fY = nY * PH;
            return fY;
        }
        public void GetRealXY(ref float fX, ref float fY, iVec2 v)
        {
            fX = GetRealX(v.X);
            fY = GetRealY(v.Y);
        }

        public int GetGridX(float fRealX)
        {
            int X = ((int)fRealX / PW);
            return X;
        }

        public int GetGridY(float fRealY)
        {
            int Y = ((int)fRealY / PH);
            return Y;
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
        // Get Path list
        public List<iVec2> GetPathList(iVec2 st, iVec2 ed)
        {
            List<iVec2> lst = new List<iVec2>();

           // int Diffx = ed.X - st.X;
            int Diffy = ed.Y - st.Y;
            iVec2 p1 = new iVec2();
            if (Diffy == 0)
            {
                p1.X = ed.X;
                p1.Y = st.Y;
            }
            else
            {
                p1.X = st.X;
                p1.Y = ed.Y;
            }
            lst.Add(p1); // center point
            lst.Add(ed);

            return lst;
        }

        // Math utility func 
        public List<iVec2> GetRangePool(iVec2 v, int max, int min = 0)
        {

            // 取得指定座標 對應距離內的 pool
            List<iVec2> lst = new List<iVec2>();

            // 正向
            for (int i = 0; i <= max; i++) // 0 不用計算
            {
                int x1 = v.X + i;           // 正
                if (x1 <= hW)
                {
                    for (int j = 0; j <= max; j++) // 0 不用計算
                    {
                        int tmp = i + j;
                        if (tmp > max || tmp < min){
							if( tmp < min ){
								continue;

							}

                            continue;
						}


                        int y1 = v.Y + j;           // 正
                        if (y1 <= hH)
                        {
                            lst.Add(new iVec2(x1, y1));

                        }

                        if (j == 0)                 // avoid 0 duplic
                            continue;

                        int y2 = v.Y - j;           // 反 
                        if (y2 >= -hH)
                        {
                            lst.Add(new iVec2(x1, y2));
                        }

                    }
                }

                if (i == 0)                      // avoid 0 duplic
                    continue;


                int x2 = v.X - i;           // 反
                if (x2 >= -hW)
                {
                    for (int j = 0; j <= max; j++) // 0 不用計算
                    {
                        int tmp = i + j;
                        if (tmp > max || tmp < min)
                            continue;          // over dist 

                        int y1 = v.Y + j;           // 正
                        if (y1 <= hH)
                        {
                            lst.Add(new iVec2(x2, y1));

                        }

                        if (j == 0)                 // avoid 0 duplic
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
                _TILE value = Layer.GetValue(v.X, v.Y);
                if (IsWalkAbleTile(value)) // 只有合法的才保留　
                {
                    lst.Add(v);
                }
            }
            return lst;
        }

        // remove ignore point
        public List<iVec2> FilterPool(ref List<iVec2> pool, ref List<iVec2> ignore)
        {
            List<iVec2> lst = new List<iVec2>();
            foreach (iVec2 v in pool)
            {
                bool bCol = false;
                foreach (iVec2 v2 in ignore)
                {
                    if (v.Collision(v2))
                    {
                        bCol = true;
                        break;
                    }

                }
                //
                if (bCol != true)
                {
                    lst.Add(v); //     
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
        public List<iVec2> FilterZocPool(iVec2 self, ref List<iVec2> pool, ref List<iVec2> enemy)
        {
            List<iVec2> lst = new List<iVec2>();
            foreach (iVec2 v in pool)
            {
                bool bCol = false;
                foreach (iVec2 v2 in enemy)
                {
                    if (self.ZocCheck(v, v2))
                    {
                        bCol = true;
                        break;
                    }

                }

                if (bCol)
                {
                    continue;
                    //continue;　// 重複的過濾掉
                }

                lst.Add(v); // 
            }

            return lst;
        }


		public List<iVec2> GetZocPool(iVec2 self, ref List<iVec2> enemy )
		{
			List<iVec2> lst = new List<iVec2>();
			foreach (iVec2 v2 in enemy)
			{
				_DIR  dir = iVec2.Get8Dir( self.X , self.Y , v2.X , v2.Y );
				// 8 dir
				switch( dir ){
					case _DIR._UP:
						lst.Add(  v2.MoveXY( 1 , 1 ) );
						lst.Add(  v2.MoveXY( -1 , 1 ) );
					break;						
					case _DIR._DOWN:
						lst.Add(  v2.MoveXY( 1 , -1 ) );
						lst.Add(  v2.MoveXY( -1 , -1 ) );
					break;						
					case _DIR._LEFT:
						lst.Add(  v2.MoveXY( -1 , 1 ) );
						lst.Add(  v2.MoveXY( -1 , -1 ) );
					break;
					case _DIR._RIGHT:
						lst.Add(  v2.MoveXY( 1 , 1 ) );
						lst.Add(  v2.MoveXY( 1 , -1 ) );
					break;
					case _DIR._RIGHT_UP:
						lst.Add(  v2.MoveXY( 0 , 1 ) );
						lst.Add(  v2.MoveXY( 1 , 0 ) );
					break;
					case _DIR._RIGHT_DOWN:
						lst.Add(  v2.MoveXY( 0 , -1 ) );
						lst.Add(  v2.MoveXY( 1 , 0 ) );
					break;
					case _DIR._LEFT_UP:
						lst.Add(  v2.MoveXY( 0 , 1 ) );
						lst.Add(  v2.MoveXY( -1 , 0 ) );
					break;
					case _DIR._LEFT_DOWN:
						lst.Add(  v2.MoveXY( 0 , -1 ) );
						lst.Add(  v2.MoveXY( -1 , 0 ) );
					break;
				}
			}

			return lst;
		}

        // Check if pos in grid
        public bool Contain(iVec2 v)
        {
            if (v.X < -hW || v.X > hW)
            {
                return false;
            }
            if (v.Y < -hH || v.Y > hH)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 存檔
        /// </summary>
        /// <param name="fileName">檔案名稱</param>
        /// <returns></returns>
        public bool Save(string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                BinaryWriter writer = new BinaryWriter(fileStream);

                try
                {
                    writer.Write(Version);

                    if (Version >= 1)
                    {
                        writer.Write(MaxW);
                        writer.Write(MaxH);
                        writer.Write(LayerNum);
                        if (Layer != null)
                        {
                            writer.Write(true);
                            Layer.Write(ref writer);
                        }
                        else
                            writer.Write(false);

                        // 地上物
                        writer.Write(cMyCell.nVersion); // record cell ver
                        writer.Write(ThingPool.Count);
                        foreach (KeyValuePair<string, List<MyThing>> pair in ThingPool)
                        {
                            if (pair.Value != null)
                            {
                                writer.Write(true);

                                writer.Write(pair.Value.Count);
                                foreach(MyThing thing in pair.Value)
                                {
                                    if (thing != null)
                                    {
                                        writer.Write(true);

                                        if (thing.Cell != null)
                                        {
                                            writer.Write(true);
                                            thing.Cell.Write(ref writer);
                                        }
                                        else
                                            writer.Write(false);

                                        writer.Write(thing.Layer);
                                    }
                                    else
                                        writer.Write(false);
                                }
                            }
                            else
                            {
                                writer.Write(false);
                            }
                        }
						// back ground tex
						writer.Write( sBackGround );

                    }
                }
                catch
                {
                    throw;
                }
            }
            return true;
        }

        public bool Load(byte[] bytes)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(bytes));
            if (reader == null) return false;

            return Load(reader);
        }

        public bool Load(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            if (fileStream == null) return false;
            BinaryReader bReader = new BinaryReader(fileStream);
            bool bResult = Load(bReader);
            fileStream.Close();
            return bResult;
        }

        public bool Load(BinaryReader reader)
        {
            if (reader == null) return false;

            //=================load ====
            try
            {
                // read from file or write to file
                int version = reader.ReadInt32();

                if (version >= 1)
                {
                    // grid param
                    MaxW = reader.ReadInt32();
                    hW = MaxW / 2;
                    MaxH = reader.ReadInt32();
                    hH = MaxH / 2;

                    LayerNum = reader.ReadInt32();

                    // layer
                    bool bLayer = reader.ReadBoolean();
                    if (bLayer)
                    {
                        Layer.Read(ref reader);
                    }

                    // Build thing
                    ThingPool.Clear();
                    int nCellVer = reader.ReadInt32();
                    int nNum = reader.ReadInt32();
                    for (int i = 0; i < nNum; i++)
                    {
                        bool bExist = reader.ReadBoolean();

                        if (bExist == false)
                            continue;

                        List<MyThing> thingList = new List<MyThing>();
                        int listCount = reader.ReadInt32();
                        for (int j = 0; j < listCount; ++j)
                        {
                            bool thingExist = reader.ReadBoolean();
                            if (!thingExist)
                                continue;

                            MyThing newThing = new MyThing();

                            bool cellExist = reader.ReadBoolean();
                            if (cellExist)
                            {
                                cMyCell cell = new cMyCell();
                                cell.Read(ref reader, nCellVer);

                                newThing.Cell = cell;
                            }

                            newThing.Layer = reader.ReadInt32();

                            thingList.Add(newThing);
                        }

                        if (thingList.Count > 0)
                            ThingPool.Add(thingList[0].Cell.GetKey(), thingList);
                    }

                    if (version >= 2)
                    {
                        // if is new ver
                        sBackGround = reader.ReadString();
                    }
                }
            }
            finally
            {

            }
            // create path findere map

			SetPixelWH ( PW , PH );
            // create path finder struct during loading
            //InitializePathFindMap (); 
			pathfinder = null;// clear to reset pathfind 
            GetPathFinder().ApplyMap(map); // apply here for new nodes

            return true;

        }

        // Layer 的實體
        cMyLayer Layer;                                   //不公開。以免被誤操作。 （兩造 座標系不同）
        /// <summary>
        /// 所有地上物
        /// </summary>
        public Dictionary<string, List<MyThing>> ThingPool = new Dictionary<string, List<MyThing>>();

        //=============================================================
        // Widget for pathfinding
        //=============================================================
        private bool[,] map;							  // path find 	
        //	private bool[,] mask;							  // ignore path find 	
        //public List<Point> DynMask;     				  // ignore for pathfind
        //private SearchParameters searchParameters;

        private PathFinder pathfinder;

        List<Point> IgnorePool;     				  // ignore for pathfind

        public PathFinder GetPathFinder()
        {
            if (pathfinder == null)
            {
                InitializePathFindMap();
                pathfinder = new PathFinder(map);
            }
            return pathfinder;
        }

        public void ClearIgnorePool()
        {
            if (IgnorePool != null)
            {
                IgnorePool.Clear();
            }
            GetPathFinder().bIsDirty = true;
        }

        public bool IsIgnorePos(iVec2 v)
        {
            if (IgnorePool == null) {
                return false;
            }

            int nX = v.X + hW;
            int nY = v.Y + hH;
            foreach (Point p in IgnorePool)
            {
                if ( (p.X == nX) && (p.Y == nY) )
                {
                    return true;
                }
            }
            return false;
        }

        public void AddIgnorePool(List<iVec2> ivecPool)
        {
            if (ivecPool == null)
            {
                //GetPathFinder ().SetIgnorePool ( IgnorePool ); // clear all mask
                return;
            }
            if (IgnorePool == null)
                IgnorePool = new List<Point>();

            foreach (iVec2 v in ivecPool)
            {
                //	mask[ v.X+hW , v.Y+hH ] = false;
                IgnorePool.Add(new Point(v.X + hW, v.Y + hH));
            }
            //GetPathFinder ().SetIgnorePool ( IgnorePool );
            GetPathFinder().bIsDirty = true;
        }

        public void AddIgnorePos(iVec2 v)
        {
            if (IgnorePool == null)
                IgnorePool = new List<Point>();

            IgnorePool.Add(new Point(v.X + hW, v.Y + hH));

            GetPathFinder().bIsDirty = true;
        }

        // path find func . take care to use it
        public List<iVec2> PathFinding(iVec2 st, iVec2 ed, int nDist)
        {
            List<iVec2> pool = new List<iVec2>();

            //InitializePathFindMap (); // maybe move out later

            var startLocation = new Point(st.X + hW, st.Y + hH);  // convert srw  to path find 
            var endLocation = new Point(ed.X + hW, ed.Y + hH);

            PathFinder pathFinder = GetPathFinder();			// new method to get path

            // check if need refresh
            if (pathFinder.bIsDirty == true)
            {
                pathFinder.ApplyMap(map);			// need apply every time for new find
                pathFinder.ApplyMaskPoint(IgnorePool);	// need apply every time. 			
                pathFinder.bIsDirty = false;
            }

            pathFinder.nMaxStep = nDist;			// max dist

            List<Point> path = pathFinder.FindPath(startLocation, endLocation);


            foreach (Point pt in path)
            {
                iVec2 pos = new iVec2(pt.X - hW, pt.Y - hH);
                pool.Add(pos);
            }

            return pool;
        }

        public List<iVec2> MoveAbleCell(iVec2 st, int nDist)
        {
            List<iVec2> pool = new List<iVec2>();

            //InitializePathFindMap (); // maybe move out later

            var startLocation = new Point(st.X + hW, st.Y + hH);  // convert srw  to path find 
            PathFinder pathFinder = GetPathFinder();			// new method to get path

            // check if need refresh
            if (pathFinder.bIsDirty == true)
            {
                pathFinder.ApplyMap(map);			// need apply every time for new find
                pathFinder.ApplyMaskPoint(IgnorePool);	// need apply every time. 			
                pathFinder.bIsDirty = false;
            }

            //pathFinder.nMaxStep = nDist;			// max dist

            List<Point> path = pathFinder.MoveAble(startLocation, nDist);


            foreach (Point pt in path)
            {
                iVec2 pos = new iVec2(pt.X - hW, pt.Y - hH);
                pool.Add(pos);
            }

            return pool;

        }

        private void InitializePathFindMap()
        {
            //  □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □
            //  □ S □ □ □ F □
            //  □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □

            this.map = new bool[MaxW, MaxH];
            for (int y = 0; y < MaxH; y++)
            {
                for (int x = 0; x < MaxW; x++)
                {
                    map[x, y] = IsWalkAbleTile(Layer.GetValue(x, y));
                }
            }
            //			this.mask = new bool[ MaxW , MaxH ];
            //			for (int y = 0; y < MaxH ; y++)
            //				for (int x = 0; x < MaxW ; x++)
            //					mask[x, y] = true;

        }

        public static bool IsWalkAbleTile(_TILE t)
        {
            if (t == _TILE._NULL ||
                 t == _TILE._RIVER ||    // 河流
                 t == _TILE._LAKE // 湖
                )
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 取得地上物的圖片名字
        /// </summary>
        /// <param name="thingValue"></param>
        /// <returns></returns>
        public static string GetThingSpriteName(int thingValue)
        {
            return "mount";
        }
    }

};