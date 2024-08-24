using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Models.DBModels
{
    public class PlayerModel
    {
        [Key]
        public int IdPlayer { get; set; }
        public string NickName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string Role { get; set; }
        public string? AdditionalInformation { get; set; }
        public int Victories { get; set; }
        public string? Avatar { get; set; }
        public virtual ICollection<PlayerSessionModel>? PlayerSessions { get; set; }
        public ICollection<FriendModel> Friends1 { get; set; }
        public ICollection<FriendModel> Friends2 { get; set; }
        public ICollection<ChatModel> Chats1 { get; set; }
        public ICollection<ChatModel> Chats2 { get; set; }
        public ICollection<MessageModel> Messages { get; set; }
    }
}
