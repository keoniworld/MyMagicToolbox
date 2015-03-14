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
		public string Code { get; set; }

		public string Name { get; set; }

		public string CodeMagicCardsInfo { get; set; }
	}
}