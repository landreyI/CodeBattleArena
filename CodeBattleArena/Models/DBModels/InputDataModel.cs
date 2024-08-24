using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Models.DBModels
{
    public class InputDataModel
    {
        [Key]
        public int IdInputData { get; set; }
        public string Data { get; set; }

        public virtual ICollection<TaskInputData>? TaskInputData { get; set; }
    }
}
