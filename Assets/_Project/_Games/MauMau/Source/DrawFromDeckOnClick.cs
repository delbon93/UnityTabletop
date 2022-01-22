using System;
using PlayingCards.Components;
using UnityEngine;

namespace Games.MauMau.Source {
    [RequireComponent(typeof(Collider))]
    public class DrawFromDeckOnClick : MonoBehaviour {

        private void OnMouseDown () {
            MauMauManager.instance.DrawToPlayerHandIfAllowed();
        }
        
    }
}