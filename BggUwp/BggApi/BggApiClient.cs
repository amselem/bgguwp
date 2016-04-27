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
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
            {
                return new List<CollectionItem>();
            }
        }
        public async Task<BoardGame> LoadBoardGame(int boardGameId)
        {
            try
            {
                Uri teamDataURI = new Uri(string.Format(BASE_URL + "/thing?id={0}&stats=1", boardGameId));
                XDocument xDoc = await ReadData(teamDataURI);

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
                                                            AverageRating = double.Parse(Boardgame.Element("item").Element("statistics").Element("ratings").Element("average").Attribute("value").Value),
                                                            GeekRating = double.Parse(Boardgame.Element("item").Element("statistics").Element("ratings").Element("bayesaverage").Attribute("value").Value),
                                                            Rank = GetRanking(Boardgame.Element("item").Element("statistics").Element("ratings").Element("ranks")),
                                                            Publishers = (from p in Boardgame.Element("item").Elements("link") where p.Attribute("type").Value == "boardgamepublisher" select p.Attribute("value").Value).ToList(),
                                                            Designers = (from p in Boardgame.Element("item").Elements("link") where p.Attribute("type").Value == "boardgamedesigner" select p.Attribute("value").Value).ToList(),
                                                            Artists = (from p in Boardgame.Element("item").Elements("link") where p.Attribute("type").Value == "boardgameartist" select p.Attribute("value").Value).ToList(),
                                                            PlayerPollResults = LoadPlayerPollResults(Boardgame.Element("item").Element("poll")),
                                                            IsExpansion = SetIsExpansion(Boardgame)
                                                        };

                return gameCollection.FirstOrDefault();
            }
            catch (Exception)
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
                rank = 0;
            else if (val.ToLower().Trim() == "not ranked")
                rank = 0;
            else if (!int.TryParse(val, out rank))
                rank = 0;

            return rank;
        }
        #endregion

        public async Task<IEnumerable<Play>> LoadLastPlays(string Username)
        {
            try
            {
                Uri teamDataURI = new Uri(string.Format(BASE_URL + "/plays?username={0}", Username));
                XDocument xDoc = await ReadData(teamDataURI);

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
            catch (Exception)
            {
                return new List<Play>();
            }
        }
        private List<PlayerStats> LoadPlayersList(XElement xElement)
        {
            List<PlayerStats> players = new List<PlayerStats>();

            if (xElement != null)
            {
                foreach (XElement p in xElement.Elements("player"))
                {
                    PlayerStats pResult = new PlayerStats()
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

                IEnumerable<SearchResult> searchResults = from Boardgame in xDoc.Descendants("item")
                                                          select new SearchResult
                                                          {
                                                              BoardGameName = GetStringValue(Boardgame.Element("name"), "value"),
                                                              BoardGameId = GetIntValue(Boardgame, "id"),
                                                              YearPublished = GetIntValue(Boardgame.Element("yearpublished"), "value")
                                                          };
                return searchResults;
            }
            catch (Exception)
            {
                return new List<SearchResult>();
            }
        }

        public async Task<string> LoadRules(int boardGameId)
        {
            string baseRulesUrl =
                "http://boardgamegeek.com/item/weblinks?ajax=1&domain=&filter=%7B%22languagefilter%22:0,%22categoryfilter%22:%222702%22%7D"; // TODO Set language filter
            Uri rulesUrl = new Uri(string.Format(baseRulesUrl + "&objectid={0}&objecttype=thing&pageid=1&showcount={1}&version=v2", boardGameId, 20));

            string data = await ReadJsonData(rulesUrl);

            if (String.IsNullOrEmpty(data))
                return string.Format("http://www.boardgamegeek.com/boardgame/{0}", boardGameId);

            RulesItem rulesData = JsonConvert.DeserializeObject<RulesItem>(data);
            if (rulesData.WebLinks.Count != 0)
                return rulesData.WebLinks.FindLast(a => a.Categories.Last() == "Rules" && a.Languages.First() == "English").Url;

            return string.Format("http://www.boardgamegeek.com/boardgame/{0}", boardGameId);
        }

        public async Task<CollectionItem> LoadCollectionItem(int boardGameId, string username, int userId)
        {
            // https://boardgamegeek.com/api/collections?objectid=187645&objecttype=thing&userid=1221304
            string baseCollIdUrl = "https://boardgamegeek.com/api/collections"; // TODO Set language filter
            Uri fullCollIdUrl = new Uri(string.Format(baseCollIdUrl + "?objectid={0}&objecttype=thing&userid={1}", boardGameId, userId));

            string data = await ReadJsonData(fullCollIdUrl);
            CollectionItemInNewApi collectionItemData = JsonConvert.DeserializeObject<CollectionItemInNewApi>(data);
            int collId = int.Parse(collectionItemData.items.FirstOrDefault().collid);
            // https://www.boardgamegeek.com/xmlapi2/collection?username=webkoala&collid=6918162

            string baseCollItemUrl = "https://www.boardgamegeek.com/xmlapi2/collection"; // TODO Set language filter
            Uri fullCollItemUrl = new Uri(string.Format(baseCollItemUrl + "?username={0}&stats=1&collid={1}", username, collId));

            XDocument xDoc = await ReadData(fullCollItemUrl);

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
                                                             UserComment = GetStringValue(colItem.Element("comment"))
                                                         };

            return baseBoardGames.FirstOrDefault();
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
        private int GetIntValue(XElement element, string attribute = null, int defaultValue = 0)
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
        private double GetDecimalValue(XElement element, string attribute = null, double defaultValue = 0)
        {
            string val = GetStringValue(element, attribute, null);
            if (val == null)
                return defaultValue;

            double retVal;
            if (!double.TryParse(val, out retVal))
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

            try
            {
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
            }
            catch (Exception ex)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    throw new Exception("Failed to download BGG data.", ex.InnerException);
            }

            return data;
        }

        private async Task<XDocument> ReadData(Uri requestUrl, CancellationTokenSource cts)
        {
            HttpClient httpClient = new HttpClient();
            XDocument data = new XDocument();
            try
            {
                data = XDocument.Parse(await httpClient.GetStringAsync(requestUrl).AsTask(cts.Token));
            }
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Cancel for " + requestUrl);
            }
            catch (Exception ex)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    throw new Exception("Failed to download BGG data.", ex.InnerException);
            }

            return data;
        }

        private async Task<string> ReadJsonData(Uri requestUrl)
        {
            string content = null;
            HttpClient httpClient = new HttpClient();

            try
            {
                content = await httpClient.GetStringAsync(requestUrl);
            }
            catch (Exception ex)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    throw new Exception("Failed to download BGG data.", ex.InnerException);
            }

            return content;
        }

        #region Editing data
        private async Task<bool> GetLoginCookies(string username, string password)
        {
            string request = string.Format("lasturl=&username={0}&password={1}", username, password);
            HttpClient httpClient = new HttpClient();

            HttpStringContent requestStringContent = new HttpStringContent(request);
            requestStringContent.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue("application/x-www-form-urlencoded");
            try
            {
                await httpClient.PostAsync(new Uri("https://www.boardgamegeek.com/login"), requestStringContent);
            }
            catch (Exception ex)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    throw new Exception("Failed to log user in.", ex.InnerException);

                return false;
            }

            return true;
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
            await GetLoginCookies(username, password);

            string requestBase = "objecttype=thing&objectid={0}&instanceid=21&ajax=1&action=additem";
            string request = string.Format(requestBase, gameId);

            return await ProcessEditRequest(request);
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
            await GetLoginCookies(username, password);

            string requestBase = "ajax=1&action=delete&collid={0}";
            string request = string.Format(requestBase, collectionItemId);

            return await ProcessEditRequest(request);
        }

        public async Task<bool> EditCollectionItemStatus(string username, string password, CollectionItem item)
        {
            // fieldname=status&collid=33940367&own=1&prevowned=1&fortrade=1&want=1&wanttobuy=1&wanttoplay=1&preordered=1&wishlist=1&wishlistpriority=2&ajax=1&action=savedata
            // if parameter is present(no regard to value) then it is set to true on BGG
            await GetLoginCookies(username, password);

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
            if (item.UserRating >= 0)
            {
                string ratingRequestBase = "fieldname=rating&collid={0}&rating={1}&ajax=1&action=savedata";
                string ratingRequest = string.Format(ratingRequestBase, item.CollectionItemId, item.UserRating);
                await ProcessEditRequest(ratingRequest);
            }
            if (!String.IsNullOrEmpty(item.UserComment))
            {
                string commentRequestBase = "fieldname=comment&collid={0}&value={1}&ajax=1&action=savedata";
                string commentRequest = string.Format(commentRequestBase, item.CollectionItemId, Uri.EscapeDataString(item.UserComment));
                await ProcessEditRequest(commentRequest);
            }

            request += "&ajax=1&action=savedata";

            return await ProcessEditRequest(request);
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
            //https://www.boardgamegeek.com/geekplay.php?objecttype=thing&objectid=104557&ajax=1&action=new
            await GetLoginCookies(username, password);

            string requestBase = "dummy=1&ajax=1&action=save&version=2&objecttype=thing&objectid={0}&playid=&action=save&playdate={1}&dateinput={2}&YUIButton=&twitter=0&savetwitterpref=0&location=&quantity={3}&length={4}&incomplete=0&nowinstats=0&comments={5}";
            string request = string.Format(requestBase, gameId, date.ToString("yyyy-MM-dd"), DateTime.Today.ToString("yyyy-MM-dd"), amount, length, comments);

            HttpClient httpClient = new HttpClient();
            HttpStringContent requestStringContent = new HttpStringContent(request);
            requestStringContent.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue("application/x-www-form-urlencoded");
            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync(new Uri("https://www.boardgamegeek.com/geekplay.php"), requestStringContent);
            }
            catch (Exception ex)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    throw new Exception("Failed to log a play.", ex.InnerException);

                return false;
            }


            return response.StatusCode == Windows.Web.Http.HttpStatusCode.Ok;
        }

        private async Task<bool> ProcessEditRequest(string request)
        {
            HttpClient httpClient = new HttpClient();
            HttpStringContent requestStringContent = new HttpStringContent(request);
            requestStringContent.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue("application/x-www-form-urlencoded");
            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync(new Uri("https://www.boardgamegeek.com/geekcollection.php"), requestStringContent);
            }
            catch (Exception ex)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    throw new Exception("Failed to process edit data request.", ex.InnerException);

                return false;
            }

            return response.StatusCode == Windows.Web.Http.HttpStatusCode.Ok;
        }
        #endregion
    }
}
