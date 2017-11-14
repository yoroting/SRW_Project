using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


public class Panel_CreateName : MonoBehaviour {

    public const string Name = "Panel_CreateName";

    public const int HALF_WIDTH = 1;
    public const int FULL_WIDTH = 2;

    public const string HALF_PATTERN = @"^[\u0021-\u007E]+$";

    //CHINESE PATTERN
    public const string CHINESE_PATTERN = @"^[\u2E80-\u2FDF\u3100-\u312F\u3400-\u4DBF\u4E00-\u9FFF\uF900-\uFAFF]+$";

    //JAPANESE PATTERN
    public const string JAPANESE_PATTERN = @"^[\u3040-\u30FF\u31F0-\u31FF]+$";

    //KOREAN PATTERN
    public const string KOREAN_PATTERN = @"^[\u1100-\u11FF\u3130-\u318F\uAC00-\uD7AF]+$";


    public GameObject OkBtn;
    public GameObject CloseBtn;    

    public GameObject FirstInput;           // 輸入姓 input
    public GameObject NameInput;           // 輸入名 input

    // Use this for initialization
    void Start()
    {
        // setting in unity edit
        //   UIEventListener.Get(OkBtn).onClick = OnOkClick;
        //   UIEventListener.Get(CloseBtn).onClick = OnCloseClick;

        UIInput finput = FirstInput.GetComponent<UIInput>();
        if (finput != null)
        {
         //   EventDelegate.Add(finput.onChange, this.OnInputChange);
        }
    

     
    }
	
	// Update is called once per frame
	void Update () {
        //UIInput finput = FirstInput.GetComponent<UIInput>();
        //if (finput != null)
        //{
        //    if (finput.value.Length > 2) {
        //        finput.value = finput.value.Substring(0, 2);
        //    }
        //}
        //UIInput ninput = NameInput.GetComponent<UIInput>();
        //if (ninput != null)
        //{
        //    if (ninput.value.Length > 3)
        //    {
        //        ninput.value = ninput.value.Substring(0, 3);
        //    }
        //}

    }

    private void OnEnable()
    {
        Init();
    }


    void Init()
    {
        UIInput finput = FirstInput.GetComponent<UIInput>();
        if (finput != null) {
            finput.value = Config.DefaultPlayerFirst;
        }
        UIInput ninput = NameInput.GetComponent<UIInput>();
        if (ninput != null)
        {
            ninput.value = Config.DefaultPlayerName;
        }
     

    }

    private void OnInputChange()
    {
        int characterLimit = 4;
        UIInput finput = FirstInput.GetComponent<UIInput>();
        string str = finput.value;
        if (characterLimit > 0 && this.GetStringByteLength(str) > characterLimit)
        {
            do
            {
                str = str.Substring(0, str.Length - 1);
            } while (this.GetStringByteLength(str) > characterLimit);
            finput.value = str;
        }

    }
    private int GetStringByteLength(string str)
    {
        byte[] bytestr = System.Text.Encoding.Default.GetBytes(str);
        return bytestr.Length;
    }

    public void OnOkClick(GameObject go)
    {
        UIInput finput = FirstInput.GetComponent<UIInput>();
        if (finput != null)
        {
            string ErrorMessage = CheckInputNameHandler(finput.value);
            if (ErrorMessage != "") {
                finput.value = "";
                Panel_CheckBox.MessageBox( ErrorMessage );
                return;
            }

            Config.PlayerFirst = finput.value;
        }
        UIInput ninput = NameInput.GetComponent<UIInput>();
        if (ninput != null)
        {
            string ErrorMessage = CheckInputNameHandler(ninput.value);
            if (ErrorMessage != "")
            {
                ninput.value = "";
                Panel_CheckBox.MessageBox(ErrorMessage);
                return;
            }
            Config.PlayerName = ninput.value;
        }
        // 進入第一關
        MainUIPanel panel = MyTool.GetPanel<MainUIPanel>( MainUIPanel.Name );

        if (panel != null) {
            panel.StartGame();
        }

        PanelManager.Instance.CloseUI(Name);
    }

    public void OnCloseClick(GameObject go)
    {        
        // 關閉UI
        PanelManager.Instance.CloseUI(Name);
        GameSystem.BtnSound(1);
    }


    #region check special char
    private string CheckInputNameHandler(string str) // return ErrorMessage
    {
        char[] strArray = str.ToCharArray();

        foreach (char c in strArray)
        {
            //check language type
            if (GetCharWidthHandler(c) != HALF_WIDTH
                && !ChineseCheckHandelr(c)
                && !JapaneseCheckHandelr(c)
                && !KoreanCheckHandelr(c))
                return "輸入文字中有不被接受的字元[1fe6df]$V1[-]，請重新輸入！".Replace("$V1", "");
            else
            {
                if (!IsIllegalText(c))
                    return "輸入文字中有不被接受的字元[1fe6df]$V1[-]，請重新輸入！".Replace("$V1", c.ToString());
            }
        }

        return "";
    }

    private int GetCharWidthHandler(char c)
    {
        string str = c.ToString();

        Regex regHalf = new Regex(HALF_PATTERN);

        if (regHalf.IsMatch(str))
        {
            return HALF_WIDTH;
        }
        return FULL_WIDTH;
    }

    private bool ChineseCheckHandelr(char c)
    {
        string str = c.ToString();

        bool isChinese = false;

        Regex regChinese = new Regex(CHINESE_PATTERN);

        if (regChinese.IsMatch(str)) isChinese = true;

        return isChinese;
    }

    private bool JapaneseCheckHandelr(char c)
    {
        string str = c.ToString();

        bool isJapanese = false;

        Regex regJapanese = new Regex(JAPANESE_PATTERN);

        if (regJapanese.IsMatch(str)) isJapanese = true;

        return isJapanese;
    }

    private bool KoreanCheckHandelr(char c)
    {
        string str = c.ToString();

        bool isKorean = false;

        Regex regKorean = new Regex(KOREAN_PATTERN);

        if (regKorean.IsMatch(str)) isKorean = true;

        return isKorean;
    }

    private bool IsIllegalText(char c)
    {
        string str = c.ToString();

        DataTable tbl = ConstDataManager.Instance.GetTable< TEXT_FILTER >();
        if (tbl != null)
        {
            foreach (TEXT_FILTER filter in tbl)
            {                
                if (str.Contains(filter.s_TEXT))
                    return false;
            }
        }
                
                return true;
    }
    #endregion

}


