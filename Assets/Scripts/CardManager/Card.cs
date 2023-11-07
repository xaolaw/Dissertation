using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Classes;
using Unity.Netcode;

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

    public void SetStartFields(Arena _arena, UnitSpawn _unitSpawn)
    {
        arena = _arena;
        unitSpawn = _unitSpawn;
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

    public bool OnPlay(int tileID, bool from_opponent = false)
    {
        // if it's not your turn and you try to play a card
        if (NetworkManager.Singleton.IsClient && !from_opponent && arena.playerTurn == false)
            return false;

        Tile tile = arena.GetSingleTile(tileID, from_opponent);

        base_ = (this.arena.playerTurn) ? arena.playerBase : arena.opponentBase;
        if (!from_opponent && !base_.TryTakeEnergy(energy))
        {
            Debug.Log("Player doesn't have enough energy");
            return false;
        }

        // if the card spawns unit (spell card can be cast on enemy units outside of your range / frontline)
        if (spawnDetails != null)
        {
            Debug.Log("spawning " + spawnDetails.cardModel);
            if (!from_opponent && !arena.IsBehindFrontline(tile, arena.playerTurn))
            {
                Debug.LogError("Spawning outside frontline");
                return false;
            }
            if (!unitSpawn.Spawn(tile, this.spawnDetails, arena.playerTurn, index))
            {
                if (!from_opponent) {
                    Debug.LogError("Spawning error");
                    return false;
                }
            }
        }
        // if the card is a spell
        else if (spellEffect != null)
        {
            switch (spellEffect.castTarget)
            {
                case "unit":
                    if (!from_opponent)
                    {
                        if (tile.character == null)
                        {
                            Debug.LogError("this spell can be only casted on units");
                            return false;
                        }
                        if ((tile.character.playerUnit == arena.playerTurn && spellEffect.target == "enemy") || (tile.character.playerUnit != arena.playerTurn && spellEffect.target == "own"))
                        {
                            Debug.LogError("this spell can be only casted on units of other player");
                            return false;
                        }
                    }
                    arena.Damage(UnitSpawn.PUTFromString(spellEffect.target), UnitSpawn.UTGFromString(spellEffect.area), tile, arena.playerTurn, spellEffect.damage);
                    break;
                case "none":
                    arena.Damage(UnitSpawn.PUTFromString(spellEffect.target), UnitSpawn.UTGFromString(spellEffect.area), tile, arena.playerTurn, spellEffect.damage);
                    break;
                default:
                    Debug.LogError("Spell has wrong cast target: " + spellEffect.castTarget);
                    return false;
            }
        }

        if (!from_opponent)
        {
            arena.PlayCard(index, tile.id);

            played = true;
            this.gameObject.SetActive(false);
            base_.TakeEnergy(energy);
            cm.update_cards(this, base_.GetEnergy());
        }
        return true;
    }

    private bool HandleSpawning()
    {
        List<Tile> tileList = arena.GetTileList();
        Tile tile = tileList.Find(obj => obj.isClicked);
        if (tile == null)
            return false;
        return OnPlay(tile.id);
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
        if(!HandleSpawning()){
            RestorePreviousPosition();
        };
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
    private void RestorePreviousPosition(){
        Vector3 previousPosition = cm.GetCardPositionInHand(this);
        rectTransform.position = previousPosition;
    }

}