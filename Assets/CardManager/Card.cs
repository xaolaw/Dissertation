using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    public string card_name;
    public string description;
    public UnitSpawn.UnitType unitType;
    public int hp;
    private CardManager cm;
    private Arena arena;
    private UnitSpawn unitSpawn;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 position;


    public void Start(){
        cm = FindObjectOfType<CardManager>();
        arena = FindObjectOfType<Arena>();
        unitSpawn = FindObjectOfType<UnitSpawn>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        //TODO change smth about card apperaing
        position = rectTransform.localPosition;
    }

    private void HandleSpawning()
    {
        List<Tile> tileList = arena.getTileList();
        Tile tile = tileList.Find(obj => obj.isClicked);

        
        if (!unitSpawn.Spawn(tile, unitType, arena.playerTurn))
        {
            Debug.LogError("Spawning error");
        }
        
    }

    public void OnPointerDown (PointerEventData eventData){
        //HandleSpawning();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = position;
        canvasGroup.blocksRaycasts = true;
        HandleSpawning();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}