using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayingCards.DeckTemplates {
    [Serializable]
    public class AllSuitsOfFacesTemplateGenerator : ICardTemplateGenerator {

        public List<CardFaces> faces;
        
        public List<Card> Generate () {
            var cards = new List<Card>();
            foreach (var face in faces) {
                cards.Add(new Card(face, CardSuits.Clubs));
                cards.Add(new Card(face, CardSuits.Diamonds));
                cards.Add(new Card(face, CardSuits.Spades));
                cards.Add(new Card(face, CardSuits.Hearts));
            }
            return cards;
        }
    }
}