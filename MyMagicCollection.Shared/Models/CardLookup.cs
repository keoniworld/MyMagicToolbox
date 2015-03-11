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
		public const string AllSetsSearchSetName = "All Sets";
		private string _searchTerm;
		private bool _searchGerman;

		private string _searchSet;
		private bool _displayDistinct;

		private bool _searchRules;

		public CardLookup()
		{
			SearchGerman = true;
			DisplayDistinct = true;

			var allSets = StaticMagicData.SetDefinitions.Select(s => s.Name).OrderBy(s => s).ToList();
			allSets.Insert(0, AllSetsSearchSetName);
			AvailableSearchSets = allSets;
			_searchSet = AllSetsSearchSetName;
		}

		public IEnumerable<string> AvailableSearchSets { get; private set; }

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
				return _searchSet;
			}

			set
			{
				_searchSet = value;
				RaisePropertyChanged(() => SearchSet);
			}
		}
	}
}