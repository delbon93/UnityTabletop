using System.Collections.Generic;
using PlayingCards.DeckTemplates;
using UnityEngine;

namespace PlayingCards.ScriptableObjects {
    [CreateAssetMenu(fileName = "Playing Card Deck Template", menuName = "Playing Cards/Deck Template", order = 0)]
    public class PlayingCardDeckTemplate : ScriptableObject, ICardTemplateGenerator {

        [SerializeField] private List<CardTemplateGeneratorProperty> cardTemplateGenerators;

        public List<Card> Generate () {
            var cards = new List<Card>();
            foreach (var templateGenerator in cardTemplateGenerators) {
                cards.AddRange(templateGenerator.Generate());
            }
            return cards;
        }
    }
}