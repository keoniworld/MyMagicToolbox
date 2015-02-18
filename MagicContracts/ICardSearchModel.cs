using System;
namespace MagicFileContracts
{
    public interface ICardSearchModel
    {
        bool SearchName { get; set; }
        bool SearchRulesText { get; set; }
        bool DistinctNames { get; set; }
        string SearchTerm { get; set; }
    }
}
