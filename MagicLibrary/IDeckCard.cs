using System;
namespace MagicLibrary
{
    public interface IDeckCard
    {
        string CardId { get; set; }
        bool IsFoil { get; set; }
        string Location { get; set; }
        string Name { get; set; }
        int Quantity { get; set; }
        string Set { get; set; }
        string SetCode { get; set; }
        // int? NumberInSet { get; set; }
    }
}
