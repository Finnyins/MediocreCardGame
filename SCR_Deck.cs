// Finn O'Brien
// Project 24 (Deck handler script)
// 12/7/2021


using System;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project24
{
    public class SCR_Deck
    {
        RNGCryptoServiceProvider shuffle = new RNGCryptoServiceProvider();
        public List<string> cards;
        public SCR_Deck(bool debug)
        {
            if (debug)
            {
                cards = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild" };
            }
            else
            {
                cards = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild" };
            }
        }
        public string Draw()
        {
            if (cards.Count > 0)
            {
                byte[] rand = new byte[4];
                shuffle.GetBytes(rand);
                int val = BitConverter.ToInt32(rand, 0);
                int card = new Random(val).Next(cards.Count());
                byte index = Convert.ToByte(card);
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
