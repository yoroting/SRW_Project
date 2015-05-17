
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenGrayLevel))]
public class TweenGrayLevelEditor : UITweenerEditor 
{

	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(120f);
		
		TweenGrayLevel tw = target as TweenGrayLevel;
		GUI.changed = false;
		
		float from = EditorGUILayout.FloatField("From", tw.from);
		float to = EditorGUILayout.FloatField("To", tw.to);
		//bool table = EditorGUILayout.Toggle("Update Table", tw.updateTable);
		
		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("Tween Change", tw);
			tw.from = from;
			tw.to = to;
			//tw.updateTable = table;
			UnityEditor.EditorUtility.SetDirty(tw);
		}
		
		DrawCommonProperties();
	}
}
