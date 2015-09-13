using UnityEngine;
using System.Collections;

public class BattleValue : MonoBehaviour {

//	static public int nValueCount=0;  // no more need
	float fOffset = 16.0f;
	float fOriginY = 0.0f;

	void OnEnable()
	{
		transform.localRotation = new Quaternion(); 
		transform.localScale = new Vector3( 1.5f, 1.5f , 1.5f);
		// clear all twr

		float x = transform.localPosition.x;
		float y = transform.localPosition.y;
		fOriginY = y;

	//	MyTool.DestoryTweens (this.gameObject);

//		TweenX twx = TweenX.Begin<TweenX> ( gameObject , 0.25f);
//		if (twx != null) {
//			twx.from = x ; 
//			twx.to = x - 16;
//			MyTool.TweenSetOneShotOnFinish( twx , OnTweenXFinish );
//			twx.Play( );
//		}
		TweenY twy = TweenY.Begin<TweenY> ( gameObject , 0.5f);
		if (twy != null) {
			twy.delay = 0.5f;
			twy.from = y ; 
			twy.to = y + 64;
			twy.style = UITweener.Style.Once;
			//MyTool.TweenSetOneShotOnFinish( twy , OnTweenXFinish );
			//twy.Play( );
		}




		TweenScale tws = TweenScale.Begin< TweenScale > ( gameObject , 0.1f );
		if (tws != null) {
			tws.SetStartToCurrentValue(); 
			tws.to = new Vector3( 1.0f, 1.0f ,1.0f);
		}

		// re start to twa
//		TweenAlpha twa = gameObject.GetComponent<TweenAlpha>(); // TweenAlpha.Begin<TweenAlpha> (this.gameObject, 1.0f );
//		if (twa != null) {
//		//	twa.Play();
//		}

		TweenAlpha twa = TweenAlpha.Begin<TweenAlpha> (this.gameObject, 0.5f );
		if (twa != null) {
//			twa.from = 1.0f;
			twa.to = 0.0f;
			twa.delay = 1.5f;
	//		MyTool.TweenSetOneShotOnFinish( twx , OnAlphaFinish );
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

	void PlayTweenY( )
	{
		TweenY twy = TweenY.Begin<TweenY> ( gameObject , 0.25f);
		if (twy != null) {
			twy.from = fOriginY ; 
			twy.to = fOriginY + fOriginY;

			MyTool.TweenSetOneShotOnFinish( twy , OnTweenXFinish );

			if( twy.direction == AnimationOrTween.Direction.Forward )
			{

			}
			else {

			}

		}
	}

	public void OnTweenXFinish()
	{
		TweenY twy = TweenY.Begin<TweenY> ( gameObject , 0.25f);
		if (twy != null) {
			twy.PlayReverse();
		}
	}

	public void OnAlphaFinish()
	{
//		nValueCount--;
		this.Recycle ();
		//NGUITools.Destroy ( this.gameObject );
		
	}
}
