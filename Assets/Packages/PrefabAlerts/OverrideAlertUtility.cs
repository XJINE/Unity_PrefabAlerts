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
            if (objectOverride.instanceObject == gameObject)
            {
                return true;
            }

            if (objectOverride.instanceObject is Component component && component.gameObject == gameObject)
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

        return false;
    }
}}