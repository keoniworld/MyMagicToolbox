using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.Shared.FileFormats.MyMagicCollection
{
	public class MyMagicCollectionCsv
	{
		private readonly CsvConfiguration _config;
		private const string _delimiter = "----- CARDSDELIMITER -----";

		public MyMagicCollectionCsv()
		{
			_config = new CsvConfiguration()
			{
				Encoding = Encoding.UTF8,
				HasHeaderRecord = true,
				CultureInfo = CultureInfo.InvariantCulture,
			};

			_config.RegisterClassMap(new MagicCollectionCsvMapper());
		}

		public void WriteFile(string fileName, MagicCollection collection)
		{
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}

			using (var textWriter = new StreamWriter(fileName))
			{
				// Write header first:
				var writer = new CsvWriter(textWriter, _config);
				{
					writer.WriteField<string>("Name");
					writer.NextRecord();

					writer.WriteField<string>(collection.Name);
					writer.NextRecord();

					if (collection.Cards != null && collection.Cards.Any())
					{
						textWriter.WriteLine(_delimiter);

						// Now write the cards
						writer.WriteHeader<MagicCollectionCard>();
						writer.WriteRecords(collection.Cards);
					}
				}
			}
		}

		public MagicCollection ReadFile(string fileName)
		{
			var content = File.ReadAllText(fileName);
			var delimiterPos = content.IndexOf(_delimiter, StringComparison.InvariantCultureIgnoreCase);
			string header = "";
			string cards = "";

			if (delimiterPos >= 0)
			{
				// delimiter found -> split text
				header = content.Substring(0, delimiterPos).Trim('\r', '\n');
				cards = content.Substring(delimiterPos + _delimiter.Length, content.Length - _delimiter.Length - delimiterPos).Trim('\r', '\n');
			}
			else
			{
				// no delimiter -> take all
				header = content;
			}

			IList<MagicCollectionCard> collectionCards = new List<MagicCollectionCard>();
			if (!string.IsNullOrWhiteSpace(cards))
			{
				using (var inputCsv = new CsvReader(new StringReader(cards)))
				{
					collectionCards = inputCsv.GetRecords<MagicCollectionCard>().ToList();
				}
			}

			var result = new MagicCollection(collectionCards);

			using (var inputCsv = new CsvReader(new StringReader(header)))
			{
				inputCsv.Read();
				result.Name = inputCsv.GetField<string>("Name");
			}

			return result;
		}
	}
}