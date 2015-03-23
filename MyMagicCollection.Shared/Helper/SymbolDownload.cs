using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyMagicCollection.Shared.Helper
{
	public class SymbolDownload
	{
		private static String urlFmt = "http://gatherer.wizards.com/handlers/image.ashx?size={0}$s&name={1}&type=symbol";

		private static string[] sizes = { "small", "medium", "large" };

		private static string[] symbols = {"W", "U", "B", "R", "G",
										  "W/U", "U/B", "B/R", "R/G", "G/W", "W/B", "U/R", "B/G", "R/W", "G/U",
										  "2/W", "2/U", "2/B", "2/R", "2/G",
										  "WP", "UP", "BP", "RP", "GP",
										  "X", "S", "T", "Q"};

		private static int minNumeric = 0;
		private static int maxNumeric = 16;

		public void Download(string targetFolder)
		{
			var realSizes = symbols.ToList();
			for (var index = minNumeric; index <= maxNumeric; ++index)
			{
				realSizes.Add(index.ToString(CultureInfo.InvariantCulture));
			}

			foreach (var size in sizes)
			{
				var baseFolder = Path.Combine(targetFolder, size);
				if (!Directory.Exists(baseFolder))
				{
					Directory.CreateDirectory(baseFolder);
				}

				foreach (var symbol in realSizes)
				{
					try
					{
						var filePart = symbol.Replace("/", "");

                        var patchedFilePart = filePart
                            .Replace("WP", "PW")
                            .Replace("UP", "PU")
                            .Replace("BP", "PB")
                            .Replace("RP", "PR")
                            .Replace("GP", "GR");

						var dest = Path.Combine(baseFolder, filePart + ".jpg");
						if (File.Exists(dest))
						{
							continue;
						}

						if (filePart.Equals("T"))
						{
							filePart = "tap";
						}
						else if (filePart.Equals("Q"))
						{
							filePart = "untap";
						}
						else if (filePart.Equals("S"))
						{
							filePart = "snow";
						}

						var source = string.Format(urlFmt, size, filePart);

						using (var client = new WebClient())
						{
							client.DownloadFile(new Uri(source), dest);
						}
					}
					catch (Exception)
					{
						// TODO: Handle this
					}
				}
			}
		}
	}
}