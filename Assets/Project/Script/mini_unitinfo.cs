using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mini_unitinfo : MonoBehaviour {

    public UILabel m_lblName;
    public UITexture m_txFace;


    public UILabel m_lblLv;
    public UILabel m_lblCp;

    public item_param m_mar;
    public item_param m_hp;
    public item_param m_def;

    

    cUnitData m_pData;

    public GameObject BuffGrid;
    // Use this for initialization
    void Start () {
        UIEventListener.Get(this.gameObject).onClick += OnCloseClick; // click to close
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetData(cUnitData pUnitData)
    {
        m_pData = pUnitData;
        Reload();
    }

    public void Reload()
    {
        string sName = MyTool.GetCharName(m_pData.n_CharID);
        m_lblName.text = sName;

        m_txFace.mainTexture = MyTool.GetCharTexture(m_pData.n_FaceID);

        m_lblLv.text = m_pData.n_Lv.ToString();
        m_lblCp.text = m_pData.n_CP.ToString();

        m_mar.SetUnit(m_pData);
        m_hp.SetUnit(m_pData);
        m_def.SetUnit(m_pData);


        // Buff

        // set buff 
        MyTool.DestoryGridItem(BuffGrid);

        foreach (KeyValuePair<int, cBuffData> pair in m_pData.Buffs.Pool)
        {
            if (pair.Value.nTime == 0) // never 0  ( 被動能力 buff)
                continue;
            if (pair.Value.tableData.n_HIDE > 0)
                continue;

            GameObject go = ResourcesManager.CreatePrefabGameObj(BuffGrid, "Prefab/Bufficon");
            if (go == null)
                continue;


            MyTool.SetBuffIcon(go, pair.Value.nID, pair.Value.GetUITime() , pair.Value.nNum );
           
        }

        MyTool.ResetScrollView(BuffGrid );
        //==============
        //UIGrid grid = BuffGrid.GetComponent<UIGrid>();
        //if (grid != null)
        //{
        //    grid.repositionNow = true;
        //    grid.Reposition();
        //}

    }


    public void SetPostType( bool bLeft = false , bool bTop = false) // 設定顯示方式 （要變換座標）
    {
      //  float fRatio = (float)Screen.width / (float)Config.WIDTH;
        // screen width
        int scw = Config.WIDTH;
        int sch = Config.HEIGHT;

        Vector3 vFrom = this.transform.localPosition;
        Vector3 vTar = this.transform.localPosition;

        UIWidget widget = this.gameObject.GetComponent<UIWidget>();
        if ( widget != null ) {

            int w = widget.width;
            int h = widget.height;
            //w = (int)(widget.width * fRatio );
            //h = (int)(widget.height * fRatio) ;

            if (bLeft == false) // 右
            {
                vFrom.x = (scw + w) / 2;
                vTar.x = (scw - w ) / 2;                
            }
            else
            {
                vFrom.x = -1 * (scw + w) / 2;
                vTar.x  = -1 * (scw - w) / 2;
            }

            if (bTop == false)
            {
                vFrom.y = vTar.y = -1 * (sch - h) / 2;
            }
            else {
                vFrom.y = vTar.y = (sch -h) / 2;
            }
        }
        //vFrom.x /= fRatio;
        //vFrom.y /= fRatio;
        //vTar.x /= fRatio;
        //vTar.y /= fRatio;

        this.transform.localPosition = vFrom;
        TweenPosition.Begin(this.gameObject, 0.2f , vTar);


    }

    void OnCloseClick(GameObject go)
    {
        go.SetActive(false);
    }
}
