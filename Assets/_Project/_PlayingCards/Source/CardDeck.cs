using System.Collections.Generic;
using System.Linq;

namespace _Project.Source.Common.Cards {
    public class CardDeck {

        private readonly IEnumerable<int> _initialCardIds;

        public int CardCount => _initialCardIds.Count();

        public List<Card> ToCardList () {
            return _initialCardIds.Select(Card.ConstructFromCardId).ToList();
        }

        public CardDeck (IEnumerable<int> initialCardIds) {
            _initialCardIds = initialCardIds;
        }

    }
}