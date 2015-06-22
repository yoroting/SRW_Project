using UnityEngine;



public class Config
{
	/// <summary>pcz檔名列表</summary>
	public static string[] COMMON_DATA_NAMES = new string[]{
		"CONSTDATA",
		"TEXTDATA",
	};

	public static int TileW = 100;
	public static int TileH = 100;
	public static int TileMAX = 40;
	public static int StartStory = 1;

	public static int TextSpeed = 0;			// 0 is very fast
	// Cache value
	public static float fScnRatio = 1.0f;		// screen (active/ real ) ratio. for cal mouse in local pos

	public static string PlayerName = "主人公";

	public static int	MaxCharLevel = 20;
	public static float CharMarLVUp = 1.0f;
	public static int 	CharBaseSp = 20;
	public static int 	CharSpLVUp = 4;


	public static float HIT	=100.0f;
	public static float DefReduce	=50.0f;			// 純防禦的減傷


	public static bool GOD = false;
	public static bool MOBAI = true;				// all mob no atk 

	public static int  WIDTH = 960;
	public static int  HEIGHT = 640;
}
