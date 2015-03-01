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
    }
}
