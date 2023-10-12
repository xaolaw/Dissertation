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
        public int cardPower { get; set; }
        public string cardType { get; set; }
        public string cardImage { get; set; }
        public string cardModel { get; set; }
        public string cardDescription { get; set; }
        public CardDetails cardDetails { get; set; }
    }

    public class CardDetails
    {
        public Deathrattle deathrattle { get; set; }
    }

    public class Deathrattle
    {

    }
}
