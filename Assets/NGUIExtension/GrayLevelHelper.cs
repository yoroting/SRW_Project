using UnityEngine;
using System.Collections;

/// <summary>
/// Gray level helper.
/// Tween gray level needs to change shader at run time
/// </summary>
public class GrayLevelHelper : MonoBehaviour {

	public static TweenGrayLevel StartTweenGrayLevel(GameObject textureObj, float duration)
	{
		UITexture texture = textureObj.GetComponent<UITexture>();
		texture.material = new Material(Shader.Find("Custom/GrayLevel"));
		TweenGrayLevel tw = TweenGrayLevel.Begin<TweenGrayLevel>(textureObj, duration);

		return tw;
	}

	public static TweenGrayLevel StartTweenGrayLevel(UITexture texture, float duration)
	{
		texture.material = new Material(Shader.Find("Custom/GrayLevel"));
		TweenGrayLevel tw = TweenGrayLevel.Begin<TweenGrayLevel>(texture.gameObject, duration);
		
		return tw;
	}
}
