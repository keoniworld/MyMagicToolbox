using System.Collections.Generic;
using MagicFileContracts;
namespace MagicDatabase
{
    public interface ICardDatabase
    {
        string DatabaseName { get; }

        void Dispose();

        Card FindCardById(string magicCardId);

        IEnumerable<Card> FindCards(ICardSearchModel searchModel);

        IEnumerable<Set> GetAllSets();
    }
}