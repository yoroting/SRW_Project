using UnityEngine;
using System.Collections;

public class Panel_Cheat : MonoBehaviour {

	public const string Name = "Panel_Cheat";

	cUnitData pData;

	public GameObject CloseBtn;            
	public GameObject OkBtn;           // God Mode
	//

	public GameObject IvValueobj;
	public GameObject ExpAddobj;
	public GameObject LvBtn;           // lv btn

	public GameObject IntValueobj;
	public GameObject IntAddobj;
	public GameObject IntDelobj;

	public GameObject ExtValueobj;
	public GameObject ExtAddobj;
	public GameObject EXtDelobj;

	public GameObject HpValueobj;
	public GameObject HpAddobj;
	public GameObject HpDelobj;

	public GameObject MpValueobj;
	public GameObject MpAddobj;
	public GameObject MpDelobj;

	public GameObject SpValueobj;
	public GameObject SpAddobj;
	public GameObject SpDelobj;

	public GameObject CpValueobj;
	public GameObject CpAddobj;
	public GameObject CpDelobj;
		

	public GameObject SchValueobj;
	public GameObject SchAddobj;
	public GameObject SchDelobj;

	public GameObject BuffValueobj;
	public GameObject BuffAddobj;
	public GameObject BuffDelobj;

	public GameObject Item1Valueobj;
	public GameObject Item1Addobj;
	public GameObject Item1Delobj;

    public GameObject SAIPoplist;           // sel Event
    public GameObject CAIPoplist;           // sel Event

    public GameObject ReactBtn;           // God Mode
	// Use this for initialization
	void Start () {
	
		UIEventListener.Get(CloseBtn).onClick += OnCloseClick; 
		UIEventListener.Get(OkBtn).onClick += OnOkClick; 

		UIEventListener.Get(LvBtn).onClick += OnSetLvClick; 


		UIEventListener.Get(IntAddobj).onClick += OnAddIntClick; 
		UIEventListener.Get(IntDelobj).onClick += OnDelIntClick; 

		UIEventListener.Get(ExtAddobj).onClick += OnAddExtClick; 
		UIEventListener.Get(EXtDelobj).onClick += OnDelExtClick; 

		UIEventListener.Get(HpAddobj).onClick += OnAddHpClick; 
		UIEventListener.Get(HpDelobj).onClick += OnDelHpClick; 

		UIEventListener.Get(MpAddobj).onClick += OnAddMpClick; 
		UIEventListener.Get(MpDelobj).onClick += OnDelMpClick; 

		UIEventListener.Get(SpAddobj).onClick += OnAddSpClick; 
		UIEventListener.Get(SpDelobj).onClick += OnDelSpClick; 

		UIEventListener.Get(CpAddobj).onClick += OnAddCpClick; 
		UIEventListener.Get(CpDelobj).onClick += OnDelCpClick; 


		UIEventListener.Get(SchAddobj).onClick += OnAddSchClick; 
		UIEventListener.Get(SchDelobj).onClick += OnDelSchClick; 

		UIEventListener.Get(BuffAddobj).onClick += OnAddBuffClick; 
		UIEventListener.Get(BuffDelobj).onClick += OnDelBuffClick; 

		UIEventListener.Get(Item1Addobj).onClick += OnAddItem1Click; 
		UIEventListener.Get(Item1Delobj).onClick += OnDelItem1Click; 


		UIEventListener.Get(ReactBtn).onClick += OnReActClick;


        UIPopupList popSAI = SAIPoplist.GetComponent<UIPopupList>();
        if (popSAI != null)
        {
            //添加触发事件
            //	EventDelegate.Add (popList.onChange, label.SetCurrentSelection);
            EventDelegate.Add(popSAI.onChange, SAIComboboxChange);

            var valuesAsArray = _AI_SEARCH.GetValues(typeof(_AI_SEARCH));
            // _AI_SEARCH._NORMAL;
            popSAI.Clear();
            foreach (_AI_SEARCH sai in valuesAsArray )
            {
                popSAI.AddItem(sai.ToString() , sai );
            }            
        }

        UIPopupList popCAI = CAIPoplist.GetComponent<UIPopupList>();
        if (popCAI != null)
        {
            //添加触发事件
            //	EventDelegate.Add (popList.onChange, label.SetCurrentSelection);
            EventDelegate.Add(popCAI.onChange, CAIComboboxChange);

            var valuesAsArray = _AI_COMBO.GetValues(typeof(_AI_COMBO));
            // _AI_SEARCH._NORMAL;
            popCAI.Clear();
            foreach (_AI_COMBO cai in valuesAsArray)
            {
                popCAI.AddItem(cai.ToString(), cai);
            }
        }
    }

	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {

        //		UIToggle god = GodChk.GetComponent<UIToggle> ();
        //		god.value =Config.GOD;
        //		UIToggle kill = KillChk.GetComponent<UIToggle> ();
        //		kill.value =Config.KILL_MODE;
        //		UIToggle ai = KillChk.GetComponent<UIToggle> ();
        //		ai.value =Config.MOBAI;
        //		
        //		UIInput min = MoneyInput.GetComponent<UIInput> ();
        //		min.value = GameDataManager.Instance.nMoney.ToString();

       
    }
	public void SetData( cUnitData data ){
		pData = data;
		if(  pData == null )
			return;
		pData.UpdateAllAttr();
		pData.UpdateAttr();


		UIInput LvInput = IvValueobj.GetComponent<UIInput>();
		if( LvInput != null ){
			LvInput.value = pData.n_Lv.ToString();
		}

		UIInput ExpInput = ExpAddobj.GetComponent<UIInput>();
		if( ExpInput != null ){
			ExpInput.value = pData.n_EXP.ToString();
		}

		//MyTool.SetLabelInt( IvValueobj , pData.n_Lv );
		//MyTool.SetLabelInt( ExpAddobj , pData.n_EXP );

		MyTool.SetLabelText( HpValueobj , string.Format( "{0}%" , (int)(pData.GetHpPercent()*100 ) )  );

		MyTool.SetLabelInt( MpValueobj , pData.n_MP );
		MyTool.SetLabelInt( SpValueobj , pData.n_SP );
		MyTool.SetLabelInt( CpValueobj , pData.n_CP );

		// get int lv
		MyTool.SetLabelInt( IntValueobj , pData.GetIntSchLv(  ) );

		// get ext lv
		MyTool.SetLabelInt( ExtValueobj , pData.GetExtSchLv(  ) );

        UIPopupList saiList = SAIPoplist.GetComponent<UIPopupList>();
        if (saiList != null)
        {
            saiList.value = pData.eSearchAI.ToString();
           // saiList.value = pData.eSearchAI;           
        }

        UIPopupList caiList = CAIPoplist.GetComponent<UIPopupList>();
        if (caiList != null)
        {
            caiList.value = pData.eComboAI.ToString();
            // saiList.value = pData.eSearchAI;           
        }
    }



