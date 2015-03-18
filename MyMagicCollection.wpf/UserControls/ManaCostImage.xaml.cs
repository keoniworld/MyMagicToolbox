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
            get { return (MagicCardDefinition)GetValue(SelectedCardProperty); }
            set { SetValue(SelectedCardProperty, value); }
        }

        public IEnumerable<string> Images { get; private set; }

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
                        .Split(new[] { '{' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length > 0)
                    {
                        var basePath = Path.Combine(PathHelper.SymbolCacheFolder, "large");
                        instance.Images = parts.Select(p => Path.Combine(basePath, p + ".jpg")).ToList();
                    }
                }

                var prop = instance.PropertyChanged;
                if (prop != null)
                {
                    prop(instance, new PropertyChangedEventArgs("Images"));
                }
            }
        }
    }
}