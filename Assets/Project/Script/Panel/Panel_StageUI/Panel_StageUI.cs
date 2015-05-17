using UnityEngine;
using System.Collections;
using MYGRIDS;


public class Panel_StageUI : MonoBehaviour {

	public GameObject BackGroundObj; // back ground
	public GameObject TilePlaneObj; // plane of all tiles sprite

	public bool ReadCompleted ;

	cMyGrids	Grids;				// main grids

	void Awake( ){
		// UI Event
		UIEventListener.Get(BackGroundObj).onClick += OnBackGroundClick;
	
		Grids = new cMyGrids ();
		ReadCompleted = false;
	}

	// Use this for initialization
	void Start () {
		// load const data
		SCENE_NAME scn = ConstDataManager.Instance.GetRow<SCENE_NAME> (  GameDataManager.Instance.nStageID );
		if (scn == null)
			return;
		string filename = "Assets\\StreamingAssets\\scn\\"+scn.s_MODLE_ID+".scn";

		bool bRes = Grids.Load( filename );
		Grids.SetPixelWH (Config.TileW, Config.TileH);  // re size
		;
		for( int i = -Grids.hW ; i <= Grids.hW ; i++ ){
			for( int j = -Grids.hW ; j <= Grids.hW ; j++ )
			{
				_TILE t = Grids.GetValue( i , j  );

				GameObject cell = GetTileCellPrefab( t ); 
				float x =0, y =0;
				Grids.GetRealXY(ref x , ref y , new iVec2( i , j ) );

				Vector3 pos = new Vector3( x , y , 0 );
				if( cell != null ){
					cell.transform.localPosition = pos; 

					//cell.name = 
				}
			}
		}


//		Grids.hW;
//		Grids.hH;
//		Grids.MaxW;
//		Grids.MaxH;
		// start to create sprite

		ReadCompleted = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy()
	{
		ReadCompleted = false;
	}


	void OnBackGroundClick(GameObject go)
	{


	}

	GameObject GetTileCellPrefab( _TILE t )
	{
		SCENE_TILE tile = ConstDataManager.Instance.GetRow<SCENE_TILE> ((int)t);
		if (tile != null) {
			//tile.s_FILE_NAME;
			GameObject cell = ResourcesManager.CreatePrefabGameObj(TilePlaneObj, "Prefab/TileCell");
			UISprite sprite = cell.GetComponent<UISprite>(); 
			if( sprite != null )
			{
				sprite.spriteName = tile.s_FILE_NAME;

			}
			return cell;
		}
			//_TILE._GREEN

		return null;
	}
}
