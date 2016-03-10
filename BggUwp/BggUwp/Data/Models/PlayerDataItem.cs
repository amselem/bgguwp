using BggApi.Models;

namespace BggUwp.Data.Models
{
    public class PlayerDataItem : Player
    {
        public PlayerDataItem() { }
        public PlayerDataItem(Player apiPlayer)
        {
            Username = apiPlayer.Username;
            UserId = apiPlayer.UserId;
            Name = apiPlayer.Name;
            StartPosition = apiPlayer.StartPosition;
            Color = apiPlayer.Color;
            Score = apiPlayer.Score;
            IsNewPlayer = apiPlayer.IsNewPlayer;
            IsWinner = apiPlayer.IsWinner;
        }
    }
}
