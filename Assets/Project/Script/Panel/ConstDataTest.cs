using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConstDataTest : MonoBehaviour {

	private UIPopupList popList;

	void Awake()
	{
		popList = GetComponent<UIPopupList>();
		if(popList == null)
			return;

		popList.Clear();
		List<DataTable> tableList = ConstDataManager.Instance.ListTables();
		foreach(DataTable table in tableList)
		{
			popList.AddItem(table.Name);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
