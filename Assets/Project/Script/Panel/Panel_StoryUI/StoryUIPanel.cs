using UnityEngine;
using System.Collections;

public class StoryUIPanel : MonoBehaviour {

	public static int m_StageID;

	void Awake(){
		// load const stage data
		// 播放  mian BGM
		DataRow row = ConstDataManager.Instance.GetRow("STORE", m_StageID );
		if( row != null )
		{
			//		string strFile = row.Field< string >("s_FILENAME");
			//		if( !String.IsNullOrEmpty( strFile ))
			//		{
			//			string audioPath = ResourcesManager.GetAudioClipPath( AudioChannelType.BGM ,  strFile );
			//			AudioManager.Instance.Play( AudioChannelType.BGM ,  audioPath );
			//		}
			
			string prePath = "Panel/Panel_char";
			GameObject preObj = Resources.Load( prePath ) as GameObject;
			if( preObj != null )
			{
				GameObject char1obj  = NGUITools.AddChild( this.gameObject , preObj );
				if( char1obj != null )
				{
					Vector3 v = new Vector3( -50 , 50 ,0 );
					char1obj.transform.position = v;
				}
				
				
				
				GameObject char2obj  = NGUITools.AddChild( this.gameObject , preObj );
				if( char2obj != null )
				{
					Vector3 v = new Vector3( 50 , +50 ,0 );
					char2obj.transform.position = v;
					
				}
			}
		}
	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
