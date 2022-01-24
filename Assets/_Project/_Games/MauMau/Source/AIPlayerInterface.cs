using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayingCards;
using PlayingCards.Components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Games.MauMau {
    public class AIPlayerInterface : APlayerInterface {

        public override IEnumerator TakeTurn () {
            yield return new WaitForSeconds(1.25f);
            var playableCards = PlayerInfo.hand.CardContainer.Where(Manager.IsCardPlayable).ToList();
            
            if (playableCards.Count > 0) {
                var chosenCard = playableCards[Random.Range(0, playableCards.Count)];
                yield return Manager.PlayCard(PlayerInfo.hand, chosenCard);
            }
            else {
                yield return Manager.DrawFromDeck(PlayerInfo.hand);
            }

            yield return new WaitForSeconds(0.5f);
        }
        
        public override IEnumerator SelectJackSuit () {
            var counts = new Dictionary<CardSuits, int> {
                { CardSuits.Clubs, 0 },
                { CardSuits.Diamonds, 0 },
                { CardSuits.Hearts, 0 },
                { CardSuits.Spades, 0 },
            };

            foreach (var playingCard in PlayerInfo.hand.CardContainer) {
                counts[playingCard.Card.suit]++;
            }

            var maxSuit = CardSuits.Clubs;
            var maxCount = 0;
            foreach (var suit in counts.Keys) {
                if (counts[suit] <= maxCount) continue;
                maxCount = counts[suit];
                maxSuit = suit;
            }

            Manager.SelectedJackSuit = maxSuit;
            yield return null;
        }

        public override IEnumerator ForcedDrawOrCounter (Func<PlayingCard, bool> counterCardsSelector) {
            var possibleCounters = PlayerInfo.hand.CardContainer.Where(counterCardsSelector).ToList();
            yield return new WaitForSeconds(1.5f);

            if (possibleCounters.Count == 0) {
                yield return Manager.ForcedDraw(PlayerInfo);
            }
            else {
                Manager.TriggerSuccessfulCounter(PlayerInfo);
                yield return Manager.PlayCard(PlayerInfo.hand, possibleCounters[Random.Range(0, possibleCounters.Count)]);
            }
            
        }
    }
}