using UnityEngine;
using UnityEngine.UI;

public class HoverAnimationController : MonoBehaviour
{
    private Animator animator;
    private RectTransform rectTransform;
    private Canvas canvas;
    private int startingSortingOrder;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startingSortingOrder = rectTransform.GetSiblingIndex();
    }

    public void BeginAnimation()
    {
        animator.SetBool("HoverBool", true);
        rectTransform.SetAsLastSibling();
    }

    public void EndAnimation()
    {
        animator.SetBool("HoverBool", false);
        rectTransform.SetSiblingIndex(startingSortingOrder);
    }
}