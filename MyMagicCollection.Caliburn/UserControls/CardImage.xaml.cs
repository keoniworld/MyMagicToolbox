using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MagicLibrary;
using PropertyChanged;

namespace MyMagicCollection.Caliburn.UserControls
{
    /// <summary>
    /// Interaction logic for CardImage.xaml
    /// </summary>
    [ImplementPropertyChanged]
    public partial class CardImage : UserControl
    {
        public static readonly DependencyProperty SelectedCardProperty =
            DependencyProperty.Register("SelectedCard", typeof(CardViewModel),
            typeof(CardImage), new FrameworkPropertyMetadata(OnImageChanged));

        public CardViewModel SelectedCard
        {
            get { return (CardViewModel)GetValue(SelectedCardProperty); }
            set { SetValue(SelectedCardProperty, value); }
        }

        [Import]
        private IApplicationSettings _settings;

        public CardImage()
        {
            MefHelper.SatisfyImports(this);

            InitializeComponent();
            rootGrid.DataContext = this;
        }

        public static void OnImageChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var instance = d as CardImage;
            if (instance != null)
            {
                instance.RenderedImage = null;

                // Trigger download and display
                var card = instance.SelectedCard;
                var cardFileName = string.Empty;
                Task.Factory.StartNew(() =>
                {
                    cardFileName = instance.DownloadImage(card);
                }).ContinueWith(task =>
                    {
                        instance.RenderedImage = cardFileName;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public string RenderedImage { get; set; }

        public string CreateCardIdPart(CardViewModel card, char delimiter)
        {
            if (!card.NumberInSet.HasValue || string.IsNullOrWhiteSpace(card.SetCode))
            {
                return null;
            }

            return string.Format(
               CultureInfo.InvariantCulture,
               "{2}{0}{2}{1}.jpg",
               card.SetCode.ToLowerInvariant(),
               card.NumberInSet,
               delimiter);
        }

        public string DownloadImage(CardViewModel card)
        {
            if (card == null)
            {
                return null;
            }

            var cache = _settings.GetImageCacheFolder();
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
                if (!localStorage.Directory.Exists)
                {
                    localStorage.Directory.Create();
                }

                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri("http://magiccards.info/scans/en" + url), localStorage.FullName);
                }
            }
            catch (Exception error)
            {
                return null;
            }

            return localStorage.FullName;
        }
    }
}