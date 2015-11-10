using UnityEngine;
using System.Collections;

public class Skill_Simple : MonoBehaviour {

	public GameObject lblName;

	public int nID;
    public int nIndex;
	public int nType; // 0- ability , 1 - skill , 2- Item , 3- fate

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnEnable()
    {
        nID = 0;
        nIndex = -1;
        nType = 0;
    }


    public void ReSize()
    {

        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }


}
