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
                    cardFileName = instance.DownloadImage(card);
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

        public string CreateCardIdPart(MagicCardDefinition card, char delimiter)
        {
            if (!card.NumberInSet.HasValue || string.IsNullOrWhiteSpace(card.SetCode))
            {
                return null;
            }

            var setCode = StaticMagicData.SetDefinitionsBySetCode[card.SetCode].CodeMagicCardsInfo;
            return string.Format(
               CultureInfo.InvariantCulture,
               "{2}{0}{2}{1}.jpg",
               setCode.ToLowerInvariant(),
               card.NumberInSet,
               delimiter);
        }

        public string DownloadImage(MagicCardDefinition card)
        {
            if (card == null)
            {
                return null;
            }

            var cache = PathHelper.ImageCacheFolder;
            var localStorage = new FileInfo(Path.Combine(cache, CreateCardIdPart(card, '\\').TrimStart('\\')));
            if (localStorage.Exists)
            {
                return localStorage.FullName;
            }

            var url = CreateCardIdPart(card, '/');
            if (url == null)
            {
                return null;
            }

            try
            {
                var stopwatch = Stopwatch.StartNew();

                if (!localStorage.Directory.Exists)
                {
                    localStorage.Directory.Create();
                }

                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri("http://magiccards.info/scans/en" + url), localStorage.FullName);
                }

                stopwatch.Stop();
                _notificationCenter.FireNotification(
                    LogLevel.Debug,
                    string.Format("Downloaded image for '{0}[{1}]' in {2}", card.NameEN, card.SetCode, stopwatch.Elapsed));
            }
            catch (Exception error)
            {
                _notificationCenter.FireNotification(
                    LogLevel.Debug,
                    string.Format("Error downloading image for '{0}[{1}]': {2}", card.NameEN, card.SetCode, error.Message));

                return null;
            }

            return localStorage.FullName;
        }
    }
}