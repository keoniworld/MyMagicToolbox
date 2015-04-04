using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyMagicCollection.Shared;
using MyMagicCollection.Shared.Helper;
using MyMagicCollection.Shared.Models;
using MyMagicCollection.Shared.ViewModels;

namespace MyMagicCollection.wpf.UserControls
{
    /// <summary>
    /// Interaction logic for BinderStatisticsControl.xaml
    /// </summary>
    public partial class BinderStatisticsControl : UserControl
    {
        public static readonly DependencyProperty SelectedCardProperty =
            DependencyProperty.Register("Binder", typeof(MagicBinderViewModel),
            typeof(BinderStatisticsControl), new FrameworkPropertyMetadata(OnBinderChanged));

        private readonly INotificationCenter _notificationCenter;

        private BinderStatisticsViewModel _viewModel;

        public BinderStatisticsControl(INotificationCenter notificationCenter)
        {
            InitializeComponent();
            _notificationCenter = notificationCenter;
        }

        public BinderStatisticsControl()
            : this(NotificationCenter.Instance)
        {
        }

        public event EventHandler<BinderStatisticsDisplayDetails> DisplayDetails;

        public MagicBinderViewModel Binder
        {
            get { return (MagicBinderViewModel)GetValue(SelectedCardProperty); }
            set { SetValue(SelectedCardProperty, value); }
        }

        public static void OnBinderChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var instance = d as BinderStatisticsControl;
            if (instance != null)
            {
                instance._viewModel = instance.Binder != null
                    ? new BinderStatisticsViewModel(instance._notificationCenter, instance.Binder)
                    : null;

                instance.rootGrid.DataContext = instance._viewModel;
            }
        }

        private void OnRefreshViewModel(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.Recalculate();
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedStatisticRow != null)
            {
                var detailsEvent = DisplayDetails;
                if (detailsEvent != null)
                {
                    detailsEvent(this, new BinderStatisticsDisplayDetails() { Details = _viewModel.SelectedStatisticRow });
                }
            }
        }
    }
}