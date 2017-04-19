using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transform的扩展，方便使用，简化代码
/// </summary>
public static class TransformExtension {

    public static void SetPositionX(this Transform t, float newX) {
        t.position = new Vector3(newX, t.position.y, t.position.z);
    }

    public static void SetPositionY(this Transform t, float newY) {
        t.position = new Vector3(t.position.x, newY, t.position.z);
    }

    public static void SetPositionZ(this Transform t, float newZ) {
        t.position = new Vector3(t.position.x, t.position.y, newZ);
    }

    public static float GetPositionX(this Transform t) {
        return t.position.x;
    }

    public static float GetPositionY(this Transform t) {
        return t.position.y;
    }

    public static float GetPositionZ(this Transform t) {
        return t.position.z;
    }
}
