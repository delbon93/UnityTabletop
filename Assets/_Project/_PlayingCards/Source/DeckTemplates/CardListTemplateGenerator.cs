using System;
using System.Collections.Generic;

namespace PlayingCards.DeckTemplates {
    [Serializable]
    public class CardListTemplateGenerator : ICardTemplateGenerator {

        public List<Card> cards;


        public List<Card> Generate () => cards;
    }
}