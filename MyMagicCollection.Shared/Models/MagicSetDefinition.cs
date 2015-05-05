using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared.Models
{
    [DebuggerDisplay("{Code} - {Name}")]
    public class MagicSetDefinition
    {
        private string _releaseDate;

        private DateTime? _releaseDateConverted;

        public string Code { get; set; }

        public string Name { get; set; }

        public string CodeMagicCardsInfo { get; set; }

        public bool IsPromoEdition { get; set; }

        public string ReleaseDate
        {
            get
            {
                return _releaseDate;
            }

            set
            {
                _releaseDate = value;
                if (string.IsNullOrWhiteSpace(_releaseDate))
                {
                    _releaseDateConverted = null;
                }
                else
                {
                    
                    _releaseDateConverted = ParseReleaseDate(_releaseDate);
                }
            }
        }

        public static DateTime? ParseReleaseDate(string releaseDate)
        {
            if (string.IsNullOrWhiteSpace(releaseDate))
            {
                return null;
            }

            var parts = releaseDate.Split(new[] { '-', '/', '.' }, StringSplitOptions.RemoveEmptyEntries);
            return new DateTime(
                int.Parse(parts[0]),
                parts.Length >= 2 ? int.Parse(parts[1]) : 12,
                parts.Length >= 3 ? int.Parse(parts[2]) : 1);
        }

        // TODO: Actual DateTime as getter
        public DateTime? ReleaseDateTime { get { return _releaseDateConverted; } }

        public string Block { get; set; }
    }
}