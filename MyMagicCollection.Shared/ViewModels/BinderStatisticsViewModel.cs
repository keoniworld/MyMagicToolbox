using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minimod.NotificationObject;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.ViewModels
{
    public class BinderStatisticsViewModel : NotificationObject
    {
        private readonly INotificationCenter _notificationCenter;
        private readonly MagicBinderViewModel _binderViewModel;

        private IEnumerable<BinderStatisticsPerSet> _statisticRows;

        private BinderStatisticsPerSet _selectedStatisticRow;

        public BinderStatisticsViewModel(
                    INotificationCenter notificationCenter,
            MagicBinderViewModel binderViewModel)
        {
            _notificationCenter = notificationCenter;
            _binderViewModel = binderViewModel;
        }

        public IEnumerable<BinderStatisticsPerSet> StatisticRows
        {
            get
            {
                return _statisticRows;
            }

            set
            {
                _statisticRows = value;
                RaisePropertyChanged(() => StatisticRows);
            }
        }

        public BinderStatisticsPerSet SelectedStatisticRow
        {
            get
            {
                return _selectedStatisticRow;
            }

            set
            {
                _selectedStatisticRow = value;
                RaisePropertyChanged(() => SelectedStatisticRow);
            }
        }

        public void Recalculate()
        {
            var allCards = _binderViewModel.Cards.ToList();

            var grouped = allCards.GroupBy(c => c.Definition.SetCode);
            var collection = new List<BinderStatisticsPerSet>();
            foreach (var group in grouped)
            {
                collection.Add(new BinderStatisticsPerSet(StaticMagicData.SetDefinitionsBySetCode[group.Key], group));
            }
            StatisticRows = collection.OrderByDescending(i => i.SetDefinition.ReleaseDateTime).ThenBy(i => i.SetDefinition.Name).ToList();
        }
    }
}