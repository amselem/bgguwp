using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BggApi.Models;

namespace BggApi
{
    public interface IBggApiClient
    {
        Task<User> LoadUserDetails(string username);
        Task<IEnumerable<HotItem>> LoadHotItems();
        Task<IEnumerable<CollectionItem>> LoadCollection(string Username);
        Task<BoardGame> LoadBoardGame(int BoardGameId);
        Task<IEnumerable<BoardGame>> Search(string query);
        Task<IEnumerable<Play>> LoadLastPlays(string Username);
        //Task<int> LogPlay(int gameId, DateTime date, int amount, string comments, int length);
        //Task<bool> AddToCollection(int gameId);
        //Task<bool> RemoveFromCollection(int collectionItemId);
        //Task<bool> EditCollectionItemStatus(int gameId);
    }
}
