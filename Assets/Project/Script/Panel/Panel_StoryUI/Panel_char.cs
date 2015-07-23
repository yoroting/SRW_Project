using UnityEngine;
using System.Collections;

public class Panel_char : MonoBehaviour {

	public GameObject FaceObj;


	public void OnEnable()
	{


	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartGray()
	{
		if (FaceObj == null)
			return;

		UITexture texture = FaceObj.GetComponent< UITexture >(); 
		
		//TweenGrayLevel.Begin <TweenGrayLevel>(  texture , 1.0f  );
		TweenGrayLevel tw = TweenGrayLevel.Begin <TweenGrayLevel >( texture.gameObject, 1.0f);
		if (tw) {
			
			tw.from = 0.0f;
			tw.to   = 1.0f;
			MyTool.TweenSetOneShotOnFinish( tw , OnDead );
		}

	}


	public void OnDead( )
	{	
		
		//GameEventManager.DispatchEvent ( evt );
	}
}
