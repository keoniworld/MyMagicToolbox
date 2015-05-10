using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using MahApps.Metro.Controls;
using Microsoft.Win32;
using MyMagicCollection.Shared;
using MyMagicCollection.Shared.Helper;
using MyMagicCollection.Shared.Models;
using NLog;

namespace MyMagicCollection.wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainViewModel _viewModel = new MainViewModel(NotificationCenter.Instance);

        public MainWindow()
        {
            InitializeComponent();
            AddHotKeys();

            DataContext = _viewModel;

            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Title = "My Magic Toolbox - " + version;
        }

        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.LookupCards();
            }
            catch (Exception error)
            {
                NotificationCenter.Instance.FireNotification(
                    LogLevel.Error,
                    "Lookup cards failed: " + error.Message);
            }
        }

        private void OnNewBinderClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                AddExtension = true,
                CheckPathExists = true,
                Title = "Create new binder",
                Filter = "Binder Files *.mmcbinder|*.mmcbinder",
                DefaultExt = ".mmcbinder",
                InitialDirectory = PathHelper.UserDataFolder,
            };

            if (dialog.ShowDialog(this) == true)
            {
                _viewModel.CreateAndSetNewBinder(dialog.FileName);
            }
        }

        private void OnAddSelectedCardToBinder(object sender, RoutedEventArgs e)
        {
            _viewModel.AddSelectedCardToBinder();
        }

        private void OnShowCollectionCards(object sender, RoutedEventArgs e)
        {
            _viewModel.ShowCollectionCards();
        }

        private void OnImportDeckbox(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog()
                {
                    Multiselect = false,
                    AddExtension = true,
                    CheckPathExists = true,
                    CheckFileExists = true,
                    Title = "Import card list",
                    Filter = "All supported files|*.csv;*.dec|Deckbox.org CSV (*.csv)|*.csv|Deck lists (*.dec)|*.dec",
                    DefaultExt = ".csv",
                    InitialDirectory = PathHelper.UserDataFolder,
                };

                if (dialog.ShowDialog(this) == true)
                {
                    _viewModel.ImportCards(dialog.FileName);
                }
            }
            catch (Exception error)
            {
                NotificationCenter.Instance.FireNotification(
                    LogLevel.Error,
                    "Exporting cards failed: " + error.Message);

                MessageBox.Show(
                    error.Message,
                    "Importing cards failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OnImportAllFromCurrentList(object sender, RoutedEventArgs e)
        {
            _viewModel.AddCurrentDisplayedList();
        }

        private void OnExportDisplayedList(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileName = DisplayExportFileDialog("Export card list");
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    _viewModel.ExportDisplayedCardsList(fileName);
                }
            }
            catch (Exception error)
            {
                NotificationCenter.Instance.FireNotification(
                    LogLevel.Error,
                    "Exporting cards failed: " + error.Message);

                MessageBox.Show(
                    error.Message,
                    "Exporting cards failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private string DisplayExportFileDialog(string title)
        {
            var dialog = new SaveFileDialog()
            {
                AddExtension = true,
                CheckPathExists = true,
                CheckFileExists = false,
                Title = title,
                Filter = "All supported files|*.csv|Deckbox.org CSV (*.csv)|*.csv",
                InitialDirectory = PathHelper.UserDataFolder,
            };

            if (dialog.ShowDialog(this) == true)
            {
                return dialog.FileName;
            }

            return null;
        }

        private void OnOpenBinder(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                AddExtension = true,
                CheckPathExists = true,
                Title = "Open binder",
                Filter = "Binder Files *.mmcbinder|*.mmcbinder",
                DefaultExt = ".mmcbinder",
                InitialDirectory = PathHelper.UserDataFolder,
            };

            if (dialog.ShowDialog(this) == true)
            {
                _viewModel.OpenBinder(dialog.FileName);
            }
        }

        private void OnPriceCollecttion(object sender, RoutedEventArgs e)
        {
            _viewModel.PriceActiveBinder();
        }

        private void OnPriceSearchResult(object sender, RoutedEventArgs e)
        {
            _viewModel.PriceSearchResult();
        }

        private void OnDownloadMissingImages(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var downloader = new SymbolDownload();
                downloader.Download(PathHelper.SymbolCacheFolder);

                var setDownload = new SetDownload(NotificationCenter.Instance);
                setDownload.Download(PathHelper.SetCacheFolder, StaticMagicData.SetDefinitions);
            });
        }

        private void OnDownloadCardImages(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var download = new CardImageDownload(NotificationCenter.Instance);

                var definitions = StaticMagicData.CardDefinitions.ToList();
                foreach (var card in definitions)
                {
                    download.DownloadImage(card, null);
                }
            });
        }

        private void OnExportCollection(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileName = DisplayExportFileDialog("Export Collection");
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    _viewModel.ExportActiveBinder(fileName);
                }
            }
            catch (Exception error)
            {
                NotificationCenter.Instance.FireNotification(
                    LogLevel.Error,
                    "Exporting cards failed: " + error.Message);

                MessageBox.Show(
                    error.Message,
                    "Exporting cards failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OnExportTrade(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileName = DisplayExportFileDialog("Export Trade List");
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    _viewModel.ExportActiveBinderTrade(fileName);
                }
            }
            catch (Exception error)
            {
                NotificationCenter.Instance.FireNotification(
                    LogLevel.Error,
                    "Exporting cards failed: " + error.Message);

                MessageBox.Show(
                    error.Message,
                    "Exporting cards failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OnExportWants(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileName = DisplayExportFileDialog("Export Wants List");
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    _viewModel.ExportActiveBinderWants(fileName);
                }
            }
            catch (Exception error)
            {
                NotificationCenter.Instance.FireNotification(
                    LogLevel.Error,
                    "Exporting cards failed: " + error.Message);

                MessageBox.Show(
                    error.Message,
                    "Exporting cards failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OnShowTradeCards(object sender, RoutedEventArgs e)
        {
            _viewModel.ShowTradeBinderCards();
        }

        private void OnShowWantCards(object sender, RoutedEventArgs e)
        {
            _viewModel.ShowWantBinderCards();
        }

        private void OnAddAllOfCurrentToTradeList(object sender, ExecutedRoutedEventArgs e)
        {
            _viewModel.AddAllFromSelectedItemToTradeList();
        }

        private void AddHotKeys()
        {
            try
            {
                //RoutedCommand firstSettings = new RoutedCommand();
                //firstSettings.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Alt));
                //CommandBindings.Add(new CommandBinding(firstSettings, My_first_event_handler));

                RoutedCommand secondSettings = new RoutedCommand();
                secondSettings.InputGestures.Add(new KeyGesture(Key.F2, ModifierKeys.None));
                CommandBindings.Add(new CommandBinding(secondSettings, OnAddAllOfCurrentToTradeList));
            }
            catch (Exception err)
            {
                NotificationCenter.Instance.FireNotification(
                    LogLevel.Error,
                    "Error setting hot keys: " + err.Message);
            }
        }

        private void OnShowLogFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                var folder = System.IO.Path.Combine(PathHelper.ExeFolder, "logs");
                Process.Start(folder);
            }
            catch (Exception err)
            {
                NotificationCenter.Instance.FireNotification(
                    LogLevel.Error,
                    "Error showing log folder: " + err.Message);
            }
        }

        private void OnAddSelectedToTradeList(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.AddSelectedToTradeList();
            }
            catch (Exception error)
            {
                NotificationCenter.Instance.FireNotification(
                    LogLevel.Error,
                    "Add to trade list: " + error.Message);

                MessageBox.Show(
                    error.Message,
                    "Add to trade list failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BinderStatisticsControl_DisplayDetails(object sender, UserControls.BinderStatisticsDisplayDetails e)
        {
            try
            {
                Task.Factory
                    .StartNew(() =>
                    {
                        _viewModel.DisplayOwnedCardsFromSet(e.Details.SetDefinition);
                    })
                    .ContinueWith((task) => mainTabCtrl.SelectedItem = searchTab, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception error)
            {
                NotificationCenter.Instance.FireNotification(
                    LogLevel.Error,
                    "Display cards from set error: " + error.Message);

                MessageBox.Show(
                    error.Message,
                    "Display cards from set failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OnResetImages(object sender, RoutedEventArgs e)
        {
            Task.Factory
                   .StartNew(() =>
                   {
                       StaticPriceDatabase.ClearImageData(NotificationCenter.Instance);
                   });
            
        }
    }
}