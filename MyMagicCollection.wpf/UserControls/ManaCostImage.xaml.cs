using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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

	public partial class ManaCostImage : UserControl, INotifyPropertyChanged
	{
		public static readonly DependencyProperty SelectedCardProperty =
			DependencyProperty.Register("SelectedCard", typeof(MagicCardDefinition),
				typeof(ManaCostImage), new FrameworkPropertyMetadata(OnImageChanged));

		private readonly INotificationCenter _notificationCenter;

		public ManaCostImage(INotificationCenter notificationCenter)
		{
			_notificationCenter = notificationCenter;

			InitializeComponent();
			rootGrid.DataContext = this;
		}

		public ManaCostImage()
			: this(NotificationCenter.Instance)
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public MagicCardDefinition SelectedCard
		{
			get
			{
				return (MagicCardDefinition)GetValue(SelectedCardProperty);
			}
			set
			{
				SetValue(SelectedCardProperty, value);
			}
		}

		public IEnumerable<BitmapImage> Images { get; private set; }

		public IEnumerable<KeyValuePair<string, KeyValuePair<string, string>>> Data { get; private set; }

		public static void OnImageChanged(
			DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			var instance = d as ManaCostImage;
			if (instance != null)
			{
				instance.Images = null;

				var card = instance.SelectedCard;
				if (card != null)
				{
					var parts = card.ManaCost
									.Replace("}", "")
									.Split(new[]
										   {
											   '{'
										   }, StringSplitOptions.RemoveEmptyEntries);

					if (parts.Length > 0)
					{
						////var basePath = Path.Combine(PathHelper.SymbolCacheFolder, "large");
						////instance.Images = parts
						////    .Select(p => Path.Combine(basePath, p + ".jpg"))
						////    .Select(f => BitmapImageCache.GetImage(f))
						////    .ToList();

						instance.Data = parts.Select(part => new KeyValuePair<string, KeyValuePair<string, string>>(ConvertCountPart(part), new KeyValuePair<string, string>(ConvertColorPart(part, true), ConvertColorPart(part, false)))).ToList();
					}
				}

				var prop = instance.PropertyChanged;
				if (prop != null)
				{
					prop(instance, new PropertyChangedEventArgs("Images"));
					prop(instance, new PropertyChangedEventArgs("Data"));
				}
			}
		}

		// TODO 14.04.2015: Phyrexian mana

		public static string ConvertColorPart(string part, bool isLeftPart)
		{
			if (part[0] == 'p' || part[0] == 'P')
			{
				part = part[1].ToString();
			}

			if (part.Length == 02)
			{
				part = isLeftPart ? part[0].ToString() : part[1].ToString();
			}


			switch (part.ToLower())
			{
				case "b":
					return "black";

				case "u":
					return "LightBlue";

				case "r":
					return "Red";

				case "g":
					return "DarkGreen";

				case "w":
					return "AntiqueWhite";

				default:
					return "Transparent";
			}
		}

		public static string ConvertCountPart(string part)
		{
			var count = 0;
			return int.TryParse(part, out count) ? part : "";
		}
	}
}