using System.Numerics;

namespace CodeBattleArena.Models.DBModels
{
    public class FriendModel
    {
        public int IdPlayer1 { get; set; }
        public int IdPlayer2 { get; set; }
        public DateTime FriendshipDate { get; set; } = DateTime.Now;

        public PlayerModel Player1 { get; set; }
        public PlayerModel Player2 { get; set; }
    }
}
