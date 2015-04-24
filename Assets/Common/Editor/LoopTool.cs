using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LoopTool {
	
//	[MenuItem("Playcoo/test")]
//	public static void Test(){
//		GameObject select = Selection.activeGameObject;
//
////		PrefabTool.CheckIsHierarchyPrefab(select);
//
//		Debug.Log(PrefabTool.CheckIsHierarchyPrefab(select));
////		BattleTeamBar battleTeamBar = select.GetComponent<BattleTeamBar>();
////		if(battleTeamBar == null)
////			return;
////
////		Func<int, int, GameObject, Component, bool> loopFunc = (count, index, prefab, component) => {
////			Debug.Log(prefab.name + "." + component.GetType(), component.gameObject);
////			if(component == battleTeamBar){
////				Debug.Log("yes");
////			}else{
////				Debug.Log("no");
////			}
////			return true;
////		};
////		LoopPrefabChildrenComponent("_Project/Resources/Panel", loopFunc, typeof(BattleTeamBar));
//	}

	[MenuItem("Custom/找出錯誤的 BoxCollider")]
	public static void Test(){
		Func<int, int, GameObject, Component, int> loopFunc = (count, index, prefab, component) => {
			if(component is BoxCollider){
//				BoxCollider collider = component as BoxCollider;
				Debug.LogWarning("有用到 BoxCollider ! 要改用 BoxCollider2D", prefab);

			}else if(component is BoxCollider2D){
				BoxCollider2D collider2D = component as BoxCollider2D;
				if(!collider2D.isTrigger){
					Debug.LogWarning("BoxCollider2D 沒勾選 Trigger", prefab);
				}
			}
			return 0;
		};
		LoopPrefabChildrenComponent("_Project/Resources/Panel", loopFunc, typeof(BoxCollider), typeof(BoxCollider2D));
	}

//	[MenuItem("Playcoo/找出使用到 BoxCollider 的 Prefab 物件")]
//	public static void FindBoxCollider(){
//		Func<int, int, GameObject, Component, int> loopFunc = (count, index, prefab, component) => {
//			BoxCollider collider = component as BoxCollider;
//			Debug.LogWarning("有用到 BoxCollider !", prefab);
//			return 0;
//		};
//		LoopPrefabChildrenComponent("_Project/Resources/Panel", loopFunc, typeof(BoxCollider));
//	}

	/// <summary>
	/// 巡訪 Prefab 跟底下子物件的 Component
	/// </summary>
	/// <param name="rootPath">ex: "_Project/Resources/Atlas"</param>
	/// <param name="loopFunc">loopFunc = (count, index, prefab, component)，傳回 0:繼續, 1:中斷, 2:下個Child, 3:下個Prefab</param>
	/// <param name="filters">ex: typeof(BoxCollider2D)</param>
	public static void LoopPrefabChildrenComponent(string rootPath, Func<int, int, GameObject, Component, int> loopFunc, params Type[] filters){
		bool hasFilter = (filters != null && filters.Length > 0);

		Func<int, int, GameObject, GameObject, int> loopChild = (count, index, prefab, child) => {
			Component[] components = child.GetComponents<Component>();
			
			for(int i=0; i<components.Length; i++){
				if(hasFilter){
					Component component = components[i];
					if(Array.IndexOf(filters, component.GetType()) == -1)
						continue;
				}
				int response = loopFunc(count, index, prefab, components[i]);
				if(response == 0)
					continue;
				else if(response == 1)
					return 1;
				else if(response == 2)
					return 0;
				else if(response == 3)
					return 2;
				else
					Debug.LogWarning("錯誤的回傳值：" + response);
//				if(!loopFunc(count, index, prefab, components[i])){
//					return false;
//				}
			}
			return 0;
		};
		
		LoopPrefabChildren(rootPath, loopChild);
	}

	/// <summary>
	/// 巡訪 Prefab 跟底下的子物件
	/// </summary>
	/// <param name="rootPath">ex: "_Project/Resources/Atlas"</param>
	/// <param name="loopFunc">loopFunc = (count, index, prefab, child)，傳回 0:繼續, 1:中斷, 2:下個Prefab</param>
	public static void LoopPrefabChildren(string rootPath, Func<int, int, GameObject, GameObject, int> loopFunc){
		int response = 0;
//		bool continueLoop = true;
		int prefabCount = 0;
		int nowIndex = -1;
		GameObject mainPrefab = null;

		Func<Transform, bool> loopChildren = (child) => {
			response = loopFunc(prefabCount, nowIndex, mainPrefab, child.gameObject);
			if(response == 0)
				return true;
			else if(response == 1)
				return false;
			else if(response == 2)
				return false;
			else
				Debug.LogWarning("錯誤的回傳值：" + response);
			return true;
//			continueLoop = loopFunc(prefabCount, nowIndex, mainPrefab, child.gameObject);
//			return continueLoop;
		};

		Func<int, int, GameObject, bool> loopPrefab = (count, index, prefab) => {
			prefabCount = count;
			nowIndex = index;
			mainPrefab = prefab;
			LoopAllChildren(prefab.transform, loopChildren);
			if(response == 0)
				return true;
			else if(response == 1)
				return false;
			else if(response == 2)
				return true;
			else
				Debug.LogWarning("錯誤的回傳值：" + response);
			return true;
//			return continueLoop;
		};

		LoopPrefabs(rootPath, loopPrefab);
	}

	/// <summary>
	/// 巡訪 Prefab
	/// </summary>
	/// <param name="rootPath">ex: "_Project/Resources/Atlas"</param>
	/// <param name="loopFunc">loopFunc = (count, index, prefab)</param>
	public static void LoopPrefabs(string rootPath, Func<int, int, GameObject, bool> loopFunc){
		string assetsPath = Application.dataPath.Replace("Assets", "");
		int assetsPathLength = assetsPath.Length;
		Func<int, int, string, bool> loopPath = (count, index, filePath) => {
			filePath = filePath.Substring(assetsPathLength, filePath.Length-assetsPathLength);
			GameObject prefab = AssetDatabase.LoadAssetAtPath(filePath, typeof(GameObject)) as GameObject;
			return loopFunc(count, index, prefab);
		};

		LoopFunc(Application.dataPath + "/" + rootPath, loopPath, "*.prefab");
	}

	/// <summary>
	/// 巡訪檔案路徑
	/// </summary>
	/// <param name="rootPath">完整路徑</param>
	/// <param name="extensions">附檔名</param>
	/// <param name="loopFunc">loopFunc = (count, index, filePath)</param>
	public static void LoopFunc(string rootPath, Func<int, int, string, bool> loopFunc, params string[] extensions){
		string[] allFilePath = null;
		int extensionsLength = extensions.Length;
		if(extensionsLength == 1){
			allFilePath = Directory.GetFiles(rootPath, extensions[0], SearchOption.AllDirectories);
		}else{
			allFilePath = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories);
		}

		int allFilePathCount = allFilePath.Length;
		for(int i=0; i<allFilePathCount; i++){
			string filePath = allFilePath[i];
			filePath = filePath.Replace("\\", "/");
			if(extensionsLength == 1){
				if(!loopFunc(allFilePathCount, i, filePath))
					break;

			}else if(extensionsLength > 1){
				string extension = Path.GetExtension(filePath);
				if(Array.IndexOf(extensions, extension) == -1)
					continue;
				if(!loopFunc(allFilePathCount, i, filePath))
					break;

			}else{
				if(!loopFunc(allFilePathCount, i, filePath))
					break;
			}
		}
	}


	
	/// <summary>
	/// 非遞迴方式尋訪所有子物件（順序：先從最底層子物件開始跑）
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="loopFunc">尋訪方法（傳回是否繼續）</param>
	private static void LoopAllChildren(Transform target, Func<Transform, bool> loopFunc){
		if(target == null) return;
		
		Transform nowTarget = target;
		//各物件已經跑到第幾個子物件列表
		Dictionary<Transform, int> map_parent_childIndex = new Dictionary<Transform, int>();
		while(true){
			//判斷第一次來此物件，並且有子物件
			if(!map_parent_childIndex.ContainsKey(nowTarget) && nowTarget.childCount > 0){
				//往下層去
				if(!map_parent_childIndex.ContainsKey(nowTarget)){
					map_parent_childIndex.Add(nowTarget, -1);
				}
				map_parent_childIndex[nowTarget]++;
				nowTarget = nowTarget.GetChild(map_parent_childIndex[nowTarget]);
				continue;
			}
			
			if(!loopFunc(nowTarget)){
				break;
			}
			
			if(nowTarget.parent != null && map_parent_childIndex.ContainsKey(nowTarget.parent)){
				if(map_parent_childIndex[nowTarget.parent] < (nowTarget.parent.childCount-1)){
					//往下一個同一層物件
					map_parent_childIndex[nowTarget.parent]++;
					nowTarget = nowTarget.parent.GetChild(map_parent_childIndex[nowTarget.parent]);
					continue;
				}
				
				//往上一層
				nowTarget = nowTarget.parent;
				continue;
			}
			
			//沒有上一層時，結束
			break;
		}
	}
}
