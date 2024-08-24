using CodeBattleArena.Models.DBModels;
using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Models.ViewModel
{
    public class TaskModel
    {
        public int? IdTask { get; set; } = null;

        [Required(ErrorMessage = "Fill in all the fields")]
        public string NameTask { get; set; }

        [Required(ErrorMessage = "Fill in all the fields")]
        public string LangProgramming { get; set; }

        [Required(ErrorMessage = "Fill in all the fields")]
        [RegularExpression("^(Easy|Medium|Hard)$", ErrorMessage = "Difficulty must be either 'Easy', 'Medium', or 'Hard'.")]
        public string Difficulty { get; set; }

        [Required(ErrorMessage = "Fill in all the fields")]
        public string TextTask { get; set; }

        [Required(ErrorMessage = "Fill in all the fields")]
        public string Preparation { get; set; }

        [Required(ErrorMessage = "Fill in all the fields")]
        public string VerificationCode { get; set; }

        public LangProgramming LangProgrammings = new LangProgramming();

        public List<TaskInputData>? taskInputDataList = new List<TaskInputData>();
    }
}
