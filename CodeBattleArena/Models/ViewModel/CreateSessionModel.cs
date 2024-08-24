using System.ComponentModel.DataAnnotations;

namespace CodeBattleArena.Models
{
    
    public class CreateSessionModel
    {
        [Required(ErrorMessage = "Fill in all the fields")]
        [MinLength(3, ErrorMessage = "The name must be at least 3 letters long")]
        [MaxLength(20, ErrorMessage = "The name must be no more than 20 letters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Fill in all the fields")]
        public string LangProgramming { get; set; }

        [RegularExpression("^(Public|Private)$", ErrorMessage = "State must be either 'Public', or 'Private'.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Fill in all the fields")]
        [Range(1, 6, ErrorMessage = "The number of people must be between 1 and 6.")]
        public int AmountPiople { get; set; }

        [RegularExpression("^(Easy|Medium|Hard)$", ErrorMessage = "Difficulty must be either 'Easy', 'Medium', or 'Hard'.")]
        public string Difficulty { get; set; }


        public LangProgramming LangProgrammings = new LangProgramming();
    }
}
