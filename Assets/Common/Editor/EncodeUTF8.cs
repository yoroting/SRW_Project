using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

public class EncodeUTF8 {
	
	[MenuItem("Custom/重新編碼為UTF8/指定資料夾")]
	public static void EncodeToUTF8()
	{
		string folderPath = EditorUtility.OpenFolderPanel("請選擇目錄", Application.dataPath, string.Empty);

		if(!string.IsNullOrEmpty(folderPath))
			EncodeFunc(folderPath);
	}
	
	[MenuItem("Custom/重新編碼為UTF8/所有cs檔")]
	public static void AllCSEncodeToUTF8()
	{
		EncodeFunc(Application.dataPath);
	}

	private static void EncodeFunc(string folderPath){
		if(string.IsNullOrEmpty(folderPath)){
			Debug.LogWarning("重新編碼失敗，指定空路徑！");
			return;
		}

		string[] filePaths = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);
		
		if (filePaths == null || filePaths.Length == 0)
			return;
		
		foreach (string path in filePaths)
		{
			string source = File.ReadAllText(path);
			
			File.WriteAllText(path, source, Encoding.UTF8);
		}

		AssetDatabase.Refresh();

		Debug.Log("完成重新編碼！");
	}
}
