using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Models.DBModels
{
    public class MessageModel
    {
        [Key]
        public int IdMessage { get; set; }
        public int IdChat { get; set; }
        public int IdSender { get; set; }
        public string MessageText { get; set; }
        public DateTime SentDateTime { get; set; }

        public ChatModel Chat { get; set; }
        public PlayerModel Sender { get; set; }
    }
}
