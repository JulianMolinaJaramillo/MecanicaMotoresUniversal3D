using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MissingScriptsFinder : MonoBehaviour
{
    [MenuItem("Tools/Find Missing Scripts in Selected")]
    static void FindMissingScriptsInSelected()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Selecciona un GameObject primero.");
            return;
        }

        GameObject root = Selection.activeGameObject;
        Transform[] allChildren = root.GetComponentsInChildren<Transform>(true);
        int count = 0;

        foreach (Transform child in allChildren)
        {
            Component[] components = child.GetComponents<Component>();
            foreach (Component comp in components)
            {
                if (comp == null)
                {
                    Debug.LogWarning($"Falta un script en: {child.name}", child.gameObject);
                    count++;
                }
            }
        }

        Debug.Log($"Total de scripts faltantes en {root.name}: {count}");
    }
}
