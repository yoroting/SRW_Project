using UnityEngine;
using System.Collections;

public class BattleValue : MonoBehaviour {

//	static public int nValueCount=0;  // no more need

	void OnEnable()
	{
		transform.localRotation = new Quaternion(); 
		transform.localScale = new Vector3( 1.0f, 1.0f ,1.0f);

		TweenAlpha tw = TweenAlpha.Begin<TweenAlpha> (this.gameObject, 1.0f );
		if (tw != null) {
			tw.from = 1.0f;
			tw.to = 0.0f;
			tw.delay = 1.5f;
		}

		//reset object alpha
		MyTool.SetAlpha (this.gameObject , 1.0f);


//		nValueCount++;
		
	}
	// Use this for initialization
	void Start () {
	//	nValueCount++;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void OnAlphaFinish()
	{
//		nValueCount--;
		this.Recycle ();
		//NGUITools.Destroy ( this.gameObject );
		
	}
}
