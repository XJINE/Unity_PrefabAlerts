using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PrefabAlerts {
[InitializeOnLoad]
public static class OverridePropertyHighlighter
{
    private const string HookedMarker  = "prefab-override-highlighter-hooked"; // to check the root is already hooked.
    private const string ColoredMarker = "prefab-override-colored";            // to check the field is already colored.

    static OverridePropertyHighlighter()
    {
        Selection.selectionChanged    += OnSelectionChanged;
        OverrideAlertSettings.Changed += OnSelectionChanged; // Re-highlight when colors change.
        OnSelectionChanged(); // Needs to call when Editor is opened.
    }

    private static void OnSelectionChanged()
    {
        var inspectorRoots = Resources.FindObjectsOfTypeAll<EditorWindow>()
                                      .Where (window => window != null && window.GetType().Name == "InspectorWindow")
                                      .Select(window => window.rootVisualElement)
                                      .Where (root   => root != null);

        foreach (var root in inspectorRoots)
        {
            EnsureHooked   (root);
            UpdateHighlight(root);
        }
    }

    private static void EnsureHooked(VisualElement root)
    {
        if (root.ClassListContains(HookedMarker))
        {
            return;
        }

        root.AddToClassList(HookedMarker);
        root.RegisterCallback<SerializedPropertyChangeEvent, VisualElement>((_, capturedRoot) => UpdateHighlight(capturedRoot), root);
    }

    private static void UpdateHighlight(VisualElement root)
    {
        // CAUTION:
        // Delay is required to wait for the UITree reconstruction to complete.
        root.schedule.Execute(() => HighlightOverrides(root)).StartingIn(delayMs:50);
    }

    private static void HighlightOverrides(VisualElement root)
    {
        if (root == null)
        {
            return;
        }

        // Reset colored fields
        foreach (var field in root.Query(className: ColoredMarker).ToList())
        {
            ApplyColor(field, isOverride: false);
            field.RemoveFromClassList(ColoredMarker);
        }

        foreach (var marker in root.Query().Where(IsActiveOverrideMarker).ToList())
        {
            var field = GetNearestFieldElement(marker);

            if (field == null)
            {
                continue;
            }

            ApplyColor(field, isOverride: true);

            if (!field.ClassListContains(ColoredMarker))
            {
                field.AddToClassList(ColoredMarker);
            }
        }

        HighlightAddedListElements(root);
    }

    private static void HighlightAddedListElements(VisualElement root)
    {
        foreach (var gameObject in Selection.gameObjects)
        {
            if (!PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                continue;
            }

            var modifications = PrefabUtility.GetPropertyModifications(gameObject);

            if (modifications == null)
            {
                continue;
            }

            foreach (var modification in modifications.Where(modification => modification.target != null
                                                                          && modification.propertyPath.EndsWith(".Array.size")))
            {
                var arrayPath = modification.propertyPath[..^".Array.size".Length];

                // NOTE:
                // modification.target points to the prefab source object,
                // so reading its array gives the prefab-side size.
                // The overridden instance size is stored in modification.value.
                var prefabProp = new SerializedObject(modification.target).FindProperty(arrayPath);

                if (prefabProp == null)
                {
                    continue;
                }

                var prefabSize = prefabProp.arraySize;

                if (!int.TryParse(modification.value, out var instanceSize) || instanceSize <= prefabSize)
                {
                    continue;
                }

                for (var i = prefabSize; i < instanceSize; i++)
                {
                    var bindingPath = $"{arrayPath}.Array.data[{i}]";

                    root.Query<PropertyField>()
                        .Where(field => field.bindingPath == bindingPath)
                        .ForEach(field =>
                        {
                            ApplyColor(field, isOverride: true);

                            if (!field.ClassListContains(ColoredMarker))
                            {
                                field.AddToClassList(ColoredMarker);
                            }
                        });
                }
            }
        }
    }

    private static bool IsActiveOverrideMarker(VisualElement element)
    {
        var hasOverrideClass = element.GetClasses().Any(classes =>
        {
            var lower = classes.ToLowerInvariant();
            return lower.Contains("override") || lower.Contains("prefab");
        });

        if (!hasOverrideClass)
        {
            return false;
        }

        return element.resolvedStyle.visibility == Visibility.Visible &&
               element.resolvedStyle.display    != DisplayStyle.None;
    }

    private static VisualElement GetNearestFieldElement(VisualElement element)
    {
        var field = element;

        while (field != null && !field.GetClasses().Any(classes => classes.Contains("unity-base-field")
                                                                || classes.Contains("unity-property-field")
                                                                || classes.Contains("unity-list-view__item")))
        {
            field = field.parent;
        }

        return field;
    }

    private static void ApplyColor(VisualElement field, bool isOverride)
    {
        var textColor = isOverride ? new StyleColor(OverrideAlertSettings.HighlighterText) : new StyleColor(StyleKeyword.Null);

        field.style.color = textColor;
        field.Query<TextElement>()                       .ForEach(text      => text.style.color                              = textColor);
        field.Query(className: "unity-toggle__checkmark").ForEach(checkmark => checkmark.style.unityBackgroundImageTintColor = textColor);

        // NOTE:
        // BackgroundColor is too much.
        // var backgroundColor = isOverride ? new StyleColor(OverrideBackgroundColor) : new StyleColor(StyleKeyword.Null);
        // var inputArea = field.Q(className: "unity-base-field__input");
        // if (inputArea != null)
        // {
        //     inputArea.style.backgroundColor = backgroundColor;
        // }
    }
}}