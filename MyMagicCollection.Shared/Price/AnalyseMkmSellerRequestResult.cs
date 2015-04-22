using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyMagicCollection.Shared.Price
{
    public class AnalyseMkmSellerRequestResult
    {
        public static IEnumerable<MkmSellerArticleData> Analyse(
            XDocument requestResult,
            IEnumerable<string> validCountries,
            IEnumerable<string> validConditions,
            IEnumerable<string> validLanguages)
        {
            List<MkmSellerArticleData> result = new List<MkmSellerArticleData>();

            if (requestResult == null)
            {
                return result;
            }

            var countryLookup = validCountries.ToDictionary(c => c);
            var conditionLookup = validConditions.ToDictionary(c => c);
            var languageLookup = validLanguages.ToDictionary(c => c);

            // get article nodes:
            var articleNodes = requestResult.Root.Elements("article").ToList();
            foreach (var article in articleNodes)
            {
                var seller = article.Element("seller");
                var country = seller.Element("country");

                if (!countryLookup.ContainsKey(country.Value))
                {
                    // invalid country -> continue
                    continue;
                }

                var condition = article.Element("condition");
                if (!conditionLookup.ContainsKey(condition.Value))
                {
                    // invalid condition -> continue
                    continue;
                }

                var language = article.Element("language").Element("languageName");
                if (!languageLookup.ContainsKey(language.Value))
                {
                    // invalid language -> continue
                    continue;
                }

                // Now extract required data and add to result:
                var item = new MkmSellerArticleData
                {
                    CardCondition = condition.Value,
                    IsFoil = Boolean.Parse(article.Element("isFoil").Value),
                    IsPlayset = Boolean.Parse(article.Element("isPlayset").Value),
                    Price = decimal.Parse(article.Element("price").Value, CultureInfo.InvariantCulture),
                    SellerCountry = country.Value,
                    SellerName = seller.Element("username").Value,
                };

                result.Add(item);
            }

            return result;
        }
    }
}