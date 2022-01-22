using System.Collections.Generic;
using System.Linq;
using Games.MauMau.Source;
using PlayingCards.Components;
using UnityEngine;

namespace Games.MauMau {
    public class MauMauUIManager : MonoBehaviour {
        
        public const string PlayerTag = "$player$";

        [SerializeField] private EventLog eventLogUi;
        
        public void Log (PlayerInfo player, string message) {
            var preppedMessage = ReplacePlaceholders(player, message);
            Log(preppedMessage);
        }

        public void Log (string message) {
            print($"[EventLog] {message}");
            eventLogUi.AddLine(message);
        }

        private string ReplacePlaceholders (PlayerInfo player, string str) {
            var colorCode = GetRichTextColorCode(player.color);
            return str.Replace(PlayerTag, $"<color={colorCode}>{player.name}</color>");
        }

        private static string GetRichTextColorCode (Color color) => $"#{ColorUtility.ToHtmlStringRGB(color)}";

    }
}