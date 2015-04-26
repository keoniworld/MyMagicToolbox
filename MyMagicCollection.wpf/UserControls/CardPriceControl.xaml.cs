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
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.wpf.UserControls
{
	/// <summary>
	/// Interaction logic for CardPriceControl.xaml
	/// </summary>
	public partial class CardPriceControl : UserControl
	{
		public static readonly DependencyProperty CardPriceProperty =
		   DependencyProperty.Register("CardPrice", typeof(MagicCardPrice),
		   typeof(CardPriceControl), new FrameworkPropertyMetadata(OnImageChanged));

		public static readonly DependencyProperty ShowLastUpdateDateProperty =
		  DependencyProperty.Register("ShowLastUpdateDate", typeof(bool),
		  typeof(CardPriceControl), new FrameworkPropertyMetadata(true));

		public CardPriceControl()
		{
			InitializeComponent();

			rootControl.DataContext = this;
		}

		public MagicCardPrice CardPrice
		{
			get { return (MagicCardPrice)GetValue(CardPriceProperty); }
			set { SetValue(CardPriceProperty, value); }
		}

		public bool ShowLastUpdateDate
		{
			get { return (bool)GetValue(ShowLastUpdateDateProperty); }
			set { SetValue(ShowLastUpdateDateProperty, value); }
		}
		public static void OnImageChanged(
		   DependencyObject d,
		   DependencyPropertyChangedEventArgs e)
		{
			var instance = d as CardPriceControl;
			////if (instance != null)
			////{
			////    instance.imageControl.Source = instance._emptyImage;

			////    // Trigger download and display
			////    var card = instance.SelectedCard;

			////    instance.SetDefinition = card != null
			////        ? StaticMagicData.SetDefinitionsBySetCode[card.SetCode]
			////        : null;

			////    var cardFileName = string.Empty;
			////    Task.Factory.StartNew(() =>
			////    {
			////        var download = new CardImageDownload(instance._notificationCenter);
			////        cardFileName = download.DownloadImage(card);
			////    }).ContinueWith(task =>
			////    {
			////        if (string.IsNullOrWhiteSpace(cardFileName))
			////        {
			////            instance.imageControl.Source = instance._emptyImage;
			////        }
			////        else
			////        {
			////            var uri = new Uri(cardFileName);
			////            var bitmap = new BitmapImage(uri);
			////            bitmap.Freeze();
			////            instance.imageControl.Source = bitmap;
			////        }
			////    }, TaskScheduler.FromCurrentSynchronizationContext());
			////}
		}
	}
}