using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Models.DBModels
{
    public class ChatModel
    {
        [Key]
        public int IdChat { get; set; }
        public int IdPlayer1 { get; set; }
        public int IdPlayer2 { get; set; }

        public PlayerModel Player1 { get; set; }
        public PlayerModel Player2 { get; set; }
        public ICollection<MessageModel> Messages { get; set; }
    }
}
