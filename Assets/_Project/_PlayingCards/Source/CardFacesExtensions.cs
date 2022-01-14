using System.Collections.Generic;

namespace _Project.Source.Common.Cards {
    public static class CardFacesExtensions {
        
        private static readonly Dictionary<CardFaces, string> _resourceIdMap = new Dictionary<CardFaces, string>() {
            { CardFaces.Ace, "A" },
            { CardFaces.Two, "2" },
            { CardFaces.Three, "3" },
            { CardFaces.Four, "4" },
            { CardFaces.Five, "5" },
            { CardFaces.Six, "6" },
            { CardFaces.Seven, "7" },
            { CardFaces.Eight, "8" },
            { CardFaces.Nine, "9" },
            { CardFaces.Ten, "T" },
            { CardFaces.Jack, "J" },
            { CardFaces.Queen, "Q" },
            { CardFaces.King, "K" },
        };

        public static string ResourceId (this CardFaces face) => _resourceIdMap[face];

        public static string DisplayName (this CardFaces face) => face.ToString();

    }
}