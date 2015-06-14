using Playcoo.Common;
public class BUFF : ConstDataRow<BUFF>
{
	public const int TableID = 51;
	public const int DigitBase = 5;
	public int n_ID;
	public int n_STACK;
	public int n_BUFF_TYPE;
	public string s_BUFF_ICON;
	public float f_DURATION;
	public string s_BUFF_CANCEL;
	public string s_BUFF_CONDITON;
	public string s_CONDITIONAL_BUFF;
	public string s_CONSTANT_BUFF;
	public string s_BUFF_FXS;
}
