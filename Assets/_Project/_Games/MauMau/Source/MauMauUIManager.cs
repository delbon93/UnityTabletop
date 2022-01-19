using System;
using System.Collections.Generic;
using System.Linq;
using PlayingCards.Components;
using UnityEngine;

namespace Games.MauMau.Source {
    public class MauMauUIManager : MonoBehaviour {
        
        public const string PlayerTag = "$player$";

        [Serializable]
        public struct PlayerInfo {
            public PlayingCardHand player;
            public string name;
            public Color color;
        }

        
        [SerializeField] private List<PlayerInfo> playerNames;
        [SerializeField] private EventLog eventLogUi;
        

        public void Log (PlayingCardHand player, string message) {
            var preppedMessage = ReplacePlaceholders(GetPlayerInfo(player), message);
            print($"[EventLog] {preppedMessage}");
            eventLogUi.AddLine(preppedMessage);
        }

        public void Log (string message) => Log(null, message);

        private string ReplacePlaceholders (PlayerInfo player, string str) {
            var colorCode = GetRichTextColorCode(player.color);
            return str.Replace(PlayerTag, $"<color={colorCode}>{player.name}</color>");
        }

        private static string GetRichTextColorCode (Color color) => $"#{ColorUtility.ToHtmlStringRGB(color)}";

        private PlayerInfo GetPlayerInfo (PlayingCardHand player) {
            foreach (var playerName in playerNames.Where(playerName => playerName.player == player))
                return playerName;
            return default;
        }
        
    }
}