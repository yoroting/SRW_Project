using UnityEngine;
using System.Collections;

/// <summary>
/// Gray level helper.
/// Tween gray level needs to change shader at run time
/// </summary>
public class GrayLevelHelper : MonoBehaviour {

	public static TweenGrayLevel StartTweenGrayLevel(GameObject textureObj, float duration , bool newmat = true )
	{
		UITexture texture = textureObj.GetComponent<UITexture>();
        //texture.shader = Shader.Find("Custom/GrayLevel");
        if (newmat == true)
        {
            texture.material = new Material(Shader.Find("Custom/GrayLevel"));
        }
        //  texture.material = new Material(Shader.Find("Custom/GrayLevel (SoftClip)"));
        TweenGrayLevel tw = TweenGrayLevel.Begin<TweenGrayLevel>(textureObj, duration);

		return tw;
	}

	public static TweenGrayLevel StartTweenGrayLevel(UITexture texture, float duration ,bool newmat = true )
	{
        //texture.shader = Shader.Find("Custom/GrayLevel");
        if (newmat == true)
        {
            texture.material = new Material(Shader.Find("Custom/GrayLevel"));
        }
        //  texture.material = new Material(Shader.Find("Custom/GrayLevel (SoftClip)")); // need a new material
        TweenGrayLevel tw = TweenGrayLevel.Begin<TweenGrayLevel>(texture.gameObject, duration);
		
		return tw;
	}
}
