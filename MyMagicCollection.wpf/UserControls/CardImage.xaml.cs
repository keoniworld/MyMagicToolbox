using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MyMagicCollection.Shared;
using MyMagicCollection.Shared.Helper;
using MyMagicCollection.Shared.Models;
using NLog;

namespace MyMagicCollection.wpf.UserControls
{
	/// <summary>
	/// Interaction logic for CardImage.xaml
	/// </summary>

	public partial class CardImage : UserControl
	{
		public static readonly DependencyProperty SelectedCardProperty =
			DependencyProperty.Register("SelectedCard", typeof(MagicCardDefinition),
			typeof(CardImage), new FrameworkPropertyMetadata(OnImageChanged));

		private readonly INotificationCenter _notificationCenter;

		private readonly BitmapImage _emptyImage;

		public CardImage(INotificationCenter notificationCenter)
		{
			_notificationCenter = notificationCenter;

			InitializeComponent();
			rootGrid.DataContext = this;

			_emptyImage = new BitmapImage();
			_emptyImage.BeginInit();
			_emptyImage.StreamSource = GetType().Assembly.GetEmbeddedResourceStream("Empty.png");
			_emptyImage.EndInit();
			_emptyImage.Freeze();
			imageControl.Source = _emptyImage;
		}

		public CardImage()
			: this(NotificationCenter.Instance)
		{
		}

		public MagicCardDefinition SelectedCard
		{
			get { return (MagicCardDefinition)GetValue(SelectedCardProperty); }
			set { SetValue(SelectedCardProperty, value); }
		}

		public static void OnImageChanged(
					DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			var instance = d as CardImage;
			if (instance != null)
			{
				instance.imageControl.Source = instance._emptyImage;

				// Trigger download and display
				var card = instance.SelectedCard;
				var cardFileName = string.Empty;
				Task.Factory.StartNew(() =>
				{
					var download = new CardImageDownload(instance._notificationCenter);
					cardFileName = download.DownloadImage(card);
				}).ContinueWith(task =>
					{
						if (string.IsNullOrWhiteSpace(cardFileName))
						{
							instance.imageControl.Source = instance._emptyImage;
						}
						else
						{
							var uri = new Uri(cardFileName);
							var bitmap = new BitmapImage(uri);
							bitmap.Freeze();
							instance.imageControl.Source = bitmap;
						}
					}, TaskScheduler.FromCurrentSynchronizationContext());
			}
		}
	}
}