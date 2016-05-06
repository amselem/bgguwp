using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Text;
using SQLite.Net;
using System.IO;
using Windows.Storage;
using BggUwp.Data.Models;
using Windows.Security.Credentials;

namespace BggUwp.Data
{
    public class DebugTraceListener : ITraceListener
    {
        public void Receive(string message)
        {
            Debug.WriteLine(message);
        }
    }
    public class StorageService
    {
        #region Helpers
        private static string DbName = "db.sqlite";

        private static SQLiteConnection DbConnection
        {
            get
            {
                return new SQLiteConnection(
                    new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(),
                    Path.Combine(ApplicationData.Current.LocalFolder.Path, DbName));
            }
        }

        private static async Task<bool> CheckFileExists(string fileName)
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
        public static void CreateDatabaseIfThereisNone()
        {
            if (!CheckFileExists(DbName).Result)
            {
                CreateDatabase();
            }
        }

        private static void CreateDatabase()
        {
            using (var db = DbConnection)
            {
                db.CreateTable<HotDataItem>();
                db.CreateTable<CollectionDataItem>();
            }
        }
        #endregion

        public static IEnumerable<HotDataItem> LoadAllHotItems()
        {
            List<HotDataItem> hotItems;
            using (var db = DbConnection)
            {
                hotItems = db.Table<HotDataItem>().ToList();
            }
            return hotItems.OrderBy(x => x.Rank);
        }

        public static void SaveAllHotItems(IEnumerable<HotDataItem> items)
        {
            int retries = 0;
            while (retries < 15)
            {
                try
                {
                    using (var db = DbConnection)
                    {
                        db.DeleteAll<HotDataItem>();
                        db.InsertAll(items);
                    }
                    break;
                }
                catch (Exception) { }
                Task.Delay(110).Wait();
                retries++;
            }
        }

        public static IEnumerable<CollectionDataItem> LoadAllCollectionItems()
        {
            IEnumerable<CollectionDataItem> collectionItems;
            using (var db = DbConnection)
            {
                collectionItems = db.Table<CollectionDataItem>().ToList();
            }
            return collectionItems;
        }

        public static void SaveAllCollectionItems(IEnumerable<CollectionDataItem> items)
        {
            int retries = 0;
            while (retries < 15)
            {
                try
                {
                    using (var db = DbConnection)
                    {
                        db.DeleteAll<CollectionDataItem>();
                        db.InsertAll(items);
                    }
                    break;
                }
                catch (Exception) { }
                Task.Delay(110).Wait();
                retries++;
            }
        }

        public static IEnumerable<HotDataItem> SearchInHotItems(string queryString)
        {
            IEnumerable<HotDataItem> results;
            using (var db = DbConnection)
            {
                results = db.Table<HotDataItem>().Where(item => item.BoardGameName.Contains(queryString)).ToList();
            }

            return results;
        }

        public static IEnumerable<CollectionDataItem> SearchInCollection(string queryString)
        {
            IEnumerable<CollectionDataItem> results;
            using (var db = DbConnection)
            {
                results = db.Table<CollectionDataItem>().Where(item => item.BoardGameName.Contains(queryString)).ToList();
            }
            return results;
        }

        public static CollectionDataItem LoadCollectionItem(int itemId)
        {
            CollectionDataItem item = new CollectionDataItem();
            using (var db = DbConnection)
            {
                item = db.Find<CollectionDataItem>(a => a.BoardGameId == itemId);
            }
            return item;
        }

        public static void SaveCollectionItem(CollectionDataItem collectionItem)
        {
            using (var db = DbConnection)
            {
                db.Delete(collectionItem);
                db.Insert(collectionItem);
            }
        }

        public static void RemoveCollectionItem(CollectionDataItem collectionItem)
        {
            using (var db = DbConnection)
            {
                db.Delete(collectionItem);
            }
        }

        private static string appName = "BGG UWP"; // TODO Change to app name REFERENCE
        private static string defaultUserName = "UWPTester"; // TODO Change before production phase
        public static void SaveUserCredentials(string username, string password)
        {
            var vault = new PasswordVault();
            RemoveCredential();

            vault.Add(new PasswordCredential(appName, username, password)); 
        }

        public static PasswordCredential RetrieveUserCredentials()
        {
            PasswordCredential credential = null;

            var vault = new PasswordVault();
            try
            {
                credential = vault.Retrieve(appName, RetrieveUserName());
            }
            catch
            {

            }

            return credential;
        }

        private static string RetrieveUserName()
        {
            PasswordCredential credential = null;

            var vault = new PasswordVault();
            try
            {
                var credentialList = vault.FindAllByResource(appName);
                if (credentialList.Count == 1)
                {
                    credential = credentialList[0];
                }
            }
            catch
            {

            }

            if (credential != null)
                return credential.UserName;

            return String.Empty;
        }

        private static void RemoveCredential()
        {
            var vault = new PasswordVault();
            try
            {
                vault.Remove(RetrieveUserCredentials());
            }
            catch (Exception)
            {

            }
        }
    }
}
