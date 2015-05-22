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
}
