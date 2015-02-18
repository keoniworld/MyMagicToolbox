using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MyMagicCollection.Caliburn.UserControls
{
    /// <summary>
    /// Interaction logic for CardGrid.xaml
    /// </summary>
    public partial class CardGrid : UserControl
    {
        // Dependency Property
        public static readonly DependencyProperty CardCollectionProperty =
             DependencyProperty.Register("CardCollection", typeof(IEnumerable<CardViewModel>),
             typeof(CardGrid), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty SelectedCardProperty =
           DependencyProperty.Register("SelectedCard", typeof(CardViewModel),
           typeof(CardGrid), new FrameworkPropertyMetadata(OnSelectedCardChanged));

        // .NET Property wrapper
        public IEnumerable<CardViewModel> CardCollection
        {
            get { return (IEnumerable<CardViewModel>)GetValue(CardCollectionProperty); }
            set { SetValue(CardCollectionProperty, value); }
        }

        public CardViewModel SelectedCard
        {
            get { return (CardViewModel)GetValue(SelectedCardProperty); }
            set { SetValue(SelectedCardProperty, value); }
        }

        public CardGrid()
        {
            InitializeComponent();

            innerGrid.DataContext = this;
        }
        public static void OnSelectedCardChanged(
           DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
        }

    }
}