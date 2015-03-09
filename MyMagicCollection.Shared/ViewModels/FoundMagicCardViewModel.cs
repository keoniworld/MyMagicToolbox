using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.ViewModels
{
    /// <summary>
    /// This view model is a wrapper for all possible search result type.
    /// </summary>
    public class FoundMagicCardViewModel
    {
        private readonly MagicCardDefinition _definition;

        private readonly MagicBinderCardViewModel _viewModel;

        public FoundMagicCardViewModel(MagicCardDefinition definition)
        {
            _definition = definition;

            MagicSetDefinition set;
            if (StaticMagicData.SetDefinitionsBySetCode.TryGetValue(_definition.SetCode, out set))
            {
                SetName = set.Name;
            }
        }

        public FoundMagicCardViewModel(MagicBinderCardViewModel card)
        {
            _definition = card.Definition;
            MagicSetDefinition set;
            if (StaticMagicData.SetDefinitionsBySetCode.TryGetValue(_definition.SetCode, out set))
            {
                SetName = set.Name;
            }

            _viewModel = card;
        }

        public string NameEN => _definition.NameEN;

        public string NameDE => _definition.NameDE;

        public string SetCode => _definition.SetCode;

        public int? Quantity => _viewModel?.Quantity;

        public int? QuantityTrade => _viewModel?.QuantityTrade;

        public MagicCardDefinition Definition => _definition;

        public string SetName { get; private set; }

        public MagicLanguage? Language => _viewModel?.Language;

        public MagicGrade? Grade => _viewModel?.Grade;

        public decimal? Price => _viewModel?.Price;

        public void UpdatePriceData(bool writeDatabase)
        {
            if (_viewModel != null)
            {
                _viewModel.UpdatePriceData(writeDatabase, true);
            }
        }

        // TODO: Add set code switching
    }
}