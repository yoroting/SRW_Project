using UnityEngine;
using System.Collections;

public class MoveOverEffect : MonoBehaviour {


	void OnEnable()
	{

		TweenAlpha tw = TweenAlpha.Begin<TweenAlpha> (this.gameObject, 3.0f );
		if (tw != null) {
			tw.from = 1.0f;
			tw.to = 0.2f;
			tw.style = UITweener.Style.PingPong;
		}
//		UIWidget wi = this.GetComponent<UIWidget >();
//		if( wi != null ){
//			wi.width = 100;  // no used here
//			wi.height = 100;
//
//		}

	}
	// Use this for initialization
	void Start () {
		UIWidget wi = this.GetComponent<UIWidget >();
		if( wi != null ){
			wi.width = 100;  // size fix to correct wh
			wi.height = 100;			
		}
	}

	// Update is called once per frame
	void Update () {
	}
}
