using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class PrefabTool {

	
//	[MenuItem("Playcoo/test")]
//	public static void Test(){
//		EditorUtility.UnloadUnusedAssetsIgnoreManagedReferences();
//
////		GameObject c = GameObject.Find("c");
////		GameObject a = GameObject.Find("a");
////		PlusPrefab(c, a);
//
//		//原型 Prefab 物件
//		GameObject ancestorPrefab = Selection.activeGameObject;
//		if(CheckIsHierarchyPrefab(ancestorPrefab))
//			return;
//
//		//識別的 Component
//		BattleActorIcon discernComponent = ancestorPrefab.GetComponent<BattleActorIcon>();
//
//		Func<int, int, GameObject, Component, int> loopFunc = (count, index, prefab, component) => {
//			if(component == discernComponent)
//				return 0;
//
//			GameObject prefabRootInst = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
//			BattleActorIcon[] instChildren = prefabRootInst.GetComponentsInChildren<BattleActorIcon>();
//			if(instChildren[0].gameObject == prefabRootInst){
//				Debug.LogWarning("待補上，目標物件為主要 Prefab");
//				return 3;
//			}
//			for(int i=0; i<instChildren.Length; i++){
//				//套上 Prefab
//				GameObject instChild = instChildren[i].gameObject;
//				Debug.Log("開始處理:" + instChild.name, prefab);
//
//				PrefabUtility.DisconnectPrefabInstance(prefabRootInst);
//				UnityEngine.Object tempPrefab = PrefabUtility.CreateEmptyPrefab("Assets/TempPrefab.prefab");
//				PrefabUtility.ReplacePrefab(instChild, tempPrefab, ReplacePrefabOptions.ConnectToPrefab);
//				PrefabUtility.ReplacePrefab(ancestorPrefab, tempPrefab, ReplacePrefabOptions.ReplaceNameBased);
//				PrefabUtility.RevertPrefabInstance(instChild);
////				break;
//			}
//
////			UnityEngine.Object tempRootPrefab = PrefabUtility.CreateEmptyPrefab("Assets/TempPrefab.prefab");
////			PrefabUtility.ReplacePrefab(prefabRootInst, tempRootPrefab, ReplacePrefabOptions.ReplaceNameBased);
////			GameObject tempPrefab2Go = AssetDatabase.LoadAssetAtPath("Assets/TempPrefab.prefab", typeof(GameObject)) as GameObject;
////			PrefabUtility.ReplacePrefab(tempPrefab2Go, prefab, ReplacePrefabOptions.ReplaceNameBased);
////			PrefabUtility.ReconnectToLastPrefab(prefabRootInst);
////			EditorUtility.SetDirty(prefab);
////			AssetDatabase.SaveAssets();
////			AssetDatabase.Refresh();
////			AssetDatabase.DeleteAsset("Assets/TempPrefab.prefab");
//
//			ReplaceObjToPrefab(prefabRootInst, prefab);
//
//			MonoBehaviour.DestroyImmediate(prefabRootInst);
//
//			return 1;
//		};
//		LoopTool.LoopPrefabChildrenComponent("_Project/Resources/Panel", loopFunc, typeof(BattleActorIcon));
//	}
//	
//	[MenuItem("Playcoo/test2")]
//	public static void Test2(){
//		GameObject ancestorPrefab = Selection.activeGameObject;
//		GameObject targetPrefab = AssetDatabase.LoadAssetAtPath("Assets/_Project/Resources/Panel/Panel_MainBattle/BattleStuffPanel/UserBattleTeamPanel.prefab", typeof(GameObject)) as GameObject;
//		ReplaceObjToPrefab(ancestorPrefab, targetPrefab);
//	}


	private static void ReplaceObjToPrefab(GameObject obj, GameObject prefab){
		if(CheckIsHierarchyPrefab(prefab)){
			Debug.LogWarning("prefab 必須是 Project 中的檔案");
			return;
		}

		UnityEngine.Object tempRootPrefab = null;
		if(!CheckIsPrefab(obj) || CheckIsHierarchyPrefab(obj)){
			//Hierarchy 中的物件需要先存成暫存的 Prefab ，在用暫存的 Prefab 覆蓋掉目標 prefab
			tempRootPrefab = PrefabUtility.CreateEmptyPrefab("Assets/TempPrefab.prefab");
			PrefabUtility.ReplacePrefab(obj, tempRootPrefab, ReplacePrefabOptions.ReplaceNameBased);
			GameObject tempPrefab2Go = AssetDatabase.LoadAssetAtPath("Assets/TempPrefab.prefab", typeof(GameObject)) as GameObject;
			PrefabUtility.ReplacePrefab(tempPrefab2Go, prefab, ReplacePrefabOptions.ReplaceNameBased);

//			PrefabUtility.ReconnectToLastPrefab(obj);
//			PrefabUtility.RevertPrefabInstance(prefabRootInst);
		}else{
			PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ReplaceNameBased);
		}

		EditorUtility.SetDirty(prefab);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		if(tempRootPrefab != null)
			AssetDatabase.DeleteAsset("Assets/TempPrefab.prefab");
	}


	/// <summary>判斷是否為 Prefab 物件（不分 Hierarchy 或 Project 中的物件）</summary>
	public static bool CheckIsPrefab(GameObject target){
		UnityEngine.Object component_root = PrefabUtility.FindPrefabRoot(target);
		UnityEngine.Object component_prefab = PrefabUtility.GetPrefabObject(component_root);
		return (component_prefab != null);
	}
	
	/// <summary>判斷是否為 Hierarchy 中的 Prefab 物件</summary>
	public static bool CheckIsHierarchyPrefab(GameObject target){
		if(!CheckIsPrefab(target))
			return false;
		GameObject prefabRoot = PrefabUtility.FindPrefabRoot(target);
		//取得 Prefab 物件在 Hierarchy 中的根物件
		UnityEngine.Object prefabParent = PrefabUtility.GetPrefabParent(prefabRoot);
		return (prefabParent != null);
	}

}
