using System.Collections.Generic;
using System.Linq;
using MyMagicCollection.Shared.FileFormats.MyMagicCollection;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.ViewModels
{
    public class MagicBinderViewModel
    {
        private MagicCollection _magicCollection;

        private IDictionary<string, MagicBinderCardViewModel> _sortedCards;

        public MagicBinderViewModel()
        {
            _sortedCards = new Dictionary<string, MagicBinderCardViewModel>();
        }

        public IEnumerable<MagicBinderCardViewModel> Cards { get; private set; }

        public void ReadFile(string fileName)
        {
            var loader = new MyMagicCollectionCsv();
            _magicCollection = loader.ReadFile(fileName);

            // Now wrap every card with a view model:
            Cards = _magicCollection.Cards
                .Select(c => new MagicBinderCardViewModel(StaticMagicData.CardDefinitionsByCardId[c.CardId], c));

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