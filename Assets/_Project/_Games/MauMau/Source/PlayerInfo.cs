using System;
using PlayingCards.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace Games.MauMau {
    [Serializable]
    public struct PlayerInfo {
        [FormerlySerializedAs("player")] public PlayingCardHand hand;
        public string name;
        public Color color;
    }
}