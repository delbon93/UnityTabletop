using System;
using System.Collections.Generic;
using System.Linq;
using PlayingCards.Components;
using UnityEngine;

namespace Games.MauMau.Source {
    public class MauMauUIManager : MonoBehaviour {
        public const string PlayerTag = "$player$";

        [Serializable]
        public struct PlayerName {
            public PlayingCardHand player;
            public string name;
        }

        [SerializeField] private List<PlayerName> playerNames;
        

        public void Log (PlayingCardHand player, string message) {
            var playerName = GetPlayerName(player);
            print(message.Replace(PlayerTag, playerName));
        }

        public void Log (string message) => Log(null, message);

        private string GetPlayerName (PlayingCardHand player) {
            foreach (var playerName in playerNames.Where(playerName => playerName.player == player))
                return playerName.name;

            return "Unknown Player";
        }
        
    }
}