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

		public string CreateCardIdPart(MagicCardDefinition card, char delimiter)
		{
			if (!card.NumberInSet.HasValue || string.IsNullOrWhiteSpace(card.SetCode))
			{
				return null;
			}

			var setCode = StaticMagicData.SetDefinitionsBySetCode[card.SetCode].CodeMagicCardsInfo;
			return string.Format(
			   CultureInfo.InvariantCulture,
			   "{2}{0}{2}{1}.jpg",
			   setCode.ToLowerInvariant(),
			   card.NumberInSet,
			   delimiter);
		}

		public string DownloadImage(MagicCardDefinition card)
		{
			if (card == null)
			{
				return null;
			}

			FileInfo localStorage = null;
			try
			{
				var cache = PathHelper.ImageCacheFolder;
				localStorage = new FileInfo(Path.Combine(cache, CreateCardIdPart(card, '\\').TrimStart('\\')));
				if (localStorage.Exists)
				{
					return localStorage.FullName;
				}

				var url = CreateCardIdPart(card, '/');
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
					client.DownloadFile(new Uri("http://magiccards.info/scans/en" + url), localStorage.FullName);
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