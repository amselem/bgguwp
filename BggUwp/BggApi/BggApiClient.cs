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
using System.Threading;
using Windows.Web.Http;
using Newtonsoft.Json;

namespace BggApi
{
    //Adapted from the BoardBoardGameGeek client library created by WebKoala
    //See this post for more information: http://boardgamegeek.com/thread/972785/c-async-api-client
    //Original source at https://github.com/WebKoala/W8BggApp

    //ReadData function based on https://github.com/ervwalter/bgg-json

    public class BggApiClient
    {
        private const string BASE_URL = "http://www.boardgamegeek.com/xmlapi2";
        public async Task<User> LoadUserDetails(string username)
        {
            try
            {
                Uri teamDataURI = new Uri(string.Format(BASE_URL + "/user?name={0}", username));

                XDocument xDoc = await ReadData(teamDataURI);

                // LINQ to XML.
                IEnumerable<User> users = from Boardgame in xDoc.Descendants("user")
                                          select new User
                                          {
                                              UserId = int.Parse(Boardgame.Attribute("id").Value),
                                              Username = Boardgame.Attribute("name").Value,
                                              Avatar = GetStringValue(Boardgame.Element("avatarlink"), "value"),
                                              FirstName = GetStringValue(Boardgame.Element("firstname"), "value"),
                                              LastName = GetStringValue(Boardgame.Element("lastname"), "value"),
                                              YearRegistered = GetStringValue(Boardgame.Element("yearregistered"), "value"),
                                          };
                return users.FirstOrDefault();

            }
            catch (Exception ex)
            {
                return new User();
            }
        }

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
            catch (Exception ex)
            {

                return new List<HotItem>();
            }
        }

