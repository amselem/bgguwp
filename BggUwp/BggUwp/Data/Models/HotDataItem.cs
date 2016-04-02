using BggUwp.Data.Models.Abstract;
using BggApi.Models;

namespace BggUwp.Data.Models
{
    public class HotDataItem : BaseItem
    {
        public HotDataItem() {}

        public HotDataItem(HotItem apiItem)
        {
            Name = apiItem.Name;
            BoardGameId = apiItem.BoardGameId;
            Rank = apiItem.Rank;
            ThumbnailPath = apiItem.BoardGameId.ToString() + "_th.jpg";
            YearPublished = apiItem.YearPublished;
        }

        private int _Ranking = 0;
        public int Rank
        {
            get
            {
                return _Ranking;
            }
            set
            {
                Set(ref _Ranking, value);
            }
        }
    }
}
