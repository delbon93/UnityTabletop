using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Games.MauMau.Source;
using PlayingCards.Components;
using UnityEngine;

namespace Games.MauMau {
    public class MauMauUIManager : MonoBehaviour {
        
        public const string PlayerTag = "$player$";

        [SerializeField] private EventLog eventLogUi;

        public bool ShowDebugMessages { get; set; } = true;
        
        public void Log (PlayerInfo player, string message) {
            var preppedMessage = ReplacePlaceholders(player, message);
            Log(preppedMessage);
        }

        public void Log (string message) {
            print($"[EventLog] {message}");
            eventLogUi.AddLine(message);
        }

        public void DebugLog (string message, [CallerMemberName] string caller = "", 
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLine = -1) {
            if (!ShowDebugMessages) return;

            var fileName = Path.GetFileName(callerFilePath);
            Log($"<color=red>[DEBUG]</color> <color=#888>{fileName}:{callerLine}</color> in {caller}: {message}");
        }

        private string ReplacePlaceholders (PlayerInfo player, string str) {
            var colorCode = GetRichTextColorCode(player.color);
            return str.Replace(PlayerTag, $"<color={colorCode}>{player.name}</color>");
        }

        private static string GetRichTextColorCode (Color color) => $"#{ColorUtility.ToHtmlStringRGB(color)}";

    }
}