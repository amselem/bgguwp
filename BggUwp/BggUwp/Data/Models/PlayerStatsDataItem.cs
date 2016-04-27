using BggApi.Models;

namespace BggUwp.Data.Models
{
    public class PlayerStatsDataItem : PlayerStats
    {
        public PlayerStatsDataItem() { }
        public PlayerStatsDataItem(PlayerStats apiPlayerStats)
        {
            Username = apiPlayerStats.Username;
            UserId = apiPlayerStats.UserId;
            PlayerId = apiPlayerStats.PlayerId;
            Name = apiPlayerStats.Name;
            StartPosition = apiPlayerStats.StartPosition;
            Color = apiPlayerStats.Color;
            Score = apiPlayerStats.Score;
            IsNewPlayer = apiPlayerStats.IsNewPlayer;
            IsWinner = apiPlayerStats.IsWinner;
        }
    }
}
