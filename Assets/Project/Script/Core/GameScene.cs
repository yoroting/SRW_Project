using UnityEngine;
using System.Collections;
using MYGRIDS;


public partial class GameScene
{
	//public delegate AudioClip GetAudioClipDelegate (string audioPath);
	
	private bool hadInit = false;
	public bool HadInit{ get{ return hadInit; } }
	
	public MyGrids Grids;				// main grids . only one
	
	
	//	private Dictionary<int, AudioChannelBase> channels = new Dictionary<int, AudioChannelBase>();
	
	public void Initial( ){
		if (hadInit)return;
		
		hadInit = true;
		//this.GetAudioClipFunc = getAudioClipFunc;
		//UnitPool = new Dictionary< int , UNIT_DATA >();
		//CampPool = new Dictionary< _CAMP , cCamp >();
		Grids = new MyGrids();
	}
	
	private static GameScene instance;
	public static GameScene Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new GameScene();

				instance.Clear();
			}
			
			return instance;
		}
	}
	
	public void Clear()
	{
		
	}
}
