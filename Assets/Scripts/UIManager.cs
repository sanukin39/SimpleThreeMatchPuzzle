using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI関連の表示を行う
public class UIManager : MonoBehaviour {

    // public.
    public Text statusText;
    public Text comboText;

    // private.
    private int comboCount = 0;

    //-------------------------------------------------------
    // Public Function
    //-------------------------------------------------------
    public void SetStatusText(string status)
    {
        statusText.text = status;
    }

    public void ResetCombo()
    {
        comboCount = 0;
        UpdateComboText();
    }

    public void AddCombo()
    {
        comboCount++;
        UpdateComboText();
    }

    //-------------------------------------------------------
    // Private Function
    //-------------------------------------------------------
    private void UpdateComboText()
    {
        comboText.text = string.Format("{0}combo", comboCount);
    }
    
}
