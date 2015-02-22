using System.Collections.Generic;
using MagicLibrary;
using MyMagicCollection.Shared;
using MyMagicCollection.Shared.Models;

namespace MagicDatabase
{
    public interface ICardDatabase
    {
        string DatabaseName { get; }

        void Dispose();

        MagicCardDefinition FindCardById(string magicCardId);

        IEnumerable<MagicCardDefinition> FindCards(ICardSearchModel searchModel);

        IEnumerable<Set> GetAllSets();
    }
}