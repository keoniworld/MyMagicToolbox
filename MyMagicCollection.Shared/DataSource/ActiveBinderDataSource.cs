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

		public override IEnumerable<MagicCardDefinition> CardDefinitions => _activeBinder.Cards.Select(c=>c.Definition).ToList();
    }
}