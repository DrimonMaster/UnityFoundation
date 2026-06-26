using UnityEditor;
using UnityEngine;
using UnityFoundation.Services;

namespace UnityFoundation.Editor
{
    [CustomEditor(typeof(LogSettings))]
    public class LogSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var categoriesProp = serializedObject.FindProperty("_categories");

            // Header row
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Category", EditorStyles.boldLabel, GUILayout.Width(130));
            GUILayout.Label("Editor",   EditorStyles.boldLabel, GUILayout.Width(60));
            GUILayout.Label("Build",    EditorStyles.boldLabel, GUILayout.Width(60));
            GUILayout.Label("Color",    EditorStyles.boldLabel, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);

            for (int i = 0; i < categoriesProp.arraySize; i++)
            {
                var element    = categoriesProp.GetArrayElementAtIndex(i);
                var editorProp = element.FindPropertyRelative("EnabledInEditor");
                var buildProp  = element.FindPropertyRelative("EnabledInBuild");
                var catProp    = element.FindPropertyRelative("Category");
                var colorProp  = element.FindPropertyRelative("Color");

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(((LogCategory)catProp.enumValueIndex).ToString(), GUILayout.Width(130));
                editorProp.boolValue = EditorGUILayout.Toggle(editorProp.boolValue, GUILayout.Width(60));
                buildProp.boolValue  = EditorGUILayout.Toggle(buildProp.boolValue,  GUILayout.Width(60));
                EditorGUILayout.PropertyField(colorProp, GUIContent.none, GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
