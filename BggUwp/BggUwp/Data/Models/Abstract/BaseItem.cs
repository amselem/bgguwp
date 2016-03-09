using SQLite.Net.Attributes;
using Template10.Mvvm;

namespace BggUwp.Data.Models.Abstract
{
    public abstract class BaseItem : ViewModelBase
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

        private string _Thumbnail = "";
        public string Thumbnail
        {
            get
            {
                return _Thumbnail;
            }
            set
            {
                Set(ref _Thumbnail, value);
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
