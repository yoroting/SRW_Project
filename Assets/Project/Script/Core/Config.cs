using UnityEngine;



public class Config
{
	/// <summary>pcz檔名列表</summary>
	public static string[] COMMON_DATA_NAMES = new string[]{
		"CONSTDATA",
		"TEXTDATA",
	};
	public static int BigMapTileW = 64;
	public static int BigMapTileH = 64;


	public static int TileW = 100;
	public static int TileH = 100;
	public static int TileMAX = 40;
	public static int StartStory = 1;


	public static int TextSpeed = 0;			// 0 is very fast
	// Cache value
	public static float fScnRatio = 1.0f;       // screen (active/ real ) ratio. for cal mouse in local pos

    public static int UnitW = 100;
    public static int UnitH = 100;

    public static string PlayerFirst = "黃";
	public static string PlayerName = "裳";

	public static string DefaultPlayerFirst = "主";
	public static string DefaultPlayerName = "人公";


	public static int	MaxCharLevel = 99;
	public static int	MaxCP = 5;
	public static float CharMarLVUp = 1.0f;
	public static int 	CharBaseSp = 40;
	public static int 	CharSpLVUp = 4;
	public static int 	CharAtkLVUp = 50;

	public static float HIT	=100.0f;
	public static float DefReduce	=50.0f;			// 純防禦的減傷
	public static float AssistRate = 8.0f;

	public static float BaseDodge	=5.0f;
	public static float BaseCirit	=5.0f;
	public static float CiritRatio	=1.5f;			// 150 %


	public static bool GOD = true;
	public static bool MOBAI = true;				// all mob no atk 
	public static bool DRAWGRID = false;
	public static bool KILL_MODE = false;
	public static bool DebugInfo = true;			// 
	public static bool FREE_MP = false;
    public static bool SHOW_LEAVE = false;          // 顯示離隊成員


    public static int  WIDTH = 960;
	public static int  HEIGHT = 640;

	public static int  sysDefSkillID = 499;

    public static int  LevelUPMoney = 1000;
    public static int  BaseMobMoney = 1000;
}
