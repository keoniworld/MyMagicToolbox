using System.Diagnostics;

namespace MyMagicCollection.Shared.Models
{
    [DebuggerDisplay("{CardId} - {NameEN} - {SetCode}")]
    public class MagicCardDefinition : IMagicCardDefinition
    {
		private string _displayNameEnCache;

        public string CardId { get; set; }

        public string NameEN { get; set; }

        public string NameDE { get; set; }

        public string NameMkm { get; set; }

		public string DisplayNameEn
		{
			get
			{
				if (_displayNameEnCache == null)
				{
					_displayNameEnCache = string.IsNullOrWhiteSpace(NameMkm) ? NameEN : NameMkm;
                }
				return _displayNameEnCache;
            }
		}

        public string SetCode { get; set; }

        public string CardType { get; set; }

        public string RulesText { get; set; }

        public string RulesTextDE { get; set; }

        public string NumberInSet { get; set; }

        public bool? LegalityStandard { get; set; }

        public bool? LegalityModern { get; set; }

        public bool? LegalityVintage { get; set; }

        public bool? LegalityPauper { get; set; }

        public bool? LegalityLegacy { get; set; }

        public bool? LegalityCommander { get; set; }

        public bool? LegalityFrenchCommander { get; set; }

        public MagicRarity? Rarity { get; set; }

        public string ManaCost { get; set; }

        public int? ConvertedManaCost { get; set; }

        public MagicCardType MagicCardType { get; set; }

        public string CardLayout { get; set; }
    }
}