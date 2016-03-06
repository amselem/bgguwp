using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BGGAPI.Models;

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
