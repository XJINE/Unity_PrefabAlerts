using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PrefabAlerts {
[InitializeOnLoad]
public static class OverrideAlertInHeader
{
    private static readonly Dictionary<GameObject, bool> Cache = new();
    private static GUIStyle _labelStyle;

    static OverrideAlertInHeader()
    {
        Editor.finishedDefaultHeaderGUI     += OnFinishedDefaultHeaderGUI;
        EditorApplication.hierarchyChanged  += Cache.Clear;
        PrefabUtility.prefabInstanceUpdated += _ => Cache.Clear();
    }

    private static void OnFinishedDefaultHeaderGUI(Editor editor)
    {
        if (editor.target is not GameObject gameObject)
        {
            return;
        }

        if (!PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            return;
        }

        if (!Cache.TryGetValue(gameObject, out var hasOverrides))
        {
            hasOverrides = OverrideAlertUtility.HasOwnOverrides(gameObject);
            Cache[gameObject] = hasOverrides;
        }

        if (!hasOverrides)
        {
            return;
        }

        var alertRect = EditorGUILayout.GetControlRect(false, height: 26);

        EditorGUI.DrawRect(alertRect, OverrideAlertSettings.HeaderBackground);

        _labelStyle ??= new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 11
        };

        _labelStyle.normal.textColor = OverrideAlertSettings.HeaderText;

        EditorGUI.LabelField(alertRect, "⚠ PREFAB OVERRIDE DETECTED", _labelStyle);
    }
}}