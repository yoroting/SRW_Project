using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UIPanel))]
public class PanelSetting : MonoBehaviour
{
    public enum Type
    {
        Normal = 1 << 0,
        Top = 1 << 1,
    }

    public Type PanelType = Type.Normal;
    public bool Mutex = true;
    public bool ExclusiveToMutex = false;
    public bool ShowBlocker = true;

    /// <summary>
    /// 開啟介面時，是否是使用編輯器的深度
    /// </summary>
//    public bool OpenedByEditorDepth { get; set; }
    /// <summary>
    /// 記錄編輯器填的深度
    /// </summary>
    public int EditorDepth { get { return _editorDepth; } }
    /// <summary>
    /// 記錄編輯器填的類型
    /// </summary>
    public Type DefaultPanelType { get { return _defaultPanelType; } }
    /// <summary>
    /// 記錄自己Child的UIPanel資訊
    /// </summary>
    public List<ChildPanelInfo> ChildPanelInfoList { get { return _childPanelInfoList; } }


    void Start()
    {
    }

    /// <summary>
    /// 將Panel的資料儲存起來(剛開始的時候會做一次,因為Panel的資料有可能會被動態改變)
    /// </summary>
    public void InitialDefaultSetting()
    {
        UIPanel uiPanel = GetComponent<UIPanel>();
        if (uiPanel == null)
        {
            Debug.LogError("<color=red>Can't find UIPanel component.(name=" + gameObject.name + ")</color>");
            return;
        }
        //Debug.Log("<color=red>InitialDefaultSetting.(Name=" + gameObject.name + ", depth="+ depth + ")</color>");

        _editorDepth = uiPanel.depth;
        _defaultPanelType = PanelType;

        UIPanel[] uiPanelList = GetComponentsInChildren<UIPanel>(true);
        if (uiPanelList == null)
        {
            Debug.Log("<color=red>Initial panel default setting is fail.</color>");
            return;
        }

        foreach (UIPanel panel in uiPanelList)
        {
            if (panel.gameObject == gameObject)
                continue;

            ChildPanelInfo info = new ChildPanelInfo();
            info.EditorDepth = panel.depth;
            info.UIPanel = panel;

            _childPanelInfoList.Add(info);
        }

        _childPanelInfoList.Sort(SortChildPanel);
    }

    private int SortChildPanel(ChildPanelInfo lhs, ChildPanelInfo rhs)
    {
        return lhs.EditorDepth.CompareTo(rhs.EditorDepth);
    }

#if _DEBUG
    private void ShowPanelData()
    {
        Debug.Log("<color=red>Panel name=" + gameObject.name + "</color>");
        foreach (ChildPanelInfo info in _childPanelInfoList)
        {
            Debug.Log("<color=red>ChildPanelInfo name=" + info.UIPanel.gameObject.name + ", EditorDepth= " + info.EditorDepth + "</color>");
        }
    }
#endif

    private int _editorDepth = 0;
    private Type _defaultPanelType = Type.Normal;
    private List<ChildPanelInfo> _childPanelInfoList = new List<ChildPanelInfo>();
}
