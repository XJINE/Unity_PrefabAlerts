using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PrefabAlerts {
[InitializeOnLoad]
public static class OverrideAlertInHierarchy
{
    private static readonly Dictionary<GameObject, bool> Cache = new();
    private static GUIStyle _labelStyle;

    static OverrideAlertInHierarchy()
    {
        EditorApplication.hierarchyWindowItemByEntityIdOnGUI += OnHierarchyGUI;
        EditorApplication.hierarchyChanged                   += Cache.Clear;
        PrefabUtility.prefabInstanceUpdated                  += _ => Cache.Clear();
        OverrideAlertSettings.Changed                        += Cache.Clear;
    }

    private static void OnHierarchyGUI(EntityId entityId, Rect selectionRect)
    {
        var gameObject = EditorUtility.EntityIdToObject(entityId) as GameObject;
        if (gameObject == null)
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

        _labelStyle ??= new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleRight,
            fontSize = 9
        };

        _labelStyle.normal.textColor = OverrideAlertSettings.HierarchyText;

        var labelRect = new Rect(selectionRect.xMin,      selectionRect.y,
                                 selectionRect.width - 5, selectionRect.height);

        GUI.Label(labelRect, "⚠ Override", _labelStyle);
    }
}}