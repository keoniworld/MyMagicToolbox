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

	public partial class SetImage : UserControl
	{
		public static readonly DependencyProperty SelectedCardProperty =
			DependencyProperty.Register("SelectedCard", typeof(MagicCardDefinition),
			typeof(SetImage), new FrameworkPropertyMetadata(OnImageChanged));

		public static readonly DependencyProperty SelectedSetProperty =
		  DependencyProperty.Register("SelectedSet", typeof(MagicSetDefinition),
		  typeof(SetImage), new FrameworkPropertyMetadata(OnImageChanged));

		private readonly INotificationCenter _notificationCenter;

		public SetImage(INotificationCenter notificationCenter)
		{
			_notificationCenter = notificationCenter;

			InitializeComponent();
			rootGrid.DataContext = this;
		}

		public SetImage()
			: this(NotificationCenter.Instance)
		{
		}

		public MagicCardDefinition SelectedCard
		{
			get { return (MagicCardDefinition)GetValue(SelectedCardProperty); }
			set { SetValue(SelectedCardProperty, value); }
		}

		public MagicSetDefinition SelectedSet
		{
			get { return (MagicSetDefinition)GetValue(SelectedSetProperty); }
			set { SetValue(SelectedSetProperty, value); }
		}

		public static void OnImageChanged(
			DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			var instance = d as SetImage;
			if (instance != null)
			{
				// Trigger download and display
				var imageLoaded = false;
				var definition = instance.SelectedSet;
				var rarity = MagicRarity.Common;

				var card = instance.SelectedCard;
				if (card != null)
				{
					definition = StaticMagicData.SetDefinitionsBySetCode[card.SetCode];
					rarity = card.Rarity ?? MagicRarity.Common;
				}

				if (definition != null)
				{
					var filePart = SetDownload.MakeSetImageName(definition, rarity);
					var cardFileName = Path.Combine(PathHelper.SetCacheFolder, "medium", filePart);

					// instance.imageToolTip.Content = definition.Name;

					if (File.Exists(cardFileName))
					{
						try
						{
							//var uri = new Uri(cardFileName);
							//var bitmap = new BitmapImage(uri);
							//bitmap.Freeze();

							var bitmap = BitmapImageCache.GetImage(cardFileName);

							instance.imageControl.Source = bitmap;
							instance.imageLabel.Text = "";

							instance.imageControl.Visibility = Visibility.Visible;
							instance.imageLabel.Visibility = Visibility.Hidden;
							imageLoaded = true;
						}
						catch (Exception error)
						{
							// TODO: error dump
							imageLoaded = false;
						}
					}
				}

				if (!imageLoaded)
				{
					instance.imageControl.Source = null;
					instance.imageLabel.Text = definition?.Code;

					instance.imageControl.Visibility = Visibility.Hidden;
					instance.imageLabel.Visibility = Visibility.Visible;
				}
			}
			else
			{
				instance.imageControl.Visibility = Visibility.Hidden;
				instance.imageLabel.Visibility = Visibility.Hidden;
			}
		}
	}
}