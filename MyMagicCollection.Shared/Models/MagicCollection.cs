using System.Collections.Generic;

namespace MyMagicCollection.Shared.Models
{
	public class MagicCollection
	{
		public MagicCollection()
			: this(new List<MagicCollectionCard>())
		{
		}

		public MagicCollection(IList<MagicCollectionCard> cards)
		{
			Cards = cards;
		}

		public string Name { get; set; }

		public IList<MagicCollectionCard> Cards { get; private set; }
	}
}