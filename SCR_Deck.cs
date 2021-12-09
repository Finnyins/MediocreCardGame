// Finn O'Brien
// Project 24 (Deck handler script)
// 12/7/2021


using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project24
{
    public class SCR_Deck
    {
        public List<string> cards;
        public SCR_Deck()
        {
            cards = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild"};
            //cards = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild"};
        }
        public string Draw()
        {
            if (cards.Count > 0)
            {
                Random rng = new Random();
                byte index = Convert.ToByte(rng.Next(cards.Count));
                string pullcard = cards[index];
                cards.RemoveAt(index);
                return pullcard;
            }
            else
            {
                return null;
            }
        }
    }
}
