using System;
using System.IO;
using System.Windows;
using MagicDatabase;
using Microsoft.Win32;
using PriceLibrary;

namespace MyMagicCollection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private UserDatabaseController _userDatabaseController;
        public MainWindow()
        {
            InitializeComponent();

            var exeFolder = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory.FullName;
            _userDatabaseController = new UserDatabaseController(Path.Combine(exeFolder, "USER_DATA"));
            
            MainViewModel = new MainViewModel(
                new CardDatabase(Path.Combine(exeFolder, "APP_DATA")),
                _userDatabaseController.UserDatabase);


            DataContext = MainViewModel;

            // TEST
            ////var helper = new RequestHelper();
            ////var result = helper.MakeRequest(RequestHelper.CreateGetProductsUrl("Thorn of Amethyst", MagicContracts.Language.English,true, null));

            ////int debug = 0;
        }

        public MainViewModel MainViewModel { get; private set; }

        private void OnOpenDeckFile(object sender, RoutedEventArgs e)
        {
            
        }

        private void OnSaveDeckboxCsv(object sender, RoutedEventArgs e)
        {
            var openDialog = new SaveFileDialog()
            {
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = ".csv",
                Filter = "Deckbox.org CSV files | *.csv",
                Title = "Select file to save",
                AddExtension = true,
            };

            if (true == openDialog.ShowDialog(this))
            {
                MainViewModel.DeckTools.SaveDeckboxCsvFile(openDialog.FileName);
            }
        }

        private void OnLoadPriceDataTools(object sender, RoutedEventArgs e)
        {
            MainViewModel.DeckTools.UpdatePriceData();
        }

        public void Dispose()
        {
            
        }
    }
}