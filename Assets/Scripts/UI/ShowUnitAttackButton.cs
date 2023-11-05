using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShowUnitAttackButton : MonoBehaviour
{
    private Button button;
    private TMP_Text textPro;
    private Arena arena;
    private bool isShowing;

    // Start is called before the first frame update
    void Start()
    {
        arena = (Arena)FindObjectOfType(typeof(Arena));
        if (!arena)
            Debug.Log("ShowUnitButton: Arena not found");

        button = GetComponent<Button>();
        textPro = GetComponentInChildren<TMP_Text>();
        Debug.Log(button);
        button.onClick.AddListener(ShowInfo);
        isShowing = false;

    }
    void ShowInfo()
    {
        isShowing = !isShowing;
        if (isShowing)
        {
            textPro.text = "Hide units attack";
        }
        else
        {
            textPro.text = "Show units attack";
        }
        arena.ShowAttackInfo(isShowing);
    }
}
