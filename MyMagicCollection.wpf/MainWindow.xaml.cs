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
            var dialog = new OpenFileDialog()
            {
                Multiselect = false,
                AddExtension = true,
                CheckPathExists = true,
                CheckFileExists = true,
                Title = "Import card list",
                Filter = "Deckbox.org CSV *.csv|*.csv",
                DefaultExt = ".csv",
                InitialDirectory = PathHelper.UserDataFolder,
            };

            if (dialog.ShowDialog(this) == true)
            {
                _viewModel.ImportCardList(dialog.FileName);
            }
        }
    }
}