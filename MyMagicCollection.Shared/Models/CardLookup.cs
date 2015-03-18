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

        private bool _searchType;

        public CardLookup()
        {
            SearchGerman = true;
            DisplayDistinct = true;

            _searchAsYouType = true;
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
                var low = value?.ToLowerInvariant();
                if (low != _searchTerm)
                {
                    _searchTerm = low;
                    RaisePropertyChanged(() => SearchTerm);
                }
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

        public bool SearchType
        {
            get
            {
                return _searchType;
            }

            set
            {
                _searchType = value;
                RaisePropertyChanged(() => SearchType);
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

        private bool _searchAsYouType;
    public bool SearchAsYouType
        {
            get
            {
                return _searchAsYouType;
            }

            set
            {
                _searchAsYouType = value;
                RaisePropertyChanged(() => SearchAsYouType);
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