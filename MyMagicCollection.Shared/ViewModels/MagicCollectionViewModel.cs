using System.Collections.Generic;
using System.Linq;
using MyMagicCollection.Shared.FileFormats.MyMagicCollection;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.ViewModels
{
    public class MagicCollectionViewModel
    {
        private MagicCollection _magicCollection;

        private IDictionary<string, MagicCollectionCardViewModel> _sortedCards;

        public MagicCollectionViewModel()
        {
            _sortedCards = new Dictionary<string, MagicCollectionCardViewModel>();
        }

        public IEnumerable<MagicCollectionCardViewModel> Cards { get; private set; }

        public void ReadFile(string fileName)
        {
            var loader = new MyMagicCollectionCsv();
            _magicCollection = loader.ReadFile(fileName);

            // Now wrap every card with a view model:
            Cards = _magicCollection.Cards
                .Select(c => new MagicCollectionCardViewModel(StaticMagicData.CardDefinitionsByCardId[c.CardId], c));

            _sortedCards = Cards.ToDictionary(c => c.RowId);
        }

        public void WriteFile(string fileName)
        {
            if (_magicCollection == null)
            {
                return;
            }

            var loader = new MyMagicCollectionCsv();
            loader.WriteFile(fileName, _magicCollection);
        }

        // TODO: Modifikationsoperationen
    }
}