	void OnCloseClick(GameObject go)
	{
		PanelManager.Instance.CloseUI( Name );
	}

	void OnOkClick(GameObject go)
	{

	}

	void OnSetLvClick(GameObject go)
	{
		int nLv = pData.n_Lv;
		int nExp = pData.n_EXP;
		UIInput LvInput = IvValueobj.GetComponent<UIInput>();
		if( LvInput != null ){
			if( int.TryParse(  LvInput.value , out nLv ) ){


			}
		}
		UIInput ExpInput = ExpAddobj.GetComponent<UIInput>();
		if( ExpInput != null ){		
			if( int.TryParse(  ExpInput.value , out nExp ) ){
				
				
			}
		}
		///===============
		/// 
		pData.SetLevel ( nLv  );
		pData.n_EXP = nExp;
	}
	

	void OnAddIntClick(GameObject go)
	{
		int nlv =  pData.GetIntSchLv();
		int nTo = MyTool.ClampInt( nlv+1 , 0 , 10 );
		pData.LearnSchool(  pData.GetIntSchID() , nTo  );

		SetData( pData );
		//MyTool.SetLabelInt( IntValueobj , pData.GetIntSchLv(  ) );
	}
	void OnDelIntClick(GameObject go)
	{
		int nlv =  pData.GetIntSchLv();
		int nTo = MyTool.ClampInt( nlv-1 , 0 , 10 );
		pData.LearnSchool(  pData.GetIntSchID() , nTo  );

		SetData( pData );
		//MyTool.SetLabelInt( IntValueobj , pData.GetIntSchLv(  ) );
	}
	void OnAddExtClick(GameObject go)
	{
		int nlv =  pData.GetExtSchLv();
		int nTo = MyTool.ClampInt( nlv+1 , 0 , 10 );
		pData.LearnSchool(  pData.GetExtSchID() , nTo  );

		SetData( pData );
		//MyTool.SetLabelInt( ExtValueobj , pData.GetExtSchLv(  ) );
	}
	void OnDelExtClick(GameObject go)
	{
		int nlv =  pData.GetExtSchLv();
		int nTo = MyTool.ClampInt( nlv-1 , 0 , 10 );
		pData.LearnSchool(  pData.GetExtSchID() , nTo  );

		SetData( pData );
		//MyTool.SetLabelInt( ExtValueobj , pData.GetExtSchLv(  ) );
	}

