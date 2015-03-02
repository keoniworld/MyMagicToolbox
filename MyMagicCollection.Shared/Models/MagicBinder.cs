using System.Collections.Generic;

namespace MyMagicCollection.Shared.Models
{
	public class MagicBinder
	{
		public MagicBinder()
			: this(new List<MagicBinderCard>())
		{
		}

		public MagicBinder(IList<MagicBinderCard> cards)
		{
			Cards = cards;
		}

		public string Name { get; set; }

		public IList<MagicBinderCard> Cards { get; private set; }
	}
}