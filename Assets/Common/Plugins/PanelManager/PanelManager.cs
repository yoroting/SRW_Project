using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public partial class PanelManager : Singleton<PanelManager>
{
	/// <summary>設定場景預設開啟的介面</summary>
	private Dictionary<string, string[]> DefaultPanelConfig_SceneName_PanelNames;

	private UIRoot __UIRoot = null;
	public UIRoot UIRoot{
		get{
			if(__UIRoot == null){
				__UIRoot = FindObjectOfType<UIRoot>();
//				if(root != null)
//					__UIRoot = root.gameObject;
			}
			if(__UIRoot == null){
				__UIRoot = NGUITools.CreateUI(false).GetComponent<UIRoot>();
			}
			return __UIRoot;
		}
	}

    public enum Depth
    {
        Zero = -10,

        #region Normal Panel的Depth範圍
        NormalMin = 0,
        NormalMax = 20,
        #endregion

        #region Top Panel的Depth範圍
        TopMin = 30,
        TopMax = 50,
        #endregion
    }

	/// <summary>遮罩 panel 名稱</summary>
	private string BlockerPanelName;

	/// <summary>啟動 PanelManager</summary>
	public void Initial(string blockerPanelName, Dictionary<string, string[]> defaultPanelConfig=null){
		DontDestroyOnLoad(gameObject);
		BlockerPanelName = blockerPanelName;
		DefaultPanelConfig_SceneName_PanelNames = defaultPanelConfig;
	}

	private void OnLevelWasLoaded(int levelIndex){
		//建立預設介面
		if(DefaultPanelConfig_SceneName_PanelNames != null){
			if(!DefaultPanelConfig_SceneName_PanelNames.ContainsKey(Application.loadedLevelName)) return;
			string[] panelNames = DefaultPanelConfig_SceneName_PanelNames[Application.loadedLevelName];
			for(int i=0; i<panelNames.Length; i++){
//				GetOrCreatUI(panelNames[i]);
				OpenUI(panelNames[i]);
			}
		}
	}
	
	/// <summary>
	/// 只取得介面，如果場景上沒有就傳回 null
	/// </summary>
	/// <returns>The get U.</returns>
	/// <param name="panelName">Panel name.</param>
	public TComponent JustGetUI<TComponent>(string panelName) where TComponent : MonoBehaviour{
		GameObject ui = JustGetUI(panelName);
		if(ui == null) return null;
		return ui.GetComponent<TComponent>();
	}

	/// <summary>
	/// 只取得介面，如果場景上沒有就傳回 null
	/// </summary>
	/// <returns>The get U.</returns>
	/// <param name="panelName">Panel name.</param>
	public GameObject JustGetUI(string panelName){
		GameObject panelObject = null;
		if (!_panelList.TryGetValue(panelName, out panelObject)){
			return null;
		}
		return panelObject;
	}

    /// <summary>
	/// 取得 Panel，回傳要取得的Panel，如果場上沒找到則建立 Panel
    /// </summary>
    /// <param name="panelName">要取得的Panel名字</param>
    public GameObject GetOrCreatUI(string panelName, bool defaultDeactive = true)
	{
		GameObject panelObject = null;
		if (!_panelList.TryGetValue(panelName, out panelObject))
		{
			if(isApplicationQuit) return null;	//app 已經結束，不建立

			string prefabPath = "Panel/" + panelName + "/" + panelName;
			GameObject prefab = Resources.Load(prefabPath) as GameObject;
			
			if(prefab == null || prefab.GetComponent<PanelSetting>() == null){
				Debug.LogError("找不到指定的 UI: " + prefabPath + "，請放到指定位置，並加上 " + typeof(PanelSetting).ToString() + " Component ！");
				return null;
			}

			Vector3 prefabPosition = prefab.transform.localPosition;
			panelObject = NGUITools.AddChild(UIRoot.gameObject, prefab);
			panelObject.transform.localPosition = prefabPosition;
			panelObject.name = panelName;
//			if(defaultDeactive)
//				NGUITools.SetActive(panelObject, false);
			AddPanel(panelObject);
		}

		return panelObject;
    }

    /// <summary>
    /// 開啟Panel，並回傳該Panel
    /// </summary>
    /// <param name="panelName">要開啟的Panel名字</param>
	/// <param name="openPanelData">開啟相關設定</param>
	public GameObject OpenUI(string panelName)
    {
		GameObject panelObject = GetOrCreatUI(panelName, true);
        if (panelObject == null){
			Debug.LogError("找不到指定的Panel: " + panelName);
            return null;
		}

		return OpenUI(panelObject);
    }

    /// <summary>
    /// 開啟Panel，並回傳該Panel
    /// </summary>
	/// <param name="gameObject">要開啟的Panel</param>
	public GameObject OpenUI(GameObject gameObject)
    {
        PanelSetting panelSetting = gameObject.GetComponent<PanelSetting>();
        if (panelSetting == null)
        {
			Debug.LogError("指定的 GameObject 沒有掛 PanelSetting: " + gameObject.name);
            return null;
        }

		return OpenUI(panelSetting);
	}

	private GameObject OpenUI(PanelSetting panelSetting)
	{
		if (panelSetting.gameObject == null)
		{
			Debug.LogError("panelSetting.gameObject == null");
			return null;
		}
		
		GameObject returnObj = null;
		
//		if (NGUITools.GetActive(panelSetting.gameObject))
//		{
//			//介面已在開啟狀態
//			AssignDepth(panelSetting);
//			returnObj = panelSetting.gameObject;
//		}else{
			
//			if (panelSetting.Mutex && !ignoreMutex)
			if (panelSetting.Mutex)
			{
				CloseMutexPanel(panelSetting.gameObject);
				RearrangeDepth(panelSetting.PanelType);
			}

			int newDepth = AssignDepth(panelSetting);
			
			NGUITools.SetActive(panelSetting.gameObject, true);
			
			if (panelSetting.ShowBlocker)
				ShowBlocker(newDepth - BlockerDepthDifference);
			
			returnObj = panelSetting.gameObject;
//		}
		
//		if(OnPanelSwitch != null){
//			StartCoroutine(CallOnPanelSwitch());
//		}


		return returnObj;
	}

    /// <summary>
    /// 關閉Panel
    /// </summary>
    /// <param name="panelName">要關閉的Panel名字</param>
    public void CloseUI(string panelName)
    {
        GameObject panelObject = JustGetUI(panelName);
        if (panelObject == null)
            return;

        CloseUI(panelObject);
    }

    /// <summary>
    /// 關閉Panel
    /// </summary>
    /// <param name="gameObject">要關閉的Panel</param>
    public void CloseUI(GameObject gameObject)
    {
        #region 基本檢查
        if (gameObject == null)
        {
			Debug.LogError("Close null gameObject in PanelManager.");
            return;
        }

        PanelSetting panelSetting = gameObject.GetComponent<PanelSetting>();
        if (panelSetting == null)
        {
			Debug.LogError("PanelSetting is null.");
            return;
        }

        if (!NGUITools.GetActive(gameObject))
            return;
        #endregion


        //Debug.Log("<color=red>CloseUI.(name=" + gameObject.name + "</color>");

        NGUITools.SetActive(gameObject, false);

        //Debug.Log("<color=red>panelSetting.ShowBlocker=" + panelSetting.ShowBlocker.ToString() + ", updateBlocker=" + updateBlocker.ToString() + "</color>");
//        if (gameObject.name != "Panel - TouchBlocker")
		if (gameObject.name != BlockerPanelName)
            UpdateBlockerDepth();

		
//		if(OnPanelSwitch != null){
//			StartCoroutine(CallOnPanelSwitch());
//		}
    }

	/// <summary>
	/// DestoryUI Panel
	/// </summary>
	/// <param name="panelName">要關閉的Panel名字</param>
	public void DestoryUI(string panelName)
	{
		GameObject panelObject = JustGetUI(panelName);
		if (panelObject == null)
			return;
		
		DestoryUI(panelObject);
	}
	
	/// <summary>
	/// DestoryUI Panel
	/// </summary>
	/// <param name="gameObject">要關閉的Panel</param>
	public void DestoryUI(GameObject gameObject)
	{
		#region 基本檢查
		if (gameObject == null)
		{
			Debug.LogError("Close null gameObject in PanelManager.");
			return;
		}
		
		PanelSetting panelSetting = gameObject.GetComponent<PanelSetting>();
		if (panelSetting == null)
		{
			Debug.LogError("PanelSetting is null.");
			return;
		}
		
		//if (!NGUITools.GetActive(gameObject))
		//	return;
		#endregion
		
		
		//Debug.Log("<color=red>CloseUI.(name=" + gameObject.name + "</color>");
		
		NGUITools.SetActive(gameObject, false);
		
		//Debug.Log("<color=red>panelSetting.ShowBlocker=" + panelSetting.ShowBlocker.ToString() + ", updateBlocker=" + updateBlocker.ToString() + "</color>");
		//        if (gameObject.name != "Panel - TouchBlocker")

		DelPanel( gameObject );

		NGUITools.Destroy( gameObject );
		if (gameObject.name != BlockerPanelName)
			UpdateBlockerDepth();
		
		
		//		if(OnPanelSwitch != null){
		//			StartCoroutine(CallOnPanelSwitch());
		//		}

	}

    /// <summary>
    /// 打開Blocker
    /// </summary>
    /// <param name="depth">Blocker深度</param>
    private void ShowBlocker(int depth)
    {
//		GameObject blocker = GetOrCreatUI("Panel - TouchBlocker");
		GameObject blocker = GetOrCreatUI(BlockerPanelName);
        if (blocker == null)
        {
			Debug.LogError("Can't find blocker.");
            return;
        }

        UIPanel panel = blocker.GetComponent<UIPanel>();
        if (panel == null)
        {
			Debug.LogError("Can't find component(UIPanel) in blocker.");
            return;
        }

        NGUITools.SetActive(blocker, true);

        panel.depth = depth;
    }


    /// <summary>
    /// 根據PanelSetting的種類，給予Panel depth，depth會比目前已經開啟的同種類Panel depth還大
    /// </summary>
    /// <param name="panelSetting">panel的資料</param>
    /// <param name="useEditorDepth">是否使用編輯器設定的深度</param>
    /// <returns>給予此Panel的depth</returns>
//    private int AssignDepth(PanelSetting panelSetting, bool useEditorDepth)
    private int AssignDepth(PanelSetting panelSetting)
    {
        if (panelSetting == null)
        {
			Debug.LogError("PanelSetting is null.");
            return 0;
        }

		// yoro : normal panel dom't assign depth.
		if (panelSetting.PanelType == PanelSetting.Type.Normal) {
			return GetPanelDepth( panelSetting.gameObject );
		}
//        panelSetting.OpenedByEditorDepth = useEditorDepth;
		
		int newDepth = GetMaxDepth(panelSetting.PanelType) + DepthPlusToNewUI;
		
		// 紀錄目前最高的深度
		int maxDepth = newDepth;
		
		// 更新自己的深度
		SetPanelDepth(panelSetting.gameObject, newDepth);
		
		#region 更新Child的深度
		int depthDifference = newDepth - panelSetting.EditorDepth;
		foreach (ChildPanelInfo childPanelInfo in panelSetting.ChildPanelInfoList)
		{
			if (childPanelInfo.UIPanel == null)
			{
				//介面底下的子 UIPanel 遺失
//				Debug.LogError("ChildPanelInfo.UIPanel is null.");
				continue;
			}
			
			int panelDepth = childPanelInfo.EditorDepth + depthDifference;
			if (panelDepth > maxDepth)
			{
				//Debug.Log("<color=red>MaxDepthChange-panelName=" + panel.gameObject.name + ", panelDepth=" + maxDepth.ToString() + "</color>");
				maxDepth = panelDepth;
			}
			
			//Debug.Log("<color=red>Assign child depth(name = " + childPanelInfo.UIPanel.gameObject.name + ", depth=" + panelDepth.ToString() + ")</color>");
			SetPanelDepth(childPanelInfo.UIPanel.gameObject, panelDepth);
		}
        #endregion

        if (IsNeedRearrangeDepth(panelSetting.PanelType, maxDepth))
        {
            maxDepth = RearrangeDepth(panelSetting.PanelType);
            // 物件  depth 已修正，需重新尋找出他 排序後的深度
            UIPanel uiPanel = panelSetting.gameObject.GetComponent<UIPanel>();
            if (uiPanel != null)
            {
                newDepth = uiPanel.depth;
            }
        }
		
		//Debug.Log("<color=red>maxDepth=" + maxDepth.ToString() + "</color>");
		
		return newDepth;
//        if (useEditorDepth)
//        {
//            // 更新自己的深度
//            SetPanelDepth(panelSetting.gameObject, panelSetting.EditorDepth);
//
//            #region 更新Child的深度
//            foreach (ChildPanelInfo childPanelInfo in panelSetting.ChildPanelInfoList)
//            {
//                if (childPanelInfo.UIPanel == null)
//                {
//                    Debug.Log("<color=red>ChildPanelInfo.UIPanel is null." + "</color>");
//                    continue;
//                }
//
//                SetPanelDepth(childPanelInfo.UIPanel.gameObject, childPanelInfo.EditorDepth);
//            }
//            #endregion
//
//            return panelSetting.EditorDepth;
//        }
//        else
//        {
//            int newDepth = GetMaxDepth(panelSetting.PanelType) + DepthPlusToNewUI;
//
//            // 紀錄目前最高的深度
//            int maxDepth = newDepth;
//
//            // 更新自己的深度
//            SetPanelDepth(panelSetting.gameObject, newDepth);
//
//            #region 更新Child的深度
//            int depthDifference = newDepth - panelSetting.EditorDepth;
//            foreach (ChildPanelInfo childPanelInfo in panelSetting.ChildPanelInfoList)
//            {
//                if (childPanelInfo.UIPanel == null)
//                {
//                    Debug.Log("<color=red>ChildPanelInfo.UIPanel is null." + "</color>");
//                    continue;
//                }
//
//                int panelDepth = childPanelInfo.EditorDepth + depthDifference;
//                if (panelDepth > maxDepth)
//                {
//                    //Debug.Log("<color=red>MaxDepthChange-panelName=" + panel.gameObject.name + ", panelDepth=" + maxDepth.ToString() + "</color>");
//                    maxDepth = panelDepth;
//                }
//
//                //Debug.Log("<color=red>Assign child depth(name = " + childPanelInfo.UIPanel.gameObject.name + ", depth=" + panelDepth.ToString() + ")</color>");
//                SetPanelDepth(childPanelInfo.UIPanel.gameObject, panelDepth);
//            }
//            #endregion
//
//            if (IsNeedRearrangeDepth(panelSetting.PanelType, maxDepth))
//                maxDepth = RearrangeDepth(panelSetting.PanelType);
//
//            //Debug.Log("<color=red>maxDepth=" + maxDepth.ToString() + "</color>");
//
//            return newDepth;
//        }
    }

	public bool CheckUIIsOpening(string panelName){
		GameObject ui = JustGetUI(panelName);
		if(ui == null) return false;
		return ui.activeInHierarchy;
	}

    private bool IsNeedRearrangeDepth(PanelSetting.Type panelType ,int newMaxDepth)
    {
		// yoro canel noraml panel auto depth
//        if (panelType == PanelSetting.Type.Normal)
  //          return (newMaxDepth < (int)Depth.NormalMin || newMaxDepth > (int)Depth.NormalMax);
    //    else 
		if (panelType == PanelSetting.Type.Top)
            return (newMaxDepth < (int)Depth.TopMin || newMaxDepth > (int)Depth.TopMax);

        return false;
    }

    /// <summary>
    /// 重新排列深度
    /// </summary>
    /// <param name="panelType">需要重新排列的Panel類型</param>
    /// <returns>目前最高深度</returns>
    private int RearrangeDepth(PanelSetting.Type panelType)
    {
        // 取得同種類的Panel List
        List<UIPanel> panelList = new List<UIPanel>();
        foreach (KeyValuePair<string, GameObject> pair in _panelList)
        {
            if (pair.Value == null)
                continue;

            if (!pair.Value.activeSelf)
                continue;

            PanelSetting panelSetting = pair.Value.GetComponent<PanelSetting>();
            if (panelSetting == null)
                continue;

			// Yoro add for avoid normal panel
			if (panelSetting.PanelType == PanelSetting.Type.Normal)
				continue;
//            if (panelSetting.OpenedByEditorDepth)
//                continue;

            UIPanel uiPanel = pair.Value.GetComponent<UIPanel>();
            if (uiPanel == null)
                continue;

            if (panelSetting.PanelType == panelType)
                panelList.Add(uiPanel);
        }

        #region 排序並且給予新的深度
        //#if _DEBUG
        //        Debug.Log("<color=red>Before sort</color>");
        //        ShowPanelData();
        //#endif

        panelList.Sort(SortPanelByDepth);
        int depthCount = 0;
        if (panelType == PanelSetting.Type.Normal)
            depthCount = (int)Depth.NormalMin;
        else if (panelType == PanelSetting.Type.Top)
            depthCount = (int)Depth.TopMin;
        foreach (UIPanel panel in panelList)
        {
            PanelSetting panelSetting = panel.gameObject.GetComponent<PanelSetting>();
            if (panelSetting == null)
                continue;

            //panel.depth = depthCount;
            panel.depth = depthCount++;

            foreach (ChildPanelInfo info in panelSetting.ChildPanelInfoList)
            {
                if (info.UIPanel == null)
                    continue;

                info.UIPanel.depth = depthCount++;
            }
        }

        return depthCount;
        //#if _DEBUG
        //        Debug.Log("<color=red>After sort</color>");
        //        ShowPanelData();
        //#endif
        #endregion
    }

	private void CloseMutexPanel(GameObject gameObject)
    {
		//因為有可能在關閉介面時，開啟其他介面而使 _panelList 加入成員，所以先建立迴圈列表
//      foreach (KeyValuePair<string, GameObject> pair in _panelList)
		int idx = 0;
		KeyValuePair<string, GameObject>[] pairs = new KeyValuePair<string, GameObject>[_panelList.Count];
        foreach (KeyValuePair<string, GameObject> pair in _panelList)
		{
			pairs[idx] = pair;
			idx++;
		}
		foreach (KeyValuePair<string, GameObject> pair in pairs)
        {
            if (pair.Value == null)
                continue;

            if (pair.Key == gameObject.name)
                continue;

            if (!pair.Value.activeSelf)
                continue;

            PanelSetting panelSetting = pair.Value.GetComponent<PanelSetting>();
            if (panelSetting == null)
                continue;

            if (panelSetting.ExclusiveToMutex)
                continue;

            //NGUITools.SetActive(pair.Value, false);
			CloseUI(pair.Value);
        }
    }


    private void AddPanel(GameObject newPanelGameObject)
    {
		
		string panelName = newPanelGameObject.name;

        if (newPanelGameObject == null)
        {
			Debug.LogError("Add null panel to PanelManager.");
            return;
        }

		if (string.IsNullOrEmpty(panelName))
        {
			Debug.LogError("Add null panel name to PanelManager.");
            return;            
        }

		if (_panelList.ContainsKey(panelName))
        {
			Debug.LogError("Duplicate add panel to PanelManager.(name=" + panelName + ")");
            return;
        }

        PanelSetting panelSetting = newPanelGameObject.GetComponent<PanelSetting>();
        if (panelSetting == null)
        {
			Debug.LogError("Can't find PanelSetting component.(name=" + panelName + ")");
            return;
        }

        panelSetting.InitialDefaultSetting();

		_panelList.Add(panelName, newPanelGameObject);
    }

	private void DelPanel(GameObject newPanelGameObject)
	{
		
		string panelName = newPanelGameObject.name;
		
		if (newPanelGameObject == null)
		{
			Debug.LogError("Del null panel to PanelManager.");
			return;
		}
		
		if (string.IsNullOrEmpty(panelName))
		{
			Debug.LogError("Del null panel name to PanelManager.");
			return;            
		}
		
		if (_panelList.ContainsKey(panelName)== false )
		{
			Debug.LogError("Del no key panel to PanelManager.(name=" + panelName + ")");
			return;
		}
		_panelList.Remove( panelName );

	}
    private int GetPanelDepth(GameObject panelObject)
    {
        if (panelObject == null)
        {
			Debug.LogError("null reference to panelObject.");
            return 0;
        }

        UIPanel uiPanel = panelObject.GetComponent<UIPanel>();
        if (uiPanel == null)
        {
			Debug.LogError("Can't find UIPanel component.");
            return 0;
        }

        return uiPanel.depth;
    }

    /// <summary>
    /// 設定Panel的深度
    /// </summary>
    /// <param name="panelObject">此Panel的GameObject</param>
    /// <param name="depth">要設定的深度</param>
    private void SetPanelDepth(GameObject panelObject, int depth)
    {
        if (panelObject == null)
        {
			Debug.LogError("null reference to panelObject.");
            return;
        }

        UIPanel uiPanel = panelObject.GetComponent<UIPanel>();
        if (uiPanel == null)
        {
			Debug.LogError("Can't find UIPanel component.");
            return;
        }

        uiPanel.depth = depth;
    }



    private int GetMaxDepth(PanelSetting.Type type)
    {
        int maxDepth = 0;
        if (type == PanelSetting.Type.Normal)
            maxDepth = (int)Depth.NormalMin;
        else if (type == PanelSetting.Type.Top)
            maxDepth = (int)Depth.TopMin;

        foreach (KeyValuePair<string,GameObject> pair in _panelList)
        {
            if (pair.Value == null)
                continue;

            PanelSetting panelSetting = pair.Value.GetComponent<PanelSetting>();
            if (panelSetting == null)
                continue;
            if ((panelSetting.PanelType & type) == 0)
                continue;

//            if (panelSetting.gameObject.name == "Panel - TouchBlocker")
			if (panelSetting.gameObject.name == BlockerPanelName)
                continue;


            int depth = GetMaxDepthInChildren(pair.Value);
            if (depth > maxDepth)
            {
                //Debug.Log("<color=red>MaxDepthChange-panelName=" + pair.Value.name + ", panelDepth=" + depth.ToString() + "</color>");
                maxDepth = depth;
            }
        }

        return maxDepth;
    }

    private int GetMaxDepthInChildren(GameObject panelObject)
    {
        if (panelObject == null)
        {
            Debug.LogError("null reference to panelObject.");
            return 0;
        }

        UIPanel[] panelList = panelObject.GetComponentsInChildren<UIPanel>();
        if (panelList == null)
            return 0;

        int maxDepth = 0;
        foreach (UIPanel panel in panelList)
        {
            if (panel == null)
                continue;

            if (panel.depth > maxDepth)
                maxDepth = panel.depth;
        }

        return maxDepth;
    }

    private int SortPanelByDepth(UIPanel lhs, UIPanel rhs)
    {
        return lhs.depth.CompareTo(rhs.depth);
    }

    private void UpdateBlockerDepth()
    {
//		GameObject blocker = JustGetUI("Panel - TouchBlocker");
		GameObject blocker = JustGetUI(BlockerPanelName);

        if (blocker == null)
        {
			//dont need update
//            Debug.Log("<color=red>Can't find blocker.</color>");
            return;
        }

        UIPanel panel = blocker.GetComponent<UIPanel>();
        if (panel == null)
            return;

        bool closeBlocker = true;
        int maxDepth = 0;
        foreach (KeyValuePair<string, GameObject> pair in _panelList)
        {
            if (pair.Value == null)
                continue;

            if (pair.Value.activeSelf == false)
                continue;

            PanelSetting panelSetting = pair.Value.GetComponent<PanelSetting>();
            if (panelSetting == null)
                continue;

            if (panelSetting.ShowBlocker == false)
                continue;

            //Debug.Log("<color=red>UpdateBlockerDepth - ShowBlocker Name=" + pair.Value.name + "</color>");
            closeBlocker = false;

            int panelDepth = GetPanelDepth(panelSetting.gameObject);
            if (panelDepth> maxDepth)
                maxDepth = panelDepth; 
        }

        //Debug.Log("<color=red>closeBlocker=" + closeBlocker.ToString() + "</color>");
        if (closeBlocker)
            CloseUI(blocker);
        else
            panel.depth = maxDepth - BlockerDepthDifference;
    }

#if _DEBUG
    void ShowPanelData()
    {
        Debug.Log("<color=red>Show _panelList content.</color>");
        foreach (KeyValuePair<string, GameObject> pair in _panelList)
        {
            if (pair.Value == null)
                continue;

            if (!pair.Value.activeSelf)
                continue;

            PanelSetting panelSetting = pair.Value.GetComponent<PanelSetting>();
            if (!panelSetting)
                continue;

            if (string.IsNullOrEmpty(panelSetting.name))
                continue;

            Debug.Log("<color=red>PanelName=" + panelSetting.name + ", Depth=" + GetPanelDepth(panelSetting.gameObject).ToString() + "</color>");
        }
    }
#endif

    /// <summary>
    /// 開啟新UI時 預設要增加的深度值
    /// </summary>
    private const int DepthPlusToNewUI = 2;
    /// <summary>
    /// Blocker與現在開啟的UI depth差
    /// </summary>
    private const int BlockerDepthDifference = 1;

    private Dictionary<string, GameObject> _panelList = new Dictionary<string, GameObject>(StringComparer.Ordinal);
}
