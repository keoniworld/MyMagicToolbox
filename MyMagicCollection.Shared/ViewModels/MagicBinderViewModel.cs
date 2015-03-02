using System.Collections.Generic;
using System.Linq;
using MyMagicCollection.Shared.FileFormats.MyMagicCollection;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.ViewModels
{
    public class MagicBinderViewModel
    {
        private MagicBinder _magicCollection;
        private string _fileName;

        private IDictionary<string, MagicBinderCardViewModel> _sortedCards;

        public MagicBinderViewModel()
            :this(null)
        {
        }

        public MagicBinderViewModel(string name)
        {
            _sortedCards = new Dictionary<string, MagicBinderCardViewModel>();
            if (!string.IsNullOrEmpty(name))
            {
                _magicCollection = new MagicBinder
                {
                    Name = name,
                };
            }
        }

        public IEnumerable<MagicBinderCardViewModel> Cards { get; private set; }

        public string Name => _magicCollection.Name;

        public void ReadFile(string fileName)
        {
            var loader = new MyMagicCollectionCsv();
            _magicCollection = loader.ReadFile(fileName);

            // Now wrap every card with a view model:
            Cards = _magicCollection.Cards
                .Select(c => new MagicBinderCardViewModel(StaticMagicData.CardDefinitionsByCardId[c.CardId], c));

            _sortedCards = Cards.ToDictionary(c => c.RowId);
            _fileName = fileName;
        }

        public void WriteFile()
        {
            WriteFile(_fileName);
        }

        public void WriteFile(string fileName)
        {
            if (_magicCollection == null)
            {
                return;
            }

            var loader = new MyMagicCollectionCsv();
            loader.WriteFile(fileName, _magicCollection);
            _fileName = fileName;
        }

        // TODO: Modifikationsoperationen
    }
}