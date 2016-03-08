using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BggApi.Models.Abstract;

namespace BggApi.Models
{
    public class CollectionItem : BasicBoardGame
    {
        public int CollectionItemId { get; set; }
        public bool Owned { get; set; }
        public bool PreviousOwned { get; set; }
        public bool ForTrade { get; set; }
        public bool Want { get; set; }
        public bool WantToPlay { get; set; }
        public bool WantToBuy { get; set; }
        public bool Wishlist { get; set; }
        public int WishlistPriority { get; set; } // > 0 if wishlist is true
        public bool PreOrdered { get; set; }
        public decimal UserRating { get; set; }
        public string UserComment { get; set; }
    }
}
