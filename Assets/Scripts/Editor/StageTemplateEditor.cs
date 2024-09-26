using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageTemplate))]
public class StageTemplateEditor : Editor
{
	private SerializedProperty easyWaves;
	private SerializedProperty normalWaves;
	private SerializedProperty hardWaves;

	private void OnEnable()
	{
		easyWaves = serializedObject.FindProperty("easyWaves");
		normalWaves = serializedObject.FindProperty("normalWaves");
		hardWaves = serializedObject.FindProperty("hardWaves");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("stageUIName"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("StageDeveloperName"));

		SerializedProperty difficultyProp = serializedObject.FindProperty("difficulty");
		difficultyProp.intValue = (int)(Difficulty)EditorGUILayout.EnumFlagsField("Difficulties", (Difficulty)difficultyProp.intValue);

		DisplayWavesForDifficulty("Easy Waves", Difficulty.easy, easyWaves);
		DisplayWavesForDifficulty("Normal Waves", Difficulty.normal, normalWaves);
		DisplayWavesForDifficulty("Hard Waves", Difficulty.hard, hardWaves);

		serializedObject.ApplyModifiedProperties();
	}


	private void DisplayWavesForDifficulty(string label, Difficulty difficultyFlag, SerializedProperty waveListProp)
	{
		if (((Difficulty)serializedObject.FindProperty("difficulty").intValue & difficultyFlag) != 0)
		{
			EditorGUILayout.PropertyField(waveListProp, new GUIContent(label), true);
		}
	}
}
