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

    public partial class FoilControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsFoilProperty =
            DependencyProperty.Register("IsFoil", typeof(bool),
            typeof(FoilControl), new FrameworkPropertyMetadata(OnFoilChanged));

        public static readonly DependencyProperty ShowLabelProperty =
                   DependencyProperty.Register("ShowLabel", typeof(bool),
                   typeof(FoilControl), new FrameworkPropertyMetadata(OnFoilChanged));

        private readonly INotificationCenter _notificationCenter;

        public FoilControl(INotificationCenter notificationCenter)
        {
            _notificationCenter = notificationCenter;
            ShowLabel = true;
            InitializeComponent();
            rootGrid.DataContext = this;
        }

        public FoilControl()
            : this(NotificationCenter.Instance)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsFoil
        {
            get { return (bool)GetValue(IsFoilProperty); }
            set { SetValue(IsFoilProperty, value); }
        }

        public bool ShowLabel
        {
            get { return (bool)GetValue(ShowLabelProperty); }
            set { SetValue(ShowLabelProperty, value); }
        }

        public int FoilIndex
        {
            get
            {
                return IsFoil ? 1 : 0;
            }

            set
            {
                IsFoil = value == 1;
            }
        }

        public static void OnFoilChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var instance = d as FoilControl;
            if (instance != null)
            {
                instance.RaisePropertyChanged("FoilIndex");
            }
        }

        private void RaisePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}