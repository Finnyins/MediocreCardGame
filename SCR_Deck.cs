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
        public SCR_Deck(bool custom, bool rndm, byte decksize, List<string> customdeck)
        {
            List<string> defaultdeck = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild", "Wild", "Wild", };
            List<string> randomcards = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Wild" };

            if (custom)
            {
                cards = customdeck;
            }
            else
            {
                cards = new List<string>();
                if (rndm)
                {
                    for (byte i = 0; i < decksize; i++)
                    {
                        byte[] rng = new byte[4];
                        shuffle.GetBytes(rng);
                        int id = BitConverter.ToInt32(rng, 0);
                        int index = new Random(id).Next(randomcards.Count());
                        cards.Add(randomcards[index]);
                    }
                }
                else
                {
                    for (byte i = 0; i < decksize; i++)
                    {
                        cards.Add(defaultdeck[i]);
                    }
                }
            }
        }
        public string Draw()
        {
            if (cards.Count > 0)
            {
                byte[] rand = new byte[4];
                shuffle.GetBytes(rand);
                int val = BitConverter.ToInt32(rand, 0);
                int index = new Random(val).Next(cards.Count());
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
