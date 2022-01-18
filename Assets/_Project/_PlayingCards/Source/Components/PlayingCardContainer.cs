using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayingCards.Components;
using UnityEngine;

namespace PlayingCards {
    public class PlayingCardContainer : MonoBehaviour, IEnumerable<PlayingCard>, IPlayingCardContainerProvider {

        private readonly List<PlayingCard> _playingCards = new List<PlayingCard>();

        public Action<PlayingCard> OnPlayingCardEnter { get; set; } = (playingCard) => { };
        public Action<PlayingCard> OnPlayingCardLeave { get; set; } = (playingCard) => { };
        public Action OnCardOrderChange { get; set; } = () => { };

        public int Count => _playingCards.Count;

        public PlayingCardContainer CardContainer => this;

        public PlayingCard this [int index] => _playingCards[index];

        public PlayingCard First => Count > 0 ? this[0] : null;
        public PlayingCard Last => Count > 0 ? this[Count - 1] : null;

        #region Move Methods

        public PlayingCard Take () => TakeAt(Count - 1);

        public IEnumerable<PlayingCard> Take (int count) {
            if (Count < count) count = Count;
            var list = _playingCards.GetRange(Count - count, count);
            _playingCards.RemoveRange(Count - count, count);
            
            list.ForEach((playingCard) => {
                Unparent(playingCard);
                OnPlayingCardLeave(playingCard);
            });
            return list;
        }

        public PlayingCard Take (PlayingCard playingCard) {
            var index = _playingCards.IndexOf(playingCard);
            return index < 0 ? null : TakeAt(index);
        }

        public PlayingCard TakeAt (int index) {
            if (Count == 0) return null;

            var playingCard = _playingCards[index];
            _playingCards.RemoveAt(index);
            
            Unparent(playingCard);
            OnPlayingCardLeave(playingCard);
            return playingCard;
        }
        

        public IEnumerable<PlayingCard> TakeAll () => Take(Count);

        public void Put (PlayingCard playingCard) => PutAt(playingCard, Count);

        public void Put (IEnumerable<PlayingCard> playingCards) => playingCards.ToList().ForEach(Put);

        public void PutAt (PlayingCard playingCard, int index) {
            _playingCards.Insert(index, playingCard);
            Parent(playingCard);
            OnPlayingCardEnter(playingCard);
        }
        
        public void TransferTo (IPlayingCardContainerProvider destination) 
            => destination.CardContainer.Put(Take());

        public void TransferTo (IPlayingCardContainerProvider destination, int count) 
            => destination.CardContainer.Put(Take(count));

        public void TransferTo (IPlayingCardContainerProvider destination, PlayingCard playingCard) =>
            destination.CardContainer.Put(Take(playingCard));
        
        public void TransferToAt (int sourceIndex, IPlayingCardContainerProvider destination) {
            destination.CardContainer.Put(TakeAt(sourceIndex));
        }

        public void TransferToAt (IPlayingCardContainerProvider destination, int destinationIndex) {
            destination.CardContainer.PutAt(Take(), destinationIndex);
        }

        public void TransferToAt (int sourceIndex, IPlayingCardContainerProvider destination, int destinationIndex) {
            destination.CardContainer.PutAt(TakeAt(sourceIndex), destinationIndex);
        }

        public void TransferAllTo (IPlayingCardContainerProvider destination) 
            => destination.CardContainer.Put(TakeAll());
        
        #endregion

        public void Shuffle () {
            var n = Count;
            var rng = new System.Random();
            while (n > 1) {
                n--;
                var k = rng.Next(n);
                (_playingCards[k], _playingCards[n]) = (_playingCards[n], _playingCards[k]);
            }

            OnCardOrderChange();
        }

        public void Sort (IComparer<PlayingCard> comparer) {
            _playingCards.Sort(comparer);
            
            OnCardOrderChange();
        }

        private void Parent (PlayingCard playingCard) {
            playingCard.transform.SetParent(transform);
        }

        private void Unparent (PlayingCard playingCard) {
            playingCard.transform.SetParent(null);
        }

        public IEnumerator<PlayingCard> GetEnumerator () => _playingCards.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}