	void OnAddHpClick(GameObject go)
	{
		int nHp = (int)(pData.GetMaxHP() *0.1f);
		pData.AddHp( nHp );

		SetData( pData );
		//MyTool.SetLabelText( HpValueobj , string.Format( "{0}%" , (int)(pData.GetHpPercent()*100 ) ) );
	}

	void OnDelHpClick(GameObject go)
	{
		int nHp = (int)(pData.GetMaxHP() *0.1f);
		pData.AddHp( -nHp );
		if (pData.n_HP <= 0) {
			pData.n_HP = 1;		// no kill unit
		}
		SetData( pData );
		//MyTool.SetLabelText( HpValueobj , string.Format( "{0}%" , (int)(pData.GetHpPercent()*100 ) ) );
	}

	void OnAddMpClick(GameObject go)
	{
		pData.AddMp( pData.GetMaxMP()  );
		SetData( pData );
		//MyTool.SetLabelInt( MpValueobj , pData.n_MP );
	}
	
	void OnDelMpClick(GameObject go)
	{
		pData.AddMp( -pData.GetMaxMP()  );

		SetData( pData );
		//MyTool.SetLabelInt( MpValueobj , pData.n_MP );
	}

	void OnAddSpClick(GameObject go)
	{
		pData.AddSp( pData.GetMaxSP()  );
		SetData( pData );
	}
	
	void OnDelSpClick(GameObject go)
	{
		pData.AddSp( -pData.GetMaxSP()  );		
		SetData( pData );
	}

	void OnAddCpClick(GameObject go)
	{
		pData.AddCp( 1  );
		SetData( pData );
	}
	
	void OnDelCpClick(GameObject go)
	{
		pData.AddCp( -1  );		
		SetData( pData );
	}

	void OnAddSchClick(GameObject go)
	{
		UIInput SchInput = SchValueobj.GetComponent<UIInput>();
		if( SchInput != null ){
			int nSch = 0;
			if( int.TryParse(  SchInput.value , out nSch ) ){
				SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL> ( nSch );
				if( sch != null ){
					pData.LearnSchool( nSch , 5 );

					pData.ActiveSchool( nSch );
				}
			}
		}
	}
	
	void OnDelSchClick(GameObject go)
	{
		UIInput SchInput = SchValueobj.GetComponent<UIInput>();
		if( SchInput != null ){
			int nSch = 0;
			if( int.TryParse(  SchInput.value , out nSch ) ){
				SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL> ( nSch );
				if( sch != null ){
					pData.ForgetSchool( nSch );
					
				}
			}
		}
	}

	void OnAddBuffClick(GameObject go)
	{
		UIInput buffInput = BuffValueobj.GetComponent<UIInput>();
		if( buffInput != null ){
			int id = 0;
			if( int.TryParse(  buffInput.value , out id ) ){
				pData.Buffs.AddBuff( id , 0 , 0 , 0 ); 
			}
		}
	}
	
	void OnDelBuffClick(GameObject go)
	{
		UIInput buffInput = BuffValueobj.GetComponent<UIInput>();
		if( buffInput != null ){
			int id = 0;
			if( int.TryParse(  buffInput.value , out id ) ){
				pData.Buffs.DelBuff ( id ); 
			}
		}		
	}

	void OnAddItem1Click(GameObject go)
	{
		UIInput itemInput = Item1Valueobj.GetComponent<UIInput>();
		if( itemInput != null ){
			int id = 0;
			if( int.TryParse(  itemInput.value , out id ) ){
				pData.EquipItem( _ITEMSLOT._SLOT0 , id ) ;
				//pData.Buffs.AddBuff( id , 0 , 0 , 0 ); 
			}
		}
	}
	
	void OnDelItem1Click(GameObject go)
	{
		UIInput itemInput = Item1Valueobj.GetComponent<UIInput>();
		if( itemInput != null ){
			int id = 0;
			if( int.TryParse(  itemInput.value , out id ) ){
				pData.EquipItem( _ITEMSLOT._SLOT0 , 0 ) ;
			}
		}
	}

	void OnReActClick(GameObject go)
	{
		pData.AddActionTime (1);
	}

    //
    void SAIComboboxChange()
    {
        UIPopupList popList = SAIPoplist.GetComponent<UIPopupList>();
        if (popList != null)
        {
            //			string s = popList.value;
            if (popList.data != null)
            {
                _AI_SEARCH sai = (_AI_SEARCH)popList.data;

                pData.eSearchAI = sai;
            }
        }
    }

    void CAIComboboxChange()
    {
        UIPopupList popList = CAIPoplist.GetComponent<UIPopupList>();
        if (popList != null)
        {
            //			string s = popList.value;
            if (popList.data != null)
            {
                _AI_COMBO cai = (_AI_COMBO)popList.data;
                pData.eComboAI = cai;
            }
        }
    }

}

