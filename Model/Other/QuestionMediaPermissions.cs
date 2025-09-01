namespace PubQuizBackend.Model.Other
{
    public class QuestionMediaPermissions
    {
        public Dictionary<int, HashSet<int>> Permissions { get; set; } = new();
    }
}
