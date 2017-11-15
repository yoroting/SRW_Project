using UnityEngine;
using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;  

public class TextLengthController : MonoBehaviour {

	public const int HALF_WIDTH = 1;
	public const int FULL_WIDTH = 2;
	
	public const string HALF_PATTERN =  @"^[\u0021-\u007E]+$";
	
	//CHINESE PATTERN
	public const string CHINESE_PATTERN = @"^[\u2E80-\u2FDF\u3100-\u312F\u3400-\u4DBF\u4E00-\u9FFF\uF900-\uFAFF]+$";
	public const string CHINESE_PATTERN1 = @"^[\u2E80-\u2FDF]+$";
	public const string CHINESE_PATTERN2 = @"^[\u3100-\u312F]+$";
	public const string CHINESE_PATTERN3 = @"^[\u3400-\u4DBF]+$";
	public const string CHINESE_PATTERN4 = @"^[\u4E00-\u9FFF]+$";
	public const string CHINESE_PATTERN5 = @"^[\uF900-\uFAFF]+$";
	
	//JAPANESE PATTERN
	public const string JAPANESE_PATTERN = @"^[\u3040-\u30FF\u31F0-\u31FF]+$";
	public const string JAPANESE_PATTERN1 = @"^[\u3040-\u30FF]+$";
	public const string JAPANESE_PATTERN2 = @"^[\u31F0-\u31FF]+$";
	
	//KOREAN PATTERN
	public const string KOREAN_PATTERN = @"^[\u1100-\u11FF\u3130-\u318F\uAC00-\uD7AF]+$";
	public const string KOREAN_PATTERN1 = @"^[\u1100-\u11FF]+$";
	public const string KOREAN_PATTERN2 = @"^[\u3130-\u318F]+$";
	public const string KOREAN_PATTERN3 = @"^[\uAC00-\uD7AF]+$";
	
	public UILabel TextField;
	
	public int Max = 0;
	
	private UIInput mUIInput;
	private string mText = "";
	
	// Use this for initialization
	void Start () 
	{
		mUIInput = gameObject.GetComponent<UIInput>();
		
		if(mUIInput != null)
		{
            //mUIInput.eventReceiver = gameObject;
            //mUIInput.functionName = "CheckInputHandler";	
        //    EventDelegate.Add(mUIInput.onChange, this.CheckInputHandler);
        }
	}
	
	// Update is called once per frame
	void Update () 
	{
		//CheckInputHandler();
	}
	
	void OnInput (string input)
	{
		//CheckInputHandler();
	}
	
	private void CheckInputHandler()
	{
		//if have no set Max chars count
		if(Max <=0) return;

		if(mUIInput == null)
		{
			mText = GetNewStringHandler(TextField.text);
			
			TextField.text = mText;
		}
		else
		{
		//	int index = TextField.text.LastIndexOf(mUIInput.caratChar);
			
			if(TextField.text.Length<1)return;

            if (mUIInput.text.Length > 0 && TextField.text[mUIInput.text.Length - 1].ToString() =="|")
            {
                mText = mUIInput.text.Remove(TextField.text.Length - 1);
            }
            else
            {
				mText = mUIInput.text;
			}
			
			mText = GetNewStringHandler(mText);
			
			mUIInput.text = mText;
		}
	}
	
	private string GetNewStringHandler(string oldStr)
	{
		char[] strArray = oldStr.ToCharArray();
		string newStr = "";
		int length = 0;
		
		foreach(char c in strArray)
		{
			//check language type
			if(GetCharWidthHandler(c) == HALF_WIDTH 
				|| ChineseCheckHandelr(c) == true 
				|| JapaneseCheckHandelr(c) == true)
			{
				//text length check
				if(length + GetCharWidthHandler(c) <= Max)
				{
					newStr += c.ToString();
					length += GetCharWidthHandler(c);
				}
			}
		}
		
		return newStr;
	}
	
	private int GetCharWidthHandler(char c)
	{
		string str = c.ToString();
		
		Regex regHalf = new Regex(HALF_PATTERN);
		
		if(regHalf.IsMatch(str))
		{
			return HALF_WIDTH;
		}
		
		//half-width
		/*
		Int32 unicodeNum = Convert.ToInt32(c);
		if(unicodeNum >= 0x0021 && unicodeNum <= 0x007E)
		{
			return HALF_WIDTH;
		}
		*/
		return FULL_WIDTH;
	}
	
	private bool ChineseCheckHandelr(char c)
	{
		string str = c.ToString();
		
		bool isChinese = false;
		
		Regex regChinese = new Regex(CHINESE_PATTERN);
		
		if(regChinese.IsMatch(str)) isChinese = true;

		return isChinese;
	}
	
	private bool JapaneseCheckHandelr(char c)
	{
		string str = c.ToString();
		
		bool isJapanese = false;
		
		Regex regJapanese = new Regex(JAPANESE_PATTERN);
		
		if(regJapanese.IsMatch(str)) isJapanese = true;

		return isJapanese;
	}
	
	private void CloseMessageBox()
	{
		//MessageBox.Hide();
	}
}
