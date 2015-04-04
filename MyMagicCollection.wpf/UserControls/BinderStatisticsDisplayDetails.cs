using System;
using MyMagicCollection.Shared.ViewModels;

namespace MyMagicCollection.wpf.UserControls
{
    public class BinderStatisticsDisplayDetails : EventArgs
    {
        public BinderStatisticsPerSet Details { get; set; }
    }
}