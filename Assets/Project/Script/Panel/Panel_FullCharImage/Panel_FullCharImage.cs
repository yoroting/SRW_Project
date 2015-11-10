using UnityEngine;
using System.Collections;

public class Panel_FullCharImage : MonoBehaviour {

    public const string Name = "Panel_FullCharImage";

    public GameObject CloseBtn;

    public GameObject FaceObj;

    // Use this for initialization
    void Start () {
        UIEventListener.Get(CloseBtn).onClick += OnCloseClick;

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetChar(int nCharID)
    {
        if (FaceObj == null)
            return;

        CHARS cdata = ConstDataManager.Instance.GetRow<CHARS>(nCharID );
        if (cdata == null)
            return;

        UITexture tex = FaceObj.GetComponent<UITexture>();
        if (tex != null)
        {
            tex.mainTexture = MyTool.GetCharTexture( nCharID , 1 );
        }
    }


    void OnCloseClick(GameObject go)
    {
        PanelManager.Instance.CloseUI(Name);
    }

}
