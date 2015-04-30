public class STORY_DATA{
     public int n_ID;
	 public string s_MODLE_ID;
	 public int n_SCENE_ID;
	 public int n_SCENE_BGM;
	 public int n_NEXT_STAGE;
	 public int n_NEXT_TALK;
	 public string s_CONTEXT;

	 //====== text ======
     public string s_MAIN_TEXT;
     public string s_STORY_TEXT;

	// Fill Data
	 public bool FillDatabyDataRow( DataRow row )
	{
		if( row == null ) return false;
		n_ID 		= row.Field< int >("n_ID");
		s_MODLE_ID  = row.Field< string >("s_MODLE_ID");
		n_SCENE_ID  = row.Field< int >("n_SCENE_ID");
		n_SCENE_BGM = row.Field< int >("n_BGM");
		n_NEXT_STAGE = row.Field< int >("n_NEXT_STAGE");
		n_NEXT_TALK = row.Field< int >("n_NEXT_TALK");
		s_CONTEXT  = row.Field< string >("s_CONTEXT");
		//		if( !String.IsNullOrEmpty( strFile ))

		return true;
	}
}
