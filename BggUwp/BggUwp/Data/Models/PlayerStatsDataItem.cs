using BggApi.Models;

namespace BggUwp.Data.Models
{
    public class PlayerStatsDataItem : PlayerStats
    {
        public PlayerStatsDataItem() { }
        public PlayerStatsDataItem(PlayerStats apiPlayerStats)
        {
            Username = apiPlayerStats.Username;
            BggUserId = apiPlayerStats.BggUserId;
            BggPlayerId = apiPlayerStats.BggPlayerId;
            Name = apiPlayerStats.Name;
            StartPosition = apiPlayerStats.StartPosition;
            Color = apiPlayerStats.Color;
            Score = apiPlayerStats.Score;
            Rating = apiPlayerStats.Rating;
            IsNewPlayer = apiPlayerStats.IsNewPlayer;
            IsWinner = apiPlayerStats.IsWinner;
        }

        public PlayerStatsDataItem(PlayerDataItem player)
        {
            Username = player.Username;
            BggUserId = player.BggUserId;
            BggPlayerId = player.BggPlayerId;
            Name = player.Name;
        }
    }
}
