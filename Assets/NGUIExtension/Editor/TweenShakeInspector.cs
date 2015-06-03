using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TweenShake))]
public class TweenShakeInspector : UITweenerEditor {


	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(120f);
		
		TweenShake tw = target as TweenShake;
		GUI.changed = false;

		float fPosRadius = EditorGUILayout.FloatField("Radius", tw.fPosRadius);
		int playPeriod = EditorGUILayout.IntField("TimeFrame", tw.playPeriod);
		float stopAfterSec = EditorGUILayout.FloatField("Stop After Second", tw.stopAfterSec);
		bool shakeX = EditorGUILayout.Toggle("Shake X", tw.shakeX);
		bool shakeY = EditorGUILayout.Toggle("Shake Y", tw.shakeY);

		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("Tween Change", tw);
			tw.fPosRadius = fPosRadius;
			tw.playPeriod = playPeriod;
			tw.stopAfterSec = stopAfterSec;
			tw.shakeX = shakeX;
			tw.shakeY = shakeY;
			UnityEditor.EditorUtility.SetDirty(tw);
		}
		
		DrawCommonProperties();
	}
}
