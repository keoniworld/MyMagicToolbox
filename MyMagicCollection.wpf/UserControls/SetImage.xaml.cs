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

        public static void OnImageChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var instance = d as SetImage;
            if (instance != null)
            {
                // Trigger download and display
                var card = instance.SelectedCard;

                var definition = StaticMagicData.SetDefinitionsBySetCode[card.SetCode];
                var filePart = SetDownload.MakeSetImageName(definition, card.Rarity.HasValue ? card.Rarity.Value : MagicRarity.Common);
                var cardFileName = Path.Combine(PathHelper.SetCacheFolder, "small", filePart);

                instance.imageToolTip.Content = definition.Name;
                var imageLoaded = false;
                if (File.Exists(cardFileName))
                {
                    try
                    {
                        var uri = new Uri(cardFileName);
                        var bitmap = new BitmapImage(uri);
                        bitmap.Freeze();
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
                
                if (!imageLoaded)
                {
                    instance.imageControl.Source = null;
                    instance.imageLabel.Text = definition.Code;

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