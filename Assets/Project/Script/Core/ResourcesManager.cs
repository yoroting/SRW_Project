/*
 *	ResourcesManager
 *
 *	Load resources from Assetbundle or Resources folder
 *
 */

using UnityEngine;
using System;

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Playcoo.Common;

public class ResourcesManager {

	public static void LoadLevel(int sceneIndex)
	{
		Application.LoadLevel(sceneIndex);
	}
	public static void LoadLevel(string sceneName)
	{
		Application.LoadLevel(sceneName);
	}

	public static string GetAudioClipPath(AudioChannelType channelType, string audioName)
	{
		switch(channelType){
		case AudioChannelType.BGM:
			return "Audio/BGM/" + audioName;
		case AudioChannelType.SoundFX:
			return "Audio/Sound/" + audioName;
		default:
			Debug.LogError("例外的 ChannelType：" + channelType.ToString());
			return null;
		}
	}

	public static AudioClip LoadClip(string audioPath)
	{
        AudioClip clip = Resources.Load<AudioClip>(audioPath);
        if (clip == null)
        {
            Debug.LogError("音效資源讀取失敗:" + audioPath);
        }
        //  clip.loadInBackground = true;
        return clip;

       
    }

	public static GameObject LoadFightBulletFx(string fxName){
		string path = "FX/FightBullet/" + fxName;
		return Resources.Load(path) as GameObject;
	}

	/// <summary>載入戰鬥場景</summary>
	public static GameObject LoadBattleScene(string battleSceneName){
		string path = "BattleScene/" + battleSceneName;
		return Resources.Load(path) as GameObject;
	}

	#region Load From Resources

	public static GameObject LoadComicAnimationPrefab(string name)
	{
		string path = "ComicAnimationPrefabs/" + name;
		GameObject gameObj = Resources.Load<GameObject>(path);
		if(gameObj == null) Debug.LogError("LoadComicAnimationPrefab failed, path = " + path);

		return gameObj;
	}
	public static GameObject CreatePrefabGameObj( GameObject parent , string sPrefabPath )
	{
		GameObject preObj = Resources.Load( sPrefabPath ) as GameObject;
		if (preObj != null) {
			return  NGUITools.AddChild ( parent, preObj);
		}
		return null;
	}

//	public static GameObject LoadModelPrefab (string name)
//	{
//		return Resources.Load("Actor/" + name, typeof(GameObject)) as GameObject;
//	}
//
//	public static GameObject LoadActorComponent (string name)
//	{
//		return Resources.Load("ActorComponent/" + name, typeof(GameObject)) as GameObject;
//	}
//
//	public static AnimationClip LoadActorAnimation (string name)
//	{
//		return Resources.Load("ActorAnimation/" + name, typeof(AnimationClip)) as AnimationClip;
//	}
//	
//	public static UIAtlas LoadActorAtlas (string name)
//	{
//		return Resources.Load("Atlas/Actor/" + name, typeof(UIAtlas)) as UIAtlas;
//	}
//
//	public static Texture2D LoadActorTexture (string name, int grade)
//	{
//		return Resources.Load(string.Format("ActorTexture/{0}/{0}_{1}", name, grade), typeof(Texture2D)) as Texture2D;
//	}
//	
//	public static UIAtlas LoadFXAtlas (string name)
//	{
//		return Resources.Load("Atlas/FX/" + name, typeof(UIAtlas)) as UIAtlas;
//	}
//
//	public static GameObject LoadFXPrefab (string name)
//	{
//        if ( string.IsNullOrEmpty(name) )
//        {
//             return null;
//        }
//
//        GameObject GetObj = Resources.Load("Effect/" + name, typeof(GameObject)) as GameObject;
//
//
//
//		return GetObj;
//	}
//
//	public static AudioClip LoadAudio (string name)
//	{
//		return Resources.Load("Sound/" + name, typeof(AudioClip)) as AudioClip;
//	}
//
//
//	public static UnityEngine.Object LoadPreRes(string name)
//	{
//        if ( string.IsNullOrEmpty(name) )
//             return null;
//
//		return Resources.Load(name, typeof(GameObject));
//	}
//
//	public static GameObject LoadWeaponPrefab (string name)
//	{
//		return Resources.Load("Weapon/" + name, typeof(GameObject)) as GameObject;
//	}
//
//	public static Texture2D LoadWeaponTexture (string name)
//	{
//		return Resources.Load("WeaponTexture/" + name, typeof(Texture2D)) as Texture2D;
//	}

	#endregion
}
