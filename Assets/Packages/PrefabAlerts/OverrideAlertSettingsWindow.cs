using UnityEditor;
using UnityEngine;

namespace PrefabAlerts {
internal sealed class OverrideAlertSettingsWindow : EditorWindow
{
    [MenuItem("Custom/Prefab Alerts")]
    private static void Open()
    {
        var window = GetWindow<OverrideAlertSettingsWindow>("Prefab Alerts");
            window.minSize = new Vector2(280, 220);
            window.Show();
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Header Alert", EditorStyles.boldLabel);
        var headerBackground = EditorGUILayout.ColorField("Background", OverrideAlertSettings.HeaderBackground);
        var headerText       = EditorGUILayout.ColorField("Text",       OverrideAlertSettings.HeaderText);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Hierarchy Alert", EditorStyles.boldLabel);
        var hierarchyText = EditorGUILayout.ColorField("Text", OverrideAlertSettings.HierarchyText);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Property Highlighter", EditorStyles.boldLabel);
        var highlighterText = EditorGUILayout.ColorField("Text", OverrideAlertSettings.HighlighterText);

        if (EditorGUI.EndChangeCheck())
        {
            OverrideAlertSettings.HeaderBackground = headerBackground;
            OverrideAlertSettings.HeaderText       = headerText;
            OverrideAlertSettings.HierarchyText    = hierarchyText;
            OverrideAlertSettings.HighlighterText  = highlighterText;

            OverrideAlertSettings.RaiseChanged();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Reset to Defaults"))
        {
            OverrideAlertSettings.ResetToDefaults();
            OverrideAlertSettings.RaiseChanged();
        }
    }
}}
