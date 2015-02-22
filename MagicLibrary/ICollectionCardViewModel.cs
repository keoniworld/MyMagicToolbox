using System;
namespace MagicLibrary
{
    public interface ICollectionCardViewModel
    {
        string NameDE { get; }
        string NameEN { get; }
        int Quantity { get; set; }
    }
}
