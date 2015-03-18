﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared.Models
{
    public interface IMagicCardDefinition
    {
        string CardId { get; }

        string NameEN { get; }

        string NameDE { get; }

        string SetCode { get; }

        string RulesText { get; }

        string RulesTextDE { get; }

        MagicRarity? Rarity { get; }
    }
}