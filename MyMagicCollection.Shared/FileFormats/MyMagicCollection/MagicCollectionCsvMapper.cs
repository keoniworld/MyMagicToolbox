using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.FileFormats.MyMagicCollection
{
	public sealed class MagicCollectionCsvMapper : CsvClassMap<MagicCollection>
	{
		public MagicCollectionCsvMapper()
		{
			Map(m => m.Cards).Ignore();
		}
	}
}
