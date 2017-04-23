using Playcoo.Common;
public class STAGE_PHASE : ConstDataRow<STAGE_PHASE>
{
	public const int TableID = 8;
	public const int DigitBase = 8;
	public int n_ID;
	public string s_MODLE_ID;
	public int n_SCENE_ID;
	public int n_PHASE;
	public string s_WIN_CONDITION;
	public string s_LOST_CONDITION;
	public string s_MISSION;
	public int n_ENEMY_BGM;
	public int n_PLAYER_BGM;
	public int n_FRIEND_BGM;
	public string s_HK_VER;
	public string s_TC_VER;
	public string s_JP_VER;
	public string s_TH_VER;
}
