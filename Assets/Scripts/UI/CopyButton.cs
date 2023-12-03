using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CopyButton : MonoBehaviour
{
    public TMP_Text text;
    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = text.text;
    }
}
