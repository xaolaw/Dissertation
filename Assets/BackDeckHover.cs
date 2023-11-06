using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BackDeckHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isHovering = false;
    private float hoverTime = 0.4f;
    private int deltaYPos = 110;
    private float startYPos;
    private float elapsedTime = 0f;

    void Start()
    {
        startYPos = GetComponent<RectTransform>().anchoredPosition.y;
    }

    void Update()
    {
        if (isHovering && elapsedTime <= hoverTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / hoverTime;
            float newYPos = Mathf.Lerp(startYPos, startYPos + deltaYPos, t);
            SetYPosition(newYPos);
        }
    }

    void SetYPosition(float newYPos)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 anchoredPosition = rectTransform.anchoredPosition;
        anchoredPosition.y = newYPos;
        rectTransform.anchoredPosition = anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHovering)
        {
            isHovering = true;
            elapsedTime = 0f;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        SetYPosition(startYPos);
    }
}
