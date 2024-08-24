namespace CodeBattleArena.Models
{
    public class LangProgramming
    {
        public List<KeyValuePair<string, string>> LangProgrammings { get; set; }

        public LangProgramming()
        {
            LangProgrammings = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("csharp", "C#"),
                new KeyValuePair<string, string>("cpp", "C++"),
                new KeyValuePair<string, string>("java", "Java"),
                new KeyValuePair<string, string>("javascript", "JavaScript"),
                new KeyValuePair<string, string>("python", "Python")
            };
        }
    }
}
