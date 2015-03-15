using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minimod.NotificationObject;

namespace MyMagicCollection.Shared.Models
{
    /// <summary>
    /// This is the model which can be bound to the UI to enter search details.
    /// </summary>
    public class CardLookup : NotificationObject
    {
        private string _searchTerm;
        private bool _searchGerman;

        private bool _displayDistinct;

        private bool _searchRules;

        private ICardLookupSetSource _setSource;

        public CardLookup()
        {
            SearchGerman = true;
            DisplayDistinct = true;

            _setSource = new CardLookupSetSourceAllSets();
        }

        public IEnumerable<string> AvailableSearchSets
        {
            get
            {
                return _setSource.AvailableSearchSets;
            }
        }

        public string SearchTerm
        {
            get
            {
                return _searchTerm;
            }

            set
            {
                _searchTerm = value;
                RaisePropertyChanged(() => SearchTerm);
            }
        }

        public ICardLookupSetSource SetSource
        {
            get
            {
                return _setSource;
            }

            set
            {
                var selected = SearchSet;
                _setSource = value;
                SearchSet = selected;

                RaisePropertyChanged(() => SetSource);
                RaisePropertyChanged(() => AvailableSearchSets);
                RaisePropertyChanged(() => SearchSet);
            }
        }

        public bool SearchGerman
        {
            get
            {
                return _searchGerman;
            }

            set
            {
                _searchGerman = value;
                RaisePropertyChanged(() => SearchGerman);
            }
        }

        public bool DisplayDistinct
        {
            get
            {
                return _displayDistinct;
            }

            set
            {
                _displayDistinct = value;
                RaisePropertyChanged(() => DisplayDistinct);
            }
        }

        public bool SearchRules
        {
            get
            {
                return _searchRules;
            }

            set
            {
                _searchRules = value;
                RaisePropertyChanged(() => SearchRules);
            }
        }

        public string SearchSet
        {
            get
            {
                return _setSource.SearchSet;
            }

            set
            {
                _setSource.SearchSet = value;
                RaisePropertyChanged(() => SearchSet);
            }
        }
    }
}