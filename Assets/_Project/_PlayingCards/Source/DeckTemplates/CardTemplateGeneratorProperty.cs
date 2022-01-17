using System;
using System.Collections.Generic;

namespace PlayingCards.DeckTemplates {
    [Serializable]
    public class CardTemplateGeneratorProperty : ICardTemplateGenerator {

        public CardTemplateGenerators templateGeneratorType;

        public AllSuitsOfFacesTemplateGenerator allSuitsOfFacesTemplateGenerator = 
            new AllSuitsOfFacesTemplateGenerator();

        public AllFacesOfSuitsTemplateGenerator allFacesOfSuitsTemplateGenerator =
            new AllFacesOfSuitsTemplateGenerator();

        public List<Card> Generate () {
            return templateGeneratorType switch {
                CardTemplateGenerators.AllSuitsOfFaces => allSuitsOfFacesTemplateGenerator.Generate(),
                CardTemplateGenerators.AllFacesOfSuits => allFacesOfSuitsTemplateGenerator.Generate(),
                _ => new List<Card>()
            };
        }
    }
}