public class CHAR_DATA{
     public int n_ID;
     public string s_NAME;
     public string s_FILENAME;
	 public int n_TAG;
	 public int n_TYPE;
     public int n_RANK;
	 public int n_FACTION;
	 public int n_LVMAX;

	 public int n_HP;
	 public int n_MP;
	 public int n_SP;
	 public int n_MAR; // 武功
	 public int n_ATK;
	 public int n_DEF;
	 public int n_CURSORAI;  //
	 public int n_COMBOAI;  //
	//===============
    

	// Fill Data
	public bool FillDatabyDataRow( DataRow row )
	{
		if( row == null ) return false;
		n_ID 		= row.Field< int >("n_ID");
		s_NAME  	= row.Field< string >("s_NAME");
		s_FILENAME  	= row.Field< string >("s_FILENAME");
		n_TAG 		= row.Field< int >("n_TAG");
		n_TYPE 		= row.Field< int >("n_TYPE");
		n_RANK 		= row.Field< int >("n_RANK");
		n_FACTION 		= row.Field< int >("n_FACTION");
		n_LVMAX 		= row.Field< int >("n_LVMAX");

		n_HP 		= row.Field< int >("n_HP");
		n_MP 		= row.Field< int >("n_MP");
		n_SP 		= row.Field< int >("n_SP");
		n_MAR 		= row.Field< int >("n_MAR");
		n_ATK 		= row.Field< int >("n_ATK");
		n_DEF 		= row.Field< int >("n_DEF");
	
		return true;
	}
    
}
