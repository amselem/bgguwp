using BggUwp.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

//  Adapted from the BoardGameGeek client library created by WebKoala
//  Original source at https://github.com/WebKoala/W8BggApp

namespace BggUwp.Data
{
    public abstract class BoardGameFilter : BindableBase
    {
        public string DisplayName { get; set; }
        private int _Amount;
        public int Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                Set(ref _Amount, value);
            }
        }
    }

    public class ExpansionFilter : BoardGameFilter
    {
        public enum ExpansionStatus
        {
            All,
            BaseBoardGame,
            Expansion
        }

        private static List<ExpansionFilter> _DefaultFilters;

        public ExpansionStatus RequestedStatus;

        public static List<ExpansionFilter> DefaultFilters
        {
            get
            {
                if (_DefaultFilters == null)
                {
                    _DefaultFilters = new List<ExpansionFilter>();
                    _DefaultFilters.Add(new ExpansionFilter { RequestedStatus = ExpansionStatus.All, DisplayName = "All types" });
                    _DefaultFilters.Add(new ExpansionFilter
                    {
                        RequestedStatus = ExpansionStatus.BaseBoardGame,
                        DisplayName = "Base-games"
                    });
                    _DefaultFilters.Add(new ExpansionFilter
                    {
                        RequestedStatus = ExpansionStatus.Expansion,
                        DisplayName = "Expansions"
                    });
                }

                return _DefaultFilters;
            }
        }

        public bool Matches(CollectionDataItem game)
        {
            var match = true;
            if (game == null)
                match = false;

            switch (RequestedStatus)
            {
                case ExpansionStatus.All:
                    match = true;
                    break;
                case ExpansionStatus.BaseBoardGame:
                    match = game.IsExpansion == false;
                    break;
                case ExpansionStatus.Expansion:
                    match = game.IsExpansion;
                    break;
                default:
                    match = false;
                    break;
            }
            return match;
        }
    }

    public class PlayerFilter : BoardGameFilter
    {
        public bool Matches(CollectionDataItem game)
        {
            var match = true;
            if (game == null)
                match = false;
            else if (Amount == 0)
            {
                match = true; //match all;
            }
            else if (game.MinPlayers > Amount)
                match = false;
            else if (game.MaxPlayers < Amount)
                match = false;

            return match;
        }
    }

    public class PlayTimeFilter : BoardGameFilter
    {
        public bool Matches(CollectionDataItem game)
        {
            var match = true;
            if (game == null)
                match = false;
            else if (Amount == 0)
            {
                match = true; //match all;
            }
            else if (game.PlayingTime == 0)
            {
                match = false;
            }
            else if (game.PlayingTime > Amount)
                match = false;

            return match;
        }
    }

    public class StatusFilter : BoardGameFilter
    {
        private static List<StatusFilter> _DefaultFilters;
        public BoardGameCollectionStatus StatusToFilterOn;

        public static List<StatusFilter> DefaultFilters
        {
            get
            {
                if (_DefaultFilters == null)
                {
                    _DefaultFilters = new List<StatusFilter>();
                    _DefaultFilters.Add(new StatusFilter
                    {
                        DisplayName = "Show all",
                        StatusToFilterOn = BoardGameCollectionStatus.All
                    });
                    _DefaultFilters.Add(new StatusFilter
                    {
                        DisplayName = "Owned",
                        StatusToFilterOn = BoardGameCollectionStatus.Owned
                    });
                    _DefaultFilters.Add(new StatusFilter
                    {
                        DisplayName = "Want to play",
                        StatusToFilterOn = BoardGameCollectionStatus.WantToPlay
                    });
                    _DefaultFilters.Add(new StatusFilter
                    {
                        DisplayName = "Want to buy",
                        StatusToFilterOn = BoardGameCollectionStatus.WantToBuy
                    });
                    _DefaultFilters.Add(new StatusFilter
                    {
                        DisplayName = "Wishlist",
                        StatusToFilterOn = BoardGameCollectionStatus.WishList
                    });
                    _DefaultFilters.Add(new StatusFilter
                    {
                        DisplayName = "For Trade",
                        StatusToFilterOn = BoardGameCollectionStatus.ForTrade
                    });
                }

                return _DefaultFilters;
            }
        }

        public bool Matches(CollectionDataItem BoardGame)
        {
            switch (StatusToFilterOn)
            {
                case BoardGameCollectionStatus.All:
                    return true;

                case BoardGameCollectionStatus.Owned:
                    return BoardGame.Owned;

                case BoardGameCollectionStatus.ForTrade:
                    return BoardGame.ForTrade;

                case BoardGameCollectionStatus.WantToBuy:
                    return BoardGame.WantToBuy;

                case BoardGameCollectionStatus.WantToPlay:
                    return BoardGame.WantToPlay;

                case BoardGameCollectionStatus.WishList:
                    return BoardGame.Wishlist;
            }
            return false;
        }
    }

    public enum BoardGameCollectionStatus
    {
        All,
        Owned,
        ForTrade,
        WantToBuy,
        WantToPlay,
        WishList,
    }

    public class BoardgameSorter
    {
        public delegate IOrderedEnumerable<CollectionDataItem> Sorter(IEnumerable<CollectionDataItem> collection);

        private static List<BoardgameSorter> _DefaultSorters;

        public Sorter SortFunction { get; set; }
        public string DisplayName { get; set; }

        public static List<BoardgameSorter> DefaultSorters
        {
            get
            {
                if (_DefaultSorters == null)
                {
                    _DefaultSorters = new List<BoardgameSorter>();

                    _DefaultSorters.Add(new BoardgameSorter
                    {
                        DisplayName = "Sort by name A-Z",
                        SortFunction = collection => collection.OrderBy(x => x.Name)
                    });

                    _DefaultSorters.Add(new BoardgameSorter
                    {
                        DisplayName = "Sort by best rating",
                        SortFunction = collection => collection.OrderByDescending(x => x.AverageRating)
                    });

                    _DefaultSorters.Add(new BoardgameSorter
                    {
                        DisplayName = "Sort by best rank",
                        SortFunction = collection => collection.OrderBy(x => x.Rank + (x.Rank < 0 ? 100000 : 0))
                        // Sneaky, maybe improve later..
                    });

                    _DefaultSorters.Add(new BoardgameSorter
                    {
                        DisplayName = "Sort by newest",
                        SortFunction = collection => collection.OrderByDescending(x => x.YearPublished)
                    });
                }
                return _DefaultSorters;
            }
        }

        public IOrderedEnumerable<CollectionDataItem> Sort(IEnumerable<CollectionDataItem> collection)
        {
            if (SortFunction != null)
            {
                return SortFunction(collection);
            }
            return collection.OrderBy(x => x.Name);
        }
    }
}