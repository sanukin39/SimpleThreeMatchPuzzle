using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

// アルファアニメーションクラス
public class AlphaTween : MonoBehaviour
{

    // public.
    public Image thisImage;

    // private.
    private float fromAlpha;
    private float toAlpha;
    private float duration;
    private Action endCallBack;
    private bool isTween;
    private float elapsedTime;

    //-------------------------------------------------------
    // MonoBehaviour Function
    //-------------------------------------------------------
    private void Awake()
    {
        thisImage = GetComponent<Image>();
    }

    // アニメーションの更新処理
    private void Update()
    {
        if (!isTween)
        {
            return;
        }

        // アニメーション開始時からの経過時間
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= duration)
        {
            // アニメーションの終了処理
            SetAlpha(toAlpha);
            isTween = false;
            if (endCallBack != null)
            {
                endCallBack();
            }

            Destroy(this);
            return;
        }

        var moveProgress = elapsedTime / duration;
        SetAlpha(Mathf.Lerp(fromAlpha, toAlpha, moveProgress));
    }


    //-------------------------------------------------------
    // Public Function
    //-------------------------------------------------------
    public void DoTween(float fAlpha, float tAlpha, float dur, Action eCallBack)
    {
        this.fromAlpha = fAlpha;
        this.toAlpha = tAlpha;
        this.duration = dur;
        this.endCallBack = eCallBack;

        SetAlpha(fAlpha);
        elapsedTime = 0;
        isTween = true;
    }

    //-------------------------------------------------------
    // Private Function
    //-------------------------------------------------------
    private void SetAlpha(float alpha)
    {
        var col = thisImage.color;
        col.a = alpha;
        thisImage.color = col;
    }
}
