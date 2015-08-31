using UnityEngine;
using System.Collections;

public class CMD_BTN : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UIWidget wi = this.GetComponent<UIWidget >();
		if( wi != null ){
			wi.width = 150;  // size fix to correct wh
			wi.height = 75;			
		}
		Vector3 v = this.transform.localScale;
		v = new Vector3 ( 1.0f , 1.0f , 1.0f );
		this.transform.localScale = v;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
