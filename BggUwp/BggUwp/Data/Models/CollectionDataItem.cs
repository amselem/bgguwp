using BggUwp.Data.Models.Abstract;
using BggApi.Models;
using SQLite.Net.Attributes;

namespace BggUwp.Data.Models
{
    public class CollectionDataItem : BoardGameItem
    {
        public CollectionDataItem() { }
        public CollectionDataItem(CollectionItem apiItem)
        {
            Name = apiItem.Name;
            YearPublished = apiItem.YearPublished;
            BoardGameId = apiItem.BoardGameId;
            ThumbnailPath = apiItem.BoardGameId.ToString() + "_th.jpg";
            MinPlayers = apiItem.MinPlayers;
            MaxPlayers = apiItem.MaxPlayers;
            PlayingTime = apiItem.PlayingTime;
            IsExpansion = apiItem.IsExpansion;
            UserRating = apiItem.UserRating;
            GeekRating = apiItem.GeekRating;
            AverageRating = apiItem.AverageRating;
            Rank = apiItem.Rank;
            NumberOfPlays = apiItem.NumberOfPlays;

            CollectionItemId = apiItem.CollectionItemId;
            Owned = apiItem.Owned;
            PreviousOwned = apiItem.PreviousOwned;
            ForTrade = apiItem.ForTrade;
            Want = apiItem.Want;
            WantToPlay = apiItem.WantToPlay;
            WantToBuy = apiItem.WantToBuy;
            Wishlist = apiItem.Wishlist;
            WishlistPriority = apiItem.WishlistPriority;
            PreOrdered = apiItem.PreOrdered;
        }

        public CollectionDataItem(CollectionDataItem item)
        {
            Name = item.Name;
            YearPublished = item.YearPublished;
            BoardGameId = item.BoardGameId;
            ThumbnailPath = item.BoardGameId.ToString() + "_th.jpg";
            MinPlayers = item.MinPlayers;
            MaxPlayers = item.MaxPlayers;
            PlayingTime = item.PlayingTime;
            IsExpansion = item.IsExpansion;
            UserRating = item.UserRating;
            GeekRating = item.GeekRating;
            AverageRating = item.AverageRating;
            Rank = item.Rank;
            NumberOfPlays = item.NumberOfPlays;

            CollectionItemId = item.CollectionItemId;
            Owned = item.Owned;
            PreviousOwned = item.PreviousOwned;
            ForTrade = item.ForTrade;
            Want = item.Want;
            WantToPlay = item.WantToPlay;
            WantToBuy = item.WantToBuy;
            Wishlist = item.Wishlist;
            WishlistPriority = item.WishlistPriority;
            PreOrdered = item.PreOrdered;
        }

        private int _CollectionItemId = 0;
        [Indexed]
        public int CollectionItemId
        {
            get
            {
                return _CollectionItemId;
            }
            set
            {
                Set(ref _CollectionItemId, value);
            }
        }
        private bool _Owned = false;
        public bool Owned
        {
            get
            {
                return _Owned;
            }
            set
            {
                Set(ref _Owned, value);
            }
        }

        private bool _PreviousOwned = false;
        public bool PreviousOwned
        {
            get
            {
                return _PreviousOwned;
            }
            set
            {
                Set(ref _PreviousOwned, value);
            }
        }

        private bool _ForTrade = false;
        public bool ForTrade
        {
            get
            {
                return _ForTrade;
            }
            set
            {
                Set(ref _ForTrade, value);
            }
        }

        private bool _Want = false;
        public bool Want
        {
            get
            {
                return _Want;
            }
            set
            {
                Set(ref _Want, value);
            }
        }

        private bool _WantToTrade = false;
        public bool WantToTrade
        {
            get
            {
                return _WantToTrade;
            }
            set
            {
                Set(ref _WantToTrade, value);
            }
        }

        private bool _WantToPlay = false;
        public bool WantToPlay
        {
            get
            {
                return _WantToPlay;
            }
            set
            {
                Set(ref _WantToPlay, value);
            }
        }

        private bool _WantToBuy = false;
        public bool WantToBuy
        {
            get
            {
                return _WantToBuy;
            }
            set
            {
                Set(ref _WantToBuy, value);
            }
        }

        private bool _Wishlist = false;
        public bool Wishlist
        {
            get
            {
                return _Wishlist;
            }
            set
            {
                Set(ref _Wishlist, value);
            }
        }

        private int _WishlistPriority = 0;
        public int WishlistPriority
        {
            get
            {
                return _WishlistPriority;
            }
            set
            {
                Set(ref _WishlistPriority, value);
            }
        }

        private bool _PreOrdered = false;
        public bool PreOrdered
        {
            get
            {
                return _PreOrdered;
            }
            set
            {
                Set(ref _PreOrdered, value);
            }
        }
        private decimal _UserRating = 0; // Rating given by current user
        public decimal UserRating
        {
            get
            {
                return _UserRating;
            }
            set
            {
                Set(ref _UserRating, value);
            }
        }
        public string UserRatingDisplay
        {
            get
            {
                string str = UserRating.ToString();
                if (str == "0")
                    return "N/A";
                else
                    return str;
            }
        }

        private int _NumberOfPlays = 0;
        public int NumberOfPlays
        {
            get
            {
                return _NumberOfPlays;
            }
            set
            {
                Set(ref _NumberOfPlays, value);
            }
        }
    }
}
