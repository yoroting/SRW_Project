using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;

public class DebugConsole : MonoBehaviour
{
	public event Action<string, string, LogType> LogEvent;

	public int maxLogLine = 100;

	private string consoleMessage = "";
	
	private List<string> logLines;
	
	private bool hasNewMessage = false;
	
	private Vector2 scrollPosition;

	private Rect textRect;

	private GUIStyle textStyle;

	private GUIStyle consoleBtnStyle;

	private GUIStyle boxStyle;

	private bool isOpened = false;

	private FPSDetector fpsDetector;

	void Awake()
	{
		fpsDetector = new FPSDetector();

		textRect = new Rect(0, 0, Screen.width, Screen.height);

		Application.RegisterLogCallback(LogHandler);
	}
	
	void LogHandler(string condition, string stack, LogType type)
	{
		if (LogEvent != null)
			LogEvent(condition, stack, type);

		if (logLines == null)
			logLines = new List<string>(maxLogLine);
		
		if (logLines.Count >= maxLogLine)
			logLines.RemoveAt(0);
		
		string color = "#FFFFFF";
		
		if (type == LogType.Warning)
		{
			color = "#CCCC00";
		}
		else if (type == LogType.Error || type == LogType.Exception)
		{
			color = "#CC0000";
			condition += stack;
		}

		string logText = string.Format("{0} {1} {2}", type.ToString(), DateTime.Now.ToString("HH:mm:ss.ff"), condition);

		logLines.Add(string.Format("<color={0}>{1}</color>", color, logText));

		hasNewMessage = true;
	}

	void Update()
	{
		if( fpsDetector != null )
			fpsDetector.EnterFrame();

#if UNITY_EDITOR
		//if (Input.GetKeyDown(KeyCode.Alpha1))
		//	DNMTools.ShowMessage("abcd");

		//if (Input.GetKeyDown(KeyCode.Alpha2))
		//	ServerService.Instance.SendCommand(new ServerTimeSyncReq());
#endif
	}

	void OnGUI()
	{
		if(Application.isEditor)
			return;

		if (textStyle == null)
		{
			textStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
			textStyle.fontSize = 24;
		}

		if (consoleBtnStyle == null)
		{
			consoleBtnStyle = new GUIStyle(GUI.skin.GetStyle("Button"));
			consoleBtnStyle.fontSize = 24;
		}

		if (boxStyle == null)
		{
			boxStyle = new GUIStyle(GUI.skin.GetStyle("Box"));
			boxStyle.fontSize = 24;
		}

		GUILayout.BeginHorizontal();

		if (fpsDetector.TargetFPS < 0 || fpsDetector.FPS < fpsDetector.TargetFPS)
		{
			GUI.Box(new Rect(200, 0, 150, 30), string.Format("FPS:{0}", fpsDetector.FPS), boxStyle);
		}

		if (GUI.Button(new Rect(0, 0, 200, 30), "Console", consoleBtnStyle))
			isOpened = !isOpened;
		
		GUILayout.EndHorizontal();

		if (isOpened)
		{
			if (GUI.Button(new Rect(0, 30, 200, 30), "GC", consoleBtnStyle))
			{
				float startTime = Time.realtimeSinceStartup;
				
				Resources.UnloadUnusedAssets();
				System.GC.Collect();
				
				Debug.Log("GC Complete, cost=" + (Time.realtimeSinceStartup - startTime) + "秒");
			}

			if (hasNewMessage)
			{
				//只在文字改變時更新
				hasNewMessage = false;

				StringBuilder sb = new StringBuilder();

				foreach (string line in logLines)
				{
					sb.AppendLine(line);
				}
				consoleMessage = sb.ToString();

				scrollPosition.y = int.MaxValue;
			}

			GUI.Box(textRect, "");

			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(textRect.width), GUILayout.Height(textRect.height));

			GUILayout.Label(consoleMessage, textStyle, GUILayout.MaxWidth(textRect.width));

			GUILayout.EndScrollView();
		}
	}
}
