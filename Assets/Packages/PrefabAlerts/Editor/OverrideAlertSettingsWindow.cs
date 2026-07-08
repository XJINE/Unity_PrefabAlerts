using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PrefabAlerts {
internal sealed class OverrideAlertSettingsWindow : EditorWindow
{
    private List<string> _ignoredPropertyPaths;
    private Vector2      _scroll;

    [MenuItem("Custom/Prefab Alerts")]
    private static void Open()
    {
        var window = GetWindow<OverrideAlertSettingsWindow>("Prefab Alerts");
            window.minSize = new Vector2(320, 320);
            window.Show();
    }

    private void OnEnable()
    {
        ReloadIgnoredPropertyPaths();
    }

    private void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

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

        DrawIgnoredPropertyPaths();

        EditorGUILayout.Space();

        if (GUILayout.Button("Reset to Defaults"))
        {
            OverrideAlertSettings.ResetToDefaults();
            ReloadIgnoredPropertyPaths();
            OverrideAlertSettings.RaiseChanged();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawIgnoredPropertyPaths()
    {
        EditorGUILayout.LabelField("Ignored Property Paths", EditorStyles.boldLabel);

        _ignoredPropertyPaths ??= new List<string>();

        var changed     = false;
        var removeIndex = -1;

        for (var i = 0; i < _ignoredPropertyPaths.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            var entry = EditorGUILayout.TextField(_ignoredPropertyPaths[i]);

            if (EditorGUI.EndChangeCheck())
            {
                _ignoredPropertyPaths[i] = entry;
                changed = true;
            }

            if (GUILayout.Button("-", GUILayout.Width(24)))
            {
                removeIndex = i;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (removeIndex >= 0)
        {
            _ignoredPropertyPaths.RemoveAt(removeIndex);
            changed = true;
        }

        if (GUILayout.Button("Add Property Path"))
        {
            _ignoredPropertyPaths.Add(string.Empty);
            changed = true;
        }

        if (changed)
        {
            OverrideAlertSettings.IgnoredPropertyPaths = _ignoredPropertyPaths.ToArray();
            OverrideAlertSettings.RaiseChanged();
        }
    }

    private void ReloadIgnoredPropertyPaths()
    {
        _ignoredPropertyPaths = new List<string>(OverrideAlertSettings.IgnoredPropertyPaths);
    }
}}