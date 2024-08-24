namespace CodeBattleArena.Models.DBModels
{
    public class PlayerSessionModel
    {
        public int IdPlayer { get; set; }
        public int IdSession { get; set; }
        public string? CodeText { get; set; }
        public string? Time { get; set; }
        public int? Memory { get; set; }
        public bool State { get; set; }
        public virtual PlayerModel Player { get; set; }
        public virtual SessionModel Session { get; set; }
    }
}
