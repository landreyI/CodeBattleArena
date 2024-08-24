using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Models.DBModels
{
    public class SessionModel
    {
        [Key]
        public int IdSession { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string LangProgramming { get; set; }
        public string State { get; set; }
        public int AmountPiople { get; set; }
        public string Difficulty { get; set; }
        public int? TaskId { get; set; }
        public int? WinnerId { get; set; }
        public int CreatorId { get; set; }
        public string? Password { get; set; }
        public DateTime DateCreating { get; set; }
        public DateTime? DateStart { get; set; }
        public bool IsFinish { get; set; }
        public virtual ICollection<PlayerSessionModel>? PlayerSessions { get; set; }

        public virtual TaskProgrammingModel TaskProgramming { get; set; }
    }
}
