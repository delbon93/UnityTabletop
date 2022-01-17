using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace PlayingCards.DeckTemplates {
    [Serializable]
    public class CardTemplateGeneratorProperty : ICardTemplateGenerator {

        public CardTemplateGeneratorType templateGeneratorType;

        public AllSuitsOfFacesTemplateGenerator allSuitsOfFacesTemplateGenerator = 
            new AllSuitsOfFacesTemplateGenerator();

        public AllFacesOfSuitsTemplateGenerator allFacesOfSuitsTemplateGenerator =
            new AllFacesOfSuitsTemplateGenerator();

        public CardListTemplateGenerator cardListTemplateGenerator = new CardListTemplateGenerator();

        public List<Card> Generate () {
            return templateGeneratorType switch {
                CardTemplateGeneratorType.AllSuitsOfFaces => allSuitsOfFacesTemplateGenerator.Generate(),
                CardTemplateGeneratorType.AllFacesOfSuits => allFacesOfSuitsTemplateGenerator.Generate(),
                CardTemplateGeneratorType.CardList => cardListTemplateGenerator.Generate(),
                _ => new List<Card>()
            };
        }
    }
}