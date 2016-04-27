using BggApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BggUwp.Data.Models
{
    public class PlayerDataItem : Player
    {
        public PlayerDataItem() { }
        public PlayerDataItem(Player apiPlayer)
        {
            Username = apiPlayer.Username;
            UserId = apiPlayer.UserId;
            PlayerId = apiPlayer.PlayerId;
            Name = apiPlayer.Name;
        }
    }
}
