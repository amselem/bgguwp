namespace BggUwp.Data.Models.Abstract
{
    public abstract class BoardGameItem : BaseItem 
    {
        private int _MinPlayers = 0;
        public int MinPlayers
        {
            get
            {
                return _MinPlayers;
            }
            set
            {
                Set(ref _MinPlayers, value);
            }
        }

        private int _MaxPlayers = 0;
        public int MaxPlayers
        {
            get
            {
                return _MaxPlayers;
            }
            set
            {
                Set(ref _MaxPlayers, value);
            }
        }

        private int _PlayingTime = 0;
        public int PlayingTime
        {
            get
            {
                return _PlayingTime;
            }
            set
            {
                Set(ref _PlayingTime, value);
            }
        }

        private bool _IsExpansion = false;
        public bool IsExpansion
        {
            get
            {
                return _IsExpansion;
            }
            set
            {
                Set(ref _IsExpansion, value);
            }
        }

        private decimal _GeekRating = 0;
        public decimal GeekRating
        {
            get
            {
                return _GeekRating;
            }
            set
            {
                Set(ref _GeekRating, value);
            }
        }

        private decimal _AverageRating = 0;
        public decimal AverageRating
        {
            get
            {
                return _AverageRating;
            }
            set
            {
                Set(ref _AverageRating, value);
            }
        }

        private int _Rank = 0;
        public int Rank
        {
            get
            {
                return _Rank;
            }
            set
            {
                Set(ref _Rank, value);
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