        public async Task<IEnumerable<CollectionItem>> LoadCollection(string Username)
        {
            IEnumerable<CollectionItem> baseBoardGames = await LoadBoardGamesFromCollection(Username, false);
            IEnumerable<CollectionItem> expansions = await LoadBoardGamesFromCollection(Username, true);

            return baseBoardGames.Concat(expansions);
        }
        private async Task<IEnumerable<CollectionItem>> LoadBoardGamesFromCollection(string Username, bool GetExpansions)
        {
            try
            {

                Uri teamDataURI = new Uri(string.Format(BASE_URL + "/collection?username={0}&stats=1&{1}",
                    Username,
                    GetExpansions ? "subtype=boardgameexpansion" : "excludesubtype=boardgameexpansion"));


                XDocument xDoc = await ReadData(teamDataURI);

                // LINQ to XML.
                IEnumerable<CollectionItem> baseBoardGames = from colItem in xDoc.Descendants("item")
                                                             select new CollectionItem
                                                             {
                                                                 Name = GetStringValue(colItem.Element("name")),
                                                                 NumberOfPlays = GetIntValue(colItem.Element("numplays")),
                                                                 YearPublished = GetIntValue(colItem.Element("yearpublished")),
                                                                 ThumbnailWeb = "http:" + GetStringValue(colItem.Element("thumbnail")),
                                                                 BoardGameId = GetIntValue(colItem, "objectid"),
                                                                 CollectionItemId = GetIntValue(colItem, "collid"),
                                                                 ForTrade = GetBoolValue(colItem.Element("status"), "fortrade"),
                                                                 Owned = GetBoolValue(colItem.Element("status"), "own"),
                                                                 PreviousOwned = GetBoolValue(colItem.Element("status"), "prevowned"),
                                                                 Want = GetBoolValue(colItem.Element("status"), "want"),
                                                                 WantToBuy = GetBoolValue(colItem.Element("status"), "wanttobuy"),
                                                                 WantToPlay = GetBoolValue(colItem.Element("status"), "wanttoplay"),
                                                                 Wishlist = GetBoolValue(colItem.Element("status"), "wishlist"),
                                                                 WishlistPriority = GetIntValue(colItem.Element("status"), "wishlistpriority"),
                                                                 PreOrdered = GetBoolValue(colItem.Element("status"), "preordered"),
                                                                 UserRating = GetDecimalValue(colItem.Element("stats").Element("rating"), "value", 0),
                                                                 AverageRating = GetDecimalValue(colItem.Element("stats").Element("rating").Element("average"), "value", 0),
                                                                 GeekRating = GetDecimalValue(colItem.Element("stats").Element("rating").Element("bayesaverage"), "value", 0),
                                                                 ImageWeb = "http:" + GetStringValue(colItem.Element("image")),
                                                                 MaxPlayers = GetIntValue(colItem.Element("stats"), "maxplayers"),
                                                                 MinPlayers = GetIntValue(colItem.Element("stats"), "minplayers"),
                                                                 PlayingTime = GetIntValue(colItem.Element("stats"), "playingtime"),
                                                                 Rank = GetRanking(colItem.Element("stats").Element("rating").Element("ranks")),
                                                                 IsExpansion = GetExpansions,
                                                                 UserComment = GetStringValue(colItem.Element("comment"))
                                                             };
                return baseBoardGames;
            }
            catch (Exception ex)
            {
                //ExceptionHandler(ex);
                return new List<CollectionItem>();
            }
        }
        public async Task<BoardGame> LoadBoardGame(int BoardGameId)
        {
            try
            {
                Uri teamDataURI = new Uri(string.Format(BASE_URL + "/thing?id={0}&stats=1", BoardGameId));
                XDocument xDoc = await ReadData(teamDataURI);
                // LINQ to XML.
                IEnumerable<BoardGame> gameCollection = from Boardgame in xDoc.Descendants("items")
                                                        select new BoardGame
                                                        {
                                                            Name = (from p in Boardgame.Element("item").Elements("name") where p.Attribute("type").Value == "primary" select p.Attribute("value").Value).SingleOrDefault(),
                                                            BoardGameId = int.Parse(Boardgame.Element("item").Attribute("id").Value),
                                                            ImageWeb = "http:" + Boardgame.Element("item").Element("image") != null ? Boardgame.Element("item").Element("image").Value : string.Empty,
                                                            ThumbnailWeb = "http:" + Boardgame.Element("item").Element("thumbnail") != null ? Boardgame.Element("item").Element("thumbnail").Value : string.Empty,
                                                            Description = WebUtility.HtmlDecode(Boardgame.Element("item").Element("description").Value),
                                                            MaxPlayers = int.Parse(Boardgame.Element("item").Element("maxplayers").Attribute("value").Value),
                                                            MinPlayers = int.Parse(Boardgame.Element("item").Element("minplayers").Attribute("value").Value),
                                                            YearPublished = int.Parse(Boardgame.Element("item").Element("yearpublished").Attribute("value").Value),
                                                            PlayingTime = int.Parse(Boardgame.Element("item").Element("playingtime").Attribute("value").Value),
                                                            AverageRating = decimal.Parse(Boardgame.Element("item").Element("statistics").Element("ratings").Element("average").Attribute("value").Value),
                                                            GeekRating = decimal.Parse(Boardgame.Element("item").Element("statistics").Element("ratings").Element("bayesaverage").Attribute("value").Value),
                                                            Rank = GetRanking(Boardgame.Element("item").Element("statistics").Element("ratings").Element("ranks")),
                                                            Publishers = (from p in Boardgame.Element("item").Elements("link") where p.Attribute("type").Value == "boardgamepublisher" select p.Attribute("value").Value).ToList(),
                                                            Designers = (from p in Boardgame.Element("item").Elements("link") where p.Attribute("type").Value == "boardgamedesigner" select p.Attribute("value").Value).ToList(),
                                                            Artists = (from p in Boardgame.Element("item").Elements("link") where p.Attribute("type").Value == "boardgameartist" select p.Attribute("value").Value).ToList(),
                                                            PlayerPollResults = LoadPlayerPollResults(Boardgame.Element("item").Element("poll")),
                                                            IsExpansion = SetIsExpansion(Boardgame)
                                                        };

                return gameCollection.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return new BoardGame();
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

        public async Task<IEnumerable<Play>> LoadLastPlays(string Username)
        {
            try
            {
                Uri teamDataURI = new Uri(string.Format(BASE_URL + "/plays?username={0}", Username));
                XDocument xDoc = await ReadData(teamDataURI);

                // LINQ to XML.
                IEnumerable<Play> gameCollection = from Boardgame in xDoc.Descendants("play")
                                                   select new Play
                                                   {
                                                       PlayId = int.Parse(Boardgame.Attribute("id").Value),
                                                       BoardGameName = Boardgame.Element("item").Attribute("name").Value,
                                                       BoardGameId = int.Parse(Boardgame.Element("item").Attribute("objectid").Value),
                                                       PlayDate = safeParseDateTime(Boardgame.Attribute("date").Value),
                                                       NumberOfPlays = int.Parse(Boardgame.Attribute("quantity").Value),
                                                       Length = int.Parse(Boardgame.Attribute("length").Value),
                                                       UserComment = GetStringValue(Boardgame.Element("comments")),
                                                       Players = LoadPlayersList(Boardgame.Element("players"))
                                                   };

                return gameCollection;
            }
            catch (Exception ex)
            {
                return new List<Play>();
            }
        }
        private List<Player> LoadPlayersList(XElement xElement)
        {
            List<Player> players = new List<Player>();

            if (xElement != null)
            {
                foreach (XElement p in xElement.Elements("player"))
                {
                    Player pResult = new Player()
                    {
                        Username = p.Attribute("username").Value,
                        UserId = int.Parse(p.Attribute("userid").Value),
                        Name = p.Attribute("name").Value,
                        StartPosition = p.Attribute("startposition").Value,
                        Color = p.Attribute("username").Value,
                        Score = GetIntValue(p, "score", 0),
                        IsNewPlayer = GetBoolValue(p, "new"),
                        IsWinner = GetBoolValue(p, "win")
                    };
                    players.Add(pResult);
                }
            }

            return players;
        }

        public async Task<IEnumerable<SearchResult>> Search(string query, CancellationTokenSource cts)
        {
            try
            {
                query = query.Replace(" ", "+");
                Uri teamDataURI = new Uri(string.Format(BASE_URL + "/search?query={0}&type=boardgame", query));

                XDocument xDoc = await ReadData(teamDataURI, cts);

                // LINQ to XML.
                IEnumerable<SearchResult> searchResults = from Boardgame in xDoc.Descendants("item")
                                                          select new SearchResult
                                                          {
                                                              BoardGameName = GetStringValue(Boardgame.Element("name"), "value"),
                                                              BoardGameId = GetIntValue(Boardgame, "id"),
                                                              YearPublished = GetIntValue(Boardgame.Element("yearpublished"), "value")
                                                          };
                return searchResults;
            }
            catch (Exception ex)
            {
                return new List<SearchResult>();
            }
        }

        public async Task<string> LoadRules(int BoardGameId)
        {
            string baseRulesUrl =
                "https://boardgamegeek.com/item/weblinks?ajax=1&domain=&filter=%7B%22languagefilter%22:0,%22categoryfilter%22:%222702%22%7D"; // TODO Set language filter
            Uri rulesUrl = new Uri(string.Format(baseRulesUrl + "&objectid={0}&objecttype=thing&pageid=1&showcount={1}&version=v2", BoardGameId, 20));

            string data = await ReadJsonData(rulesUrl);
            RulesItem rulesData = JsonConvert.DeserializeObject<RulesItem>(data);
            return rulesData.WebLinks.FindLast(a => a.Categories.Last() == "Rules" && a.Languages.First() == "English").Url;
        }

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
                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
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

        private async Task<XDocument> ReadData(Uri requestUrl, CancellationTokenSource cts)
        {
            HttpClient httpClient = new HttpClient();
            XDocument data = new XDocument();
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(requestUrl).AsTask(cts.Token);

                response.EnsureSuccessStatusCode();

                data = XDocument.Parse(await response.Content.ReadAsStringAsync().AsTask(cts.Token));
            }
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Cancel for " + requestUrl);
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

        private async Task<string> ReadJsonData(Uri requestUrl)
        {
            // Due to malformed header I cannot use GetContentAsync and ReadAsStringAsync
            // UTF-8 is now hard-coded...

            string content = null;
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();

            using (System.Net.Http.HttpResponseMessage response = await client.GetAsync(requestUrl))
            {
                byte[] data = await response.Content.ReadAsByteArrayAsync();

                content = Encoding.UTF8.GetString(data.ToArray(), 0, (int)(data.Length));
            }


            if (content != null)
            {
                return content;
            }
            else
            {
                throw new Exception("Failed to download BGG data.");
            }
        }

        #region Editing data
        private async Task<CookieContainer> GetLoginCookies(string username, string password, CookieContainer cookieJar)
        {
            string postData = string.Format("lasturl=&username={0}&password={1}", username, password);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://www.boardgamegeek.com/login");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.CookieContainer = cookieJar;


            using (Stream webpageStream = await webRequest.GetRequestStreamAsync())
            {
                webpageStream.Write(byteArray, 0, byteArray.Length);
            }
            using (WebResponse response = await webRequest.GetResponseAsync())
            {

            }

            return cookieJar;
        }

        /// <summary>
        /// Adding to collection provides CollectionItemId
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public async Task<bool> AddToCollection(string username, string password, int gameId)
        {
            CookieContainer jar = new CookieContainer();
            jar = await GetLoginCookies(username, password, jar);

            string requestBase = "objecttype=thing&objectid={0}&instanceid=21&ajax=1&action=additem";
            string request = string.Format(requestBase, gameId);

            return await ProcessEditRequest(jar, request);
        }

        /// <summary>
        /// Requires CollectionItemId
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="collectionItemId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveFromCollection(string username, string password, int collectionItemId)
        {
            CookieContainer jar = new CookieContainer();
            jar = await GetLoginCookies(username, password, jar);

            string requestBase = "ajax=1&action=delete&collid={0}";
            string request = string.Format(requestBase, collectionItemId);

            return await ProcessEditRequest(jar, request);
        }

        public async Task<bool> EditCollectionItemStatus(string username, string password, CollectionItem item)
        {
            // fieldname=status&collid=33940367&own=1&prevowned=1&fortrade=1&want=1&wanttobuy=1&wanttoplay=1&preordered=1&wishlist=1&wishlistpriority=2&ajax=1&action=savedata
            // if parameter is present(no regard to value) then it is set to true on BGG
            CookieContainer jar = new CookieContainer();
            jar = await GetLoginCookies(username, password, jar);

            if (item == null)
                return false;

            string requestBase = "fieldname=status&collid={0}";
            string request = string.Format(requestBase, item.CollectionItemId);

            if (item.Owned)
            {
                request += "&own={0}";
                request = string.Format(request, Convert.ToInt32(item.Owned));
            }
            if (item.ForTrade)
            {
                request += "&fortrade={0}";
                request = string.Format(request, Convert.ToInt32(item.ForTrade));
            }
            if (item.WantToBuy)
            {
                request += "&wanttobuy={0}";
                request = string.Format(request, Convert.ToInt32(item.WantToBuy));
            }
            if (item.WantToPlay)
            {
                request += "&wanttoplay={0}";
                request = string.Format(request, Convert.ToInt32(item.WantToPlay));
            }
            if (item.Wishlist)
            {
                request += "&wishlist={0}&wishlistpriority={1}";
                request = string.Format(request, Convert.ToInt32(item.Wishlist), item.WishlistPriority);
            }

            request += "&ajax=1&action=savedata";

            return await ProcessEditRequest(jar, request);
        }

        /// <summary>
        /// Note, if you log in succesfully with username,correctPassword and later try to do this again with username,INcorrectpassword, the BGG server will still log you in!
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="gameId"></param>
        /// <param name="date"></param>
        /// <param name="amount"></param>
        /// <param name="comments"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task<bool> LogPlay(string username, string password, int gameId, DateTime date, int amount, string comments, int length)
        {
            //http://www.boardgamegeek.com/geekplay.php?objecttype=thing&objectid=104557&ajax=1&action=new

            CookieContainer jar = new CookieContainer();
            jar = await GetLoginCookies(username, password, jar);

            string requestBase = "dummy=1&ajax=1&action=save&version=2&objecttype=thing&objectid={0}&playid=&action=save&playdate={1}&dateinput={2}&YUIButton=&twitter=0&savetwitterpref=0&location=&quantity={3}&length={4}&incomplete=0&nowinstats=0&comments={5}";
            string request = string.Format(requestBase, gameId, date.ToString("yyyy-MM-dd"), DateTime.Today.ToString("yyyy-MM-dd"), amount, length, comments);

            byte[] byteArray = Encoding.UTF8.GetBytes(request);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://www.boardgamegeek.com/geekplay.php");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.CookieContainer = jar;

            using (Stream webpageStream = await webRequest.GetRequestStreamAsync())
            {
                webpageStream.Write(byteArray, 0, byteArray.Length);
            }
            string responseText;
            using (WebResponse response = await webRequest.GetResponseAsync())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                    if (responseText == "You must login to save plays")
                        return false;
                }
            }

            return true;
        }

        private async Task<bool> ProcessEditRequest(CookieContainer jar, string request)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(request);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://www.boardgamegeek.com/geekcollection.php");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.CookieContainer = jar;

            using (Stream webpageStream = await webRequest.GetRequestStreamAsync())
            {
                webpageStream.Write(byteArray, 0, byteArray.Length);
            }
            using (WebResponse response = await webRequest.GetResponseAsync())
            {
                // if response is not OK return false
            }

            // Should return false if already in collection
            return true;
        }
        #endregion
    }
}
