namespace CodeBattleArena.Models.DBModels
{
    public class TaskInputData
    {
        public int TaskProgrammingId { get; set; }
        public TaskProgrammingModel TaskProgramming { get; set; }

        public string? Answer {  get; set; }

        public int InputDataTaskId { get; set; }
        public InputDataModel InputData { get; set; }
    }
}
