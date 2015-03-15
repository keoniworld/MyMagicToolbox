using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minimod.NotificationObject;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.VieModels;

namespace MyMagicCollection.Shared.ViewModels
{
    /// <summary>
    /// This view model is a wrapper for all possible search result type.
    /// </summary>
    public class FoundMagicCardViewModel : NotificationObject, IMagicBinderCardViewModel
    {
        private readonly MagicBinderCardViewModel _viewModel;

        public FoundMagicCardViewModel(IMagicCardDefinition definition)
        {
            _viewModel = new MagicBinderCardViewModel(definition, new MagicBinderCard() { CardId = definition.CardId });
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            UpdateSetData();
        }

        public FoundMagicCardViewModel(MagicBinderCardViewModel card)
        {
            _viewModel = card;
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            UpdateSetData();
        }

        ~FoundMagicCardViewModel()
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        public string CardId => _viewModel?.CardId;

        public string NameEN => _viewModel?.Definition?.NameEN;

        public string NameDE => _viewModel?.Definition?.NameDE;

        public string RulesText => _viewModel?.Definition?.RulesText;

        public string RulesTextDE => _viewModel?.Definition?.RulesTextDE;

        public string SetCode => _viewModel?.Definition?.SetCode;

        public int Quantity => _viewModel != null ? _viewModel.Quantity : 0;

        public int QuantityTrade => _viewModel != null ? _viewModel.QuantityTrade : 0;

        public int QuantityWanted => _viewModel != null ? _viewModel.QuantityWanted : 0;

        public bool IsFoil => _viewModel != null ? _viewModel.IsFoil : false;

        public IMagicCardDefinition Definition => _viewModel?.Definition;

        public string SetName { get; private set; }

        public MagicLanguage? Language => _viewModel?.Language;

        public MagicGrade? Grade => _viewModel?.Grade;

        public decimal? Price => _viewModel?.Price;

        public DateTime? PriceUpdateUtc => _viewModel?.CardPrice?.UpdateUtc;

        public void UpdatePriceData(bool writeDatabase)
        {
            if (_viewModel != null)
            {
                _viewModel.UpdatePriceData(writeDatabase, true);
            }
        }

        private void UpdateSetData()
        {
            MagicSetDefinition set;
            if (StaticMagicData.SetDefinitionsBySetCode.TryGetValue(_viewModel?.Definition?.SetCode, out set))
            {
                SetName = set.Name;
            }

            RaisePropertyChanged(() => SetName);
            RaisePropertyChanged(() => SetCode);
            RaisePropertyChanged(() => NameEN);
            RaisePropertyChanged(() => NameDE);
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName.ToLowerInvariant())
            {
                case "price":
                    RaisePropertyChanged(() => Price);
                    RaisePropertyChanged(() => PriceUpdateUtc);
                    break;

                case "grade":
                    RaisePropertyChanged(() => Grade);
                    break;

                case "language":
                    RaisePropertyChanged(() => Language);
                    break;

                case "definition":
                    UpdateSetData();
                    RaisePropertyChanged(() => Definition);
                    break;
            }
        }
    }
}