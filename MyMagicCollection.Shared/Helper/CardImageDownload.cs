using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MyMagicCollection.Shared.Models;
using NLog;

namespace MyMagicCollection.Shared.Helper
{
	public class CardImageDownload
	{
		private readonly INotificationCenter _notificationCenter;

		public CardImageDownload(INotificationCenter notificationCenter)
		{
			_notificationCenter = notificationCenter;
		}

		public string CreateCardIdPart(MagicCardDefinition card, char delimiter, bool useMkmName)
		{
			if (!card.NumberInSet.HasValue || string.IsNullOrWhiteSpace(card.SetCode))
			{
				return null;
			}

            var cardName = card.NameEN;
            if (useMkmName && !string.IsNullOrWhiteSpace(card.NameMkm))
            {
                cardName = card.NameMkm
                    .Replace("\\", "-")
                    .Replace(",", "-")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace("/", "-")
					.Replace("*", "X");
            }

            if (card.MagicCardType == MagicCardType.Token)
			{
				var setName = StaticMagicData.SetDefinitionsBySetCode[card.SetCode].Name
                    .ToLowerInvariant()
                    .Replace(" core set", "")
                    .Replace(" ", "-");

				return string.Format(
				   CultureInfo.InvariantCulture,
				   "{2}{0}{2}{1}.jpg",
				   setName,
                   cardName.ToLowerInvariant(),
				   delimiter);
			}

			var setCode = StaticMagicData.SetDefinitionsBySetCode[card.SetCode].CodeMagicCardsInfo;
			return string.Format(
			   CultureInfo.InvariantCulture,
			   "{2}{0}{2}{1}.jpg",
			   setCode.ToLowerInvariant(),
			   card.NumberInSet,
			   delimiter);
		}

		public string DownloadImage(MagicCardDefinition card, MagicCardPrice cardPrice)
		{
			if (card == null)
			{
				return null;
			}

            if (cardPrice == null)
            {
                cardPrice = StaticPriceDatabase.FindPrice(card, false, false, "CardImage download", false);
            }

            FileInfo localStorage = null;
			try
			{
				var cache = PathHelper.ImageCacheFolder;
				localStorage = new FileInfo(Path.Combine(cache, CreateCardIdPart(card, '\\', true).TrimStart('\\')));
				if (localStorage.Exists)
				{
					return localStorage.FullName;
				}

				var url = CreateCardIdPart(card, '/', false);
				if (url == null)
				{
					return null;
				}

				var stopwatch = Stopwatch.StartNew();

				if (!localStorage.Directory.Exists)
				{
					localStorage.Directory.Create();
				}

				using (var client = new WebClient())
				{
					var rootUrl = card.MagicCardType != MagicCardType.Token
						? "http://magiccards.info/scans/en"
						: "http://magiccards.info/extras/token";

                    var fullUrl = !string.IsNullOrWhiteSpace(cardPrice.ImagePath)
                        ? "http://www.magickartenmarkt.de/" + cardPrice.ImagePath
                        : rootUrl + url;

                    client.DownloadFile(new Uri(fullUrl), localStorage.FullName);
				}

				stopwatch.Stop();
				_notificationCenter.FireNotification(
					LogLevel.Debug,
					string.Format("Downloaded image for '{0}[{1}]' in {2}", card.NameEN, card.SetCode, stopwatch.Elapsed));
			}
			catch (Exception error)
			{
				_notificationCenter.FireNotification(
					LogLevel.Debug,
					string.Format("Error downloading image for '{0}[{1}]': {2}", card.NameEN, card.SetCode, error.Message));

				return null;
			}

			return localStorage.FullName;
		}
	}
}