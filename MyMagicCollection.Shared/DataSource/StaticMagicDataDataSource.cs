using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.ViewModels;

namespace MyMagicCollection.Shared.DataSource
{
	public class StaticMagicDataDataSource : MagicDataDataSourceBase
	{
		public override IEnumerable<MagicCardDefinition> CardDefinitions => StaticMagicData.CardDefinitions;
	}
}