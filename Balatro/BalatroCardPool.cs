using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BalatroSim
{
    //1~13 A~K
    //100,200,300,400 heart, diamond, spade, club

    public class BalatroCardPool
    {
        public List<int> cards;
        int CARD_NUMBER = 13;
        int CARD_SUIT = 4;
        int CARD_SUIT_MULTIPLE = 100;

        public BalatroCardPool()
        {
            cards = new();
            for(int i=0; i<CARD_SUIT; i++)
            {
                for(int j=0; j<CARD_NUMBER; j++)
                {
                    cards.Add(i * CARD_SUIT_MULTIPLE + j);
                }
            }
        }

        public List<int> Draw(int num)
        {
            List<int> sel = new();
            for(int i=0; i<num; i++)
            {
                int index = Random.Range(0, cards.Count - 1 - i);

                int index_fix = 0;
                for(int j=0; j<sel.Count; j++)
                {

                }
                index -= index_fix;

                
                sel.Add(cards[index]);
            }

            return sel;
        }

    }
}
