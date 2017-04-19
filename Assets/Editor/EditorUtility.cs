using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class EditorUtility {

    /// <summary>
    /// 遍历某个目录
    /// </summary>
    /// <param name="path">目录的相对路径(eg. "/Resources/Prefabs")</param>
    /// <param name="assetAction">对单个资源执行的操作</param>
    public static void TraversalAssetsInDirectory<T>(string path, UnityAction<T> assetAction) where T : Object {
        string fullPath = Application.dataPath + path;
        Debug.Log(string.Format("遍历目录:\t{0}\nfullpath:\t{1}\n", path, fullPath));
        Debug.Log("类型：" + typeof(T));
        if (!Directory.Exists(fullPath)) {
            Debug.LogError("目录不存在");
            return;
        }
        string[] dirs = Directory.GetFiles(fullPath, "*", SearchOption.AllDirectories);
        //Directory.GetFiles(fullPath, "", SearchOption.AllDirectories);
        Debug.Log("目录中文件数量:" + dirs.Length);
        foreach (string dir in dirs) {
            if (dir.EndsWith(".meta")) {
                continue;
            }
            string p = dir.Remove(0, dir.IndexOf("/Assets/") + 1);
            T go = AssetDatabase.LoadAssetAtPath<T>(p);
            if (go) {
                assetAction(go);
            }
        }
    }
}
