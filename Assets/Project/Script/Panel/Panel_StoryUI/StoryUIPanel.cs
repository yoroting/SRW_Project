using UnityEngine;
using System.Collections;

public class StoryUIPanel : MonoBehaviour {

	public STORY	m_StoryData;		// 目前操作的 story data
	private int		m_nFlowIdx;		// 腳本演到哪段

	void Awake(){
		Debug.Log("StoryUIPanel:awake");

		GameSystem.PlayBGM (2);

		// load const stage data
		// 播放  mian BGM
		DataRow row = ConstDataManager.Instance.GetRow("STAGE_STORY", GameDataManager.Instance.m_nStageID );
		if( row != null )
		{
			//		string strFile = row.Field< string >("s_FILENAME");
			//		if( !String.IsNullOrEmpty( strFile ))
			//		{
			//			string audioPath = ResourcesManager.GetAudioClipPath( AudioChannelType.BGM ,  strFile );
			//			AudioManager.Instance.Play( AudioChannelType.BGM ,  audioPath );
			GameObject char1obj = GameSystem.CreateCharacterGameObj( this.gameObject );
			if( char1obj != null )
			{
				Vector3 v = new Vector3( -50 , 50 ,0 );
				char1obj.transform.position = v;
			}
		}
	}
	// Use this for initialization
	void Start () {
		Debug.Log("StoryUIPanel:start");

	}
	
	// Update is called once per frame
	void Update () {
	//	Debug.Log("StoryUIPanel:update");

	
	}
}
