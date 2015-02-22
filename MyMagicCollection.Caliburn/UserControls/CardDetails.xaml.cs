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

namespace MyMagicCollection.Caliburn.UserControls
{
    /// <summary>
    /// Interaction logic for CardDetails.xaml
    /// </summary>
    public partial class CardDetails : UserControl
    {
        public static readonly DependencyProperty SelectedCardProperty =
           DependencyProperty.Register("SelectedCard", typeof(CardDetails),
           typeof(CardImage), new FrameworkPropertyMetadata(OnCardChanged));

        public CardViewModel SelectedCard
        {
            get { return (CardViewModel)GetValue(SelectedCardProperty); }
            set { SetValue(SelectedCardProperty, value); }
        }

        public CardDetails()
        {
            InitializeComponent();

            rootPanel.DataContext = this;
        }

        public static void OnCardChanged(
           DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        { }
    }
}
