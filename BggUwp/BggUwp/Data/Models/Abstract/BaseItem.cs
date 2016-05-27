using SQLite.Net.Attributes;
using Template10.Mvvm;

namespace BggUwp.Data.Models.Abstract
{
    public abstract class BaseItem : BindableBase
    {
        private string _BoardGameName = "";
        public string BoardGameName
        {
            get
            {
                return _BoardGameName;
            }
            set
            {
                Set(ref _BoardGameName, value);
            }
        }

        private int _BoardGameId = 0;
        [Indexed]
        public virtual int BoardGameId
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
    }
}
