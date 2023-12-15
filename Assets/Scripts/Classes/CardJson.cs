using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Classes
{
    public class CardJson 
    {
        public string cardName { get; set; }
        public int cardEnergy {get; set;}
        public string cardImage { get; set; }
        public string cardDescription { get; set; }
        public SpawnDetails spawnUnit { get; set; }
        public Effect spellEffect { get; set; }
        public int cardInDeckLimitNumber { get; set; }
    }

    public class SpawnDetails
    {
        public string cardType { get; set; }
        public string cardModel { get; set; }
        public int cardPower { get; set; }
        public int speed { get; set; }
        public string status { get; set; }
        public Effect deathrattle { get; set; }
        public Effect battlecry { get; set; }
        public Effect onTurnEnd { get; set; }
        public Effect onAttack { get; set; }
        public Effect onDamage { get; set; }
    }

    public class Effect
    {
        // only for spells, it is ignored in deathrattles
        public string castTarget { get; set; }
        public string target { get; set; }
        public string area { get; set; }
        public int? damage { get; set; }
        public int? drawCard { get; set; }
        public SpawnDetails spawn { get; set; }

        public bool ChangesPower()
        {
            return target != null && area != null && damage != null;
        }

        public System.Action<Tile, bool> GenerateAction(Arena arena, Arena.EffectReason reason)
        {
            System.Action<Tile, bool> newAction = delegate (Tile t, bool b) { };

            if (ChangesPower())
            {
                newAction = delegate (Tile origintile, bool side)
                {
                    origintile.Damage(UnitSpawn.PUTFromString(target), UnitSpawn.UTGFromString(area), side, damage.Value, reason);
                };
            }
            else if (drawCard.HasValue)
            {
                newAction = delegate (Tile origintile, bool side)
                {
                    // if my turn my card manager draws card else opponent does this
                    if (arena.playerTurn == side)
                    {
                        for (int i = 0; i < drawCard.Value; ++i)
                        {
                            arena.cardManager.DrawCard();
                        }
                    }
                };
            }
            else if (spawn != null)
            {
                newAction = delegate (Tile origintile, bool side)
                {
                    foreach (Tile tile in arena.GetEmptyTargetTiles(UnitSpawn.UTGFromString(area), origintile, side))
                    {
                        arena.unitSpawn.Spawn(tile, spawn, side, 0);
                        arena.CheckFrontline(tile.id, side);
                    }
                };
            }

            return newAction;
        }

        public bool HasAnimation()
        {
            if (damage.HasValue && damage.Value > 0)
            {
                return true;
            }
            if (spawn != null && spawn.speed > 0)
            {
                return true;
            }
            return false;
        }
    }
}
