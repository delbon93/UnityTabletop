using System;
using PlayingCards.Components;
using UnityEngine;

namespace Games.MauMau.Source {
    [RequireComponent(typeof(Collider))]
    public class DrawFromDeckOnClick : MonoBehaviour {

        private PlayingCardDeck _deck;

        private void Start () {
            _deck = GetComponent<PlayingCardDeck>();
        }

        private void OnMouseDown () {
            MauMauManager.instance.DrawToPlayerHandIfAllowed(_deck.CardContainer);
        }
        
    }
}