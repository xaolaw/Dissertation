using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }

    public class SpawnDetails
    {
        public string cardType { get; set; }
        public string cardModel { get; set; }
        public int cardPower { get; set; }
        public Effect deathrattle { get; set; }
        public Effect battlecry { get; set; }
    }

    public class Effect
    {
        // only for spells, it is ignored in deathrattles
        public string castTarget { get; set; }
        public string target { get; set; }
        public string area { get; set; }
        public int damage { get; set; }
    }
}
