using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.ViewModels;

namespace MyMagicCollection.Shared.DataSource
{
    public class ActiveBinderDataSource : MagicDataDataSourceBase
    {
        private readonly MagicBinderViewModel _activeBinder;

        public ActiveBinderDataSource(MagicBinderViewModel activeBinder)
        {
            _activeBinder = activeBinder;
        }

        public override IEnumerable<IMagicCardDefinition> CardDefinitions => _activeBinder.Cards.ToList();

        protected override IEnumerable<FoundMagicCardViewModel> MapResult(IEnumerable<IMagicCardDefinition> result)
        {
            // var ordered = _activeBinder.Cards.ToDictionary(c => c.CardId);

            return result.Select(c => new FoundMagicCardViewModel((MagicBinderCardViewModel)c)).OrderBy(c => c.NameEN).ToList();
        }
    }
}