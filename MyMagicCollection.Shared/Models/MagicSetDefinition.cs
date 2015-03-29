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
                    var parts = _releaseDate.Split('/');
                    _releaseDateConverted = new DateTime(int.Parse(parts[1]), int.Parse(parts[0]), 1);
                }
            }
        }

        // TODO: Actual DateTime as getter
        public DateTime? ReleaseDateTime { get { return _releaseDateConverted; } }

        public string Block { get; set; }
    }
}