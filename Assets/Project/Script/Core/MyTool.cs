using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
using MYGRIDS;
using _SRW;

// 
public class MyTool {

	// cache value
	public static float fScnRatio { set; get; }

	// All static func as tool
	public static float ScreenToLocX( float ScnX )
	{
		float x = (ScnX -(Screen.width/2)) * fScnRatio;	
		return x;
	}
	public static float ScreenToLocY( float ScnY )
	{
		float y = (ScnY -(Screen.height/2)) * fScnRatio;
		return y;
	}

	public static Vector3 LocToScreenX( GameObject obj )
	{
		return  UICamera.currentCamera.WorldToScreenPoint( obj.transform.position );
	}

	public static Vector3 SnyGridtoLocalPos( int x , int y ,  ref cMyGrids grids  )
	{
		Vector3 v = new Vector3();
		v.x = grids.GetRealX( x );
		v.y = grids.GetRealY( y );
		return v;

	}
	//
	public static List <GameObject > GetChildPool( GameObject obj)
	{
		if (obj == null)
			return null;

		List <GameObject > pool = new List <GameObject >();

		foreach (Transform child in obj.transform)
		{
			//child is your child transform

			pool.Add( child.gameObject );

		}
		return pool;
	}

	//
	public static void SetLabelText( GameObject go , string sText )
	{
		UILabel lbl = go.GetComponent< UILabel >();
		if( lbl != null )
		{
			lbl.text = sText;
		}
	}
	
	public static void SetLabelInt( GameObject go , int nValue )
	{
		UILabel lbl = go.GetComponent< UILabel >();
		if( lbl != null )
		{
			lbl.text =  nValue.ToString();
		}
	}
	
	public static void SetLabelFloat( GameObject go , float fValue )
	{
		UILabel lbl = go.GetComponent< UILabel >();
		if( lbl != null )
		{
			lbl.text =  fValue.ToString();
		}
	}


	public static void TweenSetOneShotOnFinish( UITweener Tween , EventDelegate.Callback call )
	{
		if (Tween != null) {
			Tween.style = UITweener.Style.Once;

			EventDelegate del = new EventDelegate( call );
			del.oneShot = true;
			Tween.SetOnFinished( del );
		}
	}
	// Get CmdID key name
	public static string GetCMDNameByID( _CMD_ID eID )
	{
		return eID.ToString ();
	}
	public static _CMD_ID GetCMDIDByName( string name )
	{
		string[] names = Enum.GetNames(typeof(_CMD_ID));
		_CMD_ID[] values = (_CMD_ID[])Enum.GetValues(typeof(_CMD_ID));
		
		for( int i = 0; i < names.Length; i++ )
		{
			if( name == names[i] ){
				return values[i];
			}

		}
		return _CMD_ID._NONE;

	}


	public static T GetPanel<T>( GameObject obj   )
	{	
		return obj.GetComponent< T > ();  
	}
	// 动态的计算出现在manualHeight的高度。
	static private void AdaptiveUI()
	{
		int ManualWidth = 960;
		int ManualHeight = 640;
		UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot>();
		if (uiRoot != null)
		{
			if (System.Convert.ToSingle(Screen.height) / Screen.width > System.Convert.ToSingle(ManualHeight) / ManualWidth)
				uiRoot.manualHeight = Mathf.RoundToInt(System.Convert.ToSingle(ManualWidth) / Screen.width * Screen.height);
			else
				uiRoot.manualHeight = ManualHeight;
		}
	}

}
