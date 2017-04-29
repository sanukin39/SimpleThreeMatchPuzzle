using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// アニメーションの管理クラス
public class TweenAnimationManager : MonoBehaviour {

    private Queue<List<AnimData>> animQueue = new Queue<List<AnimData>>();
    bool isTween;
    int tweenAnimationCount;
    int endAnimCount;

    //-------------------------------------------------------
    // MonoBehaviour Function
    //-------------------------------------------------------
    // アニメーション実行処理
    private void Update()
    {
        if (isTween)
        {
            return;
        }

        if (animQueue.Count > 0)
        {
            endAnimCount = 0;
            isTween = true;
            var queue = animQueue.Dequeue();
            tweenAnimationCount = queue.Count;
            foreach (var data in queue)
            {
                var tween = data.targetObject.AddComponent<MoveTween>();
                tween.DoTween(data.targetObject.transform.position, data.targetPosition, data.duration, () => {
                    endAnimCount++;
                    if (tweenAnimationCount == endAnimCount)
                    {
                        isTween = false;
                    }
                });
            }
        }
    }

    // アニメーションのセット処理
    public void AddListAnimData(List<AnimData> animData)
    {
        animQueue.Enqueue(animData);
    }
}
