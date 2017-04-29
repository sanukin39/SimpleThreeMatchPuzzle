using UnityEngine;

// アニメーション定義保持用構造体
public struct AnimData
{
    public GameObject targetObject;
    public Vector3 targetPosition;
    public float duration;

    public AnimData(GameObject target, Vector3 pos, float dur)
    {
        targetObject = target;
        targetPosition = pos;
        duration = dur;
    }
}