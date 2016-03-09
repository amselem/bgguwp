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

        public static bool IsThereAnyData()
        {
            
            HotDataItem item = new HotDataItem();

            using (var db = DbConnection)
            {
                // db.TraceListener = new DebugTraceListener();

                item = db.Table<HotDataItem>().ToList().First();
            }

            return item != null;
        }

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
            using (var db = DbConnection)
            {
                db.InsertOrReplaceAll(items);
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
            using (var db = DbConnection)
            {
                db.InsertOrReplaceAll(items);
            }
        }
    }
}
