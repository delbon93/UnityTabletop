using System.Collections;
using PlayingCards.Components;
using UnityEngine;

namespace Games.MauMau {
    public abstract class APlayerInterface : MonoBehaviour {

        [SerializeField] private PlayerInfo playerInfo;
        
        protected MauMauManager Manager => MauMauManager.instance;

        public PlayerInfo PlayerInfo => playerInfo;

        public abstract IEnumerator TakeTurn ();
        public abstract IEnumerator SelectJackSuit ();

    }
}