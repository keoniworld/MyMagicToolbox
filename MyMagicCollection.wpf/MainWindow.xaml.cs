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
using Microsoft.Win32;
using MyMagicCollection.Shared;

namespace MyMagicCollection.wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel = new MainViewModel(NotificationCenter.Instance);

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _viewModel;
        }

        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.LookupCards();
            }
            catch (Exception error)
            {
                // TOOD: Display error
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
                var dialog = new SaveFileDialog()
                {                    
                    AddExtension = true,
                    CheckPathExists = true,
                    CheckFileExists = false,
                    Title = "Export card list",
                    Filter = "All supported files|*.csv|Deckbox.org CSV (*.csv)|*.csv",
                    InitialDirectory = PathHelper.UserDataFolder,
                };

                if (dialog.ShowDialog(this) == true)
                {
                    _viewModel.ExportDisplayedCardsList(dialog.FileName);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(
                    error.Message,
                    "Exporting cards failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
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
    }
}