using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.VieModels
{
    public interface IMagicBinderCardViewModel : IMagicBinderCard
    {
        IMagicCardDefinition Definition { get; }

        string NameEN { get; }
        string Comment { get; }
    }
}