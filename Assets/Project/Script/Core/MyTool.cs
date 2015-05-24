using UnityEngine;
using System.Collections;
using MYGRIDS;
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
