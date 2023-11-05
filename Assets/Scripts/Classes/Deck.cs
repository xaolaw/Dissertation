using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Classes
{
    public class Deck
    {
        public string Name { get; set; }
        public int[] CardList { get; set; }
    }

    public class DeckCollection
    {
        public List<Deck> Decks;
    }
}
