using System;
using System.Collections.Generic;

namespace PlayingCards.DeckTemplates {
    public interface ICardTemplateGenerator {

        public List<Card> Generate ();

    }
}