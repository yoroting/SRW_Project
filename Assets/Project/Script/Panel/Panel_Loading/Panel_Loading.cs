using UnityEngine;
using System.Collections;

public class Panel_Loading : MonoBehaviour {
	public const string Name =	"Panel_Loading";
	public GameObject lblName;

	void OnEnable()
	{
		if (lblName != null) {
			MyTool.SetLabelText( lblName , ""  );
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowStoryName()
	{
		if (lblName != null) {
			MyTool.SetLabelText( lblName , MyTool.GetStoryName( GameDataManager.Instance.nStageID )  );
		}

	}
}
