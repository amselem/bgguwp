using BggUwp.Data.Models;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BggUwp.Data
{
    public class RoamingStorageService
    {
        public static readonly RoamingStorageService Instance;

        static RoamingStorageService()
        {
            Instance = Instance ?? new RoamingStorageService();
        }

        #region Helpers
        private string DbName = "roamingdb.sqlite";

        private SQLiteConnection DbConnection
        {
            get
            {
                return new SQLiteConnection(
                    new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(),
                    Path.Combine(ApplicationData.Current.LocalFolder.Path, DbName));
            }
        }

        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void CreateDatabaseIfThereisNone()
        {
            if (!CheckFileExists(DbName).Result)
            {
                CreateDatabase();
            }
        }

        private void CreateDatabase()
        {
            using (var db = DbConnection)
            {
                db.CreateTable<PlayerDataItem>();
            }
        }
        #endregion

        public IEnumerable<PlayerDataItem> LoadAllPlayers()
        {
            IEnumerable<PlayerDataItem> players;
            using (var db = DbConnection)
            {
                players = db.Table<PlayerDataItem>().ToList();
            }
            return players;
        }

        public PlayerDataItem LoadPlayer(int id)
        {
            PlayerDataItem player = new PlayerDataItem();
            using (var db = DbConnection)
            {
                player = db.Find<PlayerDataItem>(p => p.Id == id);
            }
            return player;
        }

        public PlayerDataItem LoadPlayer(string name)
        {
            PlayerDataItem player = new PlayerDataItem();
            using (var db = DbConnection)
            {
                player = db.Find<PlayerDataItem>(p => p.Name == name);
            }
            return player;
        }

        public void AddPlayer(PlayerDataItem player)
        {
            using (var db = DbConnection)
            {
                db.InsertOrIgnore(player);
            }
        }

        public void SavePlayer(PlayerDataItem player)
        {
            using (var db = DbConnection)
            {
                db.Update(player);
            }
        }

        public void DeletePlayer(int id)
        {
            using (var db = DbConnection)
            {
                db.Delete<PlayerDataItem>(id);
            }
        }
    }
}
