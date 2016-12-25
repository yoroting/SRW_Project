using UnityEngine;
using System.Collections;

public class CMD_BTN : MonoBehaviour {

   // public UIWidget wiget;
    // Use this for initialization
    void Start () {
        //    wiget = this.GetComponent<UIWidget>();
      //  ReSize();
    }


    void OnEnable()
    {
        ReSize();

        UIButton b = this.gameObject.GetComponent<UIButton>();
        if (b != null)
        {
            b.isEnabled = true; // disable btn
        }
    }

    void OnDisable()
    {
        //     UIEventListener.Get(obj).onClick = null;
       // transform.localScale = Vector3.one;
       // transform.localRotation = Quaternion.identity;

    }

    // Update is called once per frame
    void Update () {
	
	}

    public void ReSize()
    {
        UIWidget wiget = GetComponent<UIWidget>();
        if (wiget != null)
        {
            wiget.width = 150;  // size fix to correct wh
            wiget.height = 75;
        }
        //Vector3 v = this.transform.localScale;
        //v = new Vector3(1.0f, 1.0f, 1.0f);
        transform.localScale = Vector3.one;

        transform.localRotation = Quaternion.identity;
    }
}
