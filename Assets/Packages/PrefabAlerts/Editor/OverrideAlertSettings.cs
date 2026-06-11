using System;
using UnityEditor;
using UnityEngine;

namespace PrefabAlerts {
internal static class OverrideAlertSettings
{
    private const string KeyPrefix = "PrefabAlerts.OverrideColors.";

    private static readonly Color DefaultHeaderBackground      = new(1f, 1f, 0f, 0.70f);
    private static readonly Color DefaultHeaderText            = Color.black;
    private static readonly Color DefaultHierarchyText         = new(1f, 1f, 0f, 0.75f);
    private static readonly Color DefaultHighlighterText       = new(1f, 1f, 0f, 0.75f);

    internal static event Action Changed;

    internal static Color HeaderBackground
    {
        get => Load("HeaderBackground", DefaultHeaderBackground);
        set => Save("HeaderBackground", value);
    }

    internal static Color HeaderText
    {
        get => Load("HeaderText", DefaultHeaderText);
        set => Save("HeaderText", value);
    }

    internal static Color HierarchyText
    {
        get => Load("HierarchyText", DefaultHierarchyText);
        set => Save("HierarchyText", value);
    }

    internal static Color HighlighterText
    {
        get => Load("HighlighterText", DefaultHighlighterText);
        set => Save("HighlighterText", value);
    }

    internal static void ResetToDefaults()
    {
        HeaderBackground = DefaultHeaderBackground;
        HeaderText       = DefaultHeaderText;
        HierarchyText    = DefaultHierarchyText;
        HighlighterText  = DefaultHighlighterText;
    }

    internal static void RaiseChanged()
    {
        Changed?.Invoke();

        EditorApplication.RepaintHierarchyWindow();

        foreach (var inspector in Resources.FindObjectsOfTypeAll<EditorWindow>())
        {
            if (inspector != null && inspector.GetType().Name == "InspectorWindow")
            {
                inspector.Repaint();
            }
        }
    }

    private static Color Load(string name, Color fallback)
    {
        var stored = EditorPrefs.GetString(KeyPrefix + name, string.Empty);

        return ColorUtility.TryParseHtmlString("#" + stored, out var color) ? color : fallback;
    }

    private static void Save(string name, Color value)
    {
        EditorPrefs.SetString(KeyPrefix + name, ColorUtility.ToHtmlStringRGBA(value));
    }
}}