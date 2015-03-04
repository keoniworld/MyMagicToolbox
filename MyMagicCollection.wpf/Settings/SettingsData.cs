using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.wpf.Settings
{
    public class SettingsData
    {
        public string LoadedBinder { get; set; }

        public MagicLanguage SelectedLanguage { get; set; } = MagicLanguage.English;

        public MagicGrade SelectedGrade { get; set; } = MagicGrade.NearMint;
    }
}