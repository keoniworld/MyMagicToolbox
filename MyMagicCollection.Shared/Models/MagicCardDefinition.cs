namespace MyMagicCollection.Shared.Models
{
    public class MagicCardDefinition
    {
        public int Id { get; set; }

        public string CardId { get; set; }

        public string NameEN { get; set; }

        public string NameDE { get; set; }

        public string SetCode { get; set; }

        public string CardType { get; set; }

        public string RulesText { get; set; }

        public string RulesTextDE { get; set; }

        public int? NumberInSet { get; set; }

        public bool? LegalityStandard { get; set; }

        public bool? LegalityModern { get; set; }

        public bool? LegalityVintage { get; set; }

        public bool? LegalityPauper { get; set; }

        public bool? LegalityLegacy { get; set; }

        public bool? LegalityCommander { get; set; }

        public bool? LegalityFrenchCommander { get; set; }
    }
}