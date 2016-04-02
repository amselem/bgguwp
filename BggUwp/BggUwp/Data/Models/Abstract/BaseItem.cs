using SQLite.Net.Attributes;
using Template10.Mvvm;

namespace BggUwp.Data.Models.Abstract
{
    public abstract class BaseItem : BindableBase
    {
        private string _Name = "";
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                Set(ref _Name, value);
            }
        }

        private int _BoardGameId = 0;
        [PrimaryKey]
        public int BoardGameId
        {
            get
            {
                return _BoardGameId;
            }
            set
            {
                Set(ref _BoardGameId, value);
            }
        }

        private string _ThumbnailPath = "";
        public string ThumbnailPath
        {
            get
            {
                return _ThumbnailPath;
            }
            set
            {
                Set(ref _ThumbnailPath, value);
            }
        }

        private int _YearPublished = 0;
        public int YearPublished
        {
            get
            {
                return _YearPublished;
            }
            set
            {
                Set(ref _YearPublished, value);
            }
        }
    }
}
