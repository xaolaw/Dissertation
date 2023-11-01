using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Classes;

public class Card : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    public string cardName;
    private string description;
    //index of card in json
    private int index;
    private string model;
    private int power;
    private int energy;
    private SpawnDetails spawnDetails;
    private Effect spellEffect;

    private CardManager cm;
    private Arena arena;
    private UnitSpawn unitSpawn;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 position;
    private Base base_;
    public bool played = false;


    public void Start(){
        cm = FindObjectOfType<CardManager>();
        arena = FindObjectOfType<Arena>();
        unitSpawn = FindObjectOfType<UnitSpawn>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        //TODO change smth about card apperaing
        position = rectTransform.localPosition;
    }
    //initialize card all manadatory fields
    public void Initialize(string name_, int energy_, string image_, SpawnDetails spawnDetails_, Effect spellEffect_, int index_)
    {
        cardName = name_;
        energy = energy_;
        spawnDetails = spawnDetails_;
        spellEffect = spellEffect_;
        index = index_;
        Sprite image = Resources.Load<Sprite>(image_);

        Image imageComponent = GetComponent<Image>();

        if (imageComponent != null)
        {
            if(image == null)
            {
                Debug.LogError("Invalid path" + image_);
            }
            else
            {
                imageComponent.sprite = image;
            }
        }
        else
        {
            Debug.LogError("Invalid! imageComponent is null");
        }

    }

    private void HandleSpawning()
    {
        List<Tile> tileList = arena.getTileList();
        Tile tile = tileList.Find(obj => obj.isClicked);

        base_ = this.arena.playerTurn ? arena.playerBase : arena.opponentBase;
        if (!base_.TryTakeEnergy(energy))
        {
            Debug.Log("Player doesn't have enough energy");
            return;
        }

        // if the card spawns unit (spell card can be cast on enemy units outside of your range / frontline
        if (spawnDetails != null)
        {
            Debug.Log("spawning " + spawnDetails.cardModel);
            if (!arena.IsBehindFrontline(tile, arena.playerTurn))
            {
                Debug.LogError("Spawning outside frontline");
                return;
            }
            if (!unitSpawn.Spawn(tile, this.spawnDetails, arena.playerTurn, index))
            {
                Debug.LogError("Spawning error");
                return;
            }
            else
            {
                
            }
        }
        // if the card is a spell
        else if (spellEffect != null)
        {
            switch(spellEffect.castTarget)
            {
                case "unit":
                    if (tile.character == null)
                    {
                        Debug.LogError("this spell can be only casted on units");
                        return;
                    }
                    if ((tile.character.playerUnit == arena.playerTurn && spellEffect.target == "enemy") || (tile.character.playerUnit != arena.playerTurn && spellEffect.target == "own"))
                    {
                        Debug.LogError("this spell can be only casted on units of other player");
                        return;
                    }
                    arena.Damage(UnitSpawn.PUTFromString(spellEffect.target), UnitSpawn.UTGFromString(spellEffect.area), tile, arena.playerTurn, spellEffect.damage);
                    break;
                case "none":
                    arena.Damage(UnitSpawn.PUTFromString(spellEffect.target), UnitSpawn.UTGFromString(spellEffect.area), tile, arena.playerTurn, spellEffect.damage);
                    break;
                default:
                    Debug.LogError("Spell has wrong cast target: " + spellEffect.castTarget);
                    break;
            }
        }

        played = true;
        this.gameObject.SetActive(false);
        base_.TakeEnergy(energy);
        cm.update_cards(this);
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

    public int GetJsonIndex()
    {
        return index;
    }
    public int GetEnergy(){
        return energy;
    }

}