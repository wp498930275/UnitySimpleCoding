using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RemoveMissingComponentInPrefab : EditorWindow {

    [MenuItem("Prefab/删除Missing的脚本")]
    private static void DeleteMissingScriptsInAssets() {
        EditorUtility.TraversalAssetsInDirectory("/Resources/test", (GameObject go) => {
            Component[] components = go.GetComponentsInChildren<Component>(true);
            bool hasMissing = false;
            foreach (var item in components) {
                if (item == null) {
                    hasMissing = true;
                    break;
                }
            }
            if (!hasMissing) {
                return;
            }
            Debug.Log("存在缺失脚本. 路径:" + AssetDatabase.GetAssetPath(go));
            //有缺失脚本，实例化到场景中进行操作
            GameObject prefab = PrefabUtility.InstantiatePrefab(go) as GameObject;
            var serializedObject = new SerializedObject(prefab);
            var prop = serializedObject.FindProperty("m_Component");
            for (int j = components.Length - 1; j >= 0; j--) {
                if (components[j] == null) {
                    prop.DeleteArrayElementAtIndex(j);
                }
            }
            serializedObject.ApplyModifiedProperties();
            PrefabUtility.ReplacePrefab(prefab, go, ReplacePrefabOptions.ConnectToPrefab);
            DestroyImmediate(prefab);
        });
    }

}
