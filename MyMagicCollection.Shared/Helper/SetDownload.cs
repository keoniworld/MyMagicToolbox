using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using MyMagicCollection.Shared.Models;
using NLog;

namespace MyMagicCollection.Shared.Helper
{
    public class SetDownload
    {
        private static readonly String urlFmt =
            "http://gatherer.wizards.com/Handlers/Image.ashx?type=symbol&set={1}&size={0}&rarity={2}";

        // private static readonly string[] sizes = { "small", "medium", "large" };
        private static readonly string[] sizes = { "medium"};
        private static readonly MagicRarity[] _rarity = { MagicRarity.Mythic, MagicRarity.Common, MagicRarity.Rare, MagicRarity.Uncommon };

        private readonly INotificationCenter _notificationCenter;

        private readonly Dictionary<string, string> _setPatch = new Dictionary<string, string>()
            {
                { "2ED", "2U" },
                { "TSB", "TSP" },
                { "JU", "JUD" },
            };

        public SetDownload(INotificationCenter notificationCenter)
        {
            _notificationCenter = notificationCenter;
        }

        public static string MakeSetImageName(MagicSetDefinition definition, MagicRarity rarity)
        {
            var rarityCode = rarity.ToCode();
            return definition.Code + "_" + rarityCode + ".jpg";
        }

        public void Download(string targetFolder, IEnumerable<MagicSetDefinition> setDefinitions)
        {
            foreach (var size in sizes)
            {
                var baseFolder = Path.Combine(targetFolder, size);
                if (!Directory.Exists(baseFolder))
                {
                    Directory.CreateDirectory(baseFolder);
                }

                foreach (var setDefinition in setDefinitions)
                {
                    try
                    {
                        foreach (var rarity in _rarity)
                        {
                            var filePart = setDefinition.Code;
                            var dest = Path.Combine(baseFolder, MakeSetImageName(setDefinition, rarity));
                            if (File.Exists(dest))
                            {
                                continue;
                            }

                            string patched = "";

                            if (_setPatch.TryGetValue(filePart, out patched))
                            {
                                filePart = patched;
                            }

                            switch (filePart)
                            {
                                case "ARN":
                                    filePart = "AN";
                                    break;

                                case "ATQ":
                                    filePart = "AQ";
                                    break;

                                case "LEG":
                                    filePart = "LE";
                                    break;

                                case "DRK":
                                    filePart = "DK";
                                    break;

                                case "FEM":
                                    filePart = "FE";
                                    break;

                                case "HML":
                                    filePart = "HM";
                                    break;

                                case "ICE":
                                    filePart = "IA";
                                    break;

                                case "ALL":
                                    filePart = "AL";
                                    break;

                                case "APC":
                                    filePart = "AP";
                                    break;

                                case "TMP":
                                    filePart = "TE";
                                    break;

                                case "INV":
                                    filePart = "IN";
                                    break;

                                case "PLS":
                                    filePart = "PS";
                                    break;

                                case "WTH":
                                    filePart = "WL";
                                    break;

                                case "ULG":
                                    filePart = "GU";
                                    break;

                                case "USG":
                                    filePart = "UZ";
                                    break;

                                case "UDS":
                                    filePart = "CG";
                                    break;

                                case "ODY":
                                    filePart = "OD";
                                    break;

                                case "MMQ":
                                    filePart = "MM";
                                    break;

                                case "NMS":
                                    filePart = "NE";
                                    break;

                                case "PCY":
                                    filePart = "PR";
                                    break;

                                case "STH":
                                    filePart = "ST";
                                    break;

                                case "EXO":
                                    filePart = "EX";
                                    break;

                                case "VIS":
                                    filePart = "VI";
                                    break;

                                case "MIR":
                                    filePart = "MI";
                                    break;

                                case "7ED":
                                    filePart = "7E";
                                    break;

                                case "6ED":
                                    filePart = "6E";
                                    break;

                                case "5ED":
                                    filePart = "5E";
                                    break;

                                case "4ED":
                                    filePart = "4E";
                                    break;

                                case "3ED":
                                    filePart = "3E";
                                    break;

                                case "LEB":
                                    filePart = "2E";
                                    break;

                                case "LEA":
                                    filePart = "1E";
                                    break;
                            }

                            var file = new FileInfo(dest);
                            if (file.Exists)
                            {
                                continue;
                            }

                            var source = string.Format(urlFmt, size, filePart, rarity.ToCode());
                            using (var client = new WebClient())
                            {
                                _notificationCenter.FireNotification(LogLevel.Debug,
                                    "Downloading set image for " + setDefinition.Code + "(" + setDefinition.Name + ")...");

                                client.DownloadFile(new Uri(source), dest);

                                file.Refresh();
                                if (file.Length == 0)
                                {
                                    file.Delete();
                                    if (rarity != MagicRarity.Mythic)
                                    {
                                        _notificationCenter.FireNotification(
                                            null,
                                            "Downloading set image for " + setDefinition.Code + "(" + setDefinition.Name + ") failed for rarity " + rarity + " and size " + size + ": " + source);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _notificationCenter.FireNotification(null,
                            "Error downloading set image for " + setDefinition.Code + "(" + setDefinition.Name + "): " + error.Message);
                    }
                }
            }
        }
    }
}