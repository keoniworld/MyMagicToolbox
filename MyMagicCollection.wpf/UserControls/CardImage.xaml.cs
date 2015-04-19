using System;
using System.ComponentModel;
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
    public partial class CardImage : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty SelectedCardProperty =
            DependencyProperty.Register("SelectedCard", typeof(MagicCardDefinition),
            typeof(CardImage), new FrameworkPropertyMetadata(OnImageChanged));

        public static readonly DependencyProperty ShowDetailsProperty =
            DependencyProperty.Register("ShowDetails", typeof(bool),
            typeof(CardImage), new FrameworkPropertyMetadata(false));

        private readonly INotificationCenter _notificationCenter;

        private readonly BitmapImage _emptyImage;

        private MagicSetDefinition _setDefinition;

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

        public event PropertyChangedEventHandler PropertyChanged;

        public MagicCardDefinition SelectedCard
        {
            get { return (MagicCardDefinition)GetValue(SelectedCardProperty); }
            set { SetValue(SelectedCardProperty, value); }
        }

        public bool ShowDetails
        {
            get { return (bool)GetValue(ShowDetailsProperty); }
            set { SetValue(ShowDetailsProperty, value); }
        }

        public MagicSetDefinition SetDefinition
        {
            get
            {
                return _setDefinition;
            }

            private set
            {
                _setDefinition = value;
                RaisePropertyChanges("SetDefinition");
            }
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

                instance.SetDefinition = card != null
                    ? StaticMagicData.SetDefinitionsBySetCode[card.SetCode]
                    : null;

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

        private void RaisePropertyChanges(string propertyName)
        {
            var prop = PropertyChanged;
            if (prop != null)
            {
                prop(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}