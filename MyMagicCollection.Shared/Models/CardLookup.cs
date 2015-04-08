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

        private bool _searchAsYouType;

        public CardLookup()
        {
            Reset();

            _searchAsYouType = true;
            _setSource = new CardLookupSetSourceAllSets();
        }

        public event EventHandler<EventArgs> SearchWanted;

        public IEnumerable<string> AvailableSearchSets => _setSource.AvailableSearchSets;

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
                    RaiseSearchWanted();
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
                if (value != _setSource)
                {
                    var selected = SearchSet;
                    _setSource = value;
                    _setSource.SearchSet = selected;

                    RaisePropertyChanged(() => SetSource);
                    RaisePropertyChanged(() => AvailableSearchSets);
                    RaisePropertyChanged(() => SearchSet);

                    RaiseSearchWanted();
                }
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
                RaiseSearchWanted();
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
                RaiseSearchWanted();
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
                RaiseSearchWanted();
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
                RaiseSearchWanted();
            }
        }

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
                RaiseSearchWanted();
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
                RaiseSearchWanted();
            }
        }

        public void Reset()
        {
            _searchRules = false;
            _searchTerm = "";
            _searchGerman = true;
            _searchType = false;
            _displayDistinct = true;

            RaisePropertyChanged(() => SearchRules);
            RaisePropertyChanged(() => SearchGerman);
            RaisePropertyChanged(() => SearchType);
            RaisePropertyChanged(() => DisplayDistinct);

            RaiseSearchWanted();
        }

        private void RaiseSearchWanted()
        {
            var wanted = SearchWanted;
            if (wanted != null && _searchAsYouType)
            {
                wanted(this, EventArgs.Empty);
            }
        }
    }
}