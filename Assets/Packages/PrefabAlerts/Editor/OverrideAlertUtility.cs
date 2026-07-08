using UnityEditor;
using UnityEngine;

namespace PrefabAlerts {
internal static class OverrideAlertUtility
{
    // NOTE:
    // HasPrefabInstanceAnyOverrides evaluates the entire Prefab instance.
    // This method checks if the GameObject itself has any overrides.
    internal static bool HasOwnOverrides(GameObject gameObject)
    {
        var root = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);

        if (root == null)
        {
            return false;
        }

        // Check for any overridden properties
        foreach (var objectOverride in PrefabUtility.GetObjectOverrides(root, includeDefaultOverrides: false))
        {
            var instanceObject = objectOverride.instanceObject;

            var belongsToThis = instanceObject == gameObject
                             || (instanceObject is Component component && component.gameObject == gameObject);

            if (!belongsToThis)
            {
                continue;
            }

            if (HasNonIgnoredOverride(instanceObject))
            {
                return true;
            }
        }

        // Check for any added components
        foreach (var added in PrefabUtility.GetAddedComponents(root))
        {
            if (added.instanceComponent != null && added.instanceComponent.gameObject == gameObject)
            {
                return true;
            }
        }

        // Check for any removed components
        foreach (var removed in PrefabUtility.GetRemovedComponents(root))
        {
            if (removed.containingInstanceGameObject == gameObject)
            {
                return true;
            }
        }

        // Check for any added child GameObjects
        foreach (var added in PrefabUtility.GetAddedGameObjects(root))
        {
            if (added.instanceGameObject != null
             && added.instanceGameObject.transform.parent != null
             && added.instanceGameObject.transform.parent.gameObject == gameObject)
            {
                return true;
            }
        }

        // Check for any removed child GameObjects
        foreach (var removed in PrefabUtility.GetRemovedGameObjects(root))
        {
            if (removed.parentOfRemovedGameObjectInInstance == gameObject)
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasNonIgnoredOverride(Object instanceObject)
    {
        if (instanceObject == null)
        {
            return false;
        }

        using var serializedObject = new SerializedObject(instanceObject);

        var iterator = serializedObject.GetIterator();

        while (iterator.Next(enterChildren: true))
        {
            if (iterator.prefabOverride && !OverrideAlertSettings.IsIgnoredProperty(iterator.propertyPath))
            {
                return true;
            }
        }

        return false;
    }
}}