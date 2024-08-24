using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Models.DBModels
{
    public class TaskProgrammingModel
    {
        [Key]
        public int IdTaskProgramming { get; set; }
        public string Name { get; set; }
        public string Lang { get; set; }
        public string Difficulty { get; set; }
        public string TextTask { get; set; }
        public string Preparation { get; set; }
        public string VerificationCode {  get; set; }
        public virtual ICollection<TaskInputData>? TaskInputData { get; set; }

        public virtual ICollection<SessionModel> Sessions { get; set; }
    }
}
