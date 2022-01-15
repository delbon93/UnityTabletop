using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayingCards.Components;
using UnityEngine;

namespace PlayingCards {
    public class PlayingCardContainer : IEnumerable<PlayingCard> {

        private readonly List<PlayingCard> _playingCards = new List<PlayingCard>();

        public Transform ManagedTransform { get; set; } = null;

        public Action<PlayingCard> OnPlayingCardEnter { get; set; } = (playingCard) => { };
        public Action<PlayingCard> OnPlayingCardLeave { get; set; } = (playingCard) => { };
        public Action OnCardOrderChange { get; set; } = () => { };

        public int Count => _playingCards.Count;

        public PlayingCard this [int index] => _playingCards[index];

        public PlayingCardContainer (Transform managedTransform = null) {
            ManagedTransform = managedTransform;
        }
        
        #region Move Methods

        public PlayingCard Take () => TakeAt(Count - 1);

        public IEnumerable<PlayingCard> Take (int count) {
            if (Count < count) count = Count;
            var list = _playingCards.GetRange(Count - count, count);
            _playingCards.RemoveRange(Count - count, count);
            
            list.ForEach(Unparent);
            list.ForEach(OnPlayingCardLeave);
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

        public void Put (IEnumerable<PlayingCard> playingCards) {
            var playingCardsList = playingCards.ToList();
            
            playingCardsList.ToList().ForEach(playingCard => {
                _playingCards.Add(playingCard);
                Parent(playingCard);
                OnPlayingCardEnter(playingCard);
            });
        }

        public void PutAt (PlayingCard playingCard, int index) {
            _playingCards.Insert(index, playingCard);
            Parent(playingCard);
            OnPlayingCardEnter(playingCard);
        }
        
        public void TransferTo (PlayingCardContainer destination) => destination.Put(Take());

        public void TransferTo (PlayingCardContainer destination, int count) => destination.Put(Take(count));

        public void TransferTo (PlayingCardContainer destination, PlayingCard playingCard) =>
            destination.Put(Take(playingCard));
        
        public void TransferToAt (int sourceIndex, PlayingCardContainer destination) {
            destination.Put(TakeAt(sourceIndex));
        }

        public void TransferToAt (PlayingCardContainer destination, int destinationIndex) {
            destination.PutAt(Take(), destinationIndex);
        }

        public void TransferToAt (int sourceIndex, PlayingCardContainer destination, int destinationIndex) {
            destination.PutAt(TakeAt(sourceIndex), destinationIndex);
        }

        public void TransferAllTo (PlayingCardContainer destination) => destination.Put(TakeAll());
        
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
            playingCard.transform.SetParent(ManagedTransform);
        }

        private void Unparent (PlayingCard playingCard) {
            playingCard.transform.SetParent(null);
        }

        public IEnumerator<PlayingCard> GetEnumerator () => _playingCards.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}