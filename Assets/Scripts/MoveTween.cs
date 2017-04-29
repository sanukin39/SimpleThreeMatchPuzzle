using System;
using UnityEngine;

// 移動アニメーションクラス
public class MoveTween : MonoBehaviour {

    public Vector3 fromPosition;
    public Vector3 toPosition;
    public float duration;

    private bool isTween;
    private float elapsedTime;
    private Action endCallBack;

    //-------------------------------------------------------
    // MonoBehaviour Function
    //-------------------------------------------------------
    // アニメーションの更新処理
    private void Update(){

        if (!isTween)
        {
            return;
        }

        // アニメーション開始時からの経過時間
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= duration)
        {
            // アニメーションの終了処理
            transform.position = toPosition;
            isTween = false;
            if (endCallBack != null)
            {
                endCallBack();
            }
            Destroy(this);
            return;
        }

        //
        var moveProgress = elapsedTime / duration;
        transform.position = Vector3.Lerp(fromPosition, toPosition, moveProgress);
    }

    //-------------------------------------------------------
    // Public Function
    //-------------------------------------------------------
    // アニメーション開始処理
    public void DoTween(Vector3 from, Vector3 to, float dur, Action endcb)
    {
        fromPosition = from;
        toPosition = to;
        duration = dur;
        endCallBack = endcb;

        transform.position = from;
        elapsedTime = 0;
        isTween = true;
    }
}