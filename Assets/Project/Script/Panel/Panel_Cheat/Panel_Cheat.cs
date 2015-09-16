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

	// Use this for initialization
	void Start () {
	
		UIEventListener.Get(CloseBtn).onClick += OnCloseClick; 
		UIEventListener.Get(OkBtn).onClick += OnOkClick; 

		UIEventListener.Get(IntAddobj).onClick += OnAddIntClick; 
		UIEventListener.Get(IntDelobj).onClick += OnDelIntClick; 

		UIEventListener.Get(ExtAddobj).onClick += OnAddExtClick; 
		UIEventListener.Get(EXtDelobj).onClick += OnDelExtClick; 

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
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable () {
		
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
	}



	void OnCloseClick(GameObject go)
	{
		PanelManager.Instance.CloseUI( Name );
	}

	void OnOkClick(GameObject go)
	{

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
		pData.AddCp( pData.GetMaxCP()  );
		SetData( pData );
	}
	
	void OnDelCpClick(GameObject go)
	{
		pData.AddCp( -pData.GetMaxCP()  );		
		SetData( pData );
	}

	void OnAddSchClick(GameObject go)
	{
	
	}
	
	void OnDelSchClick(GameObject go)
	{
	
	}

	void OnAddBuffClick(GameObject go)
	{
		
	}
	
	void OnDelBuffClick(GameObject go)
	{
		
	}

	void OnAddItem1Click(GameObject go)
	{
		
	}
	
	void OnDelItem1Click(GameObject go)
	{
		
	}


}

