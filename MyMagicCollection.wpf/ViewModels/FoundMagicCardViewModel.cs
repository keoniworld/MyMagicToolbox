using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.wpf.ViewModels
{
    /// <summary>
    /// This view model is a wrapper for all possible search result type.
    /// </summary>
    public class FoundMagicCardViewModel
    {
        private readonly MagicCardDefinition _definition;

        public FoundMagicCardViewModel(MagicCardDefinition definition)
        {
            _definition = definition;

            MagicSetDefinition set;
            if (StaticMagicData.SetDefinitionsBySetCode.TryGetValue(_definition.SetCode, out set))
            {
                SetName = set.Name;
            }
        }

        public string NameEN => _definition.NameEN;

        public string NameDE => _definition.NameDE;

        public string SetCode => _definition.SetCode;

        // TODO: Das wieder raus?
        public MagicCardDefinition Definition => _definition;

        public string SetName { get; private set; }
    }
}