using Playcoo.Common;
public class Missile : ConstDataRow<Missile>
{
	public const int TableID = 14;
	public const int DigitBase = 5;
	public int n_ID;
	public string s_HEIGHT;
	public string s_MISSILE;
	public string s_FX_LAUNCH;
	public string s_FX_MISSILEHIT;
	public string s_FX_MISSILECRUSH;
	public int n_CLASS;
	public string s_PATH;
	public int n_SPEED;
	public int n_BOUNDARY;
	public int n_BUFF;
	public string s_HITEFFECT;
	public string s_HITRATE;
	public string s_BUFF_HITRATE;
	public string s_HITBACK;
	public string s_HIT_POSTPROCESS;
	public string s_CONDITION;
	public string s_FISSION;
	public int n_AMOUNT_TARGET;
	public float f_VAR_TPD;
	public float f_VAR_TMD;
	public float f_VAR_TPP;
	public float f_VAR_TMP;
	public float f_DAMAGE_PHYSICS;
	public float f_DAMAGE_MAGIC;
	public float f_PERCENT_HP;
}
