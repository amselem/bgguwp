using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BggApi.Models;

namespace BggApi
{
    // Adapted from the BoardBoardGameGeek client library created by WebKoala
    // See this post for more information: http://boardgamegeek.com/thread/972785/c-async-api-client
    // Original source at https://github.com/WebKoala/W8BggApp
    // ReadData function based on https://github.com/ervwalter/bgg-json

    public class BggApiClient
    {
        private const string BASE_URL = "http://www.boardgamegeek.com/xmlapi2";
        public async Task<IEnumerable<HotItem>> LoadHotItems()
        {
            try
            {
                Uri teamDataURI = new Uri(BASE_URL + "/hot?type=boardgame");
                XDocument xDoc = await ReadData(teamDataURI);

                // LINQ to XML.
                IEnumerable<HotItem> hotBoardGamesCollection = from Boardgame in xDoc.Descendants("item")
                                                               select new HotItem
                                                               {
                                                                   Name = Boardgame.Element("name").Attribute("value").Value,
                                                                   YearPublished = Boardgame.Element("yearpublished") != null ? int.Parse(Boardgame.Element("yearpublished").Attribute("value").Value) : 0,
                                                                   ThumbnailWeb = "http:" + Boardgame.Element("thumbnail").Attribute("value").Value,
                                                                   BoardGameId = int.Parse(Boardgame.Attribute("id").Value),
                                                                   Rank = int.Parse(Boardgame.Attribute("rank").Value)
                                                               };
                return hotBoardGamesCollection;
            }
            catch
            {
                return new List<HotItem>();
            }
        }

        #region Boardgame helpers
        private bool SetIsExpansion(XElement Boardgame)
        {
            const string IsExpansionLinkId = "1042";
            return (from p in Boardgame.Element("item").Elements("link")
                    where p.Attribute("type").Value == "boardgamecategory" && p.Attribute("id").Value == IsExpansionLinkId
                    select p.Attribute("value").Value).FirstOrDefault() != null;
        }
        private List<PlayerPollResult> LoadPlayerPollResults(XElement xElement)
        {
            List<PlayerPollResult> playerPollResult = new List<PlayerPollResult>();

            if (xElement != null)
            {
                foreach (XElement results in xElement.Elements("results"))
                {
                    PlayerPollResult pResult = new PlayerPollResult()
                    {
                        Best = GetIntResultScore(results, "Best"),
                        Recommended = GetIntResultScore(results, "Recommended"),
                        NotRecommended = GetIntResultScore(results, "Not Recommended")
                    };
                    SetNumberOfPlayers(pResult, results);
                    playerPollResult.Add(pResult);
                }
            }

            return playerPollResult;
        }
        private void SetNumberOfPlayers(PlayerPollResult pResult, XElement results)
        {
            string value = results.Attribute("numplayers").Value;
            if (value.Contains("+"))
            {
                pResult.NumberOfPlayersIsAndHigher = true;
            }
            value = value.Replace("+", string.Empty);

            int res = 0;
            int.TryParse(value, out res);

            pResult.NumberOfPlayers = res;
        }
        private int GetIntResultScore(XElement results, string selector)
        {
            int res = 0;
            try
            {
                string value = (from p in results.Elements("result") where p.Attribute("value").Value == selector select p.Attribute("numvotes").Value).FirstOrDefault();

                if (value != null)
                    int.TryParse(value, out res);
            }
            catch (Exception)
            {
                return 0;
            }

            return res;
        }
        private int GetRanking(XElement rankingElement)
        {
            string val = (from p in rankingElement.Elements("rank") where p.Attribute("id").Value == "1" select p.Attribute("value").Value).SingleOrDefault();
            int rank;

            if (val == null)
                rank = -1;
            else if (val.ToLower().Trim() == "not ranked")
                rank = -1;
            else if (!int.TryParse(val, out rank))
                rank = -1;

            return rank;
        }
        #endregion

        #region Converters
        private string GetStringValue(XElement element, string attribute = null, string defaultValue = "")
        {
            if (element == null)
                return defaultValue;

            if (attribute == null)
                return element.Value;

            XAttribute xatt = element.Attribute(attribute);
            if (xatt == null)
                return defaultValue;

            return xatt.Value;
        }
        private int GetIntValue(XElement element, string attribute = null, int defaultValue = -1)
        {
            string val = GetStringValue(element, attribute, null);
            if (val == null)
                return defaultValue;

            int retVal;
            if (!int.TryParse(val, out retVal))
                retVal = defaultValue;
            return retVal;
        }
        private bool GetBoolValue(XElement element, string attribute = null, bool defaultValue = false)
        {
            string val = GetStringValue(element, attribute, null);
            if (val == null)
                return defaultValue;

            int retVal;
            if (!int.TryParse(val, out retVal))
                return defaultValue;

            return retVal == 1;
        }
        private decimal GetDecimalValue(XElement element, string attribute = null, decimal defaultValue = -1)
        {
            string val = GetStringValue(element, attribute, null);
            if (val == null)
                return defaultValue;

            decimal retVal;
            if (!decimal.TryParse(val, out retVal))
                return defaultValue;

            return retVal;
        }
        private DateTime safeParseDateTime(string date)
        {
            DateTime dt;
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                dt = DateTime.MinValue;
            }
            return dt;
        }
        #endregion

        private async Task<XDocument> ReadData(Uri requestUrl)
        {
            // Due to malformed header I cannot use GetContentAsync and ReadAsStringAsync
            // UTF-8 is now hard-coded...

            XDocument data = null;
            int retries = 0;
            while (data == null && retries < 30)
            {
                retries++;
                var request = WebRequest.CreateHttp(requestUrl);
                request.ContinueTimeout = 15000;
                using (var response = (HttpWebResponse)(await request.GetResponseAsync()))
                {
                    if (response.StatusCode == HttpStatusCode.Accepted)
                    {
                        await Task.Delay(100);
                        continue;
                    }
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        data = XDocument.Parse(await reader.ReadToEndAsync());
                    }
                }
            }

            if (data != null)
            {
                return data;
            }
            else
            {
                throw new Exception("Failed to download BGG data.");
            }
        }
    }
}
