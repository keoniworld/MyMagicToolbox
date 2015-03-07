using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.ViewModels;

namespace MyMagicCollection.Shared.FileFormats
{
    public interface IReadCards
    {
        IEnumerable<MagicBinderCardViewModel> ReadFileContent(string content);
    }
}
