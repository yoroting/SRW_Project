using UnityEngine;
using System.Collections;

public class AttackOverEffect : MonoBehaviour {


	void OnEnable()
	{
		transform.localRotation = new Quaternion(); 
		transform.localScale = new Vector3( 1.0f, 1.0f ,1.0f);
		
		TweenAlpha tw = TweenAlpha.Begin<TweenAlpha> (this.gameObject, 3.0f );
		if (tw != null) {
			tw.from = 1.0f;
			tw.to = 0.2f;
			tw.style = UITweener.Style.PingPong;
